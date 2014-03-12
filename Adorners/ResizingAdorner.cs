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

namespace VectorSpace.Adorners
{
    public class ResizingAdorner : Adorner
    {
        // Resizing adorner uses Thumbs for visual elements.  
        // The Thumbs have built-in mouse input handling.
        Thumb topLeft, topRight, bottomLeft, bottomRight;

        // To store and manage the adorner’s visual children.
        VisualCollection visualChildren;

        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner’s visual collection.
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }


        public ResizingAdorner(UIElement adornedElement) : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            // Call a helper method to initialize the Thumbs
            // with a customized cursors.
            BuildAdornerCorner(ref topLeft, Cursors.SizeNWSE);
            BuildAdornerCorner(ref topRight, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomLeft, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomRight, Cursors.SizeNWSE);

            // Add handlers for resizing.
            bottomLeft.DragDelta += new DragDeltaEventHandler(HandleBottomLeft);
            bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
            topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeft);
            topRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);
        }


        #region Corner Handlers
        // Handler for resizing from the top-right.
        void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;
            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            TextureItem item = (TextureItem)adornedElement.DataContext;

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


        // Handler for resizing from the bottom-right.
        void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;
            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            TextureItem item = (TextureItem)adornedElement.DataContext;
            // Change the size by the amount the user drags the mouse, as long as it’s larger 
            // than the width or height of an adorner, respectively.
            item.Width = (float)Math.Max(item.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            item.Height = (float)Math.Max(args.VerticalChange + item.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-left.
        void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            TextureItem item = (TextureItem)adornedElement.DataContext;

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

        // Handler for resizing from the bottom-left.
        void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;
            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            TextureItem item = (TextureItem)adornedElement.DataContext;
            
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
        #endregion

        #region Setup Helpers
        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics.
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 10;
            cornerThumb.Opacity = 0.40;
            cornerThumb.Background = new SolidColorBrush(Colors.MediumBlue);

            visualChildren.Add(cornerThumb);
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

            // Return the final size.
            return finalSize;
        }
        #endregion

        // This method ensures that the Widths and Heights are initialized.  Sizing to content produces
        // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height
        // need to be set first.  It also sets the maximum size of the adorned element.
        void EnforceSize(FrameworkElement adornedElement)
        {
            TextureItem item = (TextureItem)adornedElement.DataContext;

            if (item.Width.Equals(Double.NaN))
                item.Width = (float)adornedElement.DesiredSize.Width;
            if (item.Height.Equals(Double.NaN))
                item.Height = (float)adornedElement.DesiredSize.Height;

            /*
            FrameworkElement parent = adornedElement.Parent as FrameworkElement;
            if (parent != null)
            {
                adornedElement.MaxHeight = parent.ActualHeight;
                adornedElement.MaxWidth = parent.ActualWidth;
            }
             */
        }

    }
}
