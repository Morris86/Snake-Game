// Author: Morris Lu, Anurup Kumar Fall 2024

using CS3500.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using CS3500.NetworkLibrary;
using System.Numerics;
using MySql.Data.MySqlClient;

namespace CS3500.NetworkController
{
    /// <summary>
    /// Handles the network communication with the Snake server and updates the game state accordingly.
    /// </summary>
    public class NetworkController
    {
        /// <summary>
        /// Manages the connection to the server.
        /// </summary>
        private NetworkConnection? networkConnection;

        /// <summary>
        /// Indicates whether the client is currently connected to the server.
        /// </summary>
        public bool IsConnected => networkConnection != null && networkConnection.IsConnected;

        /// <summary>
        /// Invoked when an error occurs during network communication.
        /// </summary>
        public Action<string> OnError { get; set; }

        /// <summary>
        /// Invoked to notify the view of status changes.
        /// </summary>
        public Action<string> OnStatusUpdate { get; set; }

        /// <summary>
        /// Invoked when the client disconnects from the server.
        /// </summary>
        public Action OnDisconnected { get; set; }

        /// <summary>
        /// Invoked when a player's data is updated.
        /// </summary>
        public Action<Player> OnPlayerUpdate { get; set; }

        /// <summary>
        /// Invoked when a powerup's data is updated.
        /// </summary>
        public Action<Powerup> OnPowerupUpdate { get; set; }

        /// <summary>
        /// The server's IP address or hostname.
        /// </summary>
        private readonly string serverAddress;

        /// <summary>
        /// The port number for connecting to the server.
        /// </summary>
        private readonly int port;

        /// <summary>
        /// Represents the game world, including players, walls, and powerups.
        /// </summary>
        public World? TheWorld { get; private set; }

        /// <summary>
        /// The unique player ID assigned by the server.
        /// </summary>
        public int playerID { get; private set; }

        /// <summary>
        /// The size of the game world (width and height).
        /// </summary>
        private int worldSize;

        /// <summary>
        /// Indicates whether the initial data has been received from the server.
        /// </summary>
        private bool receivedInitialData = false;

        /// <summary>
        /// Indicates whether the player ID has been received from the server.
        /// </summary>
        private bool playerIDReceived = false;

        /// <summary>
        /// Lock object to synchronize access
        /// </summary>
        private readonly object networkLock = new object();

        private int currentGameId; // Tracks the current game ID
        private DateTime gameStartTime; // Tracks the game start time
        private Dictionary<int, Player> trackedPlayers = new Dictionary<int, Player>(); // Tracks player data by ID
        private Dictionary<int, int> playerMaxScores = new Dictionary<int, int>(); // Tracks max scores for players
        private Dictionary<int, DateTime> playerEnterTimes = new Dictionary<int, DateTime>(); // Tracks player enter times
        private Dictionary<int, DateTime?> playerLeaveTimes = new Dictionary<int, DateTime?>(); // Tracks player leave times



        /// <summary>
        /// Initializes a new instance of the NetworkController class with the specified server address and port.
        /// </summary>
        /// <param name="serverAddress">The server's IP address or hostname.</param>
        /// <param name="port">The port number for the connection.</param>
        public NetworkController(string serverAddress, int port)
        {
            this.serverAddress = serverAddress;
            this.port = port;
            networkConnection = new NetworkConnection();

            // Initialize the action properties
            OnError = _ => { };
            OnStatusUpdate = _ => { };
            OnDisconnected = () => { };
            OnPlayerUpdate = _ => { };
            OnPowerupUpdate = _ => { };
        }

        /// <summary>
        /// Connects to the server and sends the player's name.
        /// </summary>
        /// <param name="playerName">The name of the player.</param>
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
                if (networkConnection != null && networkConnection.IsConnected)
                {
                    OnError?.Invoke("Already connected to the server.");
                    return;
                }

                // Initialize or reinitialize network connection
                networkConnection = new NetworkConnection();
                networkConnection.Connect(serverAddress, port);

                // Initialize game tracking
                StartGame();

                // Reset TheWorld and related states
                TheWorld = null;
                receivedInitialData = false;
                playerIDReceived = false;

                networkConnection.Send(playerName);
                OnStatusUpdate?.Invoke($"Connected to server as {playerName}.");

