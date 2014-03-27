using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Serialization;

namespace VectorSpace.MapData.Components
{
    /// <summary>
    /// Used to position and transform any item that is rendered on the canvas.
    /// </summary>
    [DataContract]
    public struct WorldPosition
    {
        #region Variables & Properties
        /// <summary>
        /// The items position in world coordinates
        /// </summary>
        [DataMember]
        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }
        private Point _position;

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
        [DataMember]
        public Point Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }
        private Point _origin;

        /// <summary>
        /// The items width scale 
        /// </summary>
        [DataMember]
        public float ScaleX
        {
            get { return _scaleX; }
            set { _scaleX = value; }
        }
        private float _scaleX;

        /// <summary>
        /// The items height scale
        /// </summary>
        [DataMember]
        public float ScaleY
        {
            get { return _scaleY; }
            set { _scaleY = value; }
        }
        private float _scaleY;

        /// <summary>
        /// The items rotation in degrees
        /// </summary>
        [DataMember]
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }
        private float _rotation;
        #endregion


        #region Constructors
        /// <summary>
        /// Constructs a WorldPosition
        /// </summary>
        /// <param name="origin">The rotational origin</param>
        /// <param name="scaleX">The width scale</param>
        /// <param name="scaleY">The height scale</param>
        /// <param name="rotation">The rotation in degrees</param>
        public WorldPosition(Point origin, float scaleX, float scaleY, float rotation)
        {
            _position = new Point();
            _origin = origin;
            _scaleX = scaleX;
            _scaleY = scaleY;
            _rotation = rotation;
        }
        
        /// <summary>
        /// Constructs a WorldPosition
        /// </summary>
        /// <param name="position">The world position</param>
        /// <param name="origin">The rotational origin</param>
        /// <param name="scaleX">The width scale</param>
        /// <param name="scaleY">The height scale</param>
        /// <param name="rotation">The rotation in degrees</param>
        public WorldPosition(Point position, Point origin, float scaleX, float scaleY, float rotation)
        {
            _position = position;
            _origin = origin;
            _scaleX = scaleX;
            _scaleY = scaleY;
            _rotation = rotation;
        }
        #endregion

    }
}
