using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VectorSpace.MapData.MapItems;

namespace VectorSpace.UI.Adorners
{
    public class CanvasShapeSelectionAdorner : Adorner
    {
        #region Variables
        protected ShapeItem shape;
        protected List<Thumb> thumbs;
        protected VisualCollection visualChildren;
        #endregion

        public CanvasShapeSelectionAdorner(UIElement adornedElement) : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);
            thumbs = new List<Thumb>();

            ContentPresenter path = adornedElement as ContentPresenter;
            if (path != null)
            {
                shape = path.DataContext as ShapeItem;
                if (shape != null)
                {
                    for (int i = 0; i < shape.Points.Count; i++)
                    {
                        //point = polygon.Points[i];
                        Thumb thumb = new Thumb();
                        buildCorner(ref thumb, Cursors.Hand);
                        thumb.Tag = i;
                        thumb.DragDelta += new DragDeltaEventHandler(HandleThumb_DragDelta);
                        thumbs.Add(thumb);
                    }
                }
            }
        }


        public void HandleThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ContentPresenter path = this.AdornedElement as ContentPresenter;
            if (path != null && shape != null)
            {
                Thumb thumb = (Thumb)sender;
                Point point = shape.Points[(int)thumb.Tag];

                point.X += e.HorizontalChange;
                point.Y += e.VerticalChange;

                shape.EditPoint((int)thumb.Tag, point);

                //adornedElement = polygon;
                InvalidateArrange();
            }
        }

        protected void buildCorner(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            cornerThumb = new Thumb();
            cornerThumb.Height = cornerThumb.Width = 10;
            cornerThumb.Background = Brushes.Black;
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Opacity = 0.80;
            visualChildren.Add(cornerThumb);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ContentPresenter path = this.AdornedElement as ContentPresenter;
            if (path != null && shape != null)
            {
                for (int i = 0; i < shape.Points.Count; i++)
                {
                    Point point = shape.Points[i];
                    Thumb thumb = thumbs[i];
                    thumb.Arrange(new Rect(point.X - 5, point.Y - 5, 10, 10));
                }
            }

            return finalSize;
        }

        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }

        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
    }
}
