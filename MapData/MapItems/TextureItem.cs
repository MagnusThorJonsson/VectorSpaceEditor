using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace.MapData.Components;
using VectorSpace.MapData.Interfaces;

namespace VectorSpace.MapData.MapItems
{
    public class TextureItem : IRenderable
    {
        #region Variables & Properties
        protected int layer;
        protected string name;

        public Texture Texture { get { return texture; } }
        protected Texture texture;
        protected WorldPosition position;
        protected int zIndex;
        #endregion

        #region Interface Properties
        /// <summary>
        /// Item layer id
        /// </summary>
        public int Layer
        {
            get { return layer; }
            set
            {
                if (value < 0)
                    layer = 0;
                else
                    layer = value;
            }
        }

        /// <summary>
        /// Item identifier
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// The map position and transform 
        /// </summary>
        public WorldPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Depth index
        /// </summary>
        public int ZIndex { get { return zIndex; } }
        #endregion


        #region Constructor
        /// <summary>
        /// Item constructor
        /// </summary>
        /// <param name="layer">Item layer id</param>
        /// <param name="name">Item name</param>
        /// <param name="texture">Item texture</param>
        /// <param name="position">Item world position</param>
        /// <param name="zIndex">Item depth index</param>
        public TextureItem(int layer, string name, Texture texture, WorldPosition position, int zIndex)
        {
            this.layer = layer;
            this.name = name;
            this.texture = texture;
            this.position = position;
            this.zIndex = zIndex;
        }
        #endregion


        #region Factory Methods
        public static TextureItem Create(int layer, string name, Texture texture, WorldPosition position, int zIndex = 0)
        {
            return new TextureItem(layer, name, texture, position, zIndex);
        }
        #endregion
    }
}
