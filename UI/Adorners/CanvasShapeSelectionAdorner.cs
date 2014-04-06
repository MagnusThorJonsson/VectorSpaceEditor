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
using VectorSpace.Undo;

namespace VectorSpace.UI.Adorners
{
    /// <summary>
    /// An adorner for shapes on the canvas
    /// </summary>
    public class CanvasShapeSelectionAdorner : Adorner
    {
        #region Variables
        protected ShapeItem shape;
        protected List<Thumb> thumbs;
        protected VisualCollection visualChildren;
        #endregion


        #region Constructor
        /// <summary>
        /// Constructs a selection adorner
        /// </summary>
        /// <param name="adornedElement">The adorned element</param>
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
                        buildThumb(ref thumb, Cursors.Hand);
                        thumb.Tag = i;
                        thumb.DragDelta += new DragDeltaEventHandler(HandleThumb_DragDelta);
                        thumb.DragStarted += new DragStartedEventHandler(HandleThumb_DragStarted);
                        thumbs.Add(thumb);
                    }
                }
            }
        }
        #endregion


        #region Drag Methods
        public void HandleThumb_DragStarted(object sender, DragStartedEventArgs args)
        {
            FrameworkElement element = this.AdornedElement as FrameworkElement;
            if (element != null)
            {
                ShapeItem item = element.DataContext as ShapeItem;
                ContentPresenter path = this.AdornedElement as ContentPresenter;
                if (path != null && shape != null)
                {
                    Thumb thumb = (Thumb)sender;
                    Point point = shape.Points[(int)thumb.Tag];
                    HandlePointDrag((int)thumb.Tag, point, true);
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

                HandlePointDrag((int)thumb.Tag, point);

                InvalidateArrange();
            }
        }

        /// <summary>
        /// Handles the dragging of points
        /// </summary>
        /// <param name="tag">The point being dragged</param>
        /// <param name="position">The position to move to</param>
        /// <param name="doUndo">True to save undo action, defaults to false</param>
        public void HandlePointDrag(int tag, Point position, bool doUndo = false)
        {
            if (doUndo)
            {
                Point point = shape.Points[tag];
                UndoRedoManager.Instance().Push((dummy) => HandlePointDrag(tag, new Point(point.X, point.Y), true), this);
            }

            shape.EditPoint(tag, position);
        }
        #endregion

        #region Helper & Override Methods
        /// <summary>
        /// Builds a point thumb
        /// </summary>
        /// <param name="cornerThumb">The thumb</param>
        /// <param name="customizedCursor">The cursor</param>
        protected void buildThumb(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            cornerThumb = new Thumb();
            cornerThumb.Height = cornerThumb.Width = 10;
            cornerThumb.Background = Brushes.Black;
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Opacity = 0.80;
            visualChildren.Add(cornerThumb);
        }

        /// <summary>
        /// Overrides adorner arrangement
        /// </summary>
        /// <param name="finalSize">The final size</param>
        /// <returns>A new final size</returns>
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
        #endregion
    }
}
