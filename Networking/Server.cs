// <copyright file="Server.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Anurup Kumar and Morris Lu for CS3500, Fall 2024

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace CS3500.Networking;

/// <summary>
///   Represents a server task that waits for connections on a given
///   port and calls the provided delegate when a connection is made.
/// </summary>
public static class Server
{

    /// <summary>
    ///   Wait on a TcpListener for new connections. Alert the main program
    ///   via a callback (delegate) mechanism.
    /// </summary>
    /// <param name="handleConnect">
    ///   Handler for what the user wants to do when a connection is made.
    ///   This should be run asynchronously via a new thread.
    /// </param>
    /// <param name="port"> The port (e.g., 11000) to listen on. </param>
    public static void StartServer( Action<NetworkConnection> handleConnect, int port )
    {
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Server started on port {port}.");

        while (true)
        {
            try
            {
                TcpClient client = listener.AcceptTcpClient();
                NetworkConnection connection = new NetworkConnection(client);

                // Start a new thread for each client connection
                new Thread(() => handleConnect(connection)).Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server encountered an error: {ex.Message}");
            }
        }
    }
}
