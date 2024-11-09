// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Anurup Kumar and Morris Lu for CS3500, Fall 2024

using CS3500.Networking;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CS3500.ChatServer;

/// <summary>
/// A simple ChatServer that handles multiple clients and broadcasts messages.
/// The server allows clients to join, broadcast messages to all clients, 
/// and manages disconnections. It also identifies each client by their 
/// initial message, which is treated as the client name.
/// </summary>
public partial class ChatServer
{
    /// <summary>
    /// Holds a list of all active client connections.
    /// </summary>
    private static List<NetworkConnection> clientConnections = new List<NetworkConnection>();

    /// <summary>
    /// Lock object to synchronize access to clientConnections.
    /// </summary>
    private static readonly object clientLock = new object();

    /// <summary>
    /// Entry point for the ChatServer application.
    /// Starts the server on a specified port and waits for client connections.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    private static void Main(string[] args)
    {
        Server.StartServer(HandleConnect, 11000);
        Console.ReadLine(); // Keeps the server running
    }

    /// <summary>
    /// Handles a new client connection, reads the client's name, 
    /// and continuously processes incoming messages from the client.
    /// </summary>
    /// <param name="connection">The client's network connection.</param>
    private static void HandleConnect(NetworkConnection connection)
    {
        Console.WriteLine("New client connected.");
        try
        {
            // First message is the client's name
            string? clientName = connection.ReadLine();
            if (clientName == null)
            {
                connection.Disconnect();
                return;
            }

            lock (clientLock)
            {
                clientConnections.Add(connection);
            }
            Console.WriteLine($"{clientName} joined the chat.");
            BroadcastMessage($"{clientName} has joined the chat.");

            // Process client messages
            while (true)
            {
                string? message = connection.ReadLine();
                if (message == null) break;

                Console.WriteLine($"Received from {clientName}: {message}");
                BroadcastMessage($"{clientName}: {message}");
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Client disconnected.");
        }
        finally
        {
            lock (clientLock)
            {
                clientConnections.Remove(connection);
            }
            connection.Disconnect();
        }
    }

    /// <summary>
    /// Broadcasts a message to all connected clients.
    /// </summary>
    /// <param name="message">The message to broadcast.</param>
    private static void BroadcastMessage(string message)
    {
        lock (clientLock)
        {
            foreach (var client in clientConnections)
            {
                try
                {
                    client.Send(message);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error sending message to a client.");
                }
            }
        }
    }
}

