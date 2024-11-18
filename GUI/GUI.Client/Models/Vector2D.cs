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
    public class Vector2D
    {
        [JsonPropertyName("X")]
        public int X { get; set; }

        [JsonPropertyName("Y")]
        public int Y { get; set; }

        /// <summary>
        /// Default constructor, needed for JSON serialize/deserialize
        /// </summary>
        public Vector2D() { }

        /// <summary>
        /// Two param constructor for x and y.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        public Vector2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Add two vectors with the + operator
        /// </summary>
        /// <param name="v1">The left hand side</param>
        /// <param name="v2">The right hand side</param>
        /// <returns></returns>
        public static Vector2D operator +( Vector2D v1, Vector2D v2 )
        {
            return new Vector2D( v1.X + v2.X, v1.Y + v2.Y );
        }

        /// <summary>
        /// Subtract two vectors with the - operator
        /// </summary>
        /// <param name="v1">The left hand side</param>
        /// <param name="v2">The right hand side</param>
        /// <returns></returns>
        public static Vector2D operator -( Vector2D v1, Vector2D v2 )
        {
            return new Vector2D( v1.X - v2.X, v1.Y - v2.Y );
        }

        /// <summary>
        /// Multiply a vector by a scalar
        /// This has the effect of growing (if s greater than 1) or shrinking (if s less than 1),
        /// without changing the direction.
        /// </summary>
        /// <param name="v">The vector (left-hand side of the operator)</param>
        /// <param name="s">The scalar (right-hand side of the operator)</param>
        /// <returns></returns>
        public static Vector2D operator *( Vector2D v, int s )
        {
            Vector2D retval = new Vector2D();
            retval.X = v.X * s;
            retval.Y = v.X * s;
            return retval;
        }
    }
}
