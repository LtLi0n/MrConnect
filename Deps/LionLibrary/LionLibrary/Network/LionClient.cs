using LionLibrary.Framework;
using LionLibrary.Utils;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace LionLibrary.Network
{
    ///<summary>Only available for TLS connections. For now.</summary>
    public class LionClient : IPacketPipe
    {
        ///<summary>If you send anything through this, not defined methods, go to hell :(</summary>
        public SslStream SSL { get; private set; }
        public TcpClient Client { get; private set; }
        public bool Listening { get; private set; } = false;

        private readonly Thread _listenThread;
        private readonly Thread _sendThread;
        private readonly ConcurrentQueue<PacketBuilder> _packetsToSend;
        private readonly ConcurrentDictionary<(long id, long replyId), Packet> _packetsReceived;

        private readonly (IPAddress, int) _hostIp;
        private readonly string _certName = string.Empty;
        ///<summary>If TcpClient is passed into the constructor, that indicates that the server is just serving a client.
        ///<para>Likewise, supplying host and cert_name args will assume the client is the program focus.</para></summary>
        public bool IsServerClient => string.IsNullOrEmpty(_certName);
        ///<summary>Packet id track</summary>
        private long _packetC = 0;

        public int ReadTimeout { get; set; } = 5000;
        public int WriteTimeout { get; set; } = 5000;

        public event Func<Packet, Task> PacketReceived { add { _packetReceivedEvent.Add(value); } remove { _packetReceivedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Packet, Task>> _packetReceivedEvent;

        ///<summary>Use it to force a handshake. Useful for auto-login, auto-anything that is executed once upon connection.
        ///<para>Do not use for server-side.</para></summary>
        public event Func<Task> Connected { add { _connectedEvent.Add(value); } remove { _connectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _connectedEvent;

        public event Func<Task> Disconnected { add { _disconnectedEvent.Add(value); } remove { _disconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _disconnectedEvent;

        ///<summary>Use it to force a handshake. Useful for auto-login, auto-anything that is executed once upon connection.
        ///<para>Do not use for server-side.</para></summary>
        public event Func<Task> Reconnected { add { _reconnectedEvent.Add(value); } remove { _reconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _reconnectedEvent;

        private List<long> _expectedPackets;

        ///<summary>Packet size variable for watchdog thread. Stores the length of the bytes that are available to read in <see cref="int"/>.</summary>
        private readonly byte[] _size = new byte[4];
        ///<summary>Packet size result variable for watchdog thread. Indicates a number jof bytes that are available to read.</summary>
        private int _size_result = 0;

        private readonly ILogService _logger;

        public LionClient(string host, int port, string certName, ILogService logger = null) : this(logger)
        {
            if (host == "localhost")
            {
                host = NetworkEssentials.LocalIPv4Address;
            }

            _hostIp = (IPAddress.Parse(host), port);
            _certName = certName;
        }

        public LionClient(TcpClient client, SslStream ssl, ILogService logger = null) : this(logger)
        {
            Client = client;
            SSL = ssl;

            IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            _hostIp = (endPoint.Address, endPoint.Port);
        }

        private void Clean()
        {
            _packetC = 0;
            _packetsToSend.Clear();
            _packetsReceived.Clear();
        }

        private LionClient(ILogService logger)
        {
            _logger = logger;
            _packetC = 0;
            _packetsToSend = new ConcurrentQueue<PacketBuilder>();
            _packetsReceived = new ConcurrentDictionary<(long, long), Packet>();
            _expectedPackets = new List<long>();
            _listenThread = new Thread(async () =>
            {
                try { await ListenLoop(); }
                catch (Exception ex) { _logger?.LogLine(this, ex.Message, LogSeverity.Critical); }
            });
            _sendThread = new Thread(async () =>
            {
                try { await SendLoop(); }
                catch (Exception ex) { _logger?.LogLine(this, ex.Message, LogSeverity.Critical); }
            });

            _packetReceivedEvent = new AsyncEvent<Func<Packet, Task>>();
            _connectedEvent = new AsyncEvent<Func<Task>>();
            _disconnectedEvent = new AsyncEvent<Func<Task>>();
            _reconnectedEvent = new AsyncEvent<Func<Task>>();
        }

        public void Start()
        {
            if (!Listening)
            {
                Listening = true;
                _listenThread.Start();
                _sendThread.Start();
            }
        }

        public void Stop()
        {
            if (Listening)
            {
                Listening = false;
                _listenThread.Join();
                _sendThread.Join();
            }
        }

        ///<summary>Prevent exit while connected.</summary>
        public async Task HoldAsync()
        {
            while (Listening)
            {
                await Task.Delay(1);
            }
        }

        public async Task ConnectAsync(bool is_reconnect = false)
        {
            if (IsServerClient)
            {
                throw new InvalidOperationException("Server cannot force client to reconnect, that's not how it works.");
            }

            while (true)
            {
                try
                {
                    if (Client == null || is_reconnect)
                    {
                        Client = new TcpClient(_hostIp.Item1.ToString(), _hostIp.Item2);
                        _logger?.LogLine(this, $"Connected to host: [{_hostIp.Item1.ToString()}:{_hostIp.Item2}].", LogSeverity.Debug);
                    }

                    SSL = new SslStream(
                        Client.GetStream(),
                        false,
                        new RemoteCertificateValidationCallback(ValidateServerCertificate),
                        null);

                    SSL.ReadTimeout = ReadTimeout;
                    SSL.WriteTimeout = WriteTimeout;

                    _logger?.LogLine(this, "SSL connection created.", LogSeverity.Debug);
                    await SSL.AuthenticateAsClientAsync(_certName);
                    _logger?.LogLine(this, "SSL authentication finished.", LogSeverity.Debug);

                    if (!is_reconnect) await _connectedEvent.InvokeAsync();
                    else await _reconnectedEvent.InvokeAsync();

                    break;
                }
                catch (Exception ex)
                {
                    Clean();

                    if (ex is SocketException)
                    {
                        _logger?.LogLine(
                            this,
                            is_reconnect ?
                            $"Connection lost at [{_hostIp.Item1}:{_hostIp.Item2}]. Attempting to reconnect..." :
                            $"Connection refused at [{_hostIp.Item1}:{_hostIp.Item2}]. Retrying...",
                            LogSeverity.Error);
                        await Task.Delay(1000);
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
        }

        private async Task<Packet> TryDownloadPacket()
        {
            if (!SSL.CanRead)
            {
                if (!Client.Client.Poll(Client.Client.ReceiveTimeout * 1000, SelectMode.SelectRead))
                {
                    return null;
                }
            }

            Packet packet = await DownloadPacketAsyncCore();

            if (packet != null)
            {
                await _packetReceivedEvent.InvokeAsync(packet);

                if (_expectedPackets.Contains(packet.ReplyId))
                {
                    if (!_packetsReceived.TryAdd((packet.ReplyId, packet.Id), packet))
                    {
                        _logger?.LogLine(this, "Packet received could not be added.", LogSeverity.Critical);
                    }
                }
            }

            return packet;
        }

        private async Task SendLoop()
        {
            while (Listening)
            {
                try
                {
                    if (NetworkEssentials.IsConnected(Client.Client))
                    {
                        //send packets.
                        if (!_packetsToSend.IsEmpty)
                        {
                            if (_packetsToSend.TryDequeue(out PacketBuilder pb))
                            {
                                await SendPacketAsyncCore(pb);
                            }
                        }
                    }
                    else //on disconnect.
                    {
                        Clean();
                        await _disconnectedEvent.InvokeAsync();

                        if (IsServerClient)
                        {
                            Listening = false;
                            throw new SocketException((int)SocketError.ConnectionAborted);
                        }
                        await ConnectAsync(is_reconnect: true);
                        _ = _reconnectedEvent.InvokeAsync();
                    }

                    await Task.Delay(1);
                }
                catch (Exception ex)
                {
                    if (ex is SocketException)
                    {
                        if (!IsServerClient)
                        {
                            await ConnectAsync();
                        }
                    }

                    else throw ex;
                }
            }
        }

        ///<summary>A core loop pipeline, ensuring stable sending and retrieval of incoming packets.</summary>
        private async Task ListenLoop()
        {
            while (Listening)
            {
                try
                {
                    if (NetworkEssentials.IsConnected(Client.Client))
                    {
                        //download host packets.
                        await TryDownloadPacket();
                    }

                    await Task.Delay(1);
                }
                catch { }
            }
        }

        ///<summary>Sends a packet to the host. Should remain private to ensure synchronous sending.
        ///<para>Synchronous sending can malform the <see cref="SslStream"/> buffer and create nonsense at host-side.</para>
        ///</summary>
        private async Task SendPacketAsyncCore(PacketBuilder upl_pb)
        {
            Packet upl_packet = new Packet(upl_pb);
            byte[] buffer = upl_packet.Build();
            //Send a packet.
            await SSL.WriteAsync(buffer, 0, buffer.Length);
            await SSL.FlushAsync();

            _logger?.LogLine(this, $"    SENT Packet [id: {upl_packet.Id}, reply_id: {upl_packet.ReplyId}] origin [{upl_packet.Header}].", LogSeverity.Debug);
        }

        ///<summary>Checks if the host has sent a new packet. If so, the packet is returned. Should remain private.</summary>
        ///<returns><see cref="Packet"/> sent by host.</returns>
        private async Task<Packet> DownloadPacketAsyncCore()
        {
            if (_size_result != 4)
            {
                int returned_bytes = await SSL.ReadAsync(_size, _size_result, 4 - _size_result);
                if (returned_bytes == 0) return null;
                _size_result += returned_bytes;
            }

            if (_size_result == 4)
            {
                _size_result = 0;
                int data_size = BitConverter.ToInt32(_size, 0) - 4;
                byte[] buffer = new byte[data_size];

                int obtained_bytes = 0;

                while (obtained_bytes != data_size && Listening)
                {
                    obtained_bytes += await SSL.ReadAsync(buffer, obtained_bytes, data_size - obtained_bytes);
                }

                //read & process the packet
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    Packet response = new Packet(ms);
                    _logger?.LogLine(this, $"RECEIVED Packet [id: {response.Id}, reply_id: {response.ReplyId}] origin [{response.Header}].", LogSeverity.Debug);
                    return response;
                }
            }

            return null;
        }

        public void SendPacket(PacketBuilder pb)
        {
            AssignPacketId(pb);
            _packetsToSend.Enqueue(pb);
        }

        public void SendPacket(Action<PacketBuilder> builder_action) => QueuePacketBuilder(builder_action);

        ///<summary>Send a packet and wait for a host response.</summary>
        ///<returns><see cref="Packet"/> response from the host.</returns>
        public async Task<Packet> DownloadPacketAsync(Action<PacketBuilder> builder_action)
        {
            PacketBuilder pb = ConstructPacketBuilder(builder_action);
            return await DownloadPacketAsync(pb);
        }

        ///<summary>Send a packet and wait for a host response.</summary>
        ///<returns><see cref="ResponsePacket{T}"/> response from the host.</returns>
        public async Task<Packet<T>> DownloadPacketAsync<T>(Action<PacketBuilder> builder_action)
        {
            if (Client == null)
            {
                throw new SocketException((int)SocketError.NotConnected);
            }
            PacketBuilder pb = ConstructPacketBuilder(builder_action);
            Packet response = await DownloadPacketAsync(pb);
            return new Packet<T>(response);
        }

        ///<summary>Send a packet and wait for a host response.</summary>
        ///<returns><see cref="Packet"/> response from the host.</returns>
        public async Task<Packet> DownloadPacketAsync(PacketBuilder pb)
        {
            if(Client == null)
            {
                throw new SocketException((int)SocketError.NotConnected);
            }

            AssignPacketId(pb);
            long assigned_id = pb.Id;
            _expectedPackets.Add(pb.Id);

            _packetsToSend.Enqueue(pb);

            DateTime timeout_since = DateTime.Now;
            CancellationTokenSource cancel_token = new CancellationTokenSource();

            Task<Packet> download_t = Task.Run(async () =>
            {
                while (!cancel_token.Token.IsCancellationRequested)
                {
                    var pair = _packetsReceived.FirstOrDefault(x => x.Key.replyId == assigned_id);
                    Packet packet = pair.Value;

                    if (packet != null)
                    {
                        if (_packetsReceived.TryRemove(pair.Key, out Packet result))
                        {
                            return result;
                        }
                    }

                    await Task.Delay(1);
                }
                return null;
            });

            while (!download_t.IsCompleted && (DateTime.Now - timeout_since).TotalMilliseconds < ReadTimeout)
            {
                await Task.Delay(1);
            }

            if (!download_t.IsCompleted)
            {
                cancel_token.Cancel();
                cancel_token.Dispose();
                _logger?.LogLine(this, $"TIMEOUT Packet [id: {pb.Id}, reply_id: {pb.ReplyId}] origin [{pb.Header}].", LogSeverity.Warning);
                throw new TimeoutException($"TIMEOUT Packet [id: {pb.Id}, reply_id: {pb.ReplyId}] origin [{pb.Header}].");
            }
            else
            {
                cancel_token.Dispose();
                return await download_t;
            }
        }

        ///<summary>Send a packet and wait for a host response.</summary>
        ///<returns><see cref="ResponsePacket{T}"/> response from the host.</returns>
        public async Task<Packet<T>> DownloadPacketAsync<T>(PacketBuilder pb)
        {
            Packet response = await DownloadPacketAsync(pb);
            return new Packet<T>(response);
        }

        private PacketBuilder ConstructPacketBuilder(Action<PacketBuilder> builder_action)
        {
            PacketBuilder pb = new PacketBuilder();
            builder_action(pb);
            return pb;
        }

        private void QueuePacketBuilder(Action<PacketBuilder> builder_action)
        {
            PacketBuilder pb = ConstructPacketBuilder(builder_action);
            AssignPacketId(pb);
            _packetsToSend.Enqueue(pb);
        }

        ///<summary>Force the id values to be consistent and incremental.</summary>
        private void AssignPacketId(PacketBuilder pb)
        {
            pb.Id = _packetC;
            _packetC++;
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None) return true;
            _logger?.LogLine(this, $"Certificate error: {sslPolicyErrors}", LogSeverity.Critical);
            return false;
        }

        public void LogLine(object text, LogSeverity severity = LogSeverity.Info) =>
            _logger?.LogLine(this, text, severity);

        public void Log(object text, LogSeverity severity = LogSeverity.Info) =>
            _logger?.Log(this, text, severity);
    }
}
