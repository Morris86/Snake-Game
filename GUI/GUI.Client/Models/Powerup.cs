using System.Text.Json.Serialization;

namespace CS3500.Models
{
    /// <summary>
    /// Represents a powerup object in the game. 
    /// Powerups can be collected by players to affect gameplay.
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
        public Point2D? Position { get; set; }

        /// <summary>
        /// Indicates whether this powerup has been collected and is no longer active.
        /// </summary>
        [JsonPropertyName("died")]
        public bool Died { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Powerup"/> class.
        /// This parameterless constructor is required for JSON serialization/deserialization.
        /// </summary>
        public Powerup() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Powerup"/> class with the specified ID and position.
        /// </summary>
        /// <param name="id">The unique identifier for the powerup.</param>
        /// <param name="x">The X-coordinate of the powerup's position.</param>
        /// <param name="y">The Y-coordinate of the powerup's position.</param>
        public Powerup(int id, int x, int y)
        {
            ID = id;
            Position = new Point2D(x, y);
        }
    }
}
