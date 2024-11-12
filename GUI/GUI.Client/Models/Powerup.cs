// Author: Daniel Kopta, Fall 2017
// Staff solution for CS 3500 final project
// University of Utah
using System.Text.Json.Serialization;

namespace CS3500.Models
{
    /// <summary>
    /// One of the types of objects in the game (part of the Model of MVC)
    /// </summary>
    public class Powerup
    {
        /// <summary>
        /// This powerup's unique ID
        /// </summary>
        [JsonPropertyName("power")]
        public int ID { get; set; }

        /// <summary>
        /// This powerup's position
        /// </summary>
        [JsonPropertyName("loc")]
        public Vector2D Position { get; set; }

        /// <summary>
        /// Whether or not this powerup is active and part of the live game
        /// </summary>
        [JsonPropertyName("died")]
        public bool Died { get; set; }

        /// <summary>
        /// RNG used to decide when a powerup goes away
        /// </summary>
        private Random rand = new();

        // Parameterless constructor for JSON deserialization
        public Powerup() { }

        /// <summary>
        /// Creates a new powerup with the given ID and x,y location
        /// </summary>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Powerup(int id, int x, int y)
        {
            ID = id;
            Position = new Vector2D(x, y);
        }

        /// <summary>
        /// Updates this powerup for one frame of the game.
        /// </summary>
        public void Step()
        {
            // 0.1% chance to deactivate on each frame
            if ( rand.Next( 1000 ) == 0 )
            {
                Died = false;
                return;
            }
        }

    }
}
