// Author: Daniel Kopta, Fall 2017
// CS 3500 game lab
// University of Utah

using System.Numerics;

namespace GameLab.Models
{
    /// <summary>
    /// The Model part of MVC, represents all objects in the "game"
    /// </summary>
    public class World
    {
        /// <summary>
        /// The players in the game
        /// </summary>
        public Dictionary<int, Player> Players;

        /// <summary>
        /// The powerups in the game
        /// </summary>
        public Dictionary<int, Powerup> Powerups;

        /// <summary>
        /// The size of a single side of the square world
        /// </summary>
        public int Size
        { get; private set; }

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

    }
}
