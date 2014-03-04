using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace VectorSpace.Controls
{

    /// <summary> 
    /// A Canvas which manages dragging of the UIElements it contains.   
    /// </summary> 
    /// <remarks> 
    /// Documentation: http://www.codeproject.com/KB/WPF/DraggingElementsInCanvas.aspx 
    /// </remarks> 
    public class DragCanvas : Canvas
    {
        #region Variables & Properties

        #region Interface Properties
        public static readonly DependencyProperty CanBeDraggedProperty;

        /// <summary>
        /// Allow dragging property
        /// </summary>
        public bool AllowDragging
        {
            get { return (bool)base.GetValue(AllowDraggingProperty); }
            set { base.SetValue(AllowDraggingProperty, value); }
        }
        public static readonly DependencyProperty AllowDraggingProperty;

        /// <summary>
        /// Drag out of view property
        /// </summary>
        public bool AllowDragOutOfView
        {
            get { return (bool)GetValue(AllowDragOutOfViewProperty); }
            set { SetValue(AllowDragOutOfViewProperty, value); }
        }
        public static readonly DependencyProperty AllowDragOutOfViewProperty;

        /// <summary>
        /// Holds the element currently being dragged
        /// </summary>
        public UIElement ElementBeingDragged
        {
            get
            {
                if (!this.AllowDragging)
                    return null;
                else
                    return this.elementBeingDragged;
            }
            protected set
            {
                if (this.elementBeingDragged != null)
                    this.elementBeingDragged.ReleaseMouseCapture();

                if (!this.AllowDragging)
                    this.elementBeingDragged = null;
                else
                {
                    if (DragCanvas.GetCanBeDragged(value))
                    {
                        this.elementBeingDragged = value;
                        this.elementBeingDragged.CaptureMouse();
                    }
                    else
                        this.elementBeingDragged = null;
                }
            }
        }
        #endregion

        #region Data Variables
        private UIElement elementBeingDragged;
        private Point origCursorLocation;
        private double origHorizOffset, origVertOffset;
        private bool modifyLeftOffset, modifyTopOffset;
        private bool isDragInProgress;
        #endregion

        #endregion

        
        #region Constructors
        /// <summary>
        /// Static constructor for the DragCanvas
        /// </summary>
        static DragCanvas()
        {
            AllowDraggingProperty = DependencyProperty.Register(
                "AllowDragging",
                typeof(bool),
                typeof(DragCanvas),
                new PropertyMetadata(true)
            );

            AllowDragOutOfViewProperty = DependencyProperty.Register(
                "AllowDragOutOfView",
                typeof(bool),
                typeof(DragCanvas),
                new UIPropertyMetadata(false)
            );

            CanBeDraggedProperty = DependencyProperty.RegisterAttached(
                "CanBeDragged",
                typeof(bool),
                typeof(DragCanvas),
                new UIPropertyMetadata(true)
            );
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DragCanvas()
        {
        }
        #endregion


        #region Attached Property Methods
        /// <summary>
        /// Checks whether an element can be dragged
        /// </summary>
        /// <param name="uiElement">The element to check</param>
        /// <returns>True on success</returns>
        public static bool GetCanBeDragged(UIElement uiElement)
        {
            if (uiElement == null)
                return false;

            return (bool)uiElement.GetValue(CanBeDraggedProperty);
        }

        /// <summary>
        /// Flag whether element can be dragged or not
        /// </summary>
        /// <param name="uiElement">The element to flag</param>
        /// <param name="value">True/False</param>
        public static void SetCanBeDragged(UIElement uiElement, bool value)
        {
            if (uiElement != null)
                uiElement.SetValue(CanBeDraggedProperty, value);
        }

        #endregion


        #region Interface Methods
        /// <summary>
        /// Brings element to the front of the canvas
        /// </summary>
        /// <param name="element"></param>
        public void BringToFront(UIElement element)
        {
            this.UpdateZOrder(element, true);
        }
        
        /// <summary>
        /// Sends the element to the back of the canvas
        /// </summary>
        /// <param name="element"></param>
        public void SendToBack(UIElement element)
        {
            this.UpdateZOrder(element, false);
        }

        /// <summary>
        /// Finds child elements within the canvas
        /// </summary>
        /// <param name="depObj">Element to fid</param>
        /// <returns>The found element</returns>
        public UIElement FindCanvasChild(DependencyObject depObj)
        {
            while (depObj != null)
            {
                UIElement elem = depObj as UIElement;
                if (elem != null && base.Children.Contains(elem))
                    break;
                if (depObj is Visual || depObj is Visual3D)
                    depObj = VisualTreeHelper.GetParent(depObj);
                else
                    depObj = LogicalTreeHelper.GetParent(depObj);
            }
            return depObj as UIElement;
        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Handles preview for left mouse dragging
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            // If we aren't in select mode we bail
            if (MainWindow.EditState != ApplicationEditState.Select)
                return;

            this.origCursorLocation = e.GetPosition(this);
            this.ElementBeingDragged = this.FindCanvasChild(e.Source as DependencyObject);
            if (this.ElementBeingDragged == null)
                return;

            double left = Canvas.GetLeft(this.ElementBeingDragged);
            double right = Canvas.GetRight(this.ElementBeingDragged);
            double top = Canvas.GetTop(this.ElementBeingDragged);
            double bottom = Canvas.GetBottom(this.ElementBeingDragged);
            this.origHorizOffset = ResolveOffset(left, right, out this.modifyLeftOffset);
            this.origVertOffset = ResolveOffset(top, bottom, out this.modifyTopOffset);
            e.Handled = true;

            this.isDragInProgress = true;
        }

        /// <summary>
        /// Handles preview while mouse is moving
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            // If we aren't in select mode we bail
            if (MainWindow.EditState != ApplicationEditState.Select ||
                this.ElementBeingDragged == null || !this.isDragInProgress)
            {
                this.ElementBeingDragged = null;
                this.isDragInProgress = false;
                return;
            }
           
            Point cursorLocation = e.GetPosition(this);
            double newHorizontalOffset, newVerticalOffset;

            // Calculate Offsets
            if (this.modifyLeftOffset)
                newHorizontalOffset = this.origHorizOffset + (cursorLocation.X - this.origCursorLocation.X);
            else
                newHorizontalOffset = this.origHorizOffset - (cursorLocation.X - this.origCursorLocation.X);
            if (this.modifyTopOffset)
                newVerticalOffset = this.origVertOffset + (cursorLocation.Y - this.origCursorLocation.Y);
            else
                newVerticalOffset = this.origVertOffset - (cursorLocation.Y - this.origCursorLocation.Y);
           
            if (!this.AllowDragOutOfView)
            {
                // Verify Drag Element Location
                Rect elemRect = this.CalculateDragElementRect(newHorizontalOffset, newVerticalOffset);
                bool leftAlign = elemRect.Left < 0;
                bool rightAlign = elemRect.Right > this.ActualWidth;

                if (leftAlign)
                    newHorizontalOffset = modifyLeftOffset ? 0 : this.ActualWidth - elemRect.Width;
                else if (rightAlign)
                    newHorizontalOffset = modifyLeftOffset ? this.ActualWidth - elemRect.Width : 0;

                bool topAlign = elemRect.Top < 0;
                bool bottomAlign = elemRect.Bottom > this.ActualHeight;

                if (topAlign)
                    newVerticalOffset = modifyTopOffset ? 0 : this.ActualHeight - elemRect.Height;
                else if (bottomAlign)
                    newVerticalOffset = modifyTopOffset ? this.ActualHeight - elemRect.Height : 0;
            }

            // Move Drag Element
            if (this.modifyLeftOffset)
                Canvas.SetLeft(this.ElementBeingDragged, newHorizontalOffset);
            else
                Canvas.SetRight(this.ElementBeingDragged, newHorizontalOffset);

            if (this.modifyTopOffset)
                Canvas.SetTop(this.ElementBeingDragged, newVerticalOffset);
            else
                Canvas.SetBottom(this.ElementBeingDragged, newVerticalOffset);
        }

        /// <summary>
        /// Preview for mouse button up
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            this.ElementBeingDragged = null;
            this.isDragInProgress = false;
        }
        #endregion


        #region Private Helper Methods
        /// <summary>
        /// Calculkates the rectangle size for the element being dragged
        /// </summary>
        /// <param name="newHorizOffset">The horizontal offset</param>
        /// <param name="newVertOffset">The vertical offset</param>
        /// <returns>The rectangle size</returns>
        private Rect CalculateDragElementRect(double newHorizOffset, double newVertOffset)
        {
            if (this.ElementBeingDragged == null)
                throw new InvalidOperationException("ElementBeingDragged is null.");
            
            Size elemSize = this.ElementBeingDragged.RenderSize;
            double x, y;
            if (this.modifyLeftOffset)
                x = newHorizOffset;
            else
                x = this.ActualWidth - newHorizOffset - elemSize.Width;
            
            if (this.modifyTopOffset)
                y = newVertOffset;
            else
                y = this.ActualHeight - newVertOffset - elemSize.Height;
            
            Point elemLoc = new Point(x, y);
            return new Rect(elemLoc, elemSize);
        }

        /// <summary>
        /// Handles the offset 
        /// </summary>
        /// <param name="side1"></param>
        /// <param name="side2"></param>
        /// <param name="useSide1"></param>
        /// <returns></returns>
        private static double ResolveOffset(double side1, double side2, out bool useSide1)
        {
            useSide1 = true;
            double result;
            if (Double.IsNaN(side1))
            {
                if (Double.IsNaN(side2))
                {
                    result = 0;
                }
                else
                {
                    result = side2;
                    useSide1 = false;
                }
            }
            else
            {
                result = side1;
            }
            return result;
        }

        /// <summary>
        /// Updates the Z order for an item within the canvas
        /// </summary>
        /// <param name="element">The element to update</param>
        /// <param name="bringToFront">True for closer / false for away</param>
        private void UpdateZOrder(UIElement element, bool bringToFront)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (!base.Children.Contains(element))
                throw new ArgumentException("Must be a child element of the Canvas.", "element");

            // Calculate Z-Indexes And Offsets
            int elementNewZIndex = -1;
            if (bringToFront)
            {
                foreach (UIElement elem in base.Children)
                    if (elem.Visibility != Visibility.Collapsed)
                        ++elementNewZIndex;
            }
            else
            {
                elementNewZIndex = 0;
            }
            int offset = (elementNewZIndex == 0) ? +1 : -1;
            int elementCurrentZIndex = Canvas.GetZIndex(element);


            // Update Z-Indexes
            foreach (UIElement childElement in base.Children)
            {
                if (childElement == element)
                    Canvas.SetZIndex(element, elementNewZIndex);
                else
                {
                    int zIndex = Canvas.GetZIndex(childElement);
                    if (bringToFront && elementCurrentZIndex < zIndex ||
                        !bringToFront && zIndex < elementCurrentZIndex)
                    {
                        Canvas.SetZIndex(childElement, zIndex + offset);
                    }
                }
            }
        }
        #endregion
    }     
}
