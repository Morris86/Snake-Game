// Author: Morris Lu, Anurup Kumar Fall 2024

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CS3500.Models
{
    /// <summary>
    /// One of the types of objects in the game (part of the Model of MVC)
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The unique ID of the snake/player (from the "snake" field in JSON).
        /// </summary>
        [JsonPropertyName("snake")]
        public int ID { get; set; }

        /// <summary>
        /// The player's name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// The segments of the snake's body.
        /// The first element represents the head of the snake.
        /// </summary>
        [JsonPropertyName("body")]
        public List<Point2D> Body { get; set; }

        /// <summary>
        /// The direction of the snake's movement.
        /// </summary>
        [JsonPropertyName("dir")]
        public Point2D? Direction { get; set; }

        /// <summary>
        /// The player's score.
        /// </summary>
        [JsonPropertyName("score")]
        public int Score { get; set; }

        /// <summary>
        /// Indicates if the snake died in the current frame.
        /// </summary>
        [JsonPropertyName("died")]
        public bool Died { get; set; }

        /// <summary>
        /// Indicates if the snake is currently alive.
        /// </summary>
        [JsonPropertyName("alive")]
        public bool Alive { get; set; }

        /// <summary>
        /// Indicates if the player has disconnected.
        /// </summary>
        [JsonPropertyName("dc")]
        public bool Disconnected { get; set; }

        /// <summary>
        /// Indicates if the player has just joined.
        /// </summary>
        [JsonPropertyName("join")]
        public bool JustJoined { get; set; }

        /// <summary>
        /// Default constructor required for JSON deserialization.
        /// </summary>
        public Player()
        {
            Body = new List<Point2D>();
            Name = string.Empty;
            Direction = new Point2D(0, 0);
        }


    }
}