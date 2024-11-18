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
                ProcessInitialData(data);
                return;
            }

            try
            {
                if (data.Contains("\"snake\""))
                {
                    ProcessSnakeData(data);
                }
                else if (data.Contains("\"power\""))
                {
                    ProcessPowerupData(data);
                }
                else if (data.Contains("\"wall\""))
                {
                    ProcessWallData(data);
                }
                else
                {
                    // Inline handling of unrecognized data
                    Console.WriteLine("Unrecognized data received: " + data);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON parsing error: {ex.Message} - Data: {data}");
            }
        }


        private void ProcessInitialData(string data)
        {
            if (int.TryParse(data, out int parsedValue))
            {
                if (!playerIDReceived)
                {
                    playerID = parsedValue;
                    playerIDReceived = true;
                    Console.WriteLine($"Player ID received from server: {playerID}");
                }
                else
                {
                    worldSize = parsedValue;
                    TheWorld = new World(worldSize);
                    receivedInitialData = true;
                    Console.WriteLine($"World size received and set: {worldSize}");
                    Console.WriteLine($"TheWorld initialized in NetworkController? {TheWorld != null}");

                    // Set the update actions for players and powerups
                    OnPlayerUpdate = player => TheWorld.UpdatePlayer(player);
                    OnPowerupUpdate = powerup => TheWorld.UpdatePowerup(powerup);
                }
            }
        }

        private void ProcessSnakeData(string data)
        {
            var player = JsonSerializer.Deserialize<Player>(data);
            if (player != null)
            {
                if (player.Disconnected)
                {
                    TheWorld.RemovePlayer(player.ID);
                }
                else
                {
                    TheWorld.UpdatePlayer(player);
                    OnPlayerUpdate?.Invoke(player);
                }
            }
        }

        private void ProcessPowerupData(string data)
        {
            var powerup = JsonSerializer.Deserialize<Powerup>(data);
            if (powerup != null)
            {
                if (powerup.Died)
                {
                    TheWorld.RemovePowerup(powerup.ID);
                }
                else
                {
                    TheWorld.UpdatePowerup(powerup);
                    OnPowerupUpdate?.Invoke(powerup);
                }
            }
        }

        private void ProcessWallData(string data)
        {
            var wall = JsonSerializer.Deserialize<Wall>(data);
            if (wall != null)
            {
                TheWorld.AddWall(wall);
                OnWallUpdate?.Invoke(wall);
            }
        }



        public void SendMoveCommand(string direction)
        {
            var command = JsonSerializer.Serialize(new { moving = direction });
            networkConnection.Send(command);
        }

    }
}
