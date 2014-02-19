using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace VectorSpace.MapData.Components
{
    /// <summary>
    /// Used to position and transform any item that is rendered on the canvas.
    /// </summary>
    public struct WorldPosition
    {
        #region Variables & Properties
        /// <summary>
        /// The items position in world coordinates
        /// </summary>
        public Point Position;

        /// <summary>
        /// The items position on the X axis
        /// </summary>
        public int X { get { return Position.X; } }
        
        /// <summary>
        /// The items position on the Y axis
        /// </summary>
        public int Y { get { return Position.Y; } }

        
        /// <summary>
        /// The items rotational origin position
        /// </summary>
        public Point Origin;

        /// <summary>
        /// The items width scale 
        /// </summary>
        public float ScaleX;

        /// <summary>
        /// The items height scale
        /// </summary>
        public float ScaleY;

        /// <summary>
        /// The items rotation in radians
        /// </summary>
        public float Rotation;
        #endregion


        #region Constructors
        /// <summary>
        /// Constructs a WorldPosition
        /// </summary>
        /// <param name="origin">The rotational origin</param>
        /// <param name="scaleX">The width scale</param>
        /// <param name="scaleY">The height scale</param>
        /// <param name="rotation">The rotation in radians</param>
        public WorldPosition(Point origin, float scaleX, float scaleY, float rotation)
        {
            Position = new Point();
            Origin = origin;
            ScaleX = scaleX;
            ScaleY = scaleY;
            Rotation = rotation;
        }
        
        /// <summary>
        /// Constructs a WorldPosition
        /// </summary>
        /// <param name="position">The world position</param>
        /// <param name="origin">The rotational origin</param>
        /// <param name="scaleX">The width scale</param>
        /// <param name="scaleY">The height scale</param>
        /// <param name="rotation">The rotation in radians</param>
        public WorldPosition(Point position, Point origin, float scaleX, float scaleY, float rotation)
        {
            Position = position;
            Origin = origin;
            ScaleX = scaleX;
            ScaleY = scaleY;
            Rotation = rotation;
        }
        #endregion

    }
}
