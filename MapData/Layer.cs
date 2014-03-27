using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VectorSpace.MapData.Interfaces;

namespace VectorSpace.MapData
{
    /// <summary>
    /// Map layers
    /// </summary>
    [DataContract]
    public class Layer : INotifyPropertyChanged
    {
        #region Variables & Properties
        /// <summary>
        /// Layer id (unique)
        /// </summary>
        [DataMember]        
        public string Id 
        { 
            get { return id; }
            protected set { id = value; }
        }
        protected string id;

        /// <summary>
        /// Layer name
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return name; }
            set 
            { 
                name = value;
                OnPropertyChanged("Name");
            }
        }
        protected string name;

        /// <summary>
        /// Items contained within this layer
        /// </summary>
        [DataMember]
        public ObservableCollection<IRenderable> Items 
        { 
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }
        protected ObservableCollection<IRenderable> items;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a layer
        /// </summary>
        /// <param name="id">Layer id</param>
        public Layer(string id)
        {
            this.id = id;
            this.name = "Layer " + id;

            items = new ObservableCollection<IRenderable>();
        }

        /// <summary>
        /// Creates a layer
        /// </summary>
        /// <param name="id">Layer id</param>
        /// <param name="name">Layer name</param>
        public Layer(string id, string name)
        {
            this.id = id;
            this.name = name;

            items = new ObservableCollection<IRenderable>();
        }
        #endregion

        #region Add & Remove Handlers
        /// <summary>
        /// Adds an item to the layer
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>True on success</returns>
        public bool AddItem(IRenderable item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes an item from the layer
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True on success</returns>
        public bool RemoveItem(IRenderable item)
        {
            return items.Remove(item);
        }
        #endregion


        #region Interface Handlers
        /// <summary>
        /// Property Changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handles property change updates
        /// </summary>
        /// <param name="propertyName">The name of the property that was updated</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Handles property change updates
        /// </summary>
        /// <param name="e"></param>
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }
        #endregion

    }
}
