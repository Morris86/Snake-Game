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

        /// <summary>
        /// Add two vectors with the + operator
        /// </summary>
        /// <param name="v1">The left hand side</param>
        /// <param name="v2">The right hand side</param>
        /// <returns></returns>
        public static Point2D operator +( Point2D v1, Point2D v2 )
        {
            return new Point2D( v1.X + v2.X, v1.Y + v2.Y );
        }

        /// <summary>
        /// Subtract two vectors with the - operator
        /// </summary>
        /// <param name="v1">The left hand side</param>
        /// <param name="v2">The right hand side</param>
        /// <returns></returns>
        public static Point2D operator -( Point2D v1, Point2D v2 )
        {
            return new Point2D( v1.X - v2.X, v1.Y - v2.Y );
        }

        /// <summary>
        /// Multiply a vector by a scalar
        /// This has the effect of growing (if s greater than 1) or shrinking (if s less than 1),
        /// without changing the direction.
        /// </summary>
        /// <param name="v">The vector (left-hand side of the operator)</param>
        /// <param name="s">The scalar (right-hand side of the operator)</param>
        /// <returns></returns>
        public static Point2D operator *( Point2D v, int s )
        {
            Point2D retval = new Point2D();
            retval.X = v.X * s;
            retval.Y = v.X * s;
            return retval;
        }
    }
}
