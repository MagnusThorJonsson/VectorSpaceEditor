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
using System.Windows.Media;

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
        /// The shapes stroke thickness
        /// </summary>
        [JsonProperty(Order = 7)]
        public int StrokeThickness
        {
            get { return strokeThickness; }
            set
            {
                strokeThickness = value;
                if (strokeThickness < 1)
                    strokeThickness = 1;

                OnPropertyChanged("StrokeThickness");
            }
        }
        protected int strokeThickness;

        /// <summary>
        /// The shapes stroke brush (color)
        /// </summary>
        [JsonProperty(Order = 8)]
        public Brush Stroke
        {
            get { return stroke; }
            set
            {
                stroke = value;
                OnPropertyChanged("Stroke");
            }
        }
        protected Brush stroke;

        /// <summary>
        /// The shapes fill brush (color)
        /// </summary>
        [JsonProperty(Order = 9)]
        public Brush Fill
        {
            get { return fill; }
            set
            {
                fill = value;
                OnPropertyChanged("Fill");
            }
        }
        protected Brush fill;


        /// <summary>
        /// Is the shape a polygon
        /// </summary>
        [JsonProperty(Order = 10)]
        public bool IsPolygon
        {
            get { return isPolygon; }
            set
            {
                isPolygon = value;
                OnPropertyChanged("IsPolygon");
            }
        }
        protected bool isPolygon;

        /// <summary>
        /// The shape point list
        /// </summary>
        [JsonProperty(Order = 11)]
        public ObservableCollection<Point> Points
        {
            get { return points; }
            protected set
            {
                points = value;
                OnPropertyChanged("Points");
            }
        }


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
        #endregion


        #region Constructors
        public ShapeItem(string layer, string name, WorldPosition position, int zIndex, bool isPolygon = true)
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

            this.fill = new SolidColorBrush(Colors.Red);
            this.fill.Opacity = 0.5;
            this.stroke = new SolidColorBrush(Colors.Red);
            this.strokeThickness = 1;
            this.isPolygon = isPolygon;

            this.properties = new ObservableCollection<ItemProperty>();
            this.points = new ObservableCollection<Point>();
        }

        public ShapeItem(string layer, string name, List<Point> points, WorldPosition position, int zIndex, bool isPolygon = true)
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

            this.fill = new SolidColorBrush(Colors.Red);
            this.fill.Opacity = 0.5;
            this.stroke = new SolidColorBrush(Colors.Red);
            this.strokeThickness = 1;
            this.isPolygon = isPolygon;

            this.properties = new ObservableCollection<ItemProperty>();
            this.points = new ObservableCollection<Point>(points);
        }
        #endregion


        #region Factory Methods
        /// <summary>
        /// Creates a ShapeItem
        /// </summary>
        /// <param name="layer">The layer the item is on</param>
        /// <param name="name">The name of the item</param>
        /// <param name="points">The shape point list</param>
        /// <param name="position">The item position</param>
        /// <param name="zIndex">The Z index depth of the item</param>
        /// <returns></returns>
        public static ShapeItem Create(string layer, string name, List<Point> points, WorldPosition position, int zIndex = 0)
        {
            return new ShapeItem(layer, name, points, position, zIndex);
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
