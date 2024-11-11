using CS3500.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using CS3500.NetworkLibrary;

namespace CS3500.NetworkController
{
    public class NetworkController
    {
        private readonly NetworkConnection networkConnection;
        public Action<string> OnError { get; set; }
        public Action<Player> OnPlayerUpdate { get; set; }
        public Action<Powerup> OnPowerupUpdate { get; set; }

        private int playerID;
        private int worldSize;
        private World TheWorld; // Ensure this is initialized after server handshake
        private bool receivedInitialData = false;
        private bool playerIDReceived = false; // Track if player ID is received

        public NetworkController(string serverAddress, int port)
        {
            try
            {
                networkConnection = new NetworkConnection();
                networkConnection.Connect(serverAddress, port);

                // Start receiving data asynchronously
                new Thread(StartReceivingData).Start();
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to connect: {ex.Message}");
            }
        }

        public void SendPlayerName(string name)
        {
            if (name.Length <= 16)
            {
                networkConnection.Send(name);
            }
            else
            {
                OnError?.Invoke("Player name must be 16 characters or less.");
            }
        }

        private void StartReceivingData()
        {
            while (networkConnection.IsConnected)
            {
                try
                {
                    string data = networkConnection.ReadLine();
                    ProcessServerData(data);
                }
                catch (Exception ex)
                {
                    OnError?.Invoke($"Network error: {ex.Message}");
                    break;
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

                        // Now that TheWorld is initialized, set the update actions
                        OnPlayerUpdate = player => TheWorld.UpdatePlayer(player);
                        OnPowerupUpdate = powerup => TheWorld.UpdatePowerup(powerup);
                    }
                }
            }
            else
            {
                // Handle JSON game entities (snakes, powerups) as usual
                if (data.Contains("\"snake\""))
                {
                    var player = JsonSerializer.Deserialize<Player>(data);
                    if (player != null)
                    {
                        TheWorld.UpdatePlayer(player);
                        OnPlayerUpdate?.Invoke(player); // Notify view
                    }
                }
                else if (data.Contains("\"power\""))
                {
                    var powerup = JsonSerializer.Deserialize<Powerup>(data);
                    if (powerup != null)
                    {
                        TheWorld.UpdatePowerup(powerup);
                        OnPowerupUpdate?.Invoke(powerup); // Notify view
                    }
                }
            }
        }

        public void SendMoveCommand(string direction)
        {
            var command = JsonSerializer.Serialize(new { moving = direction });
            networkConnection.Send(command);
        }

        public void Close()
        {
            networkConnection?.Disconnect();
        }
    }
}
