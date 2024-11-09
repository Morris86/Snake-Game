// Author: Daniel Kopta, Fall 2017
// Staff solution for CS 3500 final project
// University of Utah

using System;

namespace GameLab.Models
{
    /// <summary>
    /// A class to represent a Vector in 2D space
    /// </summary>
    public class Vector2D
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        /// <summary>
        /// Default constructor, needed for JSON serialize/deserialize
        /// </summary>
        public Vector2D()
        {
            X = -1;
            Y = -1;
        }

        /// <summary>
        /// Two param constructor for x and y.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        public Vector2D( double _x, double _y )
        {
            X = _x;
            Y = _y;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public Vector2D( Vector2D other )
        {
            X = other.X;
            Y = other.Y;
        }

        /// <summary>
        /// Determine if this vector is equal to another
        /// </summary>
        /// <param name="obj">The other vector</param>
        /// <returns></returns>
        public override bool Equals( object obj )
        {
            if ( obj == null )
            {
                return false;
            }

            // If parameter cannot be cast to Vector return false.
            Vector2D p = obj as Vector2D;
            if ( p == null )
            {
                return false;
            }

            return ToString() == p.ToString();
        }

        /// <summary>
        /// Determine the hashcode for this vector
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Return a string representation of this vector for debug printing
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }


        /// <summary>
        /// Clamp x and y to be within the range -1 .. 1
        /// </summary>
        public void Clamp()
        {
            if ( X > 1.0 )
                X = 1.0;
            if ( X < -1.0 )
                X = -1.0;
            if ( Y > 1.0 )
                Y = 1.0;
            if ( Y < -1.0 )
                Y = -1.0;
        }

        /// <summary>
        /// Rotate this vector clockwise by degrees
        /// Requires that this vector be normalized
        /// </summary>
        /// <param name="degrees"></param>
        public void Rotate( double degrees )
        {
            double radians = (degrees / 180) * Math.PI;

            double newX = X * Math.Cos(radians) - Y * Math.Sin(radians);
            double newY = X * Math.Sin(radians) + Y * Math.Cos(radians);

            X = newX;
            Y = newY;

            // sin and cos can return numbers outside the valid range due to floating point imprecision,
            // and poor design of C#'s math library
            Clamp();
        }

        /// <summary>
        /// Return the angle as measured in degrees clockwise from up
        /// Requires that this vector be normalized
        /// </summary>
        /// <returns></returns>
        public float ToAngleDegrees()
        {
            float theta = (float)Math.Acos(-Y);

            if ( X < 0.0 )
                theta *= -1.0f;

            // Convert to degrees
            theta *= ( 180.0f / (float) Math.PI );

            return theta;
        }

        /// <summary>
        /// Return the angle as measured in degrees clockwise from up
        /// Requires that this vector be normalized
        /// </summary>
        /// <returns></returns>
        public float ToAngleRadians()
        {
            return ToAngleDegrees() / ((float) (360.0f / (Math.PI * 2.0f)));
        }

        /// <summary>
        /// Compute the clockwise angle of the vector pointing from b to a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float AngleBetweenPoints( Vector2D a, Vector2D b )
        {
            Vector2D bToA = a - b;
            bToA.Normalize();
            return bToA.ToAngleDegrees();
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
        public static Vector2D operator *( Vector2D v, double s )
        {
            Vector2D retval = new Vector2D();
            retval.X = v.X * s;
            retval.Y = v.X * s;
            return retval;
        }

        /// <summary>
        /// Compute the length of this vector
        /// </summary>
        /// <returns></returns>
        public double Length()
        {
            return Math.Sqrt( X * X + Y * Y );
        }

        /// <summary>
        /// Set this vector's length to 1 without changing its direction
        /// </summary>
        public void Normalize()
        {
            double len = Length();
            X /= len;
            Y /= len;
        }

        /// <summary>
        /// Compute the dot product of this vector with another vector
        /// </summary>
        /// <param name="v">The other vector</param>
        /// <returns></returns>
        public double Dot( Vector2D v )
        {
            return X * v.X + Y * v.Y;
        }

    }
}
