using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace VectorSpace.UI.Converters
{
    /// <summary>
    /// Converts an List of points to a Geometry data for Path objects
    /// </summary>
    public class PointsToPathConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<Point> points = value as ObservableCollection<Point>;
            if (points != null && points.Count > 0)
            {
                Point start = points[0];
                List<LineSegment> segments = new List<LineSegment>();
                for (int i = 1; i < points.Count; i++)
                {
                    segments.Add(new LineSegment(points[i], true));
                }
                PathFigure figure = new PathFigure(start, segments, false); //true if closed
                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(figure);
                return geometry;
            }
            else
                return null;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