                // Start the receiving thread
                new Thread(StartReceivingData).Start();
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to connect: {ex.Message}");
            }
        }

        /// <summary>
        /// Disconnects from the server and cleans up resources.
        /// </summary>
        public void DisconnectFromServer()
        {
            try
            {
                lock (networkLock)
                {
                    // Safely disconnect the network connection
                    if (networkConnection != null && networkConnection.IsConnected)
                    {
                        networkConnection.Disconnect();

                        // Ensure game tracking is finalized
                        EndGame();
                    }

                    // Cleanup resources
                    networkConnection = null;
                    TheWorld = null;
                }

                // Notify the status update outside the lock to avoid blocking
                OnStatusUpdate?.Invoke("Disconnected from the server.");
                OnDisconnected?.Invoke();
            }
            catch (Exception)
            {
                OnStatusUpdate?.Invoke("Disconnected from the server.");
            }
        }

        /// <summary>
        /// Starts receiving data from the server asynchronously.
        /// </summary>
        private void StartReceivingData()
        {
            try
            {
                while (true)
                {
                    string? data = null;

                    // Lock around both the condition and ReadLine
                    lock (networkLock)
                    {
                        if (networkConnection == null || !networkConnection.IsConnected)
                        {
                            break; // Exit the loop if the connection is closed
                        }

                        try
                        {
                            data = networkConnection.ReadLine();
                        }
                        catch (IOException ex)
                        {
                            // Handle when the server connection is abruptly closed
                            OnError?.Invoke($"Connection lost: {ex.Message}");
                            DisconnectFromServer(); // Trigger disconnection cleanup
                            return;
                        }
                    }

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


        /// <summary>
        /// Processes data received from the server.
        /// </summary>
        /// <param name="data">The data received from the server.</param>
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

        /// <summary>
        /// Processes initial data received from the server (e.g., player ID and world size).
        /// </summary>
        /// <param name="data">The initial data string.</param>
        private void ProcessInitialData(string data)
        {
            if (int.TryParse(data, out int parsedValue))
            {
                if (!playerIDReceived)
                {
                    playerID = parsedValue;
                    playerIDReceived = true;
                }
                else
                {
                    worldSize = parsedValue;
                    TheWorld = new World(worldSize);
                    receivedInitialData = true;

                    // Set the update actions for players and powerups
                    OnPlayerUpdate = player => TheWorld?.UpdatePlayer(player);
                    OnPowerupUpdate = powerup => TheWorld?.UpdatePowerup(powerup);
                }
            }
        }

        /// <summary>
        /// Processes snake data received from the server.
        /// </summary>
        /// <param name="data">The JSON string representing a snake.</param>
        private void ProcessSnakeData(string data)
        {
            var player = JsonSerializer.Deserialize<Player>(data);
            if (player != null)
            {
                if (player.Disconnected)
                {
                    // Handle player disconnection
                    TrackPlayerDisconnection(player);
                    TheWorld?.RemovePlayer(player.ID);
                }
                else
                {
                    // Handle new or updated player
                    if (!trackedPlayers.ContainsKey(player.ID))
                    {
                        TrackNewPlayer(player);
                    }
                    else
                    {
                        UpdatePlayerScore(player);
                    }

                    // Update the world and notify listeners
                    TheWorld?.UpdatePlayer(player);
                    OnPlayerUpdate?.Invoke(player);
                }
            }
        }

        /// <summary>
        /// Processes powerup data received from the server.
        /// </summary>
        /// <param name="data">The JSON string representing a powerup.</param>
        private void ProcessPowerupData(string data)
        {
            var powerup = JsonSerializer.Deserialize<Powerup>(data);
            if (powerup != null)
            {
                if (powerup.Died)
                {
                    TheWorld?.RemovePowerup(powerup.ID);
                }
                else
                {
                    TheWorld?.UpdatePowerup(powerup);
                    OnPowerupUpdate?.Invoke(powerup);
                }
            }
        }

        /// <summary>
        /// Processes wall data received from the server.
        /// </summary>
        /// <param name="data">The JSON string representing a wall.</param>
        private void ProcessWallData(string data)
        {
            var wall = JsonSerializer.Deserialize<Wall>(data);
            if (wall != null)
            {
                TheWorld?.AddWall(wall);
            }
        }

        /// <summary>
        /// Sends a move command to the server indicating the player's direction.
        /// </summary>
        /// <param name="direction">The desired direction ("up", "down", "left", "right").</param>
        public void SendMoveCommand(string direction)
        {
            var command = JsonSerializer.Serialize(new { moving = direction });
            networkConnection?.Send(command);
        }

        /// <summary>
        /// The connection string.
        /// </summary>
        private const string connectionString = "server=atr.eng.utah.edu;" +
                                                "database=u1495290;" +
                                                "uid=u1495290;" +
                                                "password=BugSquashers;";


        public void StartGame()
        {
            gameStartTime = DateTime.Now;
            string startTimeStr = gameStartTime.ToString("yyyy-MM-dd HH:mm:ss");

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Create and configure the command
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "INSERT INTO Games (StartTime) VALUES (@StartTime); SELECT LAST_INSERT_ID();";
                command.Parameters.AddWithValue("@StartTime", startTimeStr);

                // Execute and retrieve the game ID
                currentGameId = Convert.ToInt32(command.ExecuteScalar());
            }

            Console.WriteLine($"Game started with ID: {currentGameId}");
        }

        private void TrackNewPlayer(Player player)
        {
            if (!trackedPlayers.ContainsKey(player.ID))
            {
                trackedPlayers[player.ID] = player;
                playerEnterTimes[player.ID] = DateTime.Now;
                playerMaxScores[player.ID] = player.Score;

                string enterTimeStr = playerEnterTimes[player.ID].ToString("yyyy-MM-dd HH:mm:ss");
                string playerName = player.Name?.Replace("'", "''") ?? "Unknown";

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Create and configure the command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "INSERT INTO Players (ID, Name, MaxScore, EnterTime, GameID) " +
                                          "VALUES (@ID, @Name, @MaxScore, @EnterTime, @GameID);";
                    command.Parameters.AddWithValue("@ID", player.ID);
                    command.Parameters.AddWithValue("@Name", playerName);
                    command.Parameters.AddWithValue("@MaxScore", player.Score);
                    command.Parameters.AddWithValue("@EnterTime", enterTimeStr);
                    command.Parameters.AddWithValue("@GameID", currentGameId);

                    // Execute the command
                    command.ExecuteNonQuery();
                }

                Console.WriteLine($"Player {playerName} (ID: {player.ID}) added to game {currentGameId}");
            }
        }

        private void UpdatePlayerScore(Player player)
        {
            if (trackedPlayers.ContainsKey(player.ID) && player.Score > playerMaxScores[player.ID])
            {
                playerMaxScores[player.ID] = player.Score;

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Create and configure the command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "UPDATE Players SET MaxScore = @MaxScore " +
                                          "WHERE ID = @ID AND GameID = @GameID;";
                    command.Parameters.AddWithValue("@MaxScore", player.Score);
                    command.Parameters.AddWithValue("@ID", player.ID);
                    command.Parameters.AddWithValue("@GameID", currentGameId);

                    // Execute the command
                    command.ExecuteNonQuery();
                }

                Console.WriteLine($"Player (ID: {player.ID}) score updated to {player.Score}");
            }
        }

        private void TrackPlayerDisconnection(Player player)
        {
            if (trackedPlayers.ContainsKey(player.ID) && !playerLeaveTimes.ContainsKey(player.ID))
            {
                playerLeaveTimes[player.ID] = DateTime.Now;
                string leaveTimeStr = playerLeaveTimes[player.ID]?.ToString("yyyy-MM-dd HH:mm:ss");

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Create and configure the command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "UPDATE Players SET LeaveTime = @LeaveTime " +
                                          "WHERE ID = @ID AND GameID = @GameID;";
                    command.Parameters.AddWithValue("@LeaveTime", leaveTimeStr);
                    command.Parameters.AddWithValue("@ID", player.ID);
                    command.Parameters.AddWithValue("@GameID", currentGameId);

                    // Execute the command
                    command.ExecuteNonQuery();
                }

                Console.WriteLine($"Player (ID: {player.ID}) disconnected at {leaveTimeStr}");
            }
        }

        private void EndGame()
        {
            string endTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Update the game's end time
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "UPDATE Games SET EndTime = @EndTime WHERE ID = @ID;";
                command.Parameters.AddWithValue("@EndTime", endTimeStr);
                command.Parameters.AddWithValue("@ID", currentGameId);
                command.ExecuteNonQuery();

                // Ensure the main player's LeaveTime matches the game's EndTime
                if (trackedPlayers.ContainsKey(playerID))
                {
                    MySqlCommand updatePlayerCommand = conn.CreateCommand();
                    updatePlayerCommand.CommandText = "UPDATE Players SET LeaveTime = @LeaveTime " +
                                                      "WHERE ID = @PlayerID AND GameID = @GameID;";
                    updatePlayerCommand.Parameters.AddWithValue("@LeaveTime", endTimeStr);
                    updatePlayerCommand.Parameters.AddWithValue("@PlayerID", playerID);
                    updatePlayerCommand.Parameters.AddWithValue("@GameID", currentGameId);
                    updatePlayerCommand.ExecuteNonQuery();

                    Console.WriteLine($"Main player (ID: {playerID}) leave time updated to {endTimeStr}");
                }
            }

            Console.WriteLine($"Game {currentGameId} ended at {endTimeStr}");
        }



    }
}
