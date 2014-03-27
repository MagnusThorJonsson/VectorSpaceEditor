using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VectorSpace.MapData.Components;
using VectorSpace.MapData.Interfaces;

namespace VectorSpace.MapData.MapItems
{
    [DataContract, KnownType(typeof(TextureItem))]
    public class TextureItem : IRenderable, IHasProperties
    {
        #region Variables & Properties
        protected string layer;
        protected string name;

        public Texture Texture { get { return texture; } }
        protected Texture texture;
        protected WorldPosition position;
        protected int zIndex;

        /// <summary>
        /// TextureItem user properties
        /// </summary>
        [DataMember]
        public ObservableCollection<ItemProperty> Properties 
        { 
            get { return properties; }
            protected set { properties = value; }
        }
        protected ObservableCollection<ItemProperty> properties;

        /// <summary>
        /// Is Selected property
        /// </summary>
        [DataMember]
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
        public float Width
        {
            get
            {
                return texture.Size.X * position.ScaleX;
            }
            set
            {
                if (value > 0f)
                {
                    position.ScaleX = value / texture.Size.X;

                    OnPropertyChanged("Position");
                    OnPropertyChanged("Width");
                }
            }
        }

        /// <summary>
        /// Current height based on scale
        /// </summary>
        public float Height
        {
            get
            {
                return texture.Size.Y * position.ScaleY;
            }
            set
            {
                if (value > 0f)
                {
                    position.ScaleY = value / texture.Size.Y;

                    OnPropertyChanged("Position");
                    OnPropertyChanged("Height");
                }
            }
        }

        /// <summary>
        /// Current angle in degrees
        /// </summary>
        public float Angle
        {
            get
            {
                return position.Rotation;
                /*
                // Radians to Degrees
                float degrees = (float)(180f / Math.PI * position.Rotation);
                
                if (degrees < 0f)
                    degrees += 360f;
                else if (degrees > 360f)
                    degrees -= 360f;
                return degrees;         
                */  
            }
            set
            {
                // Degrees to radians
                //position.Rotation = WrapRotation((float)((Math.PI / 180f) * value));
                position.Rotation = value;//(float)(Math.PI / 180f * value);

                OnPropertyChanged("Angle");
            }
        }

        #endregion


        #region Interface Properties
        /// <summary>
        /// Item layer id
        /// </summary>
        [DataMember]
        public string Layer
        {
            get { return layer; }
            set
            {
                /*
                if (value < 0)
                    layer = 0;
                else
                    layer = value;
                */
                layer = value;
                OnPropertyChanged("Layer");
            }
        }

        /// <summary>
        /// Item name
        /// </summary>
        [DataMember]
        public string Name 
        { 
            get { return name; }
            protected set { name = value; }
        }

        /// <summary>
        /// The map position and transform 
        /// </summary>
        [DataMember]
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
        /// Depth index
        /// </summary>
        [DataMember]
        public int ZIndex 
        { 
            get { return zIndex; }
            set
            {
                zIndex = value;
                OnPropertyChanged("ZIndex");
            }
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


        #region Constructor
        /// <summary>
        /// Item constructor
        /// </summary>
        /// <param name="layer">Item layer id</param>
        /// <param name="name">Item name</param>
        /// <param name="texture">Item texture</param>
        /// <param name="position">Item world position</param>
        /// <param name="zIndex">Item depth index</param>
        public TextureItem(string layer, string name, Texture texture, WorldPosition position, int zIndex)
        {
            this.layer = layer;
            this.name = name;
            this.texture = texture;
            this.position = position;
            this.zIndex = zIndex;

            this.isSelected = false;

            this.properties = new ObservableCollection<ItemProperty>();
        }
        #endregion


        #region Factory Methods
        public static TextureItem Create(string layer, string name, Texture texture, WorldPosition position, int zIndex = 0)
        {
            return new TextureItem(layer, name, texture, position, zIndex);
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// Moves the texture item by the given amount
        /// </summary>
        /// <param name="x">Amount to move on the X axis</param>
        /// <param name="y">Amount to move on the Y axis</param>
        public void Move(int x, int y)
        {
            position.Position = new System.Drawing.Point(
                x + position.Position.X, 
                y + position.Position.Y
            );

            OnPropertyChanged("Position");
        }

        /// <summary>
        /// Sets the texture item to the given position
        /// </summary>
        /// <param name="x">Position on the X axis</param>
        /// <param name="y">Position on the Y axis</param>
        public void SetPosition(int x, int y)
        {
            position.Position = new System.Drawing.Point(x, y);

            OnPropertyChanged("Position");
        }

        /// <summary>
        /// Sets the texture items scale
        /// </summary>
        /// <param name="sX">Scale width</param>
        /// <param name="sY">Scale height</param>
        public void SetScale(float sX, float sY)
        {
            if (sX <= 0f || sY <= 0f)
                return;

            position.ScaleX = sX;
            position.ScaleY = sY;

            OnPropertyChanged("Position");
            OnPropertyChanged("Width");
            OnPropertyChanged("Height");
        }

        /// <summary>
        /// Sets the texture items width scale
        /// </summary>
        /// <param name="sX">Scale width</param>
        public void SetScaleX(float sX)
        {
            if (sX <= 0f)
                return;

            position.ScaleX = sX;

            OnPropertyChanged("Position");
            OnPropertyChanged("Width");
        }

        /// <summary>
        /// Sets the texture items height scale
        /// </summary>
        /// <param name="sX">Scale height</param>
        public void SetScaleY(float sY)
        {
            if (sY <= 0f)
                return;

            position.ScaleY = sY;

            OnPropertyChanged("Position");
            OnPropertyChanged("Height");
        }
        /*
        /// <summary>
        /// Sets the texture items rotation in radians
        /// </summary>
        /// <param name="radians">rotation in radians</param>
        public void SetRotation(float radians)
        {

            position.Rotation = radians;

            OnPropertyChanged("Angle");
        }

        /// <summary>
        /// Adds to the texture items rotation
        /// </summary>
        /// <param name="radians">rotation to add in radians</param>
        public void AddRotation(float radians)
        {
            //position.Rotation += WrapRotation(radians);
            position.Rotation += radians;

            OnPropertyChanged("Angle");
        }
        */
        #endregion

        /// <summary>
        /// Wraps radian rotation 
        /// </summary>
        /// <param name="radians">Radian to wrap</param>
        /// <returns>Wrapped radian</returns>
        protected float wrapRotation(float radians)
        {
            while (radians < -Math.PI)
            {
                radians += (float)(Math.PI * 2.0);
            }
            while (radians > Math.PI)
            {
                radians -= (float)(Math.PI * 2.0);
            }

            return radians;
        }
    }
}
