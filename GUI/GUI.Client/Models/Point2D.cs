// Author: Daniel Kopta, Fall 2017
// Staff solution for CS 3500 final project
// University of Utah

using System;
using System.Text.Json.Serialization;

namespace CS3500.Models
{
    /// <summary>
    /// A class to represent a Vector in 2D space
    /// </summary>
    public class Point2D
    {
        [JsonPropertyName("X")]
        public int X { get; set; }

        [JsonPropertyName("Y")]
        public int Y { get; set; }

        /// <summary>
        /// Default constructor, needed for JSON serialize/deserialize
        /// </summary>
        public Point2D() { }

        /// <summary>
        /// Two param constructor for x and y.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
