// Author: Daniel Kopta, Fall 2017
// CS 3500 game lab
// University of Utah
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
        public string Name { get; set; }

        /// <summary>
        /// The segments of the snake's body.
        /// The first element represents the head of the snake.
        /// </summary>
        [JsonPropertyName("body")]
        public List<Vector2D> Body { get; set; }

        /// <summary>
        /// The direction of the snake's movement.
        /// </summary>
        [JsonPropertyName("dir")]
        public Vector2D Direction { get; set; }

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
            Body = new List<Vector2D>();
        }

        /// <summary>
        /// Moves the snake in the current direction, updating its body segments.
        /// </summary>
        public void Move()
        {
            // Calculate new head position based on direction
            Vector2D newHead = Body[0] + Direction;

            // Insert new head position at the beginning of the body
            Body.Insert(0, newHead);

            // Remove the last segment to simulate forward movement
            Body.RemoveAt(Body.Count - 1);
        }

        /// <summary>
        /// Grows the snake by adding a new segment at the end of the body.
        /// </summary>
        public void Grow()
        {
            // Duplicate the last segment to grow the body
            Body.Add(new Vector2D(Body[^1]));
        }

        /// <summary>
        /// Rotates the snake's direction by a specified number of degrees.
        /// </summary>
        /// <param name="degrees">The degrees to rotate the direction vector.</param>
        public void Rotate(double degrees)
        {
            Direction.Rotate(degrees);
        }
    }
}