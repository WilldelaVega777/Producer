using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace ProducerAlpha
{
    public class CircleAdorner : Adorner
    {
        //---------------------------------------------------------------------------------
        // Private Fields Section
        //---------------------------------------------------------------------------------
        private Rectangle _child = null;
        private double _leftOffset = 0;
        private double _topOffset = 0;

        //---------------------------------------------------------------------------------
        // Constructor Method Section
        //---------------------------------------------------------------------------------
        public CircleAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            VisualBrush _brush = new VisualBrush(adornedElement);

            _child = new Rectangle();
            _child.Width = adornedElement.RenderSize.Width;
            _child.Height = adornedElement.RenderSize.Height;


            DoubleAnimation animation = new DoubleAnimation(0.3, 1, new Duration(TimeSpan.FromSeconds(1)));
            animation.AutoReverse = true;
            animation.RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever;
            _brush.BeginAnimation(System.Windows.Media.Brush.OpacityProperty, animation);

            _child.Fill = _brush;
        }


        //---------------------------------------------------------------------------------
        // Override Methods Section
        //---------------------------------------------------------------------------------
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            double renderRadius = 5.0;

            drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
        }

        //---------------------------------------------------------------------------------
        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return _child.DesiredSize;
        }

        //---------------------------------------------------------------------------------
        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        //---------------------------------------------------------------------------------
        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        //---------------------------------------------------------------------------------
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        //---------------------------------------------------------------------------------
        public double LeftOffset
        {
            get
            {
                return _leftOffset;
            }
            set
            {
                _leftOffset = value;
                UpdatePosition();
            }
        }

        //---------------------------------------------------------------------------------
        public double TopOffset
        {
            get
            {
                return _topOffset;
            }
            set
            {
                _topOffset = value;
                UpdatePosition();

            }
        }

        //---------------------------------------------------------------------------------
        private void UpdatePosition()
        {
            AdornerLayer adornerLayer = this.Parent as AdornerLayer;
            if (adornerLayer != null)
            {
                adornerLayer.Update(AdornedElement);
            }
        }

        //---------------------------------------------------------------------------------
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
            return result;
        }
    }
}
