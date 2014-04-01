using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VectorSpace.UI.Converters
{
    /// <summary>
    /// Converts a bool value to a visibility flag and vice versa
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a bool to the visibility string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException("Bool value was null when trying to convert to visibility string.");


            if (value is bool)
            {
                return ((bool)value ? "Visible" : "Collapsed");
            }

            throw new ArgumentException(string.Format("Cannot convert unknown value {0}", value));
        }

        /// <summary>
        /// Converts a visibility string to a bool
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            if (s != null)
            {
                if (s.Equals("Visible"))
                {
                    return true;
                }
                else if (s.Equals("Collapsed"))
                {
                    return false;
                }
            }

            throw new ArgumentException(string.Format("Cannot convert unknown value {0}", value));
        }
    }
}
