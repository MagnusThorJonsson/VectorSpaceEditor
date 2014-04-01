using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization;
using Newtonsoft.Json;

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
        [DataMember(Order = 0)]
        [JsonProperty(Order = 1)]
        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }
        private Point _position;

        /// <summary>
        /// The items position on the X axis
        /// </summary>
        [JsonIgnore]
        public float X { get { return (float)Position.X; } }
        
        /// <summary>
        /// The items position on the Y axis
        /// </summary>
        [JsonIgnore]
        public float Y { get { return (float)Position.Y; } }

        
        /// <summary>
        /// The items rotational origin position
        /// </summary>
        [DataMember(Order = 1)]
        [JsonProperty(Order = 2)]
        public Point Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }
        private Point _origin;

        /// <summary>
        /// The items width scale 
        /// </summary>
        [DataMember(Order = 2)]
        [JsonProperty(Order = 3)]
        public float ScaleX
        {
            get { return _scaleX; }
            set { _scaleX = value; }
        }
        private float _scaleX;

        /// <summary>
        /// The items height scale
        /// </summary>
        [DataMember(Order = 3)]
        [JsonProperty(Order = 4)]
        public float ScaleY
        {
            get { return _scaleY; }
            set { _scaleY = value; }
        }
        private float _scaleY;

        /// <summary>
        /// The items rotation in degrees
        /// </summary>
        [DataMember(Order = 4)]
        [JsonProperty(Order = 5)]
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
        [JsonConstructor]
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
