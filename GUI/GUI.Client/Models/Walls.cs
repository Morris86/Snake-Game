using System.Text.Json.Serialization;

namespace CS3500.Models
{
    /// <summary>
    /// Represents a wall object in the game.
    /// Walls are obstacles that players cannot pass through.
    /// </summary>
    public class Wall
    {
        /// <summary>
        /// The unique identifier for this wall.
        /// </summary>
        [JsonPropertyName("wall")]
        public int ID { get; set; }

        /// <summary>
        /// The starting point of this wall in the game world.
        /// </summary>
        [JsonPropertyName("p1")]
        public Point2D? Point1 { get; set; }

        /// <summary>
        /// The ending point of this wall in the game world.
        /// </summary>
        [JsonPropertyName("p2")]
        public Point2D? Point2 { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Wall"/> class.
        /// This parameterless constructor is required for JSON serialization/deserialization.
        /// </summary>
        public Wall() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Wall"/> class with the specified ID and endpoints.
        /// </summary>
        /// <param name="id">The unique identifier for the wall.</param>
        /// <param name="point1">The starting point of the wall.</param>
        /// <param name="point2">The ending point of the wall.</param>
        public Wall(int id, Point2D point1, Point2D point2)
        {
            ID = id;
            Point1 = point1;
            Point2 = point2;
        }
    }
}
