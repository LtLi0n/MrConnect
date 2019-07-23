using LionLibrary.Commands;
using LionLibrary.Framework;
using LionLibrary.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using WarsOfTanoth.Boot;
using WarsOfTanoth.Modules;
using WarsOfTanoth.Network;

namespace WarsOfTanoth.Services
{
    public class SslTcpServer : ISslTcpServer
    {
        private static X509Certificate2 ServerCertificate { get; set; }

        private readonly TcpListener _listener;
        private readonly Thread _acceptThread;
        private bool _listening;

        private IServiceProvider _services;
        private readonly IAppConfig _config;
        private readonly ICommandService _cService;
        private readonly ILogService _logger;

        public SslTcpServer(IAppConfig config, ICommandService cService, ILogService logger)
        {
            _config = config;
            _cService = cService;
            _logger = logger;

            //load in a certificate
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            /*X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection coll = store.Certificates.Find(X509FindType.FindBySubjectName, config["server:cert_sn"], true);
            byte[] cert_data = coll.Export(X509ContentType.Pkcs12, config["server:cert_pwd"]);*/
            ServerCertificate = new X509Certificate2(config["server:cert_file"], config["server:cert_pwd"]);
            _logger.LogLine(this, "Certificate loaded.");

            //local ip
            if (config["server:host"] == "localhost") config["server:host"] = NetworkEssentials.LocalIPv4Address;

            _listener = new TcpListener(IPAddress.Parse(config["server:host"]), int.Parse(config["server:port"]));
            _acceptThread = new Thread(async () => await Listen(_services));
            _listening = false;
        }

        public void Start(IServiceProvider services)
        {
            _listening = true;
            _services = services;
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
            _logger.LogLine(this, $"{_listener.LocalEndpoint} Listening for incoming connections...");

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
                        Console.WriteLine(e.Message);
                    }
                }).Start();
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            string endpoint_info = client.Client.RemoteEndPoint.ToString();

            _logger.LogLine(this, $"{endpoint_info} Connected!", LogSeverity.Verbose);

            // A client has connected. Create the 
            // SslStream using the client's network stream.
            SslStream sslStream = new SslStream(client.GetStream(), false);
            LionClient lionClient = new LionClient(client, sslStream, _logger);
            SocketUser user = new SocketUser(lionClient);

            // Authenticate the server but don't require the client to authenticate.
            try
            {
                sslStream.AuthenticateAsServer(ServerCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);
                lionClient.PacketReceived += (Packet p) => ProcessPacket(user, p);
                lionClient.Start();

                await lionClient.HoldAsync();
            }
            catch (Exception e)
            {
                if (e is AuthenticationException)
                {
                    _logger.LogLine(this, $"Exception: {e.Message}", LogSeverity.Error);
                    if (e.InnerException != null)
                    {
                        _logger.LogLine(this, $"Inner exception: {e.InnerException.Message}", LogSeverity.Error);
                    }
                    _logger.LogLine(this, $"Authentication failed - closing the connection.", LogSeverity.Error);
                }

                if (user != null)
                {
                    sslStream.Close();
                    client.Close();
                    lionClient.Stop();
                    client.Dispose();
                    return;
                }
            }
            finally
            {
                //close the connection.
                //_users.Logout(lionClient);
                _logger.LogLine(this, $"{endpoint_info} Disconnected!", LogSeverity.Verbose);
            }
        }

        private async Task ProcessPacket(SocketUser user, Packet packet)
        {
            if (_cService.HasCommand(packet))
            {
                try
                {
                    await _cService[packet].ExecuteAsync(new CustomCommandContext(_cService, user, packet));
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
                    x.State = 404;
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
