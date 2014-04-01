using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VectorSpace.MapData.MapItems;

namespace VectorSpace.UI.Selectors
{
    /// <summary>
    /// Selects the correct Data Template for items on the Level Canvas
    /// </summary>
    public class CanvasItemTemplateSelector : DataTemplateSelector
    {
        #region Properties
        /// <summary>
        /// Template for TextureItem
        /// </summary>
        public DataTemplate TextureTemplate
        {
            get { return _textureTemplate; }
            set { _textureTemplate = value; }
        }
        private DataTemplate _textureTemplate = null;

        /// <summary>
        /// Template for ShapeItem
        /// </summary>
        public DataTemplate ShapeTemplate
        {
            get { return _shapeTemplate; }
            set { _shapeTemplate = value; }
        }
        private DataTemplate _shapeTemplate = null;
        #endregion


        /// <summary>
        /// Selects a data template for canvas items
        /// </summary>
        /// <param name="item">The item we're checking</param>
        /// <param name="container">The container UI item</param>
        /// <returns>The DataTemplate used by the item</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is TextureItem)
            {
                return _textureTemplate;
            }
            else if (item is ShapeItem)
            {
                return _shapeTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
