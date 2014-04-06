using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VectorSpace.MapData.Components;

namespace VectorSpace.MapData.Interfaces
{
    /// <summary>
    /// Interface for items that are rendered on the map canvas
    /// </summary>
    public interface IRenderable : INotifyPropertyChanged
    {
        #region Properties
        /// <summary>
        /// Object Type
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Property layer id
        /// </summary>
        string Layer { get; set; }

        /// <summary>
        /// Property holder name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The width of the renderable item
        /// </summary>
        float Width { get; set; }

        /// <summary>
        /// The height of the renderable item
        /// </summary>
        float Height { get; set; }
        
        /// <summary>
        /// Objects world position
        /// </summary>
        WorldPosition Position { get; set; }

        /// <summary>
        /// The ZIndex depth for the object (controlled by the layer placement)
        /// </summary>
        int ZIndex { get; set; }

        /// <summary>
        /// Current angle in degrees
        /// </summary>
        float Angle { get; set; }
        #endregion


        #region UI properties
        /// <summary>
        /// Flags whether an item is selected on the map canvas
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Flags whether an item is visible on the map canvas
        /// </summary>
        bool IsVisible { get; set; }
        #endregion


        #region Methods
        /// <summary>
        /// Moves the path by the given amount
        /// </summary>
        /// <param name="x">Amount to move on the X axis</param>
        /// <param name="y">Amount to move on the Y axis</param>
        void Move(float x, float y);
        
        /// <summary>
        /// Sets the  item to the given position
        /// </summary>
        /// <param name="x">Position on the X axis</param>
        /// <param name="y">Position on the Y axis</param>
        void SetPosition(float x, float y);

        /// <summary>
        /// Sets the item to a given position
        /// </summary>
        /// <param name="position">The position</param>
        void SetPosition(Point position);
        #endregion
    }
}
