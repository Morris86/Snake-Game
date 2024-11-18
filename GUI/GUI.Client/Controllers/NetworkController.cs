using CS3500.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using CS3500.NetworkLibrary;
using System.Numerics;

namespace CS3500.NetworkController
{
    public class NetworkController
    {
        private string serverAddress;
        private int port;
        private NetworkConnection? networkConnection;
        public bool IsConnected => networkConnection != null && networkConnection.IsConnected;
        public Action<string> OnError { get; set; }
        public Action<string> OnStatusUpdate { get; set; } // Notify view of status changes
        public Action OnDisconnected { get; set; }
        public Action<Player> OnPlayerUpdate { get; set; }
        public Action<Powerup> OnPowerupUpdate { get; set; }
        public Action<Wall> OnWallUpdate { get; set; }
        public World TheWorld { get; private set; }
        public int playerID { get; private set; }
        private int worldSize;
        private bool receivedInitialData = false;
        private bool playerIDReceived = false; // Track if player ID is received

        public NetworkController(string serverAddress, int port)
        {
            this.serverAddress = serverAddress;
            this.port = port;
            networkConnection = new NetworkConnection(); // Only initialize the connection
        }


        public void ConnectToServer(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                OnError?.Invoke("Player name is required!");
                return;
            }

            if (playerName.Length > 16)
            {
                OnError?.Invoke("Player name must be 16 characters or less.");
                return;
            }

            try
            {
                if (!networkConnection.IsConnected)
                {
                    networkConnection.Connect(serverAddress, port);
                }

                networkConnection.Send(playerName);
                OnStatusUpdate?.Invoke($"Connected to server as {playerName}.");

                new Thread(StartReceivingData).Start(); // Start receiving data asynchronously
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to connect: {ex.Message}");
            }
        }

        public void DisconnectFromServer()
        {
            try
            {
                if (networkConnection != null && networkConnection.IsConnected)
                {
                    networkConnection.Disconnect();
                }

                // Cleanup resources
                networkConnection = null;
                TheWorld = null;

                // Notify the status update
                OnStatusUpdate?.Invoke("Disconnected from the server.");
                OnDisconnected?.Invoke();
            }
            catch (Exception ex)
            {
                OnStatusUpdate?.Invoke("Disconnected from the server.");
            }
        }

        private void StartReceivingData()
        {
            try
            {
                while (networkConnection != null && networkConnection.IsConnected)
                {
                    string data = networkConnection.ReadLine();
                    if (data != null)
                    {
                        ProcessServerData(data);
                    }
                }
            }
            catch (Exception ex)
            {
                if (networkConnection != null && networkConnection.IsConnected)
                {
                    OnError?.Invoke($"Network error: {ex.Message}");
                }
            }
        }

        private void ProcessServerData(string data)
        {
            // If we haven't received the initial integer data fully, handle it
            if (!receivedInitialData)
            {
                if (int.TryParse(data, out int parsedValue))
                {
                    if (!playerIDReceived)
                    {
                        // First integer from the server is treated as the player ID
                        playerID = parsedValue;
                        playerIDReceived = true;
                        Console.WriteLine($"Player ID received from server: {playerID}");
                    }
                    else
                    {
                        // Second integer is the world size
                        worldSize = parsedValue;
                        TheWorld = new World(worldSize);
                        receivedInitialData = true; // Now we have both initial values
                        Console.WriteLine($"World size received and set: {worldSize}");
                        Console.WriteLine($"TheWorld initialized in NetworkController? {TheWorld != null}");

                        // Now that TheWorld is initialized, set the update actions
                        OnPlayerUpdate = player => TheWorld.UpdatePlayer(player);
                        OnPowerupUpdate = powerup => TheWorld.UpdatePowerup(powerup);
                    }
                }
            }
            else
            {
                try
                {
                    // Detect and process each type of JSON object correctly
                    if (data.Contains("\"snake\""))
                    {
                        var player = JsonSerializer.Deserialize<Player>(data);
                        if (player != null)
                        {
                            if (player.Disconnected)
                            {
                                // Remove the player from the world if they disconnected
                                TheWorld.RemovePlayer(player.ID);
                            }
                            else
                            {
                                // Otherwise, update or add the player normally
                                TheWorld.UpdatePlayer(player);
                                OnPlayerUpdate?.Invoke(player); // Notify view
                            }
                        }
                    }
                    else if (data.Contains("\"power\""))
                    {
                        var powerup = JsonSerializer.Deserialize<Powerup>(data);
                        if (powerup != null)
                        {
                            //Console.WriteLine($"Powerup JSON detected. Updating powerup ID: {powerup.ID}, Location: {powerup.Position}");
                            if (powerup.Died) // If the power-up is marked as "died" by the server, it should be removed
                            {
                                //Console.WriteLine($"Powerup with ID {powerup.ID} has been consumed. Removing from game.");
                                TheWorld.RemovePowerup(powerup.ID); // Remove it from the game's state
                            }
                            else
                            {
                                TheWorld.UpdatePowerup(powerup); // Otherwise, just update its position
                                OnPowerupUpdate?.Invoke(powerup);    // Notify the view, if necessary
                            }
                        }
                    }
                    else if (data.Contains("\"wall\""))
                    {
                        var wall = JsonSerializer.Deserialize<Wall>(data);
                        if (wall != null)
                        {
                            TheWorld.AddWall(wall);
                            OnWallUpdate?.Invoke(wall); // Trigger an update for the UI to redraw with the new wall data.
                        }
                    }
                    else
                    {
                        // Log unrecognized data only once
                        Console.WriteLine("Unrecognized data received: " + data);
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON parsing error: {ex.Message} - Data: {data}");
                }
            }
        }

        public void SendMoveCommand(string direction)
        {
            var command = JsonSerializer.Serialize(new { moving = direction });
            networkConnection.Send(command);
        }

        //public void Close()
        //{
        //    networkConnection?.Disconnect();
        //}
    }
}
