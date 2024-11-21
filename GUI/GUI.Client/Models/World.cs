// Author: Morris Lu, Anurup Kumar Fall 2024

using System.Numerics;

namespace CS3500.Models
{
    /// <summary>
    /// Represents the game world, containing all objects in the game.
    /// Acts as the Model in the MVC architecture.
    /// </summary>
    public class World
    {
        /// <summary>
        /// A dictionary of players in the game, keyed by their unique IDs.
        /// </summary>
        public Dictionary<int, Player> Players { get; private set; } = new Dictionary<int, Player>();

        /// <summary>
        /// A dictionary of powerups in the game, keyed by their unique IDs.
        /// </summary>
        public Dictionary<int, Powerup> Powerups { get; private set; }

        /// <summary>
        /// A dictionary of walls in the game, keyed by their unique IDs.
        /// </summary>
        public Dictionary<int, Wall> Walls { get; private set; } = new Dictionary<int, Wall>();

        /// <summary>
        /// The size of one side of the square game world.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Default constructor required for JSON serialization/deserialization.
        /// Initializes all dictionaries to empty and sets a default size.
        /// </summary>
        public World()
        {
            Players = new Dictionary<int, Player>();
            Powerups = new Dictionary<int, Powerup>();
            Walls = new Dictionary<int, Wall>();
            Size = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class with the specified size.
        /// </summary>
        /// <param name="_size">The size of one side of the square world.</param>
        public World( int _size )
        {
            Players = new Dictionary<int, Player>();
            Powerups = new Dictionary<int, Powerup>();
            Walls = new Dictionary<int, Wall>();
            Size = _size;
        }

        /// <summary>
        /// Create a copy of the world
        /// </summary>
        public World(World original)
        {
            Size = original.Size;
            Players = new Dictionary<int, Player>(original.Players);
            Powerups = new Dictionary<int, Powerup>(original.Powerups);
            Walls = new Dictionary<int, Wall>(original.Walls);
        }

        /// <summary>
        /// Updates the data for an existing player or adds a new player to the world.
        /// </summary>
        /// <param name="player">The player to update or add.</param>
        public void UpdatePlayer(Player player)
        {
            if (Players.ContainsKey(player.ID))
            {
                // Update existing player data
                Players[player.ID] = player;
            }
            else
            {
                // Add new player
                Players.Add(player.ID, player);
            }
        }

        /// <summary>
        /// Updates the data for an existing powerup or adds a new powerup to the world.
        /// </summary>
        /// <param name="powerup">The powerup to update or add.</param>
        public void UpdatePowerup(Powerup powerup)
        {
            if (powerup == null) return;

            if (Powerups.ContainsKey(powerup.ID))
            {
                Powerups[powerup.ID] = powerup;
            }
            else
            {
                Powerups.Add(powerup.ID, powerup);
            }
        }

        /// <summary>
        /// Removes a player from the world by their ID.
        /// </summary>
        /// <param name="playerID">The ID of the player to remove.</param>
        public void RemovePlayer(int playerID)
        {
            if (Players.ContainsKey(playerID))
            {
                Players.Remove(playerID);
            }
        }

        /// <summary>
        /// Removes a powerup from the world by its ID.
        /// </summary>
        /// <param name="powerupID">The ID of the powerup to remove.</param>
        public void RemovePowerup(int powerupID)
        {
            if (Powerups.ContainsKey(powerupID))
            {
                Powerups.Remove(powerupID); // This removes the power-up from the dictionary
            }
        }

        /// <summary>
        /// Adds a wall to the world if it is not already present.
        /// </summary>
        /// <param name="wall">The wall to add.</param>
        public void AddWall(Wall wall)
        {
            if (!Walls.ContainsKey(wall.ID))
            {
                Walls[wall.ID] = wall;
            }
        }
    }
}
