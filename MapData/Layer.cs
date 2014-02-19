using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace.MapData.Interfaces;

namespace VectorSpace.MapData
{
    /// <summary>
    /// Map layers
    /// </summary>
    public class Layer
    {
        #region Variables & Properties
        /// <summary>
        /// Layer id (unique)
        /// </summary>
        public int Id { get { return id; } }
        protected int id;

        /// <summary>
        /// Layer name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        protected string name;

        /// <summary>
        /// Items contained within this layer
        /// </summary>
        public ObservableCollection<IRenderable> Items { get { return items; } }
        protected ObservableCollection<IRenderable> items;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a layer
        /// </summary>
        /// <param name="id">Layer id</param>
        public Layer(int id)
        {
            this.id = id;
            this.name = "Layer " + id.ToString();
        }

        /// <summary>
        /// Creates a layer
        /// </summary>
        /// <param name="id">Layer id</param>
        /// <param name="name">Layer name</param>
        public Layer(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
        #endregion


        public bool AddItem(IRenderable item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                return true;
            }

            return false;
        }

        public bool RemoveItem(IRenderable item)
        {
            return items.Remove(item);
        }
    }
}
