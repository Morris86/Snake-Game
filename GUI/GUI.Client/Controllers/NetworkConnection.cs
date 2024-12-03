// <copyright file="NetworkConnection.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Anurup Kumar and Morris Lu for CS3500, Fall 2024

using System.Net.Sockets;
using System.Text;

namespace CS3500.NetworkLibrary
{
    /// <summary>
    /// Wraps the StreamReader, StreamWriter, and TcpClient together to provide
    /// a unified interface for network actions.
    /// </summary>
    public sealed class NetworkConnection : IDisposable
    {
        /// <summary>
        /// Represents the underlying TCP client used for the connection.
        /// </summary>
        private TcpClient _tcpClient = new();

        /// <summary>
        /// The reader for incoming messages.
        /// </summary>
        private StreamReader? _reader = null;

        /// <summary>
        /// The writer for outgoing messages.
        /// </summary>
        private StreamWriter? _writer = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkConnection"/> class
        /// with an existing TCP client.
        /// </summary>
        /// <param name="tcpClient">An already connected TcpClient.</param>
        public NetworkConnection(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            if (IsConnected)
            {
                _writer = new StreamWriter(tcpClient.GetStream(), new UTF8Encoding(false)) { AutoFlush = true };
                _reader = new StreamReader(tcpClient.GetStream(), new UTF8Encoding(false));

            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkConnection"/> class.
        /// Creates a new, unconnected TcpClient.
        /// </summary>
        public NetworkConnection() : this(new TcpClient())
        {
        }

        /// <summary>
        /// Gets a value indicating whether the connection is currently active.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                try
                {
                    return _tcpClient != null && _tcpClient.Connected && _tcpClient.GetStream().CanRead;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Connects to a remote host and port.
        /// </summary>
        /// <param name="host">The remote host's URL or IP address.</param>
        /// <param name="port">The port number to connect to.</param>
        /// <exception cref="InvalidOperationException">Thrown if already connected.</exception>
        public void Connect(string host, int port)
        {
            if (!_tcpClient.Connected)
            {
                _tcpClient.Connect(host, port);
                _reader = new StreamReader(_tcpClient.GetStream(), Encoding.UTF8);
                _writer = new StreamWriter(_tcpClient.GetStream(), Encoding.UTF8) { AutoFlush = true };
            }
            else
            {
                throw new InvalidOperationException("Already connected.");
            }
        }

        /// <summary>
        /// Sends a message to the remote endpoint. Appends a newline character to
        /// treat it as a complete message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <exception cref="InvalidOperationException">Thrown if not connected.</exception>
        public void Send(string message)
        {
            if (_writer == null)
            {
                throw new InvalidOperationException("Not connected to a remote endpoint.");
            }
            _writer.WriteLine(message);
            _writer.Flush();
        }

        /// <summary>
        /// Reads a message from the remote endpoint up to the first newline.
        /// </summary>
        /// <returns>The message received from the remote endpoint.</returns>
        /// <exception cref="InvalidOperationException">Thrown if not connected.</exception>
        /// <exception cref="IOException">Thrown if the connection is closed by the remote host.</exception>
        public string ReadLine()
        {
            if (_reader == null)
            {
                throw new InvalidOperationException("Not connected to a remote endpoint.");
            }
            return _reader.ReadLine() ?? throw new IOException("The connection was closed by the remote host.");
        }

        /// <summary>
        /// Disconnects the connection and disposes of the resources.
        /// </summary>
        public void Disconnect()
        {
            _reader?.Dispose();
            _writer?.Dispose();
            _tcpClient.Close();
        }

        /// <summary>
        /// Releases all resources used by the NetworkConnection.
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }
    }
}
