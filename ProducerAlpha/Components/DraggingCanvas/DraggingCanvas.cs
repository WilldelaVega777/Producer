using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ProducerAlpha
{
    public class DragCanvas : Canvas
    {
        //----------------------------------------------------------------------------------------------------
        // Private Fields Section
        //----------------------------------------------------------------------------------------------------
        private UIElement elementBeingDragged;
        private Point origCursorLocation;
        private double origHorizOffset, origVertOffset;
        private bool modifyLeftOffset, modifyTopOffset;
        private bool isDragInProgress;


        //----------------------------------------------------------------------------------------------------
        // Private Fields Section
        //----------------------------------------------------------------------------------------------------
        // CanBeDragged
        //----------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty CanBeDraggedProperty;
        public static bool GetCanBeDragged(UIElement uiElement)
        {
            if (uiElement == null)
            {
                return false;
            }

            return (bool)uiElement.GetValue(CanBeDraggedProperty);
        }
        public static void SetCanBeDragged(UIElement uiElement, bool value)
        {
            if (uiElement != null)
            {
                uiElement.SetValue(CanBeDraggedProperty, value);
            }
        }


        //----------------------------------------------------------------------------------------------------
        // Dependency Properties Section
        //----------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty AllowDraggingProperty;
        public static readonly DependencyProperty AllowDragOutOfViewProperty;


        //----------------------------------------------------------------------------------------------------
        // Static Constructors Section
        //----------------------------------------------------------------------------------------------------
        static DragCanvas()
        {
            AllowDraggingProperty = DependencyProperty.Register(
                "AllowDragging",
                typeof(bool),
                typeof(DragCanvas),
                new PropertyMetadata(true));

            AllowDragOutOfViewProperty = DependencyProperty.Register(
                "AllowDragOutOfView",
                typeof(bool),
                typeof(DragCanvas),
                new UIPropertyMetadata(false));

            CanBeDraggedProperty = DependencyProperty.RegisterAttached(
                "CanBeDragged",
                typeof(bool),
                typeof(DragCanvas),
                new UIPropertyMetadata(true));
        }


        //----------------------------------------------------------------------------------------------------
        // Interfaces Implementation Section
        //----------------------------------------------------------------------------------------------------
        public DragCanvas()
        {

        }

        //----------------------------------------------------------------------------------------------------
        // Constructor Method Section
        //----------------------------------------------------------------------------------------------------
        // AllowDragging
        //----------------------------------------------------------------------------------------------------
        public bool AllowDragging
        {
            get { return (bool)base.GetValue(AllowDraggingProperty); }
            set { base.SetValue(AllowDraggingProperty, value); }
        }


        //----------------------------------------------------------------------------------------------------
        // AllowDraggingOutOfView
        //----------------------------------------------------------------------------------------------------
        public bool AllowDragOutOfView
        {
            get { return (bool)GetValue(AllowDragOutOfViewProperty); }
            set { SetValue(AllowDragOutOfViewProperty, value); }
        }

        //----------------------------------------------------------------------------------------------------
        // BringToFront / SedToBack
        //----------------------------------------------------------------------------------------------------
        public void BringToFront(UIElement element)
        {
            this.UpdateZOrder(element, true);
        }

        //----------------------------------------------------------------------------------------------------
        public void SendToBack(UIElement element)
        {
            this.UpdateZOrder(element, false);
        }


        //----------------------------------------------------------------------------------------------------
        // ElementBeingDragged
        //----------------------------------------------------------------------------------------------------        
        public UIElement ElementBeingDragged
        {
            get
            {
                if (!this.AllowDragging)
                {
                    return null;
                }
                else
                {
                    return this.elementBeingDragged;
                }
            }
            protected set
            {
                if (this.elementBeingDragged != null)
                {
                    this.elementBeingDragged.ReleaseMouseCapture();
                }

                if (!this.AllowDragging)
                {
                    this.elementBeingDragged = null;
                }
                else
                {
                    if (DragCanvas.GetCanBeDragged(value))
                    {
                        this.elementBeingDragged = value;
                        this.elementBeingDragged.CaptureMouse();
                    }
                    else
                    {
                        this.elementBeingDragged = null;
                    }
                }
            }
        }


        //----------------------------------------------------------------------------------------------------
        // FindCanvasChild
        //----------------------------------------------------------------------------------------------------
        public UIElement FindCanvasChild(DependencyObject depObj)
        {
            while (depObj != null)
            {
                // If the current object is a UIElement which is a child of the
                // Canvas, exit the loop and return it.
                UIElement elem = depObj as UIElement;
                if (elem != null && base.Children.Contains(elem))
                    break;

                // VisualTreeHelper works with objects of type Visual or Visual3D.
                // If the current object is not derived from Visual or Visual3D,
                // then use the LogicalTreeHelper to find the parent element.
                if (depObj is Visual || depObj is Visual3D)
                    depObj = VisualTreeHelper.GetParent(depObj);
                else
                    depObj = LogicalTreeHelper.GetParent(depObj);
            }
            return depObj as UIElement;
        }


        //----------------------------------------------------------------------------------------------------
        // Override Methods Section
        //----------------------------------------------------------------------------------------------------
        // OnPreviewMouseLeftButtonDown
        //----------------------------------------------------------------------------------------------------
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            this.isDragInProgress = false;


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


        //----------------------------------------------------------------------------------------------------
        // OnPreviewMouseMove
        //----------------------------------------------------------------------------------------------------
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (this.ElementBeingDragged == null || !this.isDragInProgress)
            {
                return;
            }

            Point cursorLocation = e.GetPosition(this);

            double newHorizontalOffset, newVerticalOffset;


            // Determine the horizontal offset.
            if (this.modifyLeftOffset)
            {
                newHorizontalOffset = this.origHorizOffset + (cursorLocation.X - this.origCursorLocation.X);
            }
            else
            {
                newHorizontalOffset = this.origHorizOffset - (cursorLocation.X - this.origCursorLocation.X);
            }

            // Determine the vertical offset.
            if (this.modifyTopOffset)
            {
                newVerticalOffset = this.origVertOffset + (cursorLocation.Y - this.origCursorLocation.Y);
            }
            else
            {
                newVerticalOffset = this.origVertOffset - (cursorLocation.Y - this.origCursorLocation.Y);
            }


            if (!this.AllowDragOutOfView)
            {
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

            if (this.modifyLeftOffset)
                Canvas.SetLeft(this.ElementBeingDragged, newHorizontalOffset);
            else
                Canvas.SetRight(this.ElementBeingDragged, newHorizontalOffset);

            if (this.modifyTopOffset)
                Canvas.SetTop(this.ElementBeingDragged, newVerticalOffset);
            else
                Canvas.SetBottom(this.ElementBeingDragged, newVerticalOffset);

        }


        //----------------------------------------------------------------------------------------------------
        // OnHostPreviewMouseUp
        //----------------------------------------------------------------------------------------------------
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            this.ElementBeingDragged = null;
        }


        //----------------------------------------------------------------------------------------------------
        // Private Methods Section
        //----------------------------------------------------------------------------------------------------
        // CalculateDragElementRect
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


        //----------------------------------------------------------------------------------------------------
        // ResolveOffset
        //----------------------------------------------------------------------------------------------------
        private static double ResolveOffset(double side1, double side2, out bool useSide1)
        {
            useSide1 = true;
            double result;
            if (Double.IsNaN(side1))
            {
                if (Double.IsNaN(side2))
                {
                    // Both sides have no value, so set the
                    // first side to a value of zero.
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


        //----------------------------------------------------------------------------------------------------
        // UpdateZOrder
        //----------------------------------------------------------------------------------------------------
        private void UpdateZOrder(UIElement element, bool bringToFront)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (!base.Children.Contains(element))
                throw new ArgumentException("Must be a child element of the Canvas.", "element");

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
    }
}