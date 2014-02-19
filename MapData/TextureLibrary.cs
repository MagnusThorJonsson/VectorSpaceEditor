using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorSpace.MapData
{
    /// <summary>
    /// Manages a texture library package
    /// </summary>
    public class TextureLibrary 
    {
        #region Variables & Properties
        /// <summary>
        /// The library name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        protected string name;

        /// <summary>
        /// Texture collection this library contains
        /// </summary>
        public List<Texture> Textures { get { return textures; } }
        protected List<Texture> textures;
        #endregion


        #region Constructors
        /// <summary>
        /// Texture Library manager
        /// </summary>
        /// <param name="name">The name of the library</param>
        /// <param name="textures">The texture collection</param>
        public TextureLibrary(string name, List<Texture> textures)
        {
            this.name = name;
            this.textures = textures;
        }
        #endregion
    }
}
