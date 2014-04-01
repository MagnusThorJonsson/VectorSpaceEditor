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
    public class ShapePoints : ObservableCollection<Point>
    {
        #region Variables & Properties
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
        public ShapePoints(bool isPolygon)
            : base()
        {
            this.isPolygon = isPolygon;
        }

        public ShapePoints(bool isPolygon, IEnumerable<Point> list) : base(list)
        {
            this.isPolygon = isPolygon;
        }
        #endregion
    }
}
