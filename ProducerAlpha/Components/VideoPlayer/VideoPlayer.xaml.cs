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
using System.Windows.Threading;
using WMPLib;

namespace ProducerAlpha
{
    public enum VideoPlayerModes
    { 
        NOT_LOADED,
        LOADING,
        LOADED,
        PLAYING,
        PAUSED,
        STOPPED,
        MAXIMIZED,
        MINIMIZED
    }

    //-------------------------------------------------------------------------------------
    public partial class VideoPlayer : UserControl
    {
        //---------------------------------------------------------------------------------
        // Public Events Section
        //---------------------------------------------------------------------------------
        public event EventHandler Maximized;
        public event EventHandler Minimized;
        public event MouseButtonEventHandler DragButtonDown;
        
        //---------------------------------------------------------------------------------
        // Private Fields Section
        //---------------------------------------------------------------------------------
        private Uri videoUri;
        private double previousVolume;
        private bool hasGotButtonClick;
        private DispatcherTimer OTimer;
        private Storyboard OSBMediaControl;
        private MediaTimeline VideoLoader;

        private VideoPlayerModes currentMode;


        //---------------------------------------------------------------------------------
        // Public Properties Section
        //---------------------------------------------------------------------------------
        public Uri VideoUri
        {
            get
            {
                return this.videoUri;
            }

            set
            {
                this.setMode(VideoPlayerModes.LOADING);
                this.videoUri = value;
                this.OVideo.Volume = .80;
                this.OTimer.Start();
            }
        }

        //---------------------------------------------------------------------------------
        public MediaElement VideoElement
        {
            get
            {
                return this.OVideo;
            }
        }

        //---------------------------------------------------------------------------------
        public Storyboard VideoController
        {
            get
            {
                return this.OSBMediaControl;
            }
        }

        //---------------------------------------------------------------------------------
        public String DebugMessage
        {
            get
            {
                return this.ODebug.Text;
            }

            set
            {
                this.ODebug.Text = value;
            }
        }        

        //---------------------------------------------------------------------------------
        // Constructor Method Section
        //---------------------------------------------------------------------------------
        public VideoPlayer()
        {
            InitializeComponent();

            // Assign Objects
            this.OSBMediaControl                    = (this.Resources["OSBMediaControl"] as Storyboard);
            App.VideoController                     = OSBMediaControl;
            App.PreviewWindow                       = this.OVideo;
            this.VideoLoader                        = ((this.Resources["OSBMediaControl"] as Storyboard).Children[0] as MediaTimeline);

            // Add Event Handlers
            this.cmdDrag.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(cmdDrag_PreviewMouseLeftButtonDown);
            this.cmdDrag.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(cmdDrag_PreviewMouseLeftButtonUp);
            this.cmdDock.MouseLeftButtonUp += new MouseButtonEventHandler(cmdDock_MouseLeftButtonUp);

            this.cmdDock.MouseEnter += new MouseEventHandler(cmdButton_MouseEnter);
            this.cmdDrag.MouseEnter += new MouseEventHandler(cmdButton_MouseEnter);
            this.cmdPlay.MouseEnter += new MouseEventHandler(cmdButton_MouseEnter);
            this.cmdPause.MouseEnter += new MouseEventHandler(cmdButton_MouseEnter);
            this.cmdStop.MouseEnter += new MouseEventHandler(cmdButton_MouseEnter);
            this.cmdDock.MouseLeave += new MouseEventHandler(cmdButton_MouseLeave);
            this.cmdDrag.MouseLeave += new MouseEventHandler(cmdButton_MouseLeave);
            this.cmdPlay.MouseLeave += new MouseEventHandler(cmdButton_MouseLeave);
            this.cmdPause.MouseLeave += new MouseEventHandler(cmdButton_MouseLeave);
            this.cmdStop.MouseLeave += new MouseEventHandler(cmdButton_MouseLeave);

            this.cmdPlay.MouseLeftButtonUp += new MouseButtonEventHandler(cmdPlay_MouseLeftButtonUp);
            this.cmdPause.MouseLeftButtonUp += new MouseButtonEventHandler(cmdPause_MouseLeftButtonUp);
            this.cmdStop.MouseLeftButtonUp += new MouseButtonEventHandler(cmdStop_MouseLeftButtonUp);

            //this.OVideo.
            this.OVideo.MediaEnded += new RoutedEventHandler(OVideo_MediaEnded);

        }

