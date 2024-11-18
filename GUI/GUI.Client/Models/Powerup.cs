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
        public Point2D Position { get; set; }

        /// <summary>
        /// Whether or not this powerup is active and part of the live game
        /// </summary>
        [JsonPropertyName("died")]
        public bool Died { get; set; }

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
            Position = new Point2D(x, y);
        }
    }
}
