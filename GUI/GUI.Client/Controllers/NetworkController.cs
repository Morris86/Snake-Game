using CS3500.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using CS3500.NetworkLibrary;

namespace CS3500.NetworkController
{
    public class NetworkController
    {
        private NetworkConnection networkConnection;
        public Action<string> OnError { get; set; }
        public Action<Player> OnPlayerUpdate { get; set; }
        public Action<Powerup> OnPowerupUpdate { get; set; }

        private bool receivedInitialData = false;
        private int playerID;
        private int worldSize;

        public NetworkController(string serverAddress, int port)
        {
            try
            {
                networkConnection = new NetworkConnection();
                networkConnection.Connect(serverAddress, port);
                // Start receiving data in a separate thread to avoid blocking
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
                }
            }
        }

        private void ProcessServerData(string data)
        {
            if (!receivedInitialData)
            {
                if (int.TryParse(data, out int value))
                {
                    if (playerID == 0)
                    {
                        playerID = value;
                    }
                    else
                    {
                        worldSize = value;
                        receivedInitialData = true;
                    }
                }
            }
            else
            {
                if (data.Contains("\"snake\""))
                {
                    var player = JsonSerializer.Deserialize<Player>(data);
                    OnPlayerUpdate?.Invoke(player);
                }
                else if (data.Contains("\"power\""))
                {
                    var powerup = JsonSerializer.Deserialize<Powerup>(data);
                    OnPowerupUpdate?.Invoke(powerup);
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