        //---------------------------------------------------------------------------------
        // Event Handler Methods Section
        //---------------------------------------------------------------------------------
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.OTimer = new DispatcherTimer();
            OTimer.Interval = TimeSpan.FromSeconds(2);
            OTimer.Tick += new EventHandler(OTimer_Tick);

            this.setMode(VideoPlayerModes.NOT_LOADED);
        }

        //---------------------------------------------------------------------------------
        private void OTimer_Tick(object sender, EventArgs e)
        {
            this.VideoLoader.Source = this.videoUri;
            this.Load();
            this.setMode(VideoPlayerModes.LOADED);
            this.OTimer.Stop();
        }


        //---------------------------------------------------------------------------------
        private void OControlsPane_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.hasGotButtonClick)
            {
                (this.Resources["OSBAppearControls"] as Storyboard).Begin();
            }
        }

        //---------------------------------------------------------------------------------
        private void OControlsPane_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.hasGotButtonClick)
            {
                (this.Resources["OSBDisappearControls"] as Storyboard).Begin();
            }
        }

        //---------------------------------------------------------------------------------
        private void OMaximizePane_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.hasGotButtonClick)
            {
                (this.Resources["OSBAppearMaximize"] as Storyboard).Begin();
            }
        }

        //---------------------------------------------------------------------------------
        private void OMaximizePane_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.hasGotButtonClick)
            {
                (this.Resources["OSBDisappearMaximize"] as Storyboard).Begin();
            }
        }

        //---------------------------------------------------------------------------------
        private void OControlBox_MouseEnter(object sender, MouseEventArgs e)
        {
            (this.OControlBox.Resources["OControlBoxUnFade"] as Storyboard).Begin();
        }

        //---------------------------------------------------------------------------------
        private void OControlBox_MouseLeave(object sender, MouseEventArgs e)
        {
            (this.OControlBox.Resources["OControlBoxFade"] as Storyboard).Begin();
        }

        //---------------------------------------------------------------------------------
        private void cmdMaximize_Click(object sender, RoutedEventArgs e)
        {
            this.OnMaximize();
        }

        //---------------------------------------------------------------------------------
        private void OSBAppearControls_Completed(object sender, EventArgs e)
        {

        }

        //---------------------------------------------------------------------------------
        private void OSBDisappearControls_Completed(object sender, EventArgs e)
        {

        }

        //---------------------------------------------------------------------------------
        private void OSBAppearMaximize_Completed(object sender, EventArgs e)
        {

        }

        //---------------------------------------------------------------------------------
        private void OSBDisappearMaximize_Completed(object sender, EventArgs e)
        {

        }


        //----------------------------------------------------------------------------------------- 
        // Control Box
        //-----------------------------------------------------------------------------------------
        private void cmdDrag_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.OControlBox.MouseEnter -= new MouseEventHandler(OControlBox_MouseEnter);
            this.OControlBox.MouseLeave -= new MouseEventHandler(OControlBox_MouseLeave);
            this.OnDragButtonDown(sender, e);
        }

        //-----------------------------------------------------------------------------------------
        void cmdDrag_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.OControlBox.MouseEnter += new MouseEventHandler(OControlBox_MouseEnter);
            this.OControlBox.MouseLeave += new MouseEventHandler(OControlBox_MouseLeave);
        }


        //-----------------------------------------------------------------------------------------
        void cmdDock_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            // Dock Action:
            this.OnMinimize();
        }

        //-----------------------------------------------------------------------------------------
        void cmdStop_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            this.Stop();
        }

        //-----------------------------------------------------------------------------------------
        void cmdPause_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            this.Pause();
        }

        //-----------------------------------------------------------------------------------------
        void cmdPlay_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            this.Play();
        }
        
        //---------------------------------------------------------------------------------
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            this.Stop();
        }

        //---------------------------------------------------------------------------------
        private void togglePlay_Click(object sender, RoutedEventArgs e)
        {
            this.hasGotButtonClick = true;

            if (this.togglePlay.IsChecked == true)
            {
                this.Play();
            }
            else
            {
                this.Pause();
            }
        }


        //-----------------------------------------------------------------------------------------
        void cmdButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Canvas OSender = ((Canvas)sender);
            switch (OSender.Name)
            {
                case "cmdDock":
                    Path ODock = ((Path)OSender.FindName("Dock"));
                    ODock.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                    break;

                case "cmdDrag":
                    Rectangle OMove01 = ((Rectangle)OSender.FindName("Move01"));
                    Rectangle OMove02 = ((Rectangle)OSender.FindName("Move02"));
                    Rectangle OMove03 = ((Rectangle)OSender.FindName("Move03"));
                    Rectangle OMove04 = ((Rectangle)OSender.FindName("Move04"));
                    Rectangle OMove05 = ((Rectangle)OSender.FindName("Move05"));
                    Rectangle OMove06 = ((Rectangle)OSender.FindName("Move06"));
                    Rectangle OMove07 = ((Rectangle)OSender.FindName("Move07"));
                    Rectangle OMove08 = ((Rectangle)OSender.FindName("Move08"));
                    Rectangle OMove09 = ((Rectangle)OSender.FindName("Move09"));
                    Rectangle OMove10 = ((Rectangle)OSender.FindName("Move10"));
                    Rectangle OMove11 = ((Rectangle)OSender.FindName("Move11"));
                    Rectangle OMove12 = ((Rectangle)OSender.FindName("Move12"));

                    OMove01.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    OMove02.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    OMove03.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    OMove04.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    OMove05.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    OMove06.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    OMove07.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    OMove08.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    OMove09.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    OMove10.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    OMove11.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    OMove12.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;

                case "cmdPlay":
                    Path OPlay = ((Path)OSender.FindName("OPlay"));
                    OPlay.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                    break;

                case "cmdPause":
                    Rectangle OPause1 = ((Rectangle)OSender.FindName("OPause1"));
                    Rectangle OPause2 = ((Rectangle)OSender.FindName("OPause2"));
                    OPause1.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                    OPause2.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                    break;

                case "cmdStop":
                    Rectangle OStop = ((Rectangle)OSender.FindName("OStop"));
                    OStop.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                    break;
            }
        }

        //-----------------------------------------------------------------------------------------
        void cmdButton_MouseLeave(object sender, EventArgs e)
        {
            Canvas OSender = ((Canvas)sender);
            switch (OSender.Name)
            {
                case "cmdDock":
                    Path ODock = ((Path)OSender.FindName("Dock"));
                    ODock.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xF4, 0xFF, 0xF0));
                    break;

                case "cmdDrag":
                        Rectangle OMove01 = ((Rectangle)OSender.FindName("Move01"));
                        Rectangle OMove02 = ((Rectangle)OSender.FindName("Move02"));
                        Rectangle OMove03 = ((Rectangle)OSender.FindName("Move03"));
                        Rectangle OMove04 = ((Rectangle)OSender.FindName("Move04"));
                        Rectangle OMove05 = ((Rectangle)OSender.FindName("Move05"));
                        Rectangle OMove06 = ((Rectangle)OSender.FindName("Move06"));
                        Rectangle OMove07 = ((Rectangle)OSender.FindName("Move07"));
                        Rectangle OMove08 = ((Rectangle)OSender.FindName("Move08"));
                        Rectangle OMove09 = ((Rectangle)OSender.FindName("Move09"));
                        Rectangle OMove10 = ((Rectangle)OSender.FindName("Move10"));
                        Rectangle OMove11 = ((Rectangle)OSender.FindName("Move11"));
                        Rectangle OMove12 = ((Rectangle)OSender.FindName("Move12"));

                        OMove01.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x57, 0x57));
                        OMove02.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x57, 0x57));
                        OMove03.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x57, 0x57));
                        OMove04.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x57, 0x57));
                        OMove05.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x57, 0x57));
                        OMove06.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x57, 0x57));
                        OMove07.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x57, 0x57));
                        OMove08.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x57, 0x57));
                        OMove09.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x57, 0x57));
                        OMove10.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x57, 0x57));
                        OMove11.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x57, 0x57));
                        OMove12.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x57, 0x57));
                    break;

                case "cmdPlay":
                    this.OPlay.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xF4, 0xFF, 0xF0));
                    break;

                case "cmdPause":
                    OPause1.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xF4, 0xFF, 0xF0));
                    OPause2.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xF4, 0xFF, 0xF0));
                    break;

                case "cmdStop":
                    OStop.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xF4, 0xFF, 0xF0));
                    break;
            }
        }

        //---------------------------------------------------------------------------------
        // OVideo Event Handlers
        //---------------------------------------------------------------------------------
        void OVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.OVideo.Position = TimeSpan.FromSeconds(0);
            this.Pause();
        }

        //---------------------------------------------------------------------------------
        private void OVideo_MediaOpened(object sender, RoutedEventArgs e)
        {
            App.TimelineController.TotalDuration = TimeSpan.FromMilliseconds(this.OVideo.NaturalDuration.TimeSpan.TotalMilliseconds);
        }


        //---------------------------------------------------------------------------------
        // VideoLoader (MediaTimeline) Event Handlers
        //---------------------------------------------------------------------------------
        private void OMediaTimeline_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            if (App.TimelineController != null)
            {
                App.TimelineController.PlayheadPosition = TimeSpan.FromMilliseconds(this.OVideo.Position.TotalMilliseconds);
            }
        }



        //---------------------------------------------------------------------------------
        // Public Methods Section
        //---------------------------------------------------------------------------------
        public void Load()
        {
            WindowsMediaPlayerClass wmp = new WindowsMediaPlayerClass();
            IWMPMedia mediaInfo = wmp.newMedia(this.VideoLoader.Source.AbsolutePath.Replace('/', '\\').Replace("%20"," ")); 
            App.TimelineController.TotalDuration = TimeSpan.FromSeconds(mediaInfo.duration);
        }

        //---------------------------------------------------------------------------------
        public void Play()
        {
            if (this.currentMode != VideoPlayerModes.PAUSED)
            {
                this.VideoController.Begin();
            }
            else
            {
                this.VideoController.Resume();
            }
            this.setMode(VideoPlayerModes.PLAYING);
            this.OMainCanvas.Opacity = 1.0;

        }

        //---------------------------------------------------------------------------------
        public void Pause()
        {
            this.setMode(VideoPlayerModes.PAUSED);
            this.VideoController.Pause();
        }

        //---------------------------------------------------------------------------------
        public void Stop()
        {
            this.setMode(VideoPlayerModes.STOPPED);
            this.VideoController.Stop();
        }

        //---------------------------------------------------------------------------------
        public void SetVideoMute(Boolean pMute)
        {
            if (pMute)
            {
                this.previousVolume = this.OVideo.Volume;
                this.OVideo.Volume = 0.0;
            }
            else
            {
                this.OVideo.Volume = this.previousVolume;
            }
        }

        //---------------------------------------------------------------------------------
        // Private Methods Section
        //---------------------------------------------------------------------------------        
        private void setMode(VideoPlayerModes pMode)
        {
            this.currentMode = pMode;

            switch (pMode)
            { 
                case VideoPlayerModes.NOT_LOADED:
                    this.OVideo.Visibility = Visibility.Collapsed;
                    this.ImagePlaceHolder.Visibility = Visibility.Visible;
                    this.OLoadingAnimation.Visibility = Visibility.Collapsed;
                    this.OControlsPane.Visibility = Visibility.Collapsed;
                    break;

                case VideoPlayerModes.LOADING:
                    this.hasGotButtonClick = false;
                    this.OVideo.Visibility = Visibility.Collapsed;
                    this.ImagePlaceHolder.Visibility = Visibility.Collapsed;
                    this.OLoadingAnimation.Visibility = Visibility.Visible;
                    this.OControlsPane.Visibility = Visibility.Collapsed;
                    break;

                case VideoPlayerModes.LOADED:
                    this.OVideo.Visibility = Visibility.Visible;
                    this.ImagePlaceHolder.Visibility = Visibility.Collapsed;
                    this.OLoadingAnimation.Visibility = Visibility.Collapsed;
                    this.OControlsPane.Visibility = Visibility.Visible;
                    this.OMaximizePane.Visibility = Visibility.Visible;
                    break;

                case VideoPlayerModes.PLAYING:
                    this.cmdPause.Visibility = Visibility.Visible;
                    this.cmdPlay.Visibility = Visibility.Collapsed;
                    this.togglePlay.IsChecked = true;

                    this.OVideo.Visibility = Visibility.Visible;
                    this.ImagePlaceHolder.Visibility = Visibility.Collapsed;
                    break;

                case VideoPlayerModes.PAUSED:
                    this.cmdPause.Visibility = Visibility.Collapsed;
                    this.cmdPlay.Visibility = Visibility.Visible;
                    this.togglePlay.IsChecked = false;
                    break;

                case VideoPlayerModes.STOPPED:
                    this.cmdPause.Visibility = Visibility.Collapsed;
                    this.cmdPlay.Visibility = Visibility.Visible;
                    break;

                case VideoPlayerModes.MAXIMIZED:
                    this.SetValue(UserControl.HeightProperty, 160.0);
                    this.SetValue(UserControl.WidthProperty, 154.0);

                    this.OMainCanvas.SetValue(Canvas.HeightProperty, 160.0);
                    this.OMainCanvas.SetValue(Canvas.WidthProperty, 140.0);

                    this.OVideo.SetValue(Canvas.HeightProperty, 160.0);
                    this.OVideo.SetValue(Canvas.WidthProperty, 140.0);

                    this.OControlBox.Visibility = Visibility.Visible;
                    this.OControlsPane.Visibility = Visibility.Collapsed;

                    this.cmdMaximize.Click -= new System.Windows.RoutedEventHandler(this.cmdMaximize_Click);
                    
                    this.OMaximizePane.MouseEnter -= new System.Windows.Input.MouseEventHandler(this.OMaximizePane_MouseEnter);
                    this.OMaximizePane.MouseLeave -= new System.Windows.Input.MouseEventHandler(this.OMaximizePane_MouseLeave);
                    this.OMaximizePane.Visibility = Visibility.Collapsed;
                    
                    break;

                case VideoPlayerModes.MINIMIZED:
                    this.SetValue(UserControl.HeightProperty, 77.0);
                    this.SetValue(UserControl.WidthProperty, 68.0);

                    this.OMainCanvas.SetValue(Canvas.HeightProperty, 77.0);
                    this.OMainCanvas.SetValue(Canvas.WidthProperty, 68.0);

                    this.OVideo.SetValue(Canvas.HeightProperty, 77.0);
                    this.OVideo.SetValue(Canvas.WidthProperty, 68.0);

                    this.OControlBox.Visibility = Visibility.Collapsed;
                    this.OControlsPane.Visibility = Visibility.Visible;
                    
                    this.cmdMaximize.Click += new System.Windows.RoutedEventHandler(this.cmdMaximize_Click);
                    
                    this.OMaximizePane.MouseEnter += new System.Windows.Input.MouseEventHandler(this.OMaximizePane_MouseEnter);
                    this.OMaximizePane.MouseLeave += new System.Windows.Input.MouseEventHandler(this.OMaximizePane_MouseLeave);
                    this.OMaximizePane.Visibility = Visibility.Visible;
                    (this.Resources["OSBDisappearMaximize"] as Storyboard).Begin();
                    
                    break;

            }
        }


        //---------------------------------------------------------------------------------
        // Event Triggers Section
        //---------------------------------------------------------------------------------
        private void OnMaximize()
        {
            if (this.Maximized != null)
            {
                // Remove MediaElement from Collection
                this.setMode(VideoPlayerModes.MAXIMIZED);

                // Triggers the Event
                this.Maximized(this, new EventArgs());
            }
        }

        //---------------------------------------------------------------------------------
        private void OnMinimize()
        {
            if (this.Minimized != null)
            {
                // Remove MediaElement from Collection
                this.setMode(VideoPlayerModes.MINIMIZED);

                // Triggers the Event
                this.Minimized(this, new EventArgs());
            }
        }

        //---------------------------------------------------------------------------------
        private void OnDragButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.DragButtonDown != null)
            { 
                this.DragButtonDown(this, e);
            }
        }
    }
}
