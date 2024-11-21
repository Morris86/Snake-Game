// Author: Morris Lu, Anurup Kumar Fall 2024

using System;
using System.Text.Json.Serialization;

namespace CS3500.Models
{
    /// <summary>
    /// Represents a point or vector in 2D space.
    /// </summary>
    public class Point2D
    {
        /// <summary>
        /// The X-coordinate of the point in 2D space.
        /// </summary>
        [JsonPropertyName("X")]
        public int X { get; set; }

        /// <summary>
        /// The Y-coordinate of the point in 2D space.
        /// </summary>
        [JsonPropertyName("Y")]
        public int Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2D"/> class.
        /// This default constructor is required for JSON serialization/deserialization.
        /// </summary>
        public Point2D() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2D"/> class with the specified coordinates.
        /// </summary>
        /// <param name="x">The X-coordinate of the point.</param>
        /// <param name="y">The Y-coordinate of the point.</param>
        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
