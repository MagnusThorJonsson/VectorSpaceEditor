using System;
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
using VectorSpace.MapData.Interfaces;
using VectorSpace.MapData.MapItems;
using VectorSpace.Undo;

namespace VectorSpace.UI.Adorners
{
    /// <summary>
    /// Map Item Selection Adorner.
    /// Handles selection, dragging, resizing and rotation for items in the Level Canvas.
    /// </summary>
    public class CanvasTextureSelectionAdorner : Adorner
    {
        #region Variables & Properties
        // Resizing adorner uses Thumbs for visual elements.  
        protected Thumb topLeft, topRight, bottomLeft, bottomRight, rotateThumb;

        // Rotate cache variables
        protected ItemsControl parentCanvas;
        private double initialAngle;
        private RotateTransform rotateTransform;
        private Vector startVector;
        private Point centerPoint;

        // To store and manage the adorner’s visual children.
        protected VisualCollection visualChildren;

        // Override the VisualChildrenCount and GetVisualChild properties to interface with the adorner’s visual collection.
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }

        // Pen for outline box
        protected Pen linePen;
        #endregion


        #region Constructor
        /// <summary>
        /// Adorner for selected items on the Map Canvas
        /// </summary>
        /// <param name="adornedElement">The item to adorn</param>
        /// <param name="control">The parent canvas control</param>
        public CanvasTextureSelectionAdorner(UIElement adornedElement, ItemsControl control) : base(adornedElement)
        {
            // TODO: This most likely induces a memory leak (change this into a weakreferenced object)
            parentCanvas = control;
            visualChildren = new VisualCollection(this);
            
            // Prepare line pen
            linePen = new Pen(Brushes.LawnGreen, 1.0);
            linePen.DashStyle = DashStyles.Dash;

            // Call a helper method to initialize the Thumbs
            // with a customized cursors.
            BuildAdornerThumb(ref topLeft, Cursors.SizeNWSE);
            BuildAdornerThumb(ref topRight, Cursors.SizeNESW);
            BuildAdornerThumb(ref bottomLeft, Cursors.SizeNESW);
            BuildAdornerThumb(ref bottomRight, Cursors.SizeNWSE);
            // Rotate thumb
            BuildAdornerThumb(ref rotateThumb, Cursors.Cross);


            // Add handlers for resizing.
            bottomLeft.DragDelta += new DragDeltaEventHandler(HandleBottomLeft);
            bottomLeft.DragStarted += new DragStartedEventHandler(HandleResize_DragStarted);

            bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
            bottomRight.DragStarted += new DragStartedEventHandler(HandleResize_DragStarted);
            
            topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeft);
            topLeft.DragStarted += new DragStartedEventHandler(HandleResize_DragStarted);
            
            topRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);
            topRight.DragStarted += new DragStartedEventHandler(HandleResize_DragStarted);

            // Add handler for rotating
            rotateThumb.DragStarted += new DragStartedEventHandler(HandleRotate_DragStarted);
            rotateThumb.DragDelta += new DragDeltaEventHandler(HandleRotate_DragDelta);
            rotateThumb.DragCompleted += new DragCompletedEventHandler(HandleRotate_DragCompleted);
        }
        #endregion


        #region Render Overrides
        /// <summary>
        /// On Render override
        /// </summary>
        /// <param name="drawingContext">The drawing context for the adorned item</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            // Render box
            drawingContext.DrawLine(linePen, new Point(0, 0), new Point(AdornedElement.DesiredSize.Width, 0)); // Top
            drawingContext.DrawLine(linePen, new Point(0, AdornedElement.DesiredSize.Height), new Point(AdornedElement.DesiredSize.Width, AdornedElement.DesiredSize.Height)); // Bottom
            drawingContext.DrawLine(linePen, new Point(0, 0), new Point(0, AdornedElement.DesiredSize.Height)); // Left
            drawingContext.DrawLine(linePen, new Point(AdornedElement.DesiredSize.Width, 0), new Point(AdornedElement.DesiredSize.Width, AdornedElement.DesiredSize.Height)); // Right

            base.OnRender(drawingContext);
        }
        #endregion


        #region Thumb Handlers

        #region Rotate Handlers
        public void HandleRotate_DragStarted(object sender, DragStartedEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            if (adornedElement != null && this.parentCanvas != null)
            {
                Point origin = new Point(
                        adornedElement.ActualWidth * 0.5,//adornedElement.RenderTransformOrigin.X,
                        adornedElement.ActualHeight * 0.5//adornedElement.RenderTransformOrigin.Y
                );

                // Calculate center of the item being rotated (origin + canvas pos)
                this.centerPoint = adornedElement.TranslatePoint(
                    origin,
                    parentCanvas
                );

                // Get the mouse starting vector
                this.startVector = Point.Subtract(
                    Mouse.GetPosition(this.parentCanvas),
                    this.centerPoint
                );

                // Initialize rotate transform
                this.rotateTransform = adornedElement.RenderTransform as RotateTransform;
                if (this.rotateTransform == null)
                {
                    adornedElement.RenderTransform = new RotateTransform(0, origin.X, origin.Y);
                    this.initialAngle = 0;
                }
                else
                    this.initialAngle = this.rotateTransform.Angle;

                // Tag rotation undo
                HandleRotation(this.initialAngle, true);
            }
        }

        public void HandleRotate_DragCompleted(object sender, DragCompletedEventArgs args)
        {
            args.Handled = true;
        }

        public void HandleRotate_DragDelta(object sender, DragDeltaEventArgs args)
        {
            if (this.parentCanvas != null)
            {
                // Get the delta vector for the mouse position
                Vector deltaVector = Point.Subtract(
                    Mouse.GetPosition(this.parentCanvas), 
                    this.centerPoint
                );

                // Translate rotation
                HandleRotation(initialAngle + Math.Round(Vector.AngleBetween(startVector, deltaVector), 0));
            }
        }

        /// <summary>
        /// Helper method that translates the rotation
        /// </summary>
        /// <param name="angle">The rotation angle</param>
        /// <param name="doUndo">Set to true when registering undo action (used on drag start)</param>
        public void HandleRotation(double angle, bool doUndo = false)
        {
            FrameworkElement element = this.AdornedElement as FrameworkElement;
            IRenderable item = element.DataContext as IRenderable;

            if (element != null && item != null)
            {
                // Undo item rotation
                if (doUndo)
                {
                    double a = item.Angle;
                    UndoRedoManager.Instance().Push((dummy) => HandleRotation(a, true), this);
                }

                // Apply angle to rotate transform and save to the canvas item data
                RotateTransform rotateTransform = element.RenderTransform as RotateTransform;
                rotateTransform.Angle = angle;
                item.Angle = (float)rotateTransform.Angle;
                element.InvalidateMeasure();
            }
        }
        #endregion

        #region Resize Handlers
        public void HandleResize_DragStarted(object sender, DragStartedEventArgs args)
        {
            FrameworkElement element = this.AdornedElement as FrameworkElement;
            if (element != null)
            {
                IRenderable item = element.DataContext as IRenderable;
                if (item != null)
                    HandleResize(
                        new Size(item.Width, item.Height),
                        new Point(item.Position.X, item.Position.Y),
                        true
                    );
            }
        }

        // Handler for resizing from the top-right.
        public void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;
            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            IRenderable item = (IRenderable)adornedElement.DataContext;
            if (item != null)
            {
                HandleResize(
                    new Size(
                        Math.Max(item.Width + args.HorizontalChange, hitThumb.DesiredSize.Width),
                        Math.Max(item.Height - args.VerticalChange, hitThumb.DesiredSize.Height)
                    ),
                    new Point(
                        item.Position.X,
                        item.Position.Y - (Math.Max(item.Height - args.VerticalChange, hitThumb.DesiredSize.Height) - item.Height)
                    )
                );
            }
        }

        // Handler for resizing from the bottom-right.
        public void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;
            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            IRenderable item = (IRenderable)adornedElement.DataContext;
            if (item != null)
            {
                HandleResize(
                    new Size(
                        Math.Max(item.Width + args.HorizontalChange, hitThumb.DesiredSize.Width),
                        Math.Max(args.VerticalChange + item.Height, hitThumb.DesiredSize.Height)
                    ),
                    item.Position.Position
                );
            }
        }

        // Handler for resizing from the top-left.
        public void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) 
                return;

            // Make sure the size is not out of bounds
            EnforceSize(adornedElement);

            // Resize item
            IRenderable item = (IRenderable)adornedElement.DataContext;
            if (item != null)
            {
                HandleResize(
                    new Size(
                        Math.Max(item.Width - args.HorizontalChange, hitThumb.DesiredSize.Width),
                        Math.Max(item.Height - args.VerticalChange, hitThumb.DesiredSize.Height)
                    ),
                    new Point(
                        item.Position.X - (Math.Max(item.Width - args.HorizontalChange, hitThumb.DesiredSize.Width) - item.Width),
                        item.Position.Y - (Math.Max(item.Height - args.VerticalChange, hitThumb.DesiredSize.Height) - item.Height)
                    )
                );
            }
        }

        // Handler for resizing from the bottom-left.
        public void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;
            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            IRenderable item = (IRenderable)adornedElement.DataContext;
            if (item != null)
            {
                HandleResize(
                    new Size(
                        Math.Max(item.Width - args.HorizontalChange, hitThumb.DesiredSize.Width),
                        Math.Max(args.VerticalChange + item.Height, hitThumb.DesiredSize.Height)
                    ),
                    new Point(
                        (item.Position.X - (Math.Max(item.Width - args.HorizontalChange, hitThumb.DesiredSize.Width) - item.Width)),
                        item.Position.Y
                    )
                );
            }
        }

        /// <summary>
        /// Handles the resizing
        /// </summary>
        /// <param name="newSize">The new size</param>
        /// <param name="newPosition">The new position</param>
        /// <param name="doUndo">True to save to undo, defaults to false</param>
        public void HandleResize(Size newSize, Point newPosition, bool doUndo = false)
        {
            FrameworkElement element = this.AdornedElement as FrameworkElement;
            IRenderable item = element.DataContext as IRenderable;

            if (element != null && item != null)
            {
                // Undo item rotation
                if (doUndo)
                {
                    Size size = new Size(item.Width, item.Height);
                    Point pos = new Point(item.Position.X, item.Position.Y);
                    UndoRedoManager.Instance().Push((dummy) => HandleResize(size, pos, true), this);
                }

                item.Width = (float)newSize.Width;
                item.Height = (float)newSize.Height;
                item.SetPosition(newPosition);
            }
        }
        #endregion

        #endregion


        #region Setup Helpers
        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        public void BuildAdornerThumb(ref Thumb thumb, Cursor customizedCursor)
        {
            if (thumb != null) 
                return;

            thumb = new Thumb();
            // Set some arbitrary visual characteristics.
            thumb.Cursor = customizedCursor;
            thumb.Height = thumb.Width = 10;
            thumb.Opacity = 1.0;
            thumb.Background = new SolidColorBrush(Colors.LawnGreen);

            visualChildren.Add(thumb);
        }


        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            // desiredWidth and desiredHeight are the width and height of the element that’s being adorned.  
            // These will be used to place the ResizingAdorner at the corners of the adorned element.  
            double desiredWidth = AdornedElement.DesiredSize.Width;
            double desiredHeight = AdornedElement.DesiredSize.Height;
            // adornerWidth & adornerHeight are used for placement as well.
            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;

            topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            topRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            bottomLeft.Arrange(new Rect(-adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            bottomRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));

            rotateThumb.Arrange(new Rect(0, -adornerHeight / 2, adornerWidth, adornerHeight));

            // Return the final size.
            return finalSize;
        }
        #endregion


        #region Other Helpers
        // This method ensures that the Widths and Heights are initialized.  Sizing to content produces
        // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height
        // need to be set first.  It also sets the maximum size of the adorned element.
        public void EnforceSize(FrameworkElement adornedElement)
        {
            IRenderable item = (IRenderable)adornedElement.DataContext;

            if (item.Width.Equals(Double.NaN))
                item.Width = (float)adornedElement.DesiredSize.Width;
            if (item.Height.Equals(Double.NaN))
                item.Height = (float)adornedElement.DesiredSize.Height;
        }
        #endregion

    }
}
