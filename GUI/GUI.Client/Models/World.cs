// Author: Daniel Kopta, Fall 2017
// CS 3500 game lab
// University of Utah

using System.Numerics;

namespace CS3500.Models
{
    /// <summary>
    /// The Model part of MVC, represents all objects in the "game"
    /// </summary>
    public class World
    {
        /// <summary>
        /// The players in the game
        /// </summary>
        public Dictionary<int, Player> Players { get; private set; } = new Dictionary<int, Player>();

        /// <summary>
        /// The powerups in the game
        /// </summary>
        public Dictionary<int, Powerup> Powerups { get; private set; }

        /// <summary>
        /// The size of a single side of the square world
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Creates a new world with the given size
        /// </summary>
        /// <param name="_size"></param>
        public World( int _size )
        {
            Players = new Dictionary<int, Player>();
            Powerups = new Dictionary<int, Powerup>();
            Size = _size;
        }

        /// <summary>
        /// Shallow copy constructor
        /// </summary>
        /// <param name="world"></param>
        public World( World world )
        {
            Players = new(world.Players);
            Powerups = new(world.Powerups);
            Size = world.Size;
        }

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
                //Console.WriteLine($"Powerup {powerup.ID} updated. Current power-ups: {string.Join(", ", Powerups.Keys)}");
            }
        }

        public void RemovePowerup(int powerupID)
        {
            if (Powerups.ContainsKey(powerupID))
            {
                Powerups.Remove(powerupID); // This removes the power-up from the dictionary
                //Console.WriteLine($"Powerup {powerupID} removed from game world. Remaining power-ups: {string.Join(", ", Powerups.Keys)}");
            }
        }

    }
}
