using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace ProducerAlpha
{
    public enum enumBottomBarStates
    { 
        NORMAL,
        AUTOHIDING,
        VIDEO_MAXIMIZED,
        VIDEO_MINIMIZED
    }


    public partial class BottomBar : UserControl
    {
        //---------------------------------------------------------------------------------
        // Private Fields Section
        //---------------------------------------------------------------------------------
        private Boolean isHidden;
        private Boolean isMoving;

        private enumBottomBarStates currentState;





        private Point _startPoint;
        private double _originalLeft;
        private double _originalTop;
        private bool _isDown;
        private bool _isDragging;
        private UIElement _originalElement;
        private VideoAdorner _overlayElement;

        private double previousLeftOffset;
        Point CurrentPositionToSubtract;


        //---------------------------------------------------------------------------------
        // Private Fields Section
        //---------------------------------------------------------------------------------
        public BottomBar()
        {
            InitializeComponent();

            this.ExternalContainer.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(MyCanvas_PreviewMouseMove);
            this.ExternalContainer.PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(MyCanvas_PreviewMouseLeftButtonUp);
            this.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(BottomBar_PreviewKeyDown);

            this.OVideoPlayer.DragButtonDown += new MouseButtonEventHandler(OVideoPlayer_DragButtonDown);

        }

        //---------------------------------------------------------------------------------
        // Event Handler Methods Section
        //---------------------------------------------------------------------------------
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.isHidden = false;
            this.isMoving = false;

            this.setMode(enumBottomBarStates.NORMAL);

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(BottomBar_DataContextChanged);
            this.OVideoPlayer.Maximized += new EventHandler(OVideoPlayer_Maximized);
            this.OVideoPlayer.Minimized += new EventHandler(OVideoPlayer_Minimized);

        }

        //---------------------------------------------------------------------------------
        private void AutoHideHotspot_MouseEnter(object sender, MouseEventArgs e)
        {
            if ((this.chkAutoHide.IsChecked == true) && (this.isHidden == true) && (this.isMoving == false))
            {
                this.isMoving = true;
                (this.Resources["OSBAutoHide_Showing"] as Storyboard).Begin();
            }
        }

        //---------------------------------------------------------------------------------
        private void ExternalContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            if ((this.chkAutoHide.IsChecked == true) && (this.isHidden == false) && (this.isMoving == false))
            {
                this.isMoving = true;
                (this.Resources["OSBAutoHide_Hidding"] as Storyboard).Begin();
            }
        }

        //---------------------------------------------------------------------------------
        private void BottomBar_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.WSFLPlaceHolder.DataContext = null;
            this.OIdentityBar.DataContext = null;

            this.WSFLPlaceHolder.DataContext = this.DataContext;
            this.OIdentityBar.DataContext = this.DataContext;
        }

        //---------------------------------------------------------------------------------
        void OVideoPlayer_Maximized(object sender, EventArgs e)
        {
            this.ExternalContainer.Clip = null;
            OVideoPlayer.SetValue(Canvas.TopProperty, -300.0);
            OVideoPlayer.SetValue(Canvas.LeftProperty, 300.0);

            this.setMode(enumBottomBarStates.VIDEO_MAXIMIZED);
        }

        //---------------------------------------------------------------------------------
        void OVideoPlayer_Minimized(object sender, EventArgs e)
        {
            this.ExternalContainer.Clip = this.OClip;
            OVideoPlayer.SetValue(Canvas.TopProperty, 0.0);
            OVideoPlayer.SetValue(Canvas.LeftProperty, 0.0);

            this.setMode(enumBottomBarStates.VIDEO_MINIMIZED);
        }


        //---------------------------------------------------------------------------------
        // Storyboard Completed Events
        //---------------------------------------------------------------------------------
        private void OSBAutoHideHidding_Completed(object sender, EventArgs e)
        {
            this.isHidden = true;
            this.isMoving = false;
        }

        //---------------------------------------------------------------------------------
        private void OSBAutoHideShowing_Completed(object sender, EventArgs e)
        {
            this.isHidden = false;
            this.isMoving = false;
        }

        //---------------------------------------------------------------------------------
        private void OSBCenterWSFL_Completed(object sender, EventArgs e)
        {

        }

        //---------------------------------------------------------------------------------
        private void OSBRestoreWSFL_Completed(object sender, EventArgs e)
        {

        }



        //---------------------------------------------------------------------------------
        // Drag Video Interface
        //---------------------------------------------------------------------------------
        void OVideoPlayer_DragButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MyCanvas_PreviewMouseLeftButtonDown(sender, e);
        }

        //---------------------------------------------------------------------------------
        void MyCanvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.Source == this.ExternalContainer)
            {
            }
            else
            {
                _isDown = true;
                _startPoint = e.GetPosition(this.ExternalContainer);
                _originalElement = (this.OVideoPlayer as UIElement);
                this.ExternalContainer.CaptureMouse();
                this.CurrentPositionToSubtract = (System.Windows.Input.Mouse.GetPosition(this.OVideoPlayer));
                e.Handled = true;
            }
        }

        //---------------------------------------------------------------------------------
        void MyCanvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isDown)
            {
                if ((_isDragging == false) && ((Math.Abs(e.GetPosition(this.ExternalContainer).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(this.ExternalContainer).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    DragStarted();
                }
                if (_isDragging)
                {
                    DragMoved();
                }
            }
        }

        //---------------------------------------------------------------------------------
        private void DragStarted()
        {
            _isDragging = true;
            _originalLeft = Canvas.GetLeft(_originalElement);
            _originalTop = Canvas.GetTop(_originalElement);

            _overlayElement = new VideoAdorner(_originalElement);
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
            layer.Add(_overlayElement);

        }

        //---------------------------------------------------------------------------------
        private void DragMoved()
        {
            // -------------- >>> EXPERIMENTAL <<< ------------------

            // Margins
            double LEFT_MARGIN      = 0.0;
            double RIGHT_MARGIN     = 1024;
            double TOP_MARGIN       = 0.0;
            double BOTTOM_MARGIN    = 576.0;

            // Current Position
            Point CurrentPosition = (Mouse.GetPosition(this.ExternalContainer));

            // Test Variables
            double testLeft = (CurrentPosition.X - CurrentPositionToSubtract.X);
            double testRight = CurrentPosition.X;
            double testTop = (CurrentPosition.Y - CurrentPositionToSubtract.Y) + (BOTTOM_MARGIN - this.ActualHeight);
            double testBottom = (((CurrentPosition.Y + this.OVideoPlayer.ActualHeight) - CurrentPositionToSubtract.Y) + (BOTTOM_MARGIN - this.ActualHeight));

            bool leftAlign      = (testLeft < LEFT_MARGIN);
            bool rightAlign     = (testRight > RIGHT_MARGIN);
            bool topAlign       = (testTop < TOP_MARGIN);
            bool bottomAlign    = (testBottom > BOTTOM_MARGIN);

            // Horizontal
            if (leftAlign)
            {
                if (this.previousLeftOffset >= _overlayElement.LeftOffset)
                {
                    _overlayElement.LeftOffset = (((this.CurrentPositionToSubtract.X + this.OVideoPlayer.ActualWidth) * App.currentZoom) * -1);
                }
            }
            else if (rightAlign)
            {
                _overlayElement.LeftOffset = 0.0;
            }
            else
            {
                _overlayElement.LeftOffset = ((CurrentPosition.X - _startPoint.X) * App.currentZoom);
            }


            // Vertical
            if (topAlign)
            {
                _overlayElement.TopOffset = 0.0;
            }
            else if (bottomAlign)
            {
                _overlayElement.TopOffset = 0.0;
            }
            else
            {
                _overlayElement.TopOffset = ((CurrentPosition.Y - _startPoint.Y) * App.currentZoom);
            }


            // Debug:
            //this.OVideoPlayer.DebugMessage = "X: " + (CurrentPosition.X - CurrentPositionToSubtract.X).ToString() + "\n" +
            //  "Y: " + (CurrentPosition.Y - (Mouse.GetPosition(this.OVideoPlayer).Y)).ToString() + "\n" +
            //  "testTop: " + testTop.ToString() + "\n" +
            //  "testBottom: " + testBottom.ToString() + "\n";

            //this.OVideoPlayer.DebugMessage = "X: " + (CurrentPosition.X - CurrentPositionToSubtract.X).ToString() + "\n" +
            //  "topAlign " + topAlign.ToString() + "\n" +
            //  "bottomAlign: " + bottomAlign.ToString();

            this.OVideoPlayer.DebugMessage = "Experimental Sub";

            // Updates Position Buffer
            this.previousLeftOffset = _overlayElement.LeftOffset;

        }



        //---------------------------------------------------------------------------------
        void MyCanvas_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_isDown)
            {
                DragFinished(false);
                e.Handled = true;
            }
        }

        //---------------------------------------------------------------------------------
        private void DragFinished(bool cancelled)
        {
            System.Windows.Input.Mouse.Capture(null);
            if (_isDragging)
            {
                AdornerLayer.GetAdornerLayer(_overlayElement.AdornedElement).Remove(_overlayElement);

                if (cancelled == false)
                {
                    Canvas.SetTop(_originalElement, _originalTop   + (_overlayElement.TopOffset   / App.currentZoom));
                    Canvas.SetLeft(_originalElement, _originalLeft + (_overlayElement.LeftOffset  / App.currentZoom));
                }
                _overlayElement = null;

            }
            _isDragging = false;
            _isDown = false;
        }


        //---------------------------------------------------------------------------------
        void BottomBar_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape && _isDragging)
            {
                DragFinished(true);
            }
        }

        //---------------------------------------------------------------------------------
        // Public Methods Section
        //---------------------------------------------------------------------------------
        public void SetVideo(Uri pVideoUri)
        {
            this.OVideoPlayer.VideoUri = pVideoUri;
        }

        //---------------------------------------------------------------------------------
        public void PlayVideo()
        {
            this.OVideoPlayer.Play();
        }

        //---------------------------------------------------------------------------------
        public void SetVideoMute(Boolean bMute)
        {
            this.OVideoPlayer.SetVideoMute(bMute);
        }


        //---------------------------------------------------------------------------------
        // Private Methods Section
        //---------------------------------------------------------------------------------
        private void setMode(enumBottomBarStates pState)
        {
            this.currentState = pState;

            switch (this.currentState)
            { 
                case enumBottomBarStates.NORMAL:
                    this.chkAutoHide.IsEnabled = true;
                    this.chkAutoHide.Visibility = Visibility.Visible;

                    break;

                case enumBottomBarStates.AUTOHIDING:

                    break;

                case enumBottomBarStates.VIDEO_MAXIMIZED:
                    this.chkAutoHide.IsEnabled = false;
                    this.chkAutoHide.Visibility = Visibility.Collapsed;
                    this.OIdentityBar.Visibility = Visibility.Collapsed;
                    (this.Resources["OSBCenterWSFL"] as Storyboard).Begin();

                    break;

                case enumBottomBarStates.VIDEO_MINIMIZED:
                    this.chkAutoHide.IsEnabled = true;
                    this.chkAutoHide.Visibility = Visibility.Visible;
                    (this.Resources["OSBRestoreWSFL"] as Storyboard).Begin();
                    this.OIdentityBar.Visibility = Visibility.Visible;
                    this.setMode(enumBottomBarStates.NORMAL);

                    break;
            }
        }

    }
}
