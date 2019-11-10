using LionLibrary.Commands;
using LionLibrary.Framework;
using LionLibrary.Utils;
using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace LionLibrary.Network
{
    ///<summary>Tls server implementation.</summary>
    ///<typeparam name="ContextT">Leave as <see cref="SocketCommandContext"/> to ignore Init second argument if you wish to.</typeparam>
    public class LionServer<UserT, ContextT>
        where UserT : SocketUserBase
        where ContextT : SocketCommandContext
    {
        private static X509Certificate2 ServerCertificate { get; set; }

        private readonly TcpListener _listener;
        private readonly Thread _acceptThread;
        private bool _listening;

        protected IServiceProvider Services { get; private set; }
        protected ISslServerConfig Config { get; private set; }
        protected ICommandService CommandService { get; private set; }
        protected ILogService Logger { get; private set; }

        private Func<LionClient, UserT> _createUserFunc;
        private Func<ICommandService, UserT, Packet, ContextT> _createContextFunc;

        public bool Initialized { get; private set; } = false;

        public event Func<LionClient, Task> LionClientConnected { add { _connectedEvent.Add(value); } remove { _connectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<LionClient, Task>> _connectedEvent;

        public event Func<LionClient, ExitState, Task> LionClientDisconnected { add { _disconnectedEvent.Add(value); } remove { _disconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<LionClient, ExitState, Task>> _disconnectedEvent;

        ///<summary>DI-friendly constructor</summary>
        ///<param name="services">Service provider containing <see cref="ISslServerConfig"/>, <see cref="ICommandService"/> and <see cref="ILogService"/>.</param>
        public LionServer(IServiceProvider services) : this(
            config: services.GetService<ISslServerConfig>(),
            cService: services.GetService<ICommandService>(),
            logger: services.GetService<ILogService>()) { }

        public LionServer(ISslServerConfig config, ICommandService cService, ILogService logger)
        {
            Config = config ?? throw new ArgumentNullException("LionServer requires ISslServerConfig to be included within IServiceProvider.");
            CommandService = cService;
            Logger = logger;

            _connectedEvent = new AsyncEvent<Func<LionClient, Task>>();
            _disconnectedEvent = new AsyncEvent<Func<LionClient, ExitState, Task>>();

            //load in a certificate
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if(!File.Exists(Config.CertFile))
            {
                string file_not_found_ex_msg = $"Certificate was not found at [{Config.CertFile}]";
                Logger?.LogLine(this, file_not_found_ex_msg, LogSeverity.Critical);
                throw new FileNotFoundException(file_not_found_ex_msg);
            }
            ServerCertificate = new X509Certificate2(Config.CertFile, Config.CertPassword);
            Logger?.LogLine(this, "Certificate loaded.");

            //host bind
            string host = Config.Host;
            if (host == "localhost") host = NetworkEssentials.LocalIPv4Address;

            _listener = new TcpListener(IPAddress.Parse(host), Config.Port);
            _acceptThread = new Thread(async () => await Listen(Services));
            _listening = false;
        }

        public void Init(
            Func<LionClient, UserT> user_create_func,
            Func<ICommandService, UserT, Packet, ContextT> context_create_func = null)
        {
            _createUserFunc = user_create_func;
            if (context_create_func == null)
            {
                if(typeof(ContextT) == typeof(SocketCommandContext))
                {
                    context_create_func = (ICommandService cService, UserT user, Packet packet) => (ContextT)new SocketCommandContext(packet);
                    Initialized = true;
                }
                else
                {
                    Logger?.LogLine(
                        this, 
                        "context_create_func failed to initialize, because context_create_fun was not provided and the ContextT type did not match SocketCommandContext type.\n" +
                        "Please specify the context initializer if you don't want the default type - SocketCommandContext.", 
                        LogSeverity.Critical);
                }
            }
            else
            {
                _createContextFunc = context_create_func;
                Initialized = true;
            }
        }

        public void Start(IServiceProvider services)
        {
            if(!Initialized)
            {
                throw new Exception("Cannot start the server because the server failed to or was not initialized.");
            }
            _listening = true;
            Services = services;
            _acceptThread.Start();
        }

        public void Stop()
        {
            _listening = false;
            _acceptThread.Join();
        }

        private async Task Listen(IServiceProvider services)
        {
            _listener.Start();
            Logger?.LogLine(this, $"{_listener.LocalEndpoint} Listening for incoming connections...");

            while (_listening)
            {
                var client = await _listener.AcceptTcpClientAsync();
                new Thread(async () =>
                {
                    try
                    {
                        await HandleClient(client);
                    }
                    catch (Exception e)
                    {
                        Logger?.LogLine(this, e.Message, LogSeverity.Error);
                    }
                }).Start();
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            string endpoint_info = client.Client.RemoteEndPoint.ToString();

            Logger?.LogLine(this, $"{endpoint_info} Connected!", LogSeverity.Verbose);

            // A client has connected. Create the 
            // SslStream using the client's network stream.
            SslStream sslStream = new SslStream(client.GetStream(), false);
            LionClient lionClient = new LionClient(client, sslStream, Logger);
            UserT user = _createUserFunc(lionClient);
            if (user == null)
            {
                Logger?.LogLine(this, "_createUserFunc failed to create new user entity. Check to see if Init properly initializes user create function.", LogSeverity.Critical);
                return;
            }

            ExitState exit_state = ExitState.Requested;

            // Authenticate the server but don't require the client to authenticate.
            try
            {
                sslStream.AuthenticateAsServer(ServerCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);
                lionClient.PacketReceived += (Packet p) => ProcessPacket(user, p);
                lionClient.Start();
                await _connectedEvent?.InvokeAsync(lionClient);
                await lionClient.HoldAsync();
            }
            catch (Exception e)
            {
                if (e is AuthenticationException)
                {
                    Logger?.LogLine(this, $"Exception: {e.Message}", LogSeverity.Error);
                    if (e.InnerException != null)
                    {
                        Logger?.LogLine(this, $"Inner exception: {e.InnerException.Message}", LogSeverity.Error);
                    }
                    Logger?.LogLine(this, $"Authentication failed - closing the connection.", LogSeverity.Error);
                }

                if (user != null)
                {
                    sslStream.Close();
                    client.Close();
                    lionClient.Stop();
                    client.Dispose();
                    exit_state = e is SocketException ? ExitState.Unexpected : ExitState.Forced;
                    return;
                }
            }
            finally
            {
                _disconnectedEvent?.InvokeAsync(lionClient, exit_state);
                Logger?.LogLine(this, $"{endpoint_info} Disconnected!", LogSeverity.Verbose);
            }
        }

        private async Task ProcessPacket(UserT user, Packet packet)
        {
            var cmd = CommandService[packet];

            if (cmd != null)
            {
                try
                {
                    ContextT context = _createContextFunc(CommandService, user, packet);
                    if (context == null)
                    {
                        Logger?.LogLine(this, "_createContextFunc failed to create new context entity. Check to see if Init properly initializes context create function.", LogSeverity.Critical);
                        return;
                    }
                    await CommandService[packet].ExecuteAsync(context);
                }
                catch (Exception ex)
                {
                    user.ReplyMessage(packet, packet.Header, ex.Message, false);
                }
            }
            else
            {
                user.SendPacket(x =>
                {
                    x.Header = packet.Header;
                    x.ReplyId = packet.Id;
                    x.Status = StatusCode.NotFound;
                    x.Content = $"Command {packet.Header} was not found.";
                });
            }
        }

        private static bool IsConnected(Socket s) => !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);

        private static class Info
        {
            private static void DisplaySecurityLevel(SslStream stream)
            {
                Console.WriteLine("Cipher: {0} strength {1}", stream.CipherAlgorithm, stream.CipherStrength);
                Console.WriteLine("Hash: {0} strength {1}", stream.HashAlgorithm, stream.HashStrength);
                Console.WriteLine("Key exchange: {0} strength {1}", stream.KeyExchangeAlgorithm, stream.KeyExchangeStrength);
                Console.WriteLine("Protocol: {0}", stream.SslProtocol);
            }

            private static void DisplaySecurityServices(SslStream stream)
            {
                Console.WriteLine("Is authenticated: {0} as server? {1}", stream.IsAuthenticated, stream.IsServer);
                Console.WriteLine("IsSigned: {0}", stream.IsSigned);
                Console.WriteLine("Is Encrypted: {0}", stream.IsEncrypted);
            }

            private static void DisplayStreamProperties(SslStream stream)
            {
                Console.WriteLine("Can read: {0}, write {1}", stream.CanRead, stream.CanWrite);
                Console.WriteLine("Can timeout: {0}", stream.CanTimeout);
            }

            private static void DisplayCertificateInformation(SslStream stream)
            {
                Console.WriteLine("Certificate revocation list checked: {0}", stream.CheckCertRevocationStatus);

                X509Certificate localCertificate = stream.LocalCertificate;
                if (stream.LocalCertificate != null)
                {
                    Console.WriteLine("Local cert was issued to {0} and is valid from {1} until {2}.",
                        localCertificate.Subject,
                        localCertificate.GetEffectiveDateString(),
                        localCertificate.GetExpirationDateString());
                }
                else
                {
                    Console.WriteLine("Local certificate is null.");
                }
                // Display the properties of the client's certificate.
                X509Certificate remoteCertificate = stream.RemoteCertificate;
                if (stream.RemoteCertificate != null)
                {
                    Console.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                        remoteCertificate.Subject,
                        remoteCertificate.GetEffectiveDateString(),
                        remoteCertificate.GetExpirationDateString());
                }
                else
                {
                    Console.WriteLine("Remote certificate is null.");
                }
            }

            private static void DisplayUsage()
            {
                Console.WriteLine("To start the server specify:");
                Console.WriteLine("serverSync certificateFile.cer");
                Environment.Exit(1);
            }
        }
    }
}
