// Author: Daniel Kopta, Fall 2017
// CS 3500 game lab
// University of Utah

namespace GameLab.Models
{
    /// <summary>
    /// One of the types of objects in the game (part of the Model of MVC)
    /// </summary>
    public class Player
    {
        /// <summary>
        /// This player's position
        /// </summary>
        public Vector2D Position { get; private set; }

        /// <summary>
        /// This player's direction of travel
        /// </summary>
        public Vector2D Direction { get; private set; }

        /// <summary>
        /// This player's unique ID
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Whether or not this player is active and part of the live game
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// RNG used to pick a new direction when they turn
        /// </summary>
        private Random rand = new();

        /// <summary>
        /// Keeps track of the next frame on which this player will change direction
        /// </summary>
        private int nextDirChange;

        /// <summary>
        /// Keeps track of the current frame
        /// </summary>
        private int frameNum;

        /// <summary>
        /// Creates a new player with the given ID, x,y location and direction (angle)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="angle"></param>
        public Player( int id, int x, int y, double angle )
        {
            ID = id;
            Position = new Vector2D( x, y );
            Direction = new Vector2D( 1, 0 );

            Direction.Rotate( angle );

            nextDirChange = rand.Next( 300 );
        }

        /// <summary>
        /// Updates this player for one frame of the game
        /// </summary>
        /// <param name="size"></param>
        public void Step( int size )
        {
            // change directions if it's time
            if ( frameNum == nextDirChange )
            {
                frameNum = 0;
                nextDirChange = rand.Next( 300 );
                Direction.Rotate( rand.NextDouble() * 360 );
            }

            // move forward
            Position = Position + Direction;

            // let them go a little beyond the bounds of the world
            if ( Position.X < -20 || Position.X > size + 20 || Position.Y < 20 || Position.Y > size + 20 ) 
                Active = false;

            frameNum++;
        }
    }
}