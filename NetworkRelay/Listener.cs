// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Listener.cs" company="Allberg Konsult AB">
//   Allberg Konsult AB
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Allberg.NetworkRelay
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// The listener.
    /// </summary>
    internal class Listener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Listener"/> class.
        /// </summary>
        /// <param name="hostname">
        /// The hostname.
        /// </param>
        /// <param name="port">
        /// The port.
        /// </param>
        public Listener(string hostname, int port)
        {
            this.Hostname = hostname;
            this.Port = port;
        }

        /// <summary>
        /// Gets the hostname.
        /// </summary>
        public string Hostname { get; private set; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Start to listen. This blocks until finished.
        /// </summary>
        public void Listen()
        {
            Console.WriteLine("Starting to listen on port {0}", this.Port);

            var listener = new TcpListener(IPAddress.Parse("0.0.0.0"), this.Port);
            listener.Start();

            while (true)
            {
                // This call hangs until a connection is done
                var client = listener.AcceptTcpClient();

                var connectionThread = new Thread(this.HandleConnection) { IsBackground = true };
                connectionThread.Start(client);
            }
        }

        /// <summary>
        /// Handle the incoming connection.
        /// </summary>
        /// <param name="connectionObject">
        /// The connection object.
        /// </param>
        private void HandleConnection(object connectionObject)
        {
            try
            {
                var connection = connectionObject as TcpClient;
                if (connection == null)
                {
                    Console.WriteLine("WARNING! Connection is null.");
                    return;
                }

                Console.WriteLine("Connection accepted from " + connection.Client.RemoteEndPoint + " on port " + this.Port);

                using (var client = new TcpClient())
                {
                    client.Connect(this.Hostname, this.Port);

                    var sourceStream = connection.GetStream();
                    var clientStream = client.GetStream();

                    while (connection.Connected && client.Connected)
                    {
                        this.Transfer(sourceStream, clientStream);
                        this.Transfer(clientStream, sourceStream);

                        while (connection.Available == 0 && client.Available == 0 && connection.Connected && client.Connected && connection.Client.Connected && client.Client.Connected)
                        {
                            Thread.Sleep(100);
                        }
                    }

                    client.Client.Disconnect(false);
                    connection.Client.Disconnect(false);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        /// Transfer data.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        private void Transfer(TcpClient source, TcpClient target)
        {
            var available = source.Available;
            if (available == 0)
            {
                return;
            }

            var toRead = new byte[available];
            var sourceStream = source.GetStream();
            sourceStream.Read(toRead, 0, toRead.Length);

            var targetStream = target.GetStream();
            targetStream.Write(toRead, 0, toRead.Length);
            targetStream.Flush();
        }

        private void Transfer(NetworkStream source, NetworkStream target)
        {
            if (!source.DataAvailable)
            {
                return;
            }

            var buffer = new byte[255];
            var read = source.Read(buffer, 0, buffer.Length);

            // Console.WriteLine("Read {0} bytes from stream", read);

            target.Write(buffer, 0, read);
        }
    }
}
