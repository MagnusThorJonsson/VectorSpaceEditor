using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VectorSpace.MapData.Components
{
    /// <summary>
    /// Contains the points used to map out a shape
    /// </summary>
    public class ShapePoints : ObservableCollection<Point>
    {
        #region Variables & Properties
        /// <summary>
        /// Flags whether this shape is a polygon
        /// </summary>
        public bool IsPolygon
        {
            get { return isPolygon; }
            set
            {
                isPolygon = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsPolygon"));
            }
        }
        protected bool isPolygon;
        #endregion


        #region Constructors
        /// <summary>
        /// Constructs a ShapePoints object
        /// </summary>
        /// <param name="isPolygon">True to close path</param>
        public ShapePoints(bool isPolygon)
            : base()
        {
            this.isPolygon = isPolygon;
        }

        /// <summary>
        /// Constructs a ShapePoints object
        /// </summary>
        /// <param name="isPolygon">True to close path</param>
        /// <param name="list">The list of points to construct the polygon</param>
        public ShapePoints(bool isPolygon, IEnumerable<Point> list) : base(list)
        {
            this.isPolygon = isPolygon;
        }
        #endregion
    }
}
