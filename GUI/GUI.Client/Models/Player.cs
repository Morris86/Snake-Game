// Author: Daniel Kopta, Fall 2017
// CS 3500 game lab
// University of Utah

namespace CS3500.Models
{
    /// <summary>
    /// One of the types of objects in the game (part of the Model of MVC)
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The current position of the player's head (the front of the snake).
        /// </summary>
        public Vector2D Position => Body.Count > 0 ? Body[0] : new Vector2D(0, 0);

        /// <summary>
        /// The body segments of the snake, where the first element is the head, and the last is the tail.
        /// </summary>
        public List<Vector2D> Body { get; private set; } = new List<Vector2D>();

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
            Direction = new Vector2D(1, 0);
            Direction.Rotate(angle);

            // Initialize the body with the starting head position
            Body.Add(new Vector2D(x, y));

            nextDirChange = rand.Next(300);
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
        /// Updates this player for one frame of the game
        /// </summary>
        /// <param name="size"></param>
        public void Step( int size )
        {
            // Change direction if it's time
            if (frameNum == nextDirChange)
            {
                frameNum = 0;
                nextDirChange = rand.Next(300);
                Direction.Rotate(rand.NextDouble() * 360);
            }

            // Move the snake forward by updating the head and body
            Move();

            // Check if the head has gone out of bounds; deactivate if necessary
            Vector2D head = Body[0];
            if (head.X < -20 || head.X > size + 20 || head.Y < -20 || head.Y > size + 20)
                Active = false;

            frameNum++;
        }

        /// <summary>
        /// Rotates the snake's direction.
        /// </summary>
        /// <param name="degrees">The degrees to rotate the direction vector.</param>
        public void Rotate(double degrees)
        {
            Direction.Rotate(degrees);
        }
    }
}