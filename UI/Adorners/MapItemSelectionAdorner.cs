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
using VectorSpace.MapData.MapItems;

namespace VectorSpace.UI.Adorners
{
    /// <summary>
    /// Map Item Selection Adorner.
    /// Handles selection, dragging, resizing and rotation for items in the Level Canvas.
    /// </summary>
    public class MapItemSelectionAdorner : Adorner
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
        public MapItemSelectionAdorner(UIElement adornedElement, ItemsControl control) : base(adornedElement)
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
            bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
            topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeft);
            topRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);

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
            //TextureItem designerItem = (TextureItem)adornedElement.DataContext;

            if (adornedElement != null)
            {
                if (this.parentCanvas != null)
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
                    {
                        this.initialAngle = this.rotateTransform.Angle;
                    }
                }
            }
        }

        public void HandleRotate_DragCompleted(object sender, DragCompletedEventArgs args)
        {/*
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            if (adornedElement != null)
            {
                TextureItem designerItem = (TextureItem)adornedElement.DataContext;
                if (designerItem != null)
                {
                    RotateTransform rotateTransform = adornedElement.RenderTransform as RotateTransform;
                    designerItem.Angle = (float)rotateTransform.Angle;
                    //rotateTransform.Angle = 0f;
                    adornedElement.InvalidateMeasure();
                }
            }
          */
            args.Handled = true;
        }

        public void HandleRotate_DragDelta(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            TextureItem canvasItem = (TextureItem)adornedElement.DataContext;

            if (canvasItem != null && adornedElement != null && this.parentCanvas != null)
            {
                // Get the delta vector for the mouse position
                Vector deltaVector = Point.Subtract(
                    Mouse.GetPosition(this.parentCanvas), 
                    this.centerPoint
                );

                // Apply angle to rotate transform and save to the canvas item data
                RotateTransform rotateTransform = adornedElement.RenderTransform as RotateTransform;
                rotateTransform.Angle = this.initialAngle + Math.Round(Vector.AngleBetween(this.startVector, deltaVector), 0);
                canvasItem.Angle = (float)rotateTransform.Angle;
                adornedElement.InvalidateMeasure();
            }
        }
        #endregion

        #region Resize Handlers
        // Handler for resizing from the top-right.
        public void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;
            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            TextureItem item = (TextureItem)adornedElement.DataContext;
            if (item != null)
            {
                // Change the size by the amount the user drags the mouse, as long as it’s larger 
                // than the width or height of an adorner, respectively.
                //adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
                item.Width = (float)Math.Max(item.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);

                float height_old = item.Height;
                float height_new = (float)Math.Max(item.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
                float top_old = item.Position.Y;
                item.Height = height_new;

                // Adjust position
                item.SetPosition(
                    item.Position.X,
                    (int)(top_old - (height_new - height_old))
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

            TextureItem item = (TextureItem)adornedElement.DataContext;
            if (item != null)
            {
                // Change the size by the amount the user drags the mouse, as long as it’s larger 
                // than the width or height of an adorner, respectively.
                item.Width = (float)Math.Max(item.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
                item.Height = (float)Math.Max(args.VerticalChange + item.Height, hitThumb.DesiredSize.Height);
            }
        }

        // Handler for resizing from the top-left.
        public void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            TextureItem item = (TextureItem)adornedElement.DataContext;
            if (item != null)
            {
                float width_old = item.Width;
                float width_new = (float)Math.Max(item.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
                float left_old = item.Position.X;
                item.Width = width_new;

                float height_old = item.Height;
                float height_new = (float)Math.Max(item.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
                float top_old = item.Position.Y;
                item.Height = height_new;

                item.SetPosition(
                    (int)(left_old - (width_new - width_old)),
                    (int)(top_old - (height_new - height_old))
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

            TextureItem item = (TextureItem)adornedElement.DataContext;
            if (item != null)
            {
                // Change the size by the amount the user drags the mouse, as long as it’s larger 
                // than the width or height of an adorner, respectively.
                //adornedElement.Width = Math.Max(adornedElement.Width – args.HorizontalChange, hitThumb.DesiredSize.Width);
                item.Height = (float)Math.Max(args.VerticalChange + item.Height, hitThumb.DesiredSize.Height);

                float width_old = item.Width;
                float width_new = (float)Math.Max(item.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
                float left_old = item.Position.X;
                item.Width = width_new;

                item.SetPosition(
                    (int)(left_old - (width_new - width_old)),
                    item.Position.Y
                );
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
            TextureItem item = (TextureItem)adornedElement.DataContext;

            if (item.Width.Equals(Double.NaN))
                item.Width = (float)adornedElement.DesiredSize.Width;
            if (item.Height.Equals(Double.NaN))
                item.Height = (float)adornedElement.DesiredSize.Height;
        }
        #endregion

    }
}
