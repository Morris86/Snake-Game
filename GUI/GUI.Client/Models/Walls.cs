using System.Text.Json.Serialization;

namespace CS3500.Models
{
    public class Wall
    {
        [JsonPropertyName("wall")]
        public int ID { get; set; }

        [JsonPropertyName("p1")]
        public Vector2D Point1 { get; set; }

        [JsonPropertyName("p2")]
        public Vector2D Point2 { get; set; }

        // Default constructor for JSON deserialization
        public Wall() { }

        // Constructor for manually creating walls if needed
        public Wall(int id, Vector2D point1, Vector2D point2)
        {
            ID = id;
            Point1 = point1;
            Point2 = point2;
        }
    }
}
