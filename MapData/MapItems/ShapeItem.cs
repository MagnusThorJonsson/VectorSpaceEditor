using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace.MapData.Components;
using VectorSpace.MapData.Interfaces;

namespace VectorSpace.MapData.MapItems
{
    public class ShapeItem : IRenderable, IHasProperties
    {
        #region Variables
        protected string type;

        protected string layer;
        protected string name;

        protected Point size;

        protected WorldPosition position;
        protected int zIndex;

        protected ObservableCollection<Point> points;
        #endregion


        #region Properties
        /// <summary>
        /// Item type
        /// </summary>
        [JsonProperty(Order = 1)]
        public string Type
        {
            get { return type; }
            protected set { type = value; }
        }

        /// <summary>
        /// Item name
        /// </summary>
        [JsonProperty(Order = 2)]
        public string Name
        {
            get { return name; }
            protected set { name = value; }
        }

        /// <summary>
        /// Item layer id
        /// </summary>
        [JsonProperty(Order = 3)]
        public string Layer
        {
            get { return layer; }
            set
            {
                layer = value;
                OnPropertyChanged("Layer");
            }
        }

        /// <summary>
        /// Depth index
        /// </summary>
        [JsonProperty(Order = 4)]
        public int ZIndex
        {
            get { return zIndex; }
            set
            {
                zIndex = value;
                OnPropertyChanged("ZIndex");
            }
        }

        /// <summary>
        /// The map position and transform 
        /// </summary>
        [JsonProperty(Order = 5)]
        public WorldPosition Position
        {
            get { return position; }
            set
            {
                position = value;
                OnPropertyChanged("Position");
            }
        }

        /// <summary>
        /// ShapeItem user properties
        /// </summary>
        [JsonProperty(Order = 6)]
        public ObservableCollection<ItemProperty> Properties
        {
            get { return properties; }
            protected set { properties = value; }
        }
        protected ObservableCollection<ItemProperty> properties;

        /// <summary>
        /// Is Visible property
        /// </summary>
        [JsonIgnore]
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                OnPropertyChanged("IsVisible");
                OnIsVisibleChanged(EventArgs.Empty);
            }
        }
        protected bool isVisible;

        /// <summary>
        /// Is Selected property
        /// </summary>
        [JsonIgnore]
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
                OnIsSelectedChanged(EventArgs.Empty);
            }
        }
        protected bool isSelected;

        /// <summary>
        /// Current width based on scale
        /// </summary>
        [JsonIgnore]
        public float Width
        {
            get
            {
                return (float)size.X * position.ScaleX;
            }
            set
            {
                if (value > 0f)
                {
                    position.ScaleX = value / (float)size.X;

                    OnPropertyChanged("Position");
                    OnPropertyChanged("Width");
                }
            }
        }

        /// <summary>
        /// Current height based on scale
        /// </summary>
        [JsonIgnore]
        public float Height
        {
            get
            {
                return (float)size.Y * position.ScaleY;
            }
            set
            {
                if (value > 0f)
                {
                    position.ScaleY = value / (float)size.Y;

                    OnPropertyChanged("Position");
                    OnPropertyChanged("Height");
                }
            }
        }

        /// <summary>
        /// The shape point list
        /// </summary>
        public ObservableCollection<Point> Points
        {
            get { return points; }
            protected set
            {
                points = value;
                OnPropertyChanged("Points");
            }
        }
        #endregion


        #region Constructors
        public ShapeItem(string layer, string name, WorldPosition position, int zIndex)
        {
            // Set class type
            this.type = typeof(ShapeItem).Name;

            this.layer = layer;
            this.name = name;
            this.position = position;
            this.zIndex = zIndex;

            this.size = new Point();

            this.isSelected = false;
            this.isVisible = true;

            this.properties = new ObservableCollection<ItemProperty>();
            this.points = new ObservableCollection<Point>();
        }
        #endregion


        #region Shape Methods
        /// <summary>
        /// Adds a point to the list
        /// </summary>
        /// <param name="point">The point to add</param>
        public void AddPoint(Point point)
        {
            points.Add(point);
            OnPropertyChanged("Points");
        }

        /// <summary>
        /// Adds a point to the list
        /// </summary>
        /// <param name="x">The X axis</param>
        /// <param name="y">The Y axis</param>
        public void AddPoint(float x, float y)
        {
            points.Add(new Point(x, y));
            OnPropertyChanged("Points");
        }

        /// <summary>
        /// Removes a point from the list
        /// </summary>
        /// <param name="point">The point to remove</param>
        /// <returns>True on success</returns>
        public bool RemovePoint(Point point)
        {
            if (points.Contains(point))
            {
                OnPropertyChanged("Points");
                return points.Remove(point);
            }

            return false;
        }

        /// <summary>
        /// Removes a point from the list
        /// </summary>
        /// <param name="x">The X axis</param>
        /// <param name="y">The Y axis</param>
        /// <returns>True on success</returns>
        public bool RemovePoint(float x, float y)
        {
            return RemovePoint(new Point(x, y));
        }
        #endregion


        #region User Property Handlers
        /// <summary>
        /// Adds a property to the collection
        /// </summary>
        /// <param name="key">Property key</param>
        /// <param name="value">Property value</param>
        public void AddProperty(string key, string value)
        {
            Properties.Add(new ItemProperty(key, value));
        }

        /// <summary>
        /// Removes a property from the collection
        /// </summary>
        /// <param name="key">Property key</param>
        /// <returns>True on success</returns>
        public bool RemoveProperty(string key)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                if (properties[i].Key.Equals(key))
                {
                    properties.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes a property from the collection
        /// </summary>
        /// <param name="property">The property to remove</param>
        /// <returns>True on success</returns>
        public bool RemoveProperty(ItemProperty property)
        {
            return Properties.Remove(property);
        }
        #endregion


        #region Notify Interface
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

        /// <summary>
        /// Handles changes to the IsVisible flag
        /// </summary>
        /// <param name="e"></param>
        protected void OnIsVisibleChanged(EventArgs e)
        {
            EventHandler handler = IsVisibleChanged;
            if (handler != null)
                handler(this, e);
        }
        public event EventHandler IsVisibleChanged;


        /// <summary>
        /// Handles changes to the IsSelected flag
        /// </summary>
        /// <param name="e"></param>
        protected void OnIsSelectedChanged(EventArgs e)
        {
            EventHandler handler = IsSelectedChanged;
            if (handler != null)
                handler(this, e);
        }
        public event EventHandler IsSelectedChanged;
        #endregion

    }
}
