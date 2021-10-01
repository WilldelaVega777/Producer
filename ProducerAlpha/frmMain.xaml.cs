using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using System.IO;
using System.Globalization;
using Ookii.Dialogs.Wpf;
using System.Xml;
using System.Windows.Markup;
using System.ComponentModel;
using TimelineController.Components;
using Microsoft.Expression.Encoder;
using Ionic;
using Ionic.Zip;
using Ionic.Zlib;
using System.Net.Cache;
using System.Net;
using ProducerAlpha.DirectoryServer;


namespace ProducerAlpha
{
    public delegate void AsyncDelegate();

	public partial class frmMain : Window
	{

        //----------------------------------------------------------------------------------------------------------
        // Public Constants Section
        //----------------------------------------------------------------------------------------------------------
        public const Int32 WM_EXITSIZEMOVE                                      = 0x232;

        public const double MAIN_LEFT_PANE_TOP_MARGIN                           = 1;
        public const double COLLAPSED_EXPANDER_PLUS_BOTTOM_MARGIN_HEIGHT        = 27;
        public const double COLLAPSED_EXPANDER_HEIGHT                           = 24;
        public const double SHOW_INFO_CONTRIBUTORS_OPEN                         = 544;
        public const double SHOW_INFO_CONTRIBUTORS_CLOSED                       = 245;
        public const int    SCRIPT_PANE                                         = 0;
        public const int    SHOWINFO_PANE                                       = 0;

        public const double MAX_CHANNEL_LOGO_WIDTH                              = 107;
        public const double MAX_CHANNEL_LOGO_HEIGHT                             = 88;
        public const double MIN_CHANNEL_LOGO_WIDTH                              = 80;
        public const double MIN_CHANNEL_LOGO_HEIGHT                             = 66;

        public const double MAX_FRAME_WIDTH                                     = 2048;
        public const double MAX_FRAME_HEIGHT                                    = 1152;
        public const double MIN_FRAME_WIDTH                                     = 1024;
        public const double MIN_FRAME_HEIGHT                                    = 576;

        public const double DEFAULT_MEMORIZED_SCRIPT_PANE_HEIGHT                = 335;

        
        //----------------------------------------------------------------------------------------------------------
        // Public Fields (Application Level Globals)
        //----------------------------------------------------------------------------------------------------------
        public ObservableCollection<Script> scripts;
        public BindingList<Frame> framesWithXamlContent;
        public BindingList<Asset> brokenResourceLinks;

        //----------------------------------------------------------------------------------------------------------
        // Private Fields (Application Level Globals)
        //----------------------------------------------------------------------------------------------------------
        private int idCounter;
        private bool bLoaded;

        private bool t1EventComesFromExpandOrCollapse;
        private bool t2EventComesFromExpandOrCollapse;

        private Frame currentFrame;

        // The Project Path
        private string ProjectFullPath;
        private string ProjectDirectory;
        private string PublishFullPath;
        private string PublishDirectory;
        private string PublishFileName;
        private string SettingsDirectory;
        private string ProjectFile;
        private string VideoPath;

        private Project currentProject;
        private Show currentShow;
        private LayoutSettings LayoutSettings;

        private String currentXamlContent;

        // Application Mode
        private enumMainAppModes currentMode;
        private enumMainAppModes previousMode;

        // Exception Handler
        ExceptionSink OExceptionSink;

        // Control Framework Limited Preview
        private Point _startPoint;
        private double _originalLeft;
        private double _originalTop;
        private bool _isDown;
        private bool _isDragging;
        private UIElement _originalElement;
        
        
        // Publishing
        private CircleAdorner _overlayElement;        
        private bool cancelEncode;
        private Thread publishThread;
        private CookieContainer Cookies;
        private DirectorySuscriptions ODirectoryServer;


        //----------------------------------------------------------------------------------------------------------
        // Dependency Properties Section
        //----------------------------------------------------------------------------------------------------------
        public static readonly DependencyProperty IsEncodingIdleProperty = DependencyProperty.Register("IsEncodingIdle", typeof(bool), typeof(frmMain), new PropertyMetadata(true));

        //----------------------------------------------------------------------------------------------------------
        // Properties Section
        //----------------------------------------------------------------------------------------------------------
        public bool IsEncodingIdle
        {
            get
            {
                return (bool)GetValue(IsEncodingIdleProperty);
            }
        }

        //----------------------------------------------------------------------------------------------------------
        // Routed Commands Section
        //----------------------------------------------------------------------------------------------------------        
        public static readonly RoutedUICommand ImportCommand            = new RoutedUICommand(Properties.Resources.ImportCommand, "Import", typeof(frmMain));
        public static readonly RoutedUICommand ExportCommand            = new RoutedUICommand(Properties.Resources.ExportCommand, "Export", typeof(frmMain));
        public static readonly RoutedUICommand PreviewCommand           = new RoutedUICommand(Properties.Resources.PreviewCommand, "Preview", typeof(frmMain));
        public static readonly RoutedUICommand PublishCommand           = new RoutedUICommand(Properties.Resources.PublishCommand, "Publish", typeof(frmMain));
        public static readonly RoutedUICommand HelpContentsCommand      = new RoutedUICommand(Properties.Resources.HelpContentsCommand, "HelpContents", typeof(frmMain));
        public static readonly RoutedUICommand AboutCommand             = new RoutedUICommand(Properties.Resources.AboutCommand, "AboutProducer", typeof(frmMain));

        public static readonly RoutedUICommand SaveConsole              = new RoutedUICommand(Properties.Resources.SaveConsoleCommand, "SaveConsole", typeof(frmMain));
        public static readonly RoutedUICommand ClearConsole             = new RoutedUICommand(Properties.Resources.ClearConsoleCommand, "ClearConsole", typeof(frmMain));

        //----------------------------------------------------------------------------------------------------------
        // Constructor Section
        //----------------------------------------------------------------------------------------------------------
		public frmMain()
		{
            // Create Xaml Components
            this.InitializeComponent();

            // Load App Objects
            App.TimelineController = this.OTLC;

            App.TimelineController.PreviewWindow    = this.OBBar.OVideoPlayer.VideoElement;
            App.TimelineController.VideoController  = this.OBBar.OVideoPlayer.VideoController;


            // Create Project Object / Settings
            this.currentProject = new Project();
            this.LayoutSettings = this.currentProject.Settings.LayoutSettings;
            this.framesWithXamlContent = new BindingList<Frame>();
            this.brokenResourceLinks = new BindingList<Asset>();

            // Assign values to Config Fields
            this.ProjectFullPath =
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            System.IO.Path.Combine(App.ApplicationFolderName, @"Projects\NewProject.wayvproj"));

            this.SettingsDirectory =
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            System.IO.Path.Combine(App.ApplicationFolderName, @"Settings"));

            this.ProjectDirectory =
            System.IO.Path.GetDirectoryName(ProjectFullPath);

            this.ProjectFile =
            System.IO.Path.GetFileName(ProjectFullPath);

            // Layout Settings
            this.LayoutSettings.ShowPaneTopPanelHeight = SHOW_INFO_CONTRIBUTORS_CLOSED;

            // Exception Handler
            this.OExceptionSink = new ExceptionSink();
            this.OExceptionSink.UIManaging += new ExceptionEventHandler(OExceptionSink_UIManaging);
            this.OExceptionSink.TraceReporting += new ExceptionEventHandler(OExceptionSink_TraceReporting);

            // Error Reporter
            this.OErrorReporter.Close += new EventHandler(OErrorReporter_Close);
            this.OErrorReporter.Save += new ExceptionEventHandler(OErrorReporter_Save);

            // Control Framework Basic Preview
            this.ControlsPlaceHolder.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ControlsPlaceHolder_PreviewMouseLeftButtonDown);
            this.ControlsPlaceHolder.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(ControlsPlaceHolder_PreviewMouseMove);
            this.ControlsPlaceHolder.PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(ControlsPlaceHolder_PreviewMouseLeftButtonUp);
            this.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(ControlsPlaceHolder_PreviewKeyDown);

            this.VideoPath = "WebShowVideo.wmv";

            this.currentXamlContent = "";

            // Directory Suscriptions Proxy
            this.ODirectoryServer = new DirectoryServer.DirectorySuscriptions();
            this.Cookies = new CookieContainer();
            ODirectoryServer.CookieContainer = this.Cookies;
		}

        //----------------------------------------------------------------------------------------------------------
        void currentShow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        //----------------------------------------------------------------------------------------------------------
        // Event Handler Section
        //----------------------------------------------------------------------------------------------------------
        // Main Window
        //----------------------------------------------------------------------------------------------------------
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.bLoaded = true;
            this.setMode(enumMainAppModes.START);
            this.setMode(enumMainAppModes.WELCOME);
            this.ZoomToExtents();
        }

        //----------------------------------------------------------------------------------------------------------
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this.IsEncodingIdle == false)
            {
                MessageBoxResult result = MessageBox.Show("This Webshow is still publishing. Are you sure you want to exit?", "Producer Alpha", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    this.cancelEncode = true;
                    this.publishThread.Join();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }



        //----------------------------------------------------------------------------------------------------------
        // Main Menu: Commands
        //----------------------------------------------------------------------------------------------------------
        private void CommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command.Equals(ApplicationCommands.New))
            {
                this.createNewShow();
            }

            if (e.Command.Equals(ApplicationCommands.Open))
            {
                this.openShow();
            }
            if (e.Command.Equals(ApplicationCommands.Save))
            {
                this.SaveShow(false);
            }
            if (e.Command.Equals(frmMain.PublishCommand))
            {
                this.publishShow();
            }
            else if (e.Command.Equals(ApplicationCommands.Close))
            {
                this.exit();
            }

            // Debug: Change This
            else if (e.Command.Equals(frmMain.PreviewCommand))
            {

            }

            else if (e.Command.Equals(frmMain.SaveConsole))
            {
                this.saveConsole();
            }
            else if (e.Command.Equals(frmMain.ClearConsole))
            {
                this.clearConsole();
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void CommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            switch ((e.Command as RoutedUICommand).Text)
            { 
                case "Save":
                    e.CanExecute = this.currentShow.IsDirty;
                    break;

                case "SaveConsole":
                    e.CanExecute = (this.txtConsole.Text.Length > 0);
                    break;

                case "ClearConsole":
                    e.CanExecute = (this.txtConsole.Text.Length > 0);
                    break;

                default:
                    e.CanExecute = true;
                    break;
            }

        }

        //----------------------------------------------------------------------------------------------------------
        // Welcome Dialog: Commands
        //----------------------------------------------------------------------------------------------------------
        private void WelcomeUserControl_NewButtonClick(object sender, RoutedEventArgs e)
        {
            this.createNewShow();
        }

        //----------------------------------------------------------------------------------------------------------
        private void Welcome_OpenButtonClick(object sender, RoutedEventArgs e)
        {
            this.openShow();
        }

        //----------------------------------------------------------------------------------------------------------
        private void Welcome_OpenRecentFileButtonClick(object sender, RoutedEventArgs e)
        {
            string FileName = "";
            if (e.OriginalSource is Button)
            {
                Button item = (e.OriginalSource as Button);
                FileName = item.CommandParameter as string;
            }

            if (e.OriginalSource is MenuItem)
            {
                MenuItem item = (e.OriginalSource as MenuItem);
                FileName = item.CommandParameter as string;
            }


            if (!string.IsNullOrEmpty(FileName))
            {
                if (System.IO.Path.GetFileName(FileName) == this.ProjectFile)
                {
                    MessageBox.Show(Properties.Resources.OpenningSameProjectMessage, 
                                    Properties.Resources.OpenningSameProject,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Hand);
                    return;
                }

                this.SaveShow(true);
                this.ProjectFullPath = FileName;
                this.ProjectDirectory = System.IO.Path.GetDirectoryName(FileName);
                this.ProjectFile = System.IO.Path.GetFileName(FileName);

                // Refreshes currentProject/Show
                Show showHandler = WayvFS.OpenShow(this.ProjectFullPath);
                Project projectHandler = new Project();
                projectHandler.Show = showHandler;
                this.currentProject = null;
                this.currentProject = projectHandler;
                this.currentShow = this.currentProject.Show;

                // Fix Paths
                this.FixProjectPath();

                // Remove the file from its current position and add it back to the top/most recent position.
                App.RecentProjects.Remove(FileName);
                App.RecentProjects.Insert(0, FileName);
                BuildOpenMenu();
                this.currentShow.setDirty(false);

                // Sets Mode
                this.setMode(enumMainAppModes.IDLE);

                // Reset Bindings
                this.RecreateBindings(false);

                // Load Video Markers
                this.LoadVOMarkers();
            }
        }


        //----------------------------------------------------------------------------------------------------------
        // New Dialog: Commands
        //----------------------------------------------------------------------------------------------------------
        private void NewDialog_CloseButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.previousMode == enumMainAppModes.WELCOME)
            {
                this.setMode(enumMainAppModes.WELCOME);
            }
            else
            {
                this.setMode(enumMainAppModes.IDLE);
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void NewDialog_CancelButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.previousMode == enumMainAppModes.WELCOME)
            {
                this.setMode(enumMainAppModes.WELCOME);
            }
            else
            {
                this.setMode(enumMainAppModes.IDLE);
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void NewDialog_OKButtonClick(object sender, RoutedEventArgs e)
        {
            // Sets Project Path Properties
            this.ProjectFullPath = (this.NewDialog.ProjectLocation + @"\" + this.NewDialog.ProjectName + @"\" + this.NewDialog.ProjectName + ".wayvproj");
            this.ProjectDirectory = this.NewDialog.ProjectLocation + @"\" + this.NewDialog.ProjectName + @"\";
            this.ProjectFile = (this.NewDialog.ProjectName + ".wayvproj");

            // Creates Empty Project File to serve as Reference
            WayvFS.CreateEmptyProject(this.ProjectFullPath);
            try
            {
                System.IO.Directory.SetCurrentDirectory(this.ProjectDirectory);
            }
            catch (System.Exception ex)
            {
                this.OExceptionSink.Manage(ex);
            }

            // DataBinding
            this.RecreateBindings(true);

            // Sets Project Show Title
            this.currentShow.Title = this.NewDialog.ShowTitle;

            // Sets Project Show Author
            ObservableCollection<Producer> colAuthor = new ObservableCollection<Producer>();
            Producer OAuthor = new Producer();
            OAuthor.Name = this.NewDialog.ShowAuthor;
            colAuthor.Add(OAuthor);
            this.currentShow.Credits.Producers = colAuthor;

            // Switches Mode
            this.setMode(enumMainAppModes.CREATED);
            this.setMode(enumMainAppModes.IDLE);

        }


        //----------------------------------------------------------------------------------------------------------
        // Menu Command Implementations
        //----------------------------------------------------------------------------------------------------------
        private void createNewShow()
        {
            // Prompts to Save if Necessary
            this.SaveShow(true);

            // Shows the New Dialog
            this.setMode(enumMainAppModes.SHOW_NEW);

        }

        //----------------------------------------------------------------------------------------------------------
        private void openShow()
        {
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Filter = "Wayv Project Files (*.wayvproj)|*.wayvproj";
            dialog.InitialDirectory = App.BaseProjectPath;

            if ((bool)dialog.ShowDialog(this))
            {
                // Set Environment Variables
                this.ProjectFullPath = dialog.FileName;
                this.ProjectDirectory = System.IO.Path.GetDirectoryName(dialog.FileName);
                System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(dialog.FileName));
                this.ProjectFile = System.IO.Path.GetFileName(dialog.FileName);
                
                // Open File
                Show showHandler = WayvFS.OpenShow(this.ProjectFullPath);
                
                // Restores Object Graph
                Project projectHandler = new Project();
                projectHandler.Show = showHandler;
                this.currentProject = projectHandler;
                this.currentShow = this.currentProject.Show;

                // Fix Paths
                this.FixProjectPath();

                // Sets Mode
                this.setMode(enumMainAppModes.IDLE);

                // Restores Bindings
                this.RecreateBindings(false);

                this.LoadVOMarkers();
            }            
        }

        //----------------------------------------------------------------------------------------------------------
        private void LoadVOMarkers()
        {
            foreach (VOMarker OVOMarker in this.currentShow.Markers)
            {
                Frame OFrame = this.currentShow.Frames.FirstOrDefault(c => c.Id == OVOMarker.Id);
                OFrame.DesignTimeHelper.HasSyncLink = true;
                this.OTLC.AddMarker(OVOMarker.Id, OVOMarker.Value);
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void SaveShow(bool bAsk)
        {
            if (this.currentShow == null)
            {
                return;
            }

            if (!this.currentShow.IsDirty)
            {
                return;
            }

            if (bAsk)
            {
                MessageBoxResult result = MessageBox.Show(Properties.Resources.NotSavedMessage,
                    Properties.Resources.NotSaved, MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    bAsk = false;
                }
            }

            if (!bAsk)
            {
                if (!string.IsNullOrEmpty(this.ProjectFullPath))
                {
                    FlowDocument fd;
                    foreach (Frame f in this.currentShow.Frames)
                    {
                        ListBoxItem OListBoxItem = ScriptListBox.ItemContainerGenerator.ContainerFromItem(f) as ListBoxItem;

                        if (OListBoxItem != null)
                        {
                            ContentPresenter OContentPresenter = FindVisualChild<ContentPresenter>(OListBoxItem);
                            fd = (OListBoxItem.ContentTemplate.FindName("ORTEditor", OContentPresenter) as RichTextBox).Document;
                            f.Script.ScriptFlowDocument = fd;
                        }
                    }

                    this.SaveVOMarkers();

                    this.currentShow.Save(this.ProjectFullPath);

                    if (!App.RecentProjects.Contains(this.ProjectFullPath))
                    {
                        App.RecentProjects.Add(this.ProjectFullPath);
                        BuildOpenMenu();
                    }
                }
            
            }
        }


        //----------------------------------------------------------------------------------------------------------
        private void SaveVOMarkers()
        {
            this.currentShow.Markers.Clear();

            VOMarker OVOMarker;
            foreach (TimelineController.Components.Marker OMarker in this.OTLC.Markers)
            { 
                OVOMarker = new VOMarker();
                OVOMarker.Id = OMarker.Id;
                OVOMarker.Value = OMarker.Value;
                this.currentShow.Markers.Add(OVOMarker);
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void publishShow()
        {
            if (this.canPublish())
            {
                // Create Folders
                WayvFS.CreateReleaseFolders(this.ProjectFullPath);

                if (MessageBox.Show("This process will take some time, do you want to publish?",
                                                    "Producer Alpha",
                                                    MessageBoxButton.YesNo,
                                                    MessageBoxImage.Question
                    ) == MessageBoxResult.Yes)
                {

                    // Gets Export Path
                    VistaSaveFileDialog dialog = new VistaSaveFileDialog();
                    dialog.Filter = "Zip Package Files (*.zip)|*.zip";
                    dialog.FileName = "ShowPackage.zip";
                    dialog.InitialDirectory = App.BaseProjectPath;

                    if ((bool)dialog.ShowDialog(this))
                    {
                        // Set Environment Variables
                        this.PublishFullPath = dialog.FileName;
                        this.PublishDirectory = System.IO.Path.GetDirectoryName(dialog.FileName);
                        this.PublishFileName = System.IO.Path.GetFileName(dialog.FileName);


                        this.setMode(enumMainAppModes.BUSY_PUBLISHING);

                        // Start Publishing in another Thread
                        this.startPublishThread();
                    }
                    else
                    {
                        return;
                    }                    
                    
                }
                else
                {
                    MessageBox.Show("Publish Cancelled by User Action","Producer Alpha",MessageBoxButton.OK,MessageBoxImage.Hand);
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void BuildOpenMenu()
        {
            // Clear existing menu items
            OpenMenu.Items.Clear();

            // MenuItem for opening files
            MenuItem openMenuItem = new MenuItem();
            openMenuItem.Header = Properties.Resources.MenuOpen;
            openMenuItem.Command = ApplicationCommands.Open;
            OpenMenu.Items.Add(openMenuItem);

            // Add the recent files to the menu as menu items
            if (App.RecentProjects.Count > 0)
            {
                // Separator between the open menu and the recent files
                OpenMenu.Items.Add(new Separator());

                foreach (string file in App.RecentProjects)
                {
                    MenuItem item = new MenuItem();
                    item.Header = System.IO.Path.GetFileName(file.Substring(0,file.IndexOf(".")));
                    item.CommandParameter = file;
                    item.Click += new RoutedEventHandler(Welcome_OpenRecentFileButtonClick);

                    OpenMenu.Items.Add(item);
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void exit()
        {
            //MessageBoxResult mbr =
            //    MessageBox.Show("Do you really want to close the application?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);

            //if (mbr == MessageBoxResult.No)
            //{
            //    return;
            //}
         
            // Save the file automatically
            if ((currentShow.IsDirty) && (!string.IsNullOrEmpty(currentShow.FullyQualifiedFileName)))
            {
                this.SaveShow(true);
            }

            this.Close();
            Application.Current.Shutdown();
        }

        //----------------------------------------------------------------------------------------------------------
        // Scripts Panel Buttons
        //----------------------------------------------------------------------------------------------------------
        private void cmdAddFrame_Click(object sender, RoutedEventArgs e)
        {
            if (this.mnuAddWebSlide.IsChecked)
            {
                WebSlide OWebSlide = new WebSlide(new Script(idCounter++, "New Webslide", "/Assets/Bitmaps/ScriptItemDataTemplate/ImageThumb.png", new FlowDocument()));
                this.currentShow.Frames.Add(OWebSlide);
            }

            if (this.mnuAddVideoFrame.IsChecked)
            {
                VideoFrame OVideoFrame = new VideoFrame(new Script(idCounter++, "New VideoFrame", "/Assets/Bitmaps/ScriptItemDataTemplate/VideoThumb.png", new FlowDocument()));
                this.currentShow.Frames.Add(OVideoFrame);
            }

            if (this.mnuAddConversationalPoll.IsChecked)
            {
                ConversationalPoll OConversationalPoll = new ConversationalPoll(new Script(idCounter++, "New Conversational Poll", "/Assets/Bitmaps/ScriptItemDataTemplate/ConversationalPollThumb.png", new FlowDocument()));
                this.currentShow.Frames.Add(OConversationalPoll);
            }

            if (this.mnuAddPageShot.IsChecked)
            {
                PageShot OPageShot = new PageShot(new Script(idCounter++, "New PageShot", "/Assets/Bitmaps/ScriptItemDataTemplate/PageShotThumb.png", new FlowDocument()));
                this.currentShow.Frames.Add(OPageShot);
            }

            if (this.mnuAddPageFrame.IsChecked)
            {
                PageFrame OPageFrame = new PageFrame(new Script(idCounter++, "New PageFrame", "/Assets/Bitmaps/ScriptItemDataTemplate/PageFrameThumb.png", new FlowDocument()));
                this.currentShow.Frames.Add(OPageFrame);
            }

            
        }

        //----------------------------------------------------------------------------------------------------------
        // Add SplitButton Menu Options
        private void mnuAddWebSlide_Click(object sender, RoutedEventArgs e)
        {
            this.uncheckAddFrameMenu();
            this.mnuAddWebSlide.IsChecked = true;
            this.cmdAddFrame.ToolTip = Properties.Resources.AddNewWebSlide;
        }

        //----------------------------------------------------------------------------------------------------------
        private void mnuAddVideoFrame_Click(object sender, RoutedEventArgs e)
        {
            this.uncheckAddFrameMenu();
            this.mnuAddVideoFrame.IsChecked = true;
            this.cmdAddFrame.ToolTip = Properties.Resources.AddNewVideoFrame;
        }

        //----------------------------------------------------------------------------------------------------------
        private void mnuAddConversationalPoll_Click(object sender, RoutedEventArgs e)
        {
            this.uncheckAddFrameMenu();
            this.mnuAddConversationalPoll.IsChecked = true;
            this.cmdAddFrame.ToolTip = Properties.Resources.AddNewConversationalPoll;
        }

        //----------------------------------------------------------------------------------------------------------
        private void mnuAddPageShot_Click(object sender, RoutedEventArgs e)
        {
            this.uncheckAddFrameMenu();
            this.mnuAddPageShot.IsChecked = true;
            this.cmdAddFrame.ToolTip = Properties.Resources.AddNewPageShot;
        }

        //----------------------------------------------------------------------------------------------------------
        private void mnuAddPageFrame_Click(object sender, RoutedEventArgs e)
        {
            this.uncheckAddFrameMenu();
            this.mnuAddPageFrame.IsChecked = true;
            this.cmdAddFrame.ToolTip = Properties.Resources.AddNewPageFrame;
        }
        //----------------------------------------------------------------------------------------------------------
        private void uncheckAddFrameMenu()
        {
            this.mnuAddWebSlide.IsChecked = false;
            this.mnuAddVideoFrame.IsChecked = false;
            this.mnuAddConversationalPoll.IsChecked = false;
            this.mnuAddPageShot.IsChecked = false;
            this.mnuAddPageFrame.IsChecked = false;
        }

        //----------------------------------------------------------------------------------------------------------
        private void cmdRemoveScriptItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.ScriptListBox.SelectedItem != null)
            {
                MessageBoxResult mbr = 
                    MessageBox.Show(Properties.Resources.DeleteFrameMessage, 
                    Properties.Resources.DeleteFrame, 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);

                if (mbr == MessageBoxResult.Yes)
                {

                    this.currentShow.Frames.RemoveAt((this.ScriptListBox.Items.IndexOf(this.ScriptListBox.SelectedItem)));

                    if ((this.currentShow.Markers.Count <  this.currentFrame.Id) && (this.currentShow.Markers[this.currentFrame.Id] != null))
                    {
                        VOMarker VOMarker2Remove = this.currentShow.Markers[this.currentFrame.Id];
                        this.currentShow.Markers.Remove(VOMarker2Remove);

                        if (this.OTLC.Markers[this.currentFrame.Id] != null)
                        {
                            this.OTLC.RemoveMarker(this.OTLC.Markers[this.currentFrame.Id].Value);
                        }
                    }
                    
                    this.ApplyScriptDataBinding();
                    this.SaveShow(false);
                }

            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void cmdMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.ScriptListBox.SelectedIndex > 0)
            {
                this.currentShow.Frames[this.ScriptListBox.SelectedIndex - 1].Id = (this.currentShow.Frames[this.ScriptListBox.SelectedIndex-1].Id + 1);
                this.currentShow.Frames[this.ScriptListBox.SelectedIndex].Id = (this.currentShow.Frames[this.ScriptListBox.SelectedIndex].Id - 1);

                this.currentShow.Frames.Move(this.ScriptListBox.SelectedIndex, this.ScriptListBox.SelectedIndex - 1);
                
                this.ApplyScriptDataBinding();
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void cmdMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.ScriptListBox.SelectedIndex < (this.currentShow.Frames.Count-1))
            {
                this.currentShow.Frames[this.ScriptListBox.SelectedIndex + 1].Id = (this.currentShow.Frames[this.ScriptListBox.SelectedIndex+1].Id - 1);
                this.currentShow.Frames[this.ScriptListBox.SelectedIndex].Id = (this.currentShow.Frames[this.ScriptListBox.SelectedIndex].Id + 1);

                this.currentShow.Frames.Move(this.ScriptListBox.SelectedIndex, this.ScriptListBox.SelectedIndex + 1);
                this.ApplyScriptDataBinding();
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void cmdToggleExapandMode_Checked(object sender, RoutedEventArgs e)
        {
            int selectedIndex = this.ScriptListBox.SelectedIndex;
            this.collapseAllBut(selectedIndex);
            this.ScriptListBox.SelectedIndex = selectedIndex;
        }

        //----------------------------------------------------------------------------------------------------------
        private void cmdToggleExapandMode_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        //----------------------------------------------------------------------------------------------------------
        private void cmdExpandItems_Click(object sender, RoutedEventArgs e)
        {
            this.LayoutSettings.IsToggleModeActive = false;
            this.ExpandAllScriptItems();
        }


        //----------------------------------------------------------------------------------------------------------
        private void cmdCollapseItems_Click(object sender, RoutedEventArgs e)
        {
            this.CollapseAllScriptItems();
        }

        //----------------------------------------------------------------------------------------------------------
        // Script ListBox
        //----------------------------------------------------------------------------------------------------------    
        private void ScriptListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.currentFrame = (this.ScriptListBox.SelectedItem as Frame);
            this.BindCurrentFrame();
        }


        //----------------------------------------------------------------------------------------------------------
        // Script Item: Button Matrix
        //----------------------------------------------------------------------------------------------------------    
        private void ScriptListBox_ButtonClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement OElement = (e.OriginalSource as FrameworkElement);
            String sName = "Unknown";
            if (OElement.Name != "")
            {
                sName = OElement.Name;
            }

            ToolTipService.SetShowDuration(OElement, 3600000);

            ListBoxItem OListBoxItem = ScriptListBox.ItemContainerGenerator.ContainerFromItem(OElement.DataContext) as ListBoxItem;

            if (OListBoxItem != null)
            {
                ContentPresenter OContentPresenter = FindVisualChild<ContentPresenter>(OListBoxItem);

                switch (sName)
                {
                    case "cmdAnimation":
                        if (((OListBoxItem.ContentTemplate.FindName("OExtendedItemExpander", OContentPresenter) as Expander).IsExpanded == false)
                            && ((OElement as ToggleButton).IsChecked == true))
                        {
                            (OListBoxItem.ContentTemplate.FindName("OExtendedItemExpander", OContentPresenter) as Expander).IsExpanded = true;
                        }

                        if (!(OListBoxItem.ContentTemplate.FindName("ExpanderForAnimation", OContentPresenter) as Expander).IsExpanded)
                        {
                            (OListBoxItem.ContentTemplate.FindName("ExpanderForAnimation", OContentPresenter) as Expander).IsExpanded = true;
                        }
                        else
                        {
                            (OListBoxItem.ContentTemplate.FindName("ExpanderForAnimation", OContentPresenter) as Expander).IsExpanded = false;
                            (OElement as ToggleButton).IsChecked = false;
                        }
                        break;

                    case "cmdOverlays":
                        if (((OListBoxItem.ContentTemplate.FindName("OExtendedItemExpander", OContentPresenter) as Expander).IsExpanded == false)
                            && ((OElement as ToggleButton).IsChecked == true))
                        {
                            (OListBoxItem.ContentTemplate.FindName("OExtendedItemExpander", OContentPresenter) as Expander).IsExpanded = true;
                        }

                        if (!(OListBoxItem.ContentTemplate.FindName("ExpanderForOverlays", OContentPresenter) as Expander).IsExpanded)
                        {
                            (OListBoxItem.ContentTemplate.FindName("ExpanderForOverlays", OContentPresenter) as Expander).IsExpanded = true;
                        }
                        else
                        {
                            (OListBoxItem.ContentTemplate.FindName("ExpanderForOverlays", OContentPresenter) as Expander).IsExpanded = false;
                            (OElement as ToggleButton).IsChecked = false;
                        }
                        break;


                    case "cmdControls":
                        if (((OListBoxItem.ContentTemplate.FindName("OExtendedItemExpander", OContentPresenter) as Expander).IsExpanded == false)
                            && ((OElement as ToggleButton).IsChecked == true))
                        {
                            (OListBoxItem.ContentTemplate.FindName("OExtendedItemExpander", OContentPresenter) as Expander).IsExpanded = true;
                        }

                        if (!(OListBoxItem.ContentTemplate.FindName("ExpanderForControls", OContentPresenter) as Expander).IsExpanded)
                        {
                            (OListBoxItem.ContentTemplate.FindName("ExpanderForControls", OContentPresenter) as Expander).IsExpanded = true;
                        }
                        else
                        {
                            (OListBoxItem.ContentTemplate.FindName("ExpanderForControls", OContentPresenter) as Expander).IsExpanded = false;
                            (OElement as ToggleButton).IsChecked = false;
                        }
                        break;

                    case "cmdImage":
                        if (((OListBoxItem.ContentTemplate.FindName("OExtendedItemExpander", OContentPresenter) as Expander).IsExpanded == false)
                            && ((OElement as ToggleButton).IsChecked == true))
                        {
                            (OListBoxItem.ContentTemplate.FindName("OExtendedItemExpander", OContentPresenter) as Expander).IsExpanded = true;
                        }

                        if (!(OListBoxItem.ContentTemplate.FindName("ExpanderForImage", OContentPresenter) as Expander).IsExpanded)
                        {
                            (OListBoxItem.ContentTemplate.FindName("ExpanderForImage", OContentPresenter) as Expander).IsExpanded = true;
                        }
                        else
                        {
                            (OListBoxItem.ContentTemplate.FindName("ExpanderForImage", OContentPresenter) as Expander).IsExpanded = false;
                            (OElement as ToggleButton).IsChecked = false;
                        }
                        break;

                    case "cmdSoundtrack":
                        if (((OListBoxItem.ContentTemplate.FindName("OExtendedItemExpander", OContentPresenter) as Expander).IsExpanded == false)
                            && ((OElement as ToggleButton).IsChecked == true))
                        {
                            (OListBoxItem.ContentTemplate.FindName("OExtendedItemExpander", OContentPresenter) as Expander).IsExpanded = true;
                        }

                        if (!(OListBoxItem.ContentTemplate.FindName("ExpanderForSoundtrack", OContentPresenter) as Expander).IsExpanded)
                        {
                            (OListBoxItem.ContentTemplate.FindName("ExpanderForSoundtrack", OContentPresenter) as Expander).IsExpanded = true;
                        }
                        else
                        {
                            (OListBoxItem.ContentTemplate.FindName("ExpanderForSoundtrack", OContentPresenter) as Expander).IsExpanded = false;
                            (OElement as ToggleButton).IsChecked = false;
                        }
                        break;

                    case "cmdVideo":
                        if (((OListBoxItem.ContentTemplate.FindName("OExtendedItemExpander", OContentPresenter) as Expander).IsExpanded == false)
                            && ((OElement as ToggleButton).IsChecked == true))
                        {
                            (OListBoxItem.ContentTemplate.FindName("OExtendedItemExpander", OContentPresenter) as Expander).IsExpanded = true;
                        }

                        if (!(OListBoxItem.ContentTemplate.FindName("ExpanderForVideo", OContentPresenter) as Expander).IsExpanded)
                        {
                            (OListBoxItem.ContentTemplate.FindName("ExpanderForVideo", OContentPresenter) as Expander).IsExpanded = true;
                        }
                        else
                        {
                            (OListBoxItem.ContentTemplate.FindName("ExpanderForVideo", OContentPresenter) as Expander).IsExpanded = false;
                            (OElement as ToggleButton).IsChecked = false;
                        }
                        break;

                    case "cmdGoToSyncLink":
                        Frame OFrame = (OElement.DataContext as Frame);
                        if (OFrame.Id != this.currentFrame.Id)
                        {
                            OListBoxItem.IsSelected = true;
                        }
                        this.OBBar.OVideoPlayer.VideoController.Seek(this.currentShow.Markers[this.currentFrame.Id - 1].Value);
                        break;
                }

                e.Handled = true;
            }

        }

        //----------------------------------------------------------------------------------------------------------
        private void ScriptListBox_Expanded(object sender, RoutedEventArgs e)
        {
            FrameworkElement OElement = (e.OriginalSource as FrameworkElement);

            String sName = "Unknown";
            if (OElement.Name != "")
            {
                sName = OElement.Name;
            }

            ListBoxItem OListBoxItem = ScriptListBox.ItemContainerGenerator.ContainerFromItem(OElement.DataContext) as ListBoxItem;

            if (OListBoxItem != null)
            {
                ContentPresenter OContentPresenter = FindVisualChild<ContentPresenter>(OListBoxItem);

                switch (sName)
                {
                    case "ExpanderForAnimation":
                        (OListBoxItem.ContentTemplate.FindName("cmdAnimation", OContentPresenter) as ToggleButton).IsChecked = true;
                        ToolTipService.SetShowDuration((OListBoxItem.ContentTemplate.FindName("cmdAnimation", OContentPresenter) as ToggleButton), 3600000);
                        break;

                    case "ExpanderForOverlays":
                        (OListBoxItem.ContentTemplate.FindName("cmdOverlays", OContentPresenter) as ToggleButton).IsChecked = true;
                        ToolTipService.SetShowDuration((OListBoxItem.ContentTemplate.FindName("cmdOverlays", OContentPresenter) as ToggleButton), 3600000);
                        break;

                    case "ExpanderForControls":
                        (OListBoxItem.ContentTemplate.FindName("cmdControls", OContentPresenter) as ToggleButton).IsChecked = true;
                        ToolTipService.SetShowDuration((OListBoxItem.ContentTemplate.FindName("cmdControls", OContentPresenter) as ToggleButton), 3600000);
                        break;

                    case "ExpanderForImage":
                        (OListBoxItem.ContentTemplate.FindName("cmdImage", OContentPresenter) as ToggleButton).IsChecked = true;
                        ToolTipService.SetShowDuration((OListBoxItem.ContentTemplate.FindName("cmdImage", OContentPresenter) as ToggleButton), 3600000);
                        break;

                    case "ExpanderForSoundtrack":
                        (OListBoxItem.ContentTemplate.FindName("cmdSoundtrack", OContentPresenter) as ToggleButton).IsChecked = true;
                        ToolTipService.SetShowDuration((OListBoxItem.ContentTemplate.FindName("cmdSoundtrack", OContentPresenter) as ToggleButton), 3600000);
                        break;

                    case "ExpanderForVideo":
                        (OListBoxItem.ContentTemplate.FindName("cmdVideo", OContentPresenter) as ToggleButton).IsChecked = true;
                        ToolTipService.SetShowDuration((OListBoxItem.ContentTemplate.FindName("cmdVideo", OContentPresenter) as ToggleButton), 3600000);
                        break;

                    case "ScriptItemExpander":

                        //--------------------------------------------------------------------------------------------
                        // FrameType Header
                        //--------------------------------------------------------------------------------------------
                        if (OListBoxItem.ContentTemplate.FindName("imgFrameType", OContentPresenter) != null)
                        {
                            Grid HeaderGrid = (OListBoxItem.ContentTemplate.FindName("HeaderGrid", OContentPresenter) as Grid);
                            if (HeaderGrid != null)
                            {
                                Storyboard fadeOut = (HeaderGrid.Resources["imgTypeFadeOut"] as Storyboard);
                                fadeOut.Begin();
                            }
                            Storyboard ThumbfadeOut = (HeaderGrid.Resources["FrameThumbPreviewFadeOut"] as Storyboard);
                            ThumbfadeOut.Begin();
                        }


                        //--------------------------------------------------------------------------------------------
                        // ToggleExpandeMode
                        //--------------------------------------------------------------------------------------------
                        if ((this.LayoutSettings.IsToggleModeActive == true) && (OListBoxItem.ContentTemplate.FindName("ScriptItemExpander", OContentPresenter) != null))
                        {
                            int currentIndex = this.ScriptListBox.ItemContainerGenerator.IndexFromContainer(OListBoxItem);
                            this.collapseAllBut(currentIndex);
                        }

                        break;
                }

            }

            e.Handled = true;
        }


        //----------------------------------------------------------------------------------------------------------
        private void ScriptListBox_Collapsed(object sender, RoutedEventArgs e)
        {
            FrameworkElement OElement = (e.OriginalSource as FrameworkElement);

            String sName = "Unknown";
            if (OElement.Name != "")
            {
                sName = OElement.Name;
            }

            ListBoxItem OListBoxItem = ScriptListBox.ItemContainerGenerator.ContainerFromItem(OElement.DataContext) as ListBoxItem;

            if (OListBoxItem != null)
            {
                ContentPresenter OContentPresenter = FindVisualChild<ContentPresenter>(OListBoxItem);

                switch (sName)
                {
                    case "ExpanderForAnimation":
                        (OListBoxItem.ContentTemplate.FindName("cmdAnimation", OContentPresenter) as ToggleButton).IsChecked = false;
                        break;

                    case "ExpanderForOverlays":
                        (OListBoxItem.ContentTemplate.FindName("cmdOverlays", OContentPresenter) as ToggleButton).IsChecked = false;
                        break;

                    case "ExpanderForControls":
                        (OListBoxItem.ContentTemplate.FindName("cmdControls", OContentPresenter) as ToggleButton).IsChecked = false;
                        break;

                    case "ExpanderForImage":
                        (OListBoxItem.ContentTemplate.FindName("cmdImage", OContentPresenter) as ToggleButton).IsChecked = false;
                        break;

                    case "ExpanderForSoundtrack":
                        (OListBoxItem.ContentTemplate.FindName("cmdSoundtrack", OContentPresenter) as ToggleButton).IsChecked = false;
                        break;

                    case "ExpanderForVideo":
                        (OListBoxItem.ContentTemplate.FindName("cmdVideo", OContentPresenter) as ToggleButton).IsChecked = false;
                        break;


                    case "ScriptItemExpander":
                        if (OListBoxItem.ContentTemplate.FindName("imgFrameType", OContentPresenter) != null)
                        {
                            Image imgFrameType = (OListBoxItem.ContentTemplate.FindName("imgFrameType", OContentPresenter) as Image);

                            if (OElement.DataContext is WebSlide)
                            {
                                Uri src = new Uri("/Assets/Icons/ScriptItemDataTemplate/FrameTypes/WebSlideIcon.png", UriKind.Relative);
                                BitmapImage img = new BitmapImage(src);
                                imgFrameType.SetValue(Image.SourceProperty, img);
                                imgFrameType.ToolTip = Properties.Resources.WebSlide;
                            }

                            if (OElement.DataContext is VideoFrame)
                            {
                                Uri src = new Uri("/Assets/Icons/ScriptItemDataTemplate/FrameTypes/VideoFrameIcon.png", UriKind.Relative);
                                BitmapImage img = new BitmapImage(src);
                                imgFrameType.SetValue(Image.SourceProperty, img);
                                imgFrameType.ToolTip = Properties.Resources.VideoFrame;
                            }

                            if (OElement.DataContext is ConversationalPoll)
                            {
                                Uri src = new Uri("/Assets/Icons/ScriptItemDataTemplate/FrameTypes/CPIcon.png", UriKind.Relative);
                                BitmapImage img = new BitmapImage(src);
                                imgFrameType.SetValue(Image.SourceProperty, img);
                                imgFrameType.ToolTip = Properties.Resources.ConversationalPoll;
                            }

                            if (OElement.DataContext is PageShot)
                            {
                                Uri src = new Uri("/Assets/Icons/ScriptItemDataTemplate/FrameTypes/PageShotIcon.png", UriKind.Relative);
                                BitmapImage img = new BitmapImage(src);
                                imgFrameType.SetValue(Image.SourceProperty, img);
                                imgFrameType.ToolTip = Properties.Resources.PageShot;
                            }

                            if (OElement.DataContext is PageFrame)
                            {
                                Uri src = new Uri("/Assets/Icons/ScriptItemDataTemplate/FrameTypes/PageFrameIcon.png", UriKind.Relative);
                                BitmapImage img = new BitmapImage(src);
                                imgFrameType.SetValue(Image.SourceProperty, img);
                                imgFrameType.ToolTip = Properties.Resources.PageFrame;
                            }


                            //--------------------------------------------------------------------------------------------
                            Grid HeaderGrid = (OListBoxItem.ContentTemplate.FindName("HeaderGrid", OContentPresenter) as Grid);
                            Storyboard fadeIn = (Storyboard)HeaderGrid.Resources["imgTypeFadeIn"];
                            fadeIn.Begin();
                            Storyboard ThumbfadeIn = (HeaderGrid.Resources["FrameThumbPreviewFadeIn"] as Storyboard);
                            ThumbfadeIn.Begin();

                            OListBoxItem.IsSelected = true;
                        }

                        break;
                }

            }

            e.Handled = true;
        }

        //----------------------------------------------------------------------------------------------------------
        // Script Item: RichTextBox
        //----------------------------------------------------------------------------------------------------------
        private void ScriptListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is RichTextBox)
            {
                RichTextBox rtb = e.OriginalSource as RichTextBox;
                if (rtb.DataContext != null)
                {
                    this.ScriptListBox.SelectedItem = rtb.DataContext;
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void ScriptListBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is RichTextBox)
            {
                this.currentShow.IsDirty = true;
            }
        }       
        
        //----------------------------------------------------------------------------------------------------------
        // Property Pane Events
        //----------------------------------------------------------------------------------------------------------
        private void ContributorsExpander_Expanded(object sender, RoutedEventArgs e)
        {
            this.MainShowPane.RowDefinitions[SHOWINFO_PANE].Height =
                new GridLength(SHOW_INFO_CONTRIBUTORS_OPEN);

            this.LayoutSettings.ShowPaneTopPanelHeight = SHOW_INFO_CONTRIBUTORS_OPEN;

            e.Handled = true;
        }

        //----------------------------------------------------------------------------------------------------------
        private void ContributorsExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            this.MainShowPane.RowDefinitions[SHOWINFO_PANE].Height =
                new GridLength(SHOW_INFO_CONTRIBUTORS_CLOSED);

            this.LayoutSettings.ShowPaneTopPanelHeight = SHOW_INFO_CONTRIBUTORS_CLOSED;

            e.Handled = true;
        }


        //----------------------------------------------------------------------------------------------------------
        // ViewPane Events
        //----------------------------------------------------------------------------------------------------------
        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.DocumentSize.ScaleX = e.NewValue;
            this.DocumentSize.ScaleY = e.NewValue;

            App.currentZoom = e.NewValue;
        }

        //----------------------------------------------------------------------------------------------------------
        private void cmdZoomToExtents_Click(object sender, RoutedEventArgs e)
        {
            this.ZoomToExtents();
        }

        //----------------------------------------------------------------------------------------------------------
        private void toggleRecording_Checked(object sender, RoutedEventArgs e)
        {
            // TODO: Move to Mode
            this.setMode(enumMainAppModes.RECORDING_ANIMATION);
        }

        //----------------------------------------------------------------------------------------------------------
        private void toggleRecording_Unchecked(object sender, RoutedEventArgs e)
        {
            // TODO: Move to Mode Idle
            this.setMode(enumMainAppModes.IDLE);
        }

        //----------------------------------------------------------------------------------------------------------
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        //----------------------------------------------------------------------------------------------------------
        private void toggleMute_Checked(object sender, RoutedEventArgs e)
        {
            this.VolumeSlider.IsEnabled = false;
            this.OBBar.SetVideoMute(true);
        }

        //----------------------------------------------------------------------------------------------------------
        private void toggleMute_Unchecked(object sender, RoutedEventArgs e)
        {
            this.VolumeSlider.IsEnabled = true;
            this.OBBar.SetVideoMute(false);
        }



        //----------------------------------------------------------------------------------------------------------
        private void cmdInsertNVideo_Click(object sender, RoutedEventArgs e)
        {
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Filter = "WMV Files (*.wmv)|*.wmv";
            dialog.InitialDirectory = App.BaseProjectPath;
            string sNVideoPath;
            string sNVideoDirectoryPath;
            string sNVideoFileNamePath;

            if ((bool)dialog.ShowDialog(this))
            {
                sNVideoPath = dialog.FileName;
                sNVideoDirectoryPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                sNVideoFileNamePath = System.IO.Path.GetFileName(dialog.FileName);
                this.VideoPath = sNVideoFileNamePath;

                Boolean bAssign = true;

                try
                {
                    File.Copy(sNVideoPath, this.ProjectDirectory + @"\Resources\" + sNVideoFileNamePath, true);
                }
                catch (IOException ex)
                {
                    this.OExceptionSink.Manage(ex);
                }

                try
                {
                    if (bAssign == true)
                    {
                        // Assign to Video Player
                        this.currentShow.NVideoPath = sNVideoFileNamePath;
                        this.OBBar.SetVideo(new Uri((this.ProjectDirectory + @"\Resources\" + sNVideoFileNamePath), UriKind.Absolute));
                    }
                }
                catch (System.Exception ex)
                {
                    this.OExceptionSink.Manage(ex);
                }
            } 
        }

        //----------------------------------------------------------------------------------------------------------
        private void cmdInsertXamlFromBlend_Click(object sender, RoutedEventArgs e)
        {
            // Get Confirmation from the User
            MessageBoxResult mbr = MessageBox.Show(Properties.Resources.LoadXamlMessage1 +
                System.Environment.NewLine + System.Environment.NewLine +
                Properties.Resources.LoadXamlMessage2,
                Properties.Resources.LoadXaml,
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (mbr == MessageBoxResult.No)
            {
                return;
            }

            // Get Open Dialog Information
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Filter = "Xaml Files (*.xaml)|*.xaml";
            dialog.InitialDirectory = App.BaseProjectPath;
            string sXamlFilePath;
            string sXamlDirectoryPath;
            string sXamlFileNamePath;
            string destPath;

            if ((bool)dialog.ShowDialog(this))
            {
                // Sets Vars
                sXamlFilePath      = dialog.FileName;
                sXamlDirectoryPath  = System.IO.Path.GetDirectoryName(dialog.FileName);
                sXamlFileNamePath   = System.IO.Path.GetFileName(dialog.FileName);
                destPath            = (this.ProjectDirectory + @"\Resources\" + sXamlFileNamePath);

                // Declares Objects for Parsing Resources
                XmlDocument sourceDoc = new XmlDocument();
                XmlDocument destinationDoc = new XmlDocument();

                XmlTextReader xmlReaderForParse = new XmlTextReader(File.OpenText(sXamlFilePath));

                // Parse Resources
                try
                {

                    // Parses Xaml Content
                    sourceDoc.Load(xmlReaderForParse);

                    XmlNodeList nodeList = sourceDoc.SelectNodes("//*");

                    foreach (XmlNode node in nodeList)
                    {
                        if (node.Name.ToUpper() == "IMAGE")
                        {
                            if (node.Attributes["Source"] != null)
                            {
                                string Name = node.Attributes["x:Name"].Value;
                                string Source = node.Attributes["Source"].Value;
                                
                               (this.currentFrame as IUsesAssets).Assets.Add(new Asset(this.currentFrame.Id,Name,enumAssetTypes.ASSET_FOR_XAML_ANIMATION, Source, this.ProjectDirectory + @"\Resources\"));

                                if (!File.Exists((this.ProjectDirectory + @"\Resources\" + System.IO.Path.GetFileName(Source))) &&
                                    (Source != null) && 
                                    (Source != "")
                                )
                                {
                                    File.Copy((sXamlDirectoryPath + @"\" + Source),
                                        (this.ProjectDirectory + @"\Resources\" + System.IO.Path.GetFileName(Source)),
                                        true);
                                }

                                node.Attributes["Source"].Value = System.IO.Path.GetFileName(Source);

                                // Replace Background Image
                                if ((Name == "OMovingBackground") && (this.currentFrame is IAcceptsBackgroundImage))
                                {
                                    // Save Current Image Path
                                    (this.currentFrame as IAcceptsBackgroundImage).InactiveBackgroundImagePath =
                                        (this.currentFrame as IAcceptsBackgroundImage).BackgroundImagePath;


                                    // Change Background Image
                                    (this.currentFrame as IAcceptsBackgroundImage).BackgroundImagePath = 
                                        this.ProjectDirectory + @"\Resources\" + Source;

                                    this.LoadBackgroundImage();
                                }
                            }
                        }
                    }

                    // Cleans Xaml
                    XmlNodeList CanvasSourceList = sourceDoc.GetElementsByTagName("Canvas");
                    if (
                        (CanvasSourceList[0].Attributes["x:Name"].Value == "LayoutRoot") && 
                        (CanvasSourceList[0].Name == "Canvas")
                       )
                    {
                        // Seems like Xaml was done according to the spec..
                        XmlNode xMainCanvas = CanvasSourceList[0];

                        // Add xMainCanvas to Destination Document
                        XmlNode xMoveReadyCanvas = this.RenameNode(xMainCanvas, "Canvas", destinationDoc, xMainCanvas.NamespaceURI);
                        xMoveReadyCanvas.AppendChild(destinationDoc.CreateElement("Canvas.Resources", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"));
                        destinationDoc.AppendChild(xMoveReadyCanvas);

                        XmlNode ResourcesSource = sourceDoc.GetElementsByTagName("UserControl.Resources")[0];
                        XmlNode xMoveReadyStoryboard = null;
                        XmlNode ResourcesDestination = destinationDoc.GetElementsByTagName("Canvas.Resources")[0];


                        foreach (XmlNode OStoryboard in ResourcesSource.ChildNodes)
                        {
                            xMoveReadyStoryboard = this.RenameNode(OStoryboard, OStoryboard.Name, destinationDoc, xMainCanvas.NamespaceURI);
                            if (
                                 (xMoveReadyStoryboard.Attributes["x:Key"] != null) &&
                                 (xMoveReadyStoryboard.Attributes["x:Key"].Value == "OSBMain") &&
                                 (xMoveReadyStoryboard.Attributes["Name"] == null)
                               )
                            {
                                xMoveReadyStoryboard.Attributes.Append(destinationDoc.CreateAttribute("Name"));
                                xMoveReadyStoryboard.Attributes["Name"].Value = "OSBMain";
                            }

                            ResourcesDestination.AppendChild(xMoveReadyStoryboard);
                        }

                        this.showTrace(destinationDoc.OuterXml);

                    }
                    else
                    {
                        throw new Exception("LayoutRoot not found while analyzing Xaml");
                    }

                    // Saves Xaml into Frame Object
                    currentFrame.DesignTimeHelper.XamlContent = destinationDoc.OuterXml;

                    // Creates Xaml Link
                    // Get Confirmation from the User
                    MessageBoxResult mbrLink = MessageBox.Show(Properties.Resources.CreateXamlLinkMessage1 +
                        System.Environment.NewLine + System.Environment.NewLine +
                        Properties.Resources.CreateXamlLinkMessage2,
                        Properties.Resources.CreateXamlLink,
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (mbrLink == MessageBoxResult.Yes)
                    {
                        currentFrame.DesignTimeHelper.XamlInfo.XamlPath = sXamlFilePath;
                        currentFrame.DesignTimeHelper.XamlInfo.DateOfLastModification = File.GetLastWriteTime(sXamlFilePath);
                        this.UpdateXamlLinksPane();
                    }
                    else
                    {
                        currentFrame.DesignTimeHelper.XamlInfo.XamlPath = "Empty";
                        currentFrame.DesignTimeHelper.XamlInfo.DateOfLastModification = DateTime.Now;
                        this.UpdateXamlLinksPane();
                    }

                    if (!this.framesWithXamlContent.Contains(this.currentFrame))
                    {
                        this.framesWithXamlContent.Add(this.currentFrame);
                    }

                }
                catch (XmlException ex)
                {
                    this.OErrorReporter.ErrorText = ex.Message + "\n\n" + "CallStack: " + "\n" + ex.StackTrace;
                    this.OErrorReporter.Visibility = Visibility.Visible;
                    this.OExceptionSink.Manage(ex);
                }
                catch (System.IO.IOException ex)
                {
                    this.OExceptionSink.Manage(ex);
                }
                finally
                {
                    xmlReaderForParse.Close();
                }

                (this.currentFrame as IUsesAssets).XamlContent = true;

                if (this.currentFrame.DesignTimeHelper.XamlContent != null)
                {
                    this.toggleFrameAnimation.IsEnabled = true;
                }


                this.LoadXamlAnimation();

            }
        }


        //----------------------------------------------------------------------------------------------------------
        private void cmdInsertChannelImage_Click(object sender, RoutedEventArgs e)
        {
            this.setMode(enumMainAppModes.SELECTING_WEBCHANNEL);
        }


        //----------------------------------------------------------------------------------------------------------
        private void WebChannelSelectorDialog_OKButtonClick(object sender, RoutedEventArgs e)
        {
            // Save The User's Selection
            ProducerAlpha.WebChannelInfo OSelectedChannel = WebChannelSelectorDialog.SelectedChannel;

            this.currentShow.Metadata.WebChannelId = OSelectedChannel.WebChannelId;


            this.currentShow.ChannelLogoPath =
               (this.ProjectDirectory + @"\Resources\" + "WebChannelLogo.png");


            if (File.Exists(this.currentShow.ChannelLogoPath))
            {
                this.WebChannelLogo.Source = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();


                File.Delete(this.currentShow.ChannelLogoPath);
            }

            OSelectedChannel.WebChannelLogo.Save(this.currentShow.ChannelLogoPath);

            BitmapImage OSource = new BitmapImage();
            OSource.BeginInit();
            OSource.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            OSource.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

            OSource.UriSource = new Uri(this.currentShow.ChannelLogoPath, UriKind.Absolute);
            OSource.CacheOption = BitmapCacheOption.OnLoad;
            OSource.EndInit();

            this.WebChannelLogo.Source = OSource;

            this.currentShow.IsDirty = true;

            // Close Dialog and return to idle state
            this.setMode(enumMainAppModes.IDLE);
        }

        //----------------------------------------------------------------------------------------------------------
        private void WebChannelSelectorDialog_CancelButtonClick(object sender, RoutedEventArgs e)
        {
            // Close Dialog and return to idle state
            this.setMode(enumMainAppModes.IDLE);

        }

        
        //----------------------------------------------------------------------------------------------------------
        private void cmdInsertImageBackground_Click(object sender, RoutedEventArgs e)
        {
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Filter = "JPEG Files (*.jpg)|*.jpg";
            dialog.InitialDirectory = App.BaseProjectPath;
            string sFrameImagePath;
            string sFrameDirectoryPath;
            string sFrameFileNamePath;

            if ((bool)dialog.ShowDialog(this))
            {
                sFrameImagePath = dialog.FileName;
                sFrameDirectoryPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                sFrameFileNamePath = System.IO.Path.GetFileName(dialog.FileName);

                try
                {
                    if (!File.Exists(this.ProjectDirectory + @"\Resources\" + sFrameFileNamePath))
                    {
                        File.Copy(sFrameImagePath, this.ProjectDirectory + @"\Resources\" + "WebSlideBackground_" +
                          this.currentFrame.Id.ToString() + ".jpg", false);
                    }
                }
                catch (IOException ex)
                {
                    this.OExceptionSink.Manage(ex);
                }

                try
                {
                    BitmapImage Source = new BitmapImage();
                    Source.BeginInit();
                    Source.CacheOption = BitmapCacheOption.OnLoad;
                    Source.UriSource = new Uri(this.ProjectDirectory + @"\Resources\" + "WebSlideBackground_" + 
                          this.currentFrame.Id.ToString() + ".jpg");
                    Source.EndInit();

                    BitmapSource OSourceForValidation = ((BitmapSource)Source);
                    Boolean bAssign = true;

                    // Validates Image Size
                    if ((OSourceForValidation.PixelWidth > MAX_FRAME_WIDTH) ||

                        (OSourceForValidation.PixelHeight > MAX_FRAME_HEIGHT)

                        )
                    {
                        MessageBox.Show(Properties.Resources.FrameTooLargeMessage,
                                        Properties.Resources.FrameTooLarge,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Hand);
                        bAssign = false;
                    }

                    if ((OSourceForValidation.PixelWidth < MIN_FRAME_WIDTH) ||

                        (OSourceForValidation.PixelHeight < MIN_FRAME_HEIGHT)

                        )
                    {
                        MessageBox.Show(Properties.Resources.FrameTooSmallMessage,
                                        Properties.Resources.FrameTooSmall,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Hand);
                        bAssign = false;
                    }

                    if ((bAssign == true) && (OSourceForValidation.PixelHeight != (OSourceForValidation.PixelWidth * .5625)))
                    {
                        MessageBox.Show(Properties.Resources.FrameIsNot16By9Message_Part1 + 
                                        " " + (OSourceForValidation.PixelWidth * .5625).ToString() + " " +
                                        Properties.Resources.FrameIsNot16By9Message_Part2,
                                        Properties.Resources.FrameIsNot16By9,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Hand);
                        bAssign = false;
                    }


                    if (bAssign == true)
                    {
                        // Assign to Assets Collection
                        if(!(this.currentFrame as IUsesAssets).Assets.Any(p => p.FileID == "OMovingBackground"))
                        {
                            (this.currentFrame as IUsesAssets).Assets.Add(
                                new Asset(this.currentFrame.Id,
                                            "OMovingBackground", 
                                            enumAssetTypes.FRAME_BACKGROUND, 
                                            "WebSlideBackground.jpg", 
                                            this.ProjectDirectory + @"\Resources\"));
                        }
                        // Assign to Frame Object
                        (this.currentFrame as IAcceptsBackgroundImage).BackgroundImagePath =
                                (this.ProjectDirectory + @"\Resources\" + "WebSlideBackground_" + 
                                    this.currentFrame.Id.ToString() + ".jpg");
                            
                        this.BindCurrentFrame();
                    }
                    else
                    {
                        OSourceForValidation = null;
                        Source.StreamSource.Dispose();
                        Source = null;

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                catch (System.Exception ex)
                {
                    this.OExceptionSink.Manage(ex);
                }
            } 

        }


        //----------------------------------------------------------------------------------------------------------
        private void cmdInsertMoreMenu_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Image OMoreMenuStunt = new System.Windows.Controls.Image();
            OMoreMenuStunt.SetValue(Canvas.LeftProperty, 550.0);
            OMoreMenuStunt.SetValue(Canvas.TopProperty, 200.0);
            OMoreMenuStunt.SetValue(UIElement.OpacityProperty, 0.7);
            OMoreMenuStunt.Source = new BitmapImage(new Uri(@"\Assets\Bitmaps\Controls\MoreMenuStunt.png", UriKind.Relative));
            this.ControlsPlaceHolder.Children.Add(OMoreMenuStunt);
        }


        //----------------------------------------------------------------------------------------------------------
        private void toggleFrameAnimation_Checked(object sender, RoutedEventArgs e)
        {
            if (this.currentMode != enumMainAppModes.PLAYING_ANIMATION)
            {
                if ((this.XamlContentPlaceHolder.Children.Count > 0) && (this.XamlContentPlaceHolder.Children[0] != null))
                {
                    if (((this.XamlContentPlaceHolder.Children[0] as Canvas).FindName("OSBMain") as Storyboard) == null)
                    {
                        MessageBox.Show(Properties.Resources.NoOSBMainMessage,
                                        Properties.Resources.NoOSBMain,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error
                        );
                        return;
                    }

                    // Set Frame Animation Completed Event
                    ((this.XamlContentPlaceHolder.Children[0] as Canvas).FindName("OSBMain") as Storyboard).Completed +=
                        new EventHandler(FrameAnimation_Completed);

                    // Change Mode to Playing_Animation
                    this.setMode(enumMainAppModes.PLAYING_ANIMATION);

                    // Start Frame Animation
                    ((this.XamlContentPlaceHolder.Children[0] as Canvas).FindName("OSBMain") as Storyboard).Begin();
                }
            }
            else
            {
                // Restart Frame Animation
                this.ViewPaneBorder.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 0));
                ((this.XamlContentPlaceHolder.Children[0] as Canvas).FindName("OSBMain") as Storyboard).Resume();
            }

        }

        //----------------------------------------------------------------------------------------------------------
        private void toggleFrameAnimation_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.currentMode == enumMainAppModes.PLAYING_ANIMATION)
            {
                // Pause Frame Animation
                this.ViewPaneBorder.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255,255,128,0));
                ((this.XamlContentPlaceHolder.Children[0] as Canvas).FindName("OSBMain") as Storyboard).Pause();
            }
        }

        //----------------------------------------------------------------------------------------------------------
        void FrameAnimation_Completed(object sender, EventArgs e)
        {
            ((this.XamlContentPlaceHolder.Children[0] as Canvas).FindName("OSBMain") as Storyboard).Completed -=
                new EventHandler(FrameAnimation_Completed);

            // Change Mode to Idle
            this.setMode(enumMainAppModes.IDLE);

            // Reset PlayAnimation ToggleButton
            this.toggleFrameAnimation.IsChecked = false;
        }

        //----------------------------------------------------------------------------------------------------------
        // Event Handler Methods Section: Exception
        //----------------------------------------------------------------------------------------------------------
        void OExceptionSink_UIManaging(object sender, ExceptionEventArgs e)
        {
            this.OErrorReporter.ErrorText = e.ExceptionMessage;
            this.OErrorReporter.ErrorStackTrace =  "CallStack: " + e.ExceptionStackTrace;
            this.setMode(enumMainAppModes.XAML_ERROR_SHOW);
        }

        //----------------------------------------------------------------------------------------------------------
        void OExceptionSink_TraceReporting(object sender, ExceptionEventArgs e)
        {
            this.txtConsole.Text += "\n" + "[" + DateTime.Now.ToString() + "]" + " - " +
                "An Error has ocurred: " + e.ExceptionMessage + "\n\n" + e.ExceptionStackTrace + "\n\n";
        }


        //----------------------------------------------------------------------------------------------------------
        void OErrorReporter_Close(object sender, EventArgs e)
        {
            this.setMode(enumMainAppModes.XAML_ERROR_HIDE);
        }

        //----------------------------------------------------------------------------------------------------------
        void OErrorReporter_Save(object sender, ExceptionEventArgs e)
        {
            VistaSaveFileDialog dialog = new VistaSaveFileDialog();
            dialog.Filter = "TXT Files (*.txt)|*.txt";
            if (!Directory.Exists(this.ProjectDirectory + @"\XamlErrLogs\"))
            {
                Directory.CreateDirectory(this.ProjectDirectory + @"\XamlErrLogs\");
            }

            dialog.InitialDirectory = (this.ProjectDirectory + @"\XamlErrLogs\");
            
            string sErrFilePath;

            if ((bool)dialog.ShowDialog(this))
            {
                sErrFilePath = dialog.FileName;

                string sContent = e.ExceptionMessage + "\n\n\n" + "StackTrace: " + e.ExceptionStackTrace;

                WayvFS.SaveXamlError(sErrFilePath, sContent);

            }
        }


        //----------------------------------------------------------------------------------------------------------
        // Event Handler Methods Section: Control Framework Basic Preview
        //----------------------------------------------------------------------------------------------------------
        void ControlsPlaceHolder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == this.ControlsPlaceHolder)
            {
            }
            else
            {
                _isDown = true;
                _startPoint = e.GetPosition(this.ControlsPlaceHolder);
                _originalElement = e.Source as UIElement;
                this.ControlsPlaceHolder.CaptureMouse();
                e.Handled = true;
            }
        }

        //----------------------------------------------------------------------------------------------------------
        void ControlsPlaceHolder_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isDown)
            {
                if ((_isDragging == false) && ((Math.Abs(e.GetPosition(this.ControlsPlaceHolder).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(this.ControlsPlaceHolder).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    DragStarted();
                }
                if (_isDragging)
                {
                    DragMoved();
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void DragStarted()
        {
            _isDragging = true;
            _originalLeft = Canvas.GetLeft(_originalElement);
            _originalTop = Canvas.GetTop(_originalElement);

            _overlayElement = new CircleAdorner(_originalElement);
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
            layer.Add(_overlayElement);

        }

        //----------------------------------------------------------------------------------------------------------
        private void DragMoved()
        {
            Point CurrentPosition = System.Windows.Input.Mouse.GetPosition(this.ControlsPlaceHolder);

            _overlayElement.LeftOffset = ((CurrentPosition.X - _startPoint.X) * App.currentZoom);
            _overlayElement.TopOffset  = ((CurrentPosition.Y - _startPoint.Y) * App.currentZoom);

        }

        //----------------------------------------------------------------------------------------------------------
        void ControlsPlaceHolder_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_isDown)
            {
                DragFinished(false);
                e.Handled = true;
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void DragFinished(bool cancelled)
        {
            System.Windows.Input.Mouse.Capture(null);
            if (_isDragging)
            {
                AdornerLayer.GetAdornerLayer(_overlayElement.AdornedElement).Remove(_overlayElement);

                if (cancelled == false)
                {
                    Canvas.SetTop(_originalElement, _originalTop + _overlayElement.TopOffset);
                    Canvas.SetLeft(_originalElement, _originalLeft + _overlayElement.LeftOffset);
                }
                _overlayElement = null;

            }
            _isDragging = false;
            _isDown = false;
        }


        //----------------------------------------------------------------------------------------------------------
        void ControlsPlaceHolder_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape && _isDragging)
            {
                DragFinished(true);
            }
        }



        //----------------------------------------------------------------------------------------------------------
        // Control Pane Event Handler Methods Section
        //----------------------------------------------------------------------------------------------------------
        // SyncLinks
        //----------------------------------------------------------------------------------------------------------
        private void cmdAddMarker_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentFrame != null)
            {
                this.OTLC.AddMarker(this.currentFrame.Id);
                this.currentFrame.DesignTimeHelper.HasSyncLink = true;
                this.SaveVOMarkers();
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void cmdRemoveMarker_Click(object sender, RoutedEventArgs e)
        {
            int iMarkerId = this.OTLC.RemoveMarker();
            if (iMarkerId != 0)
            {
                Frame OFrame = this.currentShow.Frames.FirstOrDefault(c => c.Id == iMarkerId);
                OFrame.DesignTimeHelper.HasSyncLink = false;
            }
            this.SaveVOMarkers();
        }

        //----------------------------------------------------------------------------------------------------------
        private void OTLC_VideoPauseRequested(object sender, EventArgs e)
        {
            this.OBBar.OVideoPlayer.Pause();
        }


        //----------------------------------------------------------------------------------------------------------
        // Xaml Links
        //----------------------------------------------------------------------------------------------------------
        private void ManageXamlLinksListView_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement OElement = (e.OriginalSource as FrameworkElement);

            String sName = "Unknown";
            if (OElement.Name != "")
            {
                sName = OElement.Name;
            }

            ListViewItem OListViewItem = this.ManageXamlLinksListView.ItemContainerGenerator.ContainerFromItem(OElement.DataContext) as ListViewItem;

            if (OListViewItem != null)
            {
                ContentPresenter OContentPresenter = FindVisualChild<ContentPresenter>(OListViewItem);

                //(OListViewItem.ContentTemplate.FindName("cmdAnimation", OContentPresenter) as ToggleButton).IsChecked = false;
                
                switch (sName)
                {
                    case "cmdXamlLinkRemoveXaml":
                        this.RemoveXamlAnimation((OListViewItem.DataContext as Frame));
                        break;

                    case "cmdXamlLinkCreateLink":
                        this.CreateXamlLink((OListViewItem.DataContext as Frame));
                        break;

                    case "cmdXamlLinkUpdateLink":
                        this.UpdateXamlLink((OListViewItem.DataContext as Frame));
                        break;

                    case "cmdXamlLinkRemoveLink":
                        this.RemoveXamlLink((OListViewItem.DataContext as Frame));
                        break;


                }

                // Allow Saving
                this.currentShow.IsDirty = true;
            }

        }


        //----------------------------------------------------------------------------------------------------------
        private void ManageXamlLinksListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement OElement = (e.OriginalSource as FrameworkElement);

            String sName = "Unknown";
            if (OElement.Name != "")
            {
                sName = OElement.Name;
            }

            ListViewItem OListViewItem = 
                this.ManageXamlLinksListView.ItemContainerGenerator.ContainerFromItem(OElement.DataContext) as ListViewItem;

            if (OListViewItem != null)
            {
                this.ScriptListBox.SelectedItem = (OListViewItem.DataContext as Frame);
            }

        }


        //----------------------------------------------------------------------------------------------------------
        private void RemoveXamlAnimation(Frame frame)
        {
            if (frame == this.currentFrame)
            {
                this.UnLoadXamlAnimation();
            }

            this.RemoveXamlLink(frame);

            if (this.framesWithXamlContent.Contains(frame))
            {
                this.framesWithXamlContent.Remove(frame);
            }

            // Restores Background Image
            if (frame is IAcceptsBackgroundImage)
            {
                (frame as IAcceptsBackgroundImage).BackgroundImagePath =
                    (frame as IAcceptsBackgroundImage).InactiveBackgroundImagePath;



                if (frame == this.currentFrame)
                { 
                    // Updates the UI
                    this.BindCurrentFrame();
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void CreateXamlLink(Frame frame)
        {
            String sXamlFilePath = SelectFile("Xaml");

            if (sXamlFilePath != "")
            {
                // Save Data
                frame.DesignTimeHelper.XamlInfo.XamlPath = sXamlFilePath;
                frame.DesignTimeHelper.XamlInfo.DateOfLastModification = File.GetLastWriteTime(sXamlFilePath);
                this.UpdateXamlLinksPane();
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void CreateXamlLink(Frame frame, String sXamlFilePath)
        {
            if (sXamlFilePath != "")
            {
                // Save Data
                frame.DesignTimeHelper.XamlInfo.XamlPath = sXamlFilePath;
                frame.DesignTimeHelper.XamlInfo.DateOfLastModification = File.GetLastWriteTime(sXamlFilePath);
                this.UpdateXamlLinksPane();
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void RemoveXamlLink(Frame frame)
        {
            frame.DesignTimeHelper.XamlInfo.XamlPath = "Empty";
            frame.DesignTimeHelper.XamlInfo.DateOfLastModification = DateTime.Now;
            frame.DesignTimeHelper.XamlContent = null;
        }

        //----------------------------------------------------------------------------------------------------------
        private void UpdateXamlLink(Frame frame)
        {
            String sXamlFilePath = SelectFile("Xaml");
            this.RemoveXamlLink(frame);
            this.CreateXamlLink(frame,sXamlFilePath);
        }

        
        //----------------------------------------------------------------------------------------------------------
        private String SelectFile(String pType)
        {
            String sRetVal = "";
            // Get Open Dialog Information
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Filter = pType + " Files (*." + pType.ToLower() + ")|*." + pType.ToLower();
            dialog.InitialDirectory = App.BaseProjectPath;

            if ((bool)dialog.ShowDialog(this))
            {
                // Sets Vars
                sRetVal = dialog.FileName;
            }

            return sRetVal;
        }

        //----------------------------------------------------------------------------------------------------------
        private void ManageBrokenResourceLinksListView_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement OElement = (e.OriginalSource as FrameworkElement);

            String sName = "Unknown";
            if (OElement.Name != "")
            {
                sName = OElement.Name;
            }

            ListViewItem OListViewItem = this.ManageBrokenResourceLinksListView.ItemContainerGenerator.ContainerFromItem(OElement.DataContext) as ListViewItem;

            if (OListViewItem != null)
            {
                ContentPresenter OContentPresenter = FindVisualChild<ContentPresenter>(OListViewItem);

                //(OListViewItem.ContentTemplate.FindName("cmdAnimation", OContentPresenter) as ToggleButton).IsChecked = false;

                if (sName == "cmdResourceLinkImport")
                {
                    this.FixResourceReference(OListViewItem.DataContext as Asset);
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void FixResourceReference(Asset a)
        {
            String sFile = this.SelectFile(a.Type);

            if (sFile != "")
            {
                // Copy File into Resources Folder
                String fixedResourcePath = this.ProjectDirectory + @"\Resources\" + System.IO.Path.GetFileName(sFile);
                try
                {
                    File.Copy(sFile, fixedResourcePath, false);
                }
                catch (System.IO.IOException ex)
                {
                    this.OExceptionSink.Manage(ex);
                }

                // Update Object Graph with correct Resource
                if (a.Description == enumAssetTypes.NVIDEO)
                {
                    this.currentShow.NVideoPath = fixedResourcePath;
                }

                if (a.Description == enumAssetTypes.CHANNEL_LOGO)
                {
                    this.currentShow.ChannelLogoPath = fixedResourcePath;
                }

                if (a.Description == enumAssetTypes.DEFAULT_THUMBNAIL_PATH)
                {
                    this.currentShow.DefaultThumbnailPath = fixedResourcePath;
                }

                if (a.Description == enumAssetTypes.FRAME_BACKGROUND)
                {
                    foreach (Frame f in this.currentShow.Frames)
                    {
                        if ((f.Id == a.FrameID) && (f is IAcceptsBackgroundImage))
                        {
                            (f as IAcceptsBackgroundImage).BackgroundImagePath = fixedResourcePath;
                        }
                    }
                }

                if (a.Description == enumAssetTypes.ASSET_FOR_XAML_ANIMATION)
                {
                    foreach (Frame f in this.currentShow.Frames)
                    {
                        if ((f.Id == a.FrameID) && (f is IUsesAssets))
                        {
                            foreach (Asset a2 in (f as IUsesAssets).Assets)
                            {
                                //if (a2.)
                                //{
                                //    = fixedResourcePath;
                                //}
                            }
                        }
                    }
                }

            }
        }




        //----------------------------------------------------------------------------------------------------------
        // Private Methods Section
        //----------------------------------------------------------------------------------------------------------
        // Private Methods Section : Frame Pane
        //----------------------------------------------------------------------------------------------------------
        private void CollapseAllScriptItems()
        {
            foreach (Frame OFrame in this.ScriptListBox.Items)
            {
                ListBoxItem OListBoxItem = (this.ScriptListBox.ItemContainerGenerator.ContainerFromItem(OFrame) as ListBoxItem);
                ContentPresenter OContentPresenter = FindVisualChild<ContentPresenter>(OListBoxItem);
                Expander ScriptItemExpander = (OListBoxItem.ContentTemplate.FindName("ScriptItemExpander", OContentPresenter) as Expander);
                ScriptItemExpander.IsExpanded = false;
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void ExpandAllScriptItems()
        {
            foreach (Frame OFrame in this.ScriptListBox.Items)
            {
                ListBoxItem OListBoxItem = (this.ScriptListBox.ItemContainerGenerator.ContainerFromItem(OFrame) as ListBoxItem);
                ContentPresenter OContentPresenter = FindVisualChild<ContentPresenter>(OListBoxItem);
                Expander ScriptItemExpander = (OListBoxItem.ContentTemplate.FindName("ScriptItemExpander", OContentPresenter) as Expander);
                ScriptItemExpander.IsExpanded = true;
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void collapseAllBut(int index)
        {
            for (int iCounter = 0; iCounter < this.ScriptListBox.Items.Count; iCounter++)
            {
                if (iCounter != index)
                {
                    this.changeScriptExpanderState(iCounter, false);
                }
            }
        }


        //----------------------------------------------------------------------------------------------------------
        private void expandAllBut(int index)
        {
            for (int iCounter = 0; iCounter < this.ScriptListBox.Items.Count; iCounter++)
            {
                if (iCounter != index)
                {
                    this.changeScriptExpanderState(iCounter, true);
                }
            }
        }


        //----------------------------------------------------------------------------------------------------------
        private void changeScriptExpanderState(int whichOne, bool expanded)
        {
            Frame OFrame = (this.ScriptListBox.Items[whichOne] as Frame);

            ListBoxItem OListBoxItem =
                (this.ScriptListBox.ItemContainerGenerator.ContainerFromItem(OFrame) as ListBoxItem);
            ContentPresenter OContentPresenter = FindVisualChild<ContentPresenter>(OListBoxItem);
            Expander ScriptItemExpander = (OListBoxItem.ContentTemplate.FindName("ScriptItemExpander", OContentPresenter) as Expander);
            ScriptItemExpander.IsExpanded = expanded;
        }

        //----------------------------------------------------------------------------------------------------------
        // Private Methods : Utility
        //----------------------------------------------------------------------------------------------------------
        private childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }


        //----------------------------------------------------------------------------------------------------------
        // Private Methods: Overrides Section
        //----------------------------------------------------------------------------------------------------------
        // Resize Window Proportionally
        //----------------------------------------------------------------------------------------------------------
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = HwndSource.FromVisual(this) as HwndSource;
            if (source != null)
            {
                //source.AddHook(new HwndSourceHook(WinProc));
            }
        }


        //----------------------------------------------------------------------------------------------------------
        // Private Methods: API Methods / Resize Window/Components Events
        //----------------------------------------------------------------------------------------------------------
        // Resize Window Proportionally
        //----------------------------------------------------------------------------------------------------------
        private IntPtr WinProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled)
        {
            IntPtr result = IntPtr.Zero;
            switch (msg)
            {
                case WM_EXITSIZEMOVE:
                    this.Height = (this.Width * 718);
                    break;
            }
            return result;
        }


        //----------------------------------------------------------------------------------------------------------
        // DataBinding Methods
        //----------------------------------------------------------------------------------------------------------
        private void RecreateBindings(bool pResetData)
        {
            // Resets DataBinding
            this.ScriptListBox.DataContext = null;
            this.ScriptListBox.ItemsSource = null;
            this.ShowInfoPane.DataContext = null;
            this.OBBar.DataContext = null;
            this.OShowPropertyGrid.DataContext = null;
            this.OShowPropertyGrid.SelectedObject = null;
            this.cmdToggleExapandMode.DataContext = null;
            this.VolumeSlider.DataContext = null;
            this.toggleMute.DataContext = null;
            this.ManageXamlLinksListView.DataContext = null;
            this.ManageBrokenResourceLinksListView.DataContext = null;

            // Reset Data
            if (pResetData)
            {
                this.ResetData();
            }

            // Sets DataBinding
            if (this.currentShow.ChannelLogoPath != null)
            {
                BitmapImage OSource = new BitmapImage();
                OSource.BeginInit();
                OSource.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                OSource.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

                OSource.UriSource = new Uri(this.currentShow.ChannelLogoPath, UriKind.Absolute);
                OSource.CacheOption = BitmapCacheOption.OnLoad;
                OSource.EndInit();
                this.WebChannelLogo.Source = OSource;
                        
            }

            this.ScriptListBox.DataContext = this.currentShow;
            this.ScriptListBox.ItemsSource = this.currentShow.Frames;
            this.ShowInfoPane.DataContext = this.currentShow;
            this.OBBar.DataContext = this.currentShow;
            this.OShowPropertyGrid.DataContext = this.currentShow;
            this.OShowPropertyGrid.SelectedObject = this.currentShow;
            this.cmdToggleExapandMode.DataContext = this.LayoutSettings;
            this.VolumeSlider.DataContext = this.currentShow.DesignTimeHelper;
            this.toggleMute.DataContext = this.currentShow;

            if (this.currentShow.NVideoPath != "")
            {
                this.OBBar.SetVideo(new Uri(this.ProjectDirectory + @"\Resources\" + this.currentShow.NVideoPath, UriKind.Absolute));
            }


            // Control Pane Views
            this.LoadFramesWithXamlContent();
            this.ManageXamlLinksListView.ItemsSource = this.framesWithXamlContent;
            this.LoadBrokenResourceLinks();
            this.ManageBrokenResourceLinksListView.ItemsSource = this.brokenResourceLinks;
            
            // Frame DataBinding
            this.BindCurrentFrame();

            // Marks Show as not Dirty
            this.currentShow.IsDirty = false;
        }

        //----------------------------------------------------------------------------------------------------------
        private void LoadFramesWithXamlContent()
        {
            // Clears and Reloads the Collection
            this.framesWithXamlContent.Clear();

            foreach (Frame f in this.currentShow.Frames)
            {
                if (f.DesignTimeHelper.XamlContent != null)
                {
                    if (!this.framesWithXamlContent.Contains(f))
                    {
                        this.framesWithXamlContent.Add(f);
                    }
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void LoadBrokenResourceLinks()
        {
            // Clears and Reloads the Collection
            this.brokenResourceLinks.Clear();
            //------------------------------------------------------------------------------------------------------
            // Routine to load Broken Resources
            //------------------------------------------------------------------------------------------------------
            // Show Resources
            //------------------------------------------------------------------------------------------------------
            // NVideo
            if (!File.Exists(this.ProjectDirectory + @"\Resources\" + this.currentShow.NVideoPath))
            {
                this.brokenResourceLinks.Add(new Asset(0, "NVideoPath", enumAssetTypes.NVIDEO, System.IO.Path.GetFileName(this.currentShow.NVideoPath), this.ProjectDirectory + @"\Resources\" + this.currentShow.NVideoPath));
            }
            //------------------------------------------------------------------------------------------------------
            // Channel Image
            if (!File.Exists(this.currentShow.ChannelLogoPath))
            {
                this.brokenResourceLinks.Add(new Asset(0,"ChannelLogoPath",enumAssetTypes.CHANNEL_LOGO, System.IO.Path.GetFileName(this.currentShow.ChannelLogoPath), this.currentShow.ChannelLogoPath));
            }
            
            //------------------------------------------------------------------------------------------------------
            // Default Thumbnail Path
            if (!File.Exists(this.currentShow.DefaultThumbnailPath))
            {
                this.brokenResourceLinks.Add(new Asset(0,"DefaultThumbnailPath",enumAssetTypes.DEFAULT_THUMBNAIL_PATH, System.IO.Path.GetFileName(this.currentShow.DefaultThumbnailPath), this.currentShow.DefaultThumbnailPath));
            }

            //------------------------------------------------------------------------------------------------------
            // Frame Resources
            //------------------------------------------------------------------------------------------------------
            foreach (Frame f in this.currentShow.Frames)
            {
                // Background Image
                if (f is IAcceptsBackgroundImage)
                {
                    if ((!File.Exists((f as IAcceptsBackgroundImage).BackgroundImagePath)) && ((f as IAcceptsBackgroundImage).BackgroundImagePath.IndexOf("IAcceptsImageBackgroundFramePlaceHolder.jpg") == -1))
                    {
                        this.brokenResourceLinks.Add(new Asset(f.Id,"BackgroundImagePath",enumAssetTypes.FRAME_BACKGROUND, System.IO.Path.GetFileName((f as IAcceptsBackgroundImage).BackgroundImagePath), (f as IAcceptsBackgroundImage).BackgroundImagePath));
                    }
                }

                // Assets Collection
                if (f is IUsesAssets)
                {
                    foreach (Asset a in (f as IUsesAssets).Assets)
                    {
                        if (!File.Exists((this.ProjectDirectory + @"\Resources\" + System.IO.Path.GetFileName(a.FileName))))
                        {
                            if (a.Description == 0)
                            {
                                a.Description = enumAssetTypes.ASSET_FOR_XAML_ANIMATION;
                            }
                            this.brokenResourceLinks.Add(a);
                        }
                    }
                }
            }
        }
        
        //----------------------------------------------------------------------------------------------------------
        private void BindCurrentFrame()
        {
            // Sets Data Context for View Pane as the Currently Selected Frame
            if (this.currentFrame != null)
            {
        
                // Updates Toolbox
                this.Toolbox.DataContext = this.currentFrame;

                if (this.currentFrame.DesignTimeHelper.XamlContent != null)
                {
                    this.LoadXamlAnimation();
                    this.toggleFrameAnimation.IsEnabled = true;
                }
                else
                {
                    this.UnLoadXamlAnimation();
                    this.toggleFrameAnimation.IsEnabled = false;
                }

                this.LoadBackgroundImage();

                // Updates Properties Pane <<< Fix Bug Here!!
                this.OPropertyGrid.SelectedObject = this.currentFrame;
            }

        }

        //----------------------------------------------------------------------------------------------------------
        private void UpdateXamlLinksPane()
        {
            // Updates the Bindings by Brute Force...

        }

        //----------------------------------------------------------------------------------------------------------
        private void ResetData()
        {
            this.currentProject.Show = new Show();
            this.currentShow = this.currentProject.Show;
        }


        //----------------------------------------------------------------------------------------------------------
        // ViewPane Methods
        //----------------------------------------------------------------------------------------------------------
        private void ZoomToExtents()
        {
            double ocw = this.ViewPaneBorder.ActualWidth;
            double och = this.ViewPaneBorder.ActualHeight;
            double icw = 1024;
            double ich = 576;

            double wd = ((ocw >= icw) ? (ocw - icw) : (icw - ocw));
            double hd = ((och >= ich) ? (och - ich) : (ich - och));

            double result = ((((wd >= hd) ? (och/ich) : (ocw/icw)))*.9);

            this.DocumentSize.ScaleY = result;
            this.DocumentSize.ScaleX = result;

            this.ZoomSlider.Value = result;
        }

        //----------------------------------------------------------------------------------------------------------
        // ScriptPane Private Methods
        //----------------------------------------------------------------------------------------------------------
        private void ApplyScriptDataBinding()
        {
            int iCounter = 1;
            foreach (Frame OFrame in this.ScriptListBox.Items)
            {
                ListBoxItem OListBoxItem = (this.ScriptListBox.ItemContainerGenerator.ContainerFromItem(OFrame) as ListBoxItem);
                ContentPresenter OContentPresenter = FindVisualChild<ContentPresenter>(OListBoxItem);
                OListBoxItem.ApplyTemplate();
                Label lblFrameNumber = (OListBoxItem.ContentTemplate.FindName("lblFrameNumber", OContentPresenter) as Label);
                lblFrameNumber.Content = iCounter.ToString();
                iCounter++;
            }

            // Control Pane Views
            this.ManageXamlLinksListView.ItemsSource = null;
            this.ManageXamlLinksListView.ItemsSource = this.framesWithXamlContent;
            
            this.currentShow.IsDirty = true;
        }

        //----------------------------------------------------------------------------------------------------------
        private void LoadBackgroundImage()
        {
            // Updates Background
            if (this.currentFrame is IAcceptsBackgroundImage)
            {
                if (((this.currentFrame as IAcceptsBackgroundImage).BackgroundImagePath != null) &&
                    ((this.currentFrame as IAcceptsBackgroundImage).BackgroundImagePath.IndexOf("IAcceptsImageBackgroundFramePlaceHolder.jpg") == -1)
                    )
                {
                    this.OFrameBackground.Source =
                        new BitmapImage(new Uri((this.currentFrame as IAcceptsBackgroundImage).BackgroundImagePath, UriKind.Absolute));
                }
                else
                {
                    (this.currentFrame as IAcceptsBackgroundImage).BackgroundImagePath = 
                        @"\Assets\Bitmaps\ViewPane\IAcceptsImageBackgroundFramePlaceHolder.jpg";

                    switch (this.currentFrame.GetType().ToString())
                    { 
                        case "ProducerAlpha.WebSlide":
                            (this.currentFrame as Frame).Script.ImageUrl = 
                                "/Assets/Bitmaps/ScriptItemDataTemplate/ImageThumb.png";
                            break;

                        case "ProducerAlpha.VideoFrame":
                            (this.currentFrame as Frame).Script.ImageUrl =
                                "/Assets/Bitmaps/ScriptItemDataTemplate/VideoThumb.png";
                            break;

                        case "ProducerAlpha.ConversationalPoll":
                            (this.currentFrame as Frame).Script.ImageUrl =
                                "/Assets/Bitmaps/ScriptItemDataTemplate/ConversationalPollThumb.png";
                            break;
                    }

                    this.OFrameBackground.Source =
                        new BitmapImage(new Uri(@"\Assets\Bitmaps\ViewPane\IAcceptsImageBackgroundFramePlaceHolder.jpg", UriKind.Relative));
                }
            }
            else
            {
                this.OFrameBackground.Source =
                    new BitmapImage(new Uri(@"\Assets\Bitmaps\ViewPane\FramePlaceHolder.jpg", UriKind.Relative));
            }

        }

        //----------------------------------------------------------------------------------------------------------
        private void LoadXamlAnimation()
        {
            // Declares Objects for Openning Xaml
            System.Xml.XmlTextReader xmlReader = null;

            // Open Xaml
            try
            {
                // Get the Xaml
                xmlReader = new XmlTextReader(
                    new MemoryStream(
                            ASCIIEncoding.Default.GetBytes(
                                this.currentFrame.DesignTimeHelper.XamlContent
                            )
                     )
                );

                Canvas importedXamlGraph = (Canvas)XamlReader.Load(xmlReader);

                // Debug: (Makes Imported Xaml Visible)
                // importedXamlGraph.SetValue(Canvas.BackgroundProperty, new SolidColorBrush(Color.FromArgb(88, 00, 00, 55)));


                // Insert Resources into Xaml
                foreach (Asset asset in (this.currentFrame as IUsesAssets).Assets)
                {
                    String sID = asset.FileID;
                    String sFileName = System.IO.Path.GetFileName(asset.FileName);

                    Image OImage = (importedXamlGraph.FindName(sID) as Image);
                    OImage.Source = new BitmapImage(
                        new Uri(
                            this.ProjectDirectory + @"\Resources\" + sFileName,
                            UriKind.Absolute
                        )
                    );
                }

                // Loads into UI
                this.UnLoadXamlAnimation();
                this.XamlContentPlaceHolder.Children.Add(importedXamlGraph);
            }

            catch (Exception ex)
            {
                this.OExceptionSink.Manage(ex);
            }

            finally
            {
                if (xmlReader != null) { xmlReader.Close(); }
            }

        }

        //----------------------------------------------------------------------------------------------------------
        private void UnLoadXamlAnimation()
        {
            // Delete Previous Xaml on Xaml Reload...
            if (this.XamlContentPlaceHolder.Children.Count > 0)
            {
                this.XamlContentPlaceHolder.Children.Remove(
                    this.XamlContentPlaceHolder.Children[0]);
                this.XamlContentPlaceHolder.Children.Clear();
            }
            this.XamlContentPlaceHolder.Children.Clear();
        }


        //----------------------------------------------------------------------------------------------------------
        // Handle Script Pane Grid/Expanders
        //----------------------------------------------------------------------------------------------------------
        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.LayoutSettings.ApplyMainLayout) // if the refresh was *NOT* product of a PropertiesPane Collapse
            {
                if (!((this.ScriptPane.IsExpanded == false) && (this.PropertiesPane.IsExpanded == false)))
                {
                    double MainScriptPaneHeight = 0;
                    // Resize the MainScriptPane and assign the correct Height
                    MainScriptPaneHeight = ((this.PaneContainer.ActualHeight - ScriptTab.ActualHeight) - MAIN_LEFT_PANE_TOP_MARGIN);
                    this.MainScriptPane.Height = MainScriptPaneHeight;



                    if (this.PropertiesPane.IsExpanded) // Properties Pane *NOT* collapsed
                    {
                        // Set the Maximum Height of the Properties Pane to the Height of the Control Pane
                        this.ScriptPaneDivider.MaxHeight = (MainScriptPaneHeight - this.ControlPane.ActualHeight);
                    }
                    else
                    {
                        this.ChangeFramePaneHeight((MainScriptPaneHeight - COLLAPSED_EXPANDER_PLUS_BOTTOM_MARGIN_HEIGHT));
                    }
                }

                // Resize the MainShowPane and assign the correct Height
                if (!((this.ShowInfoPane.IsExpanded == false) && (this.ShowPropertiesPane.IsExpanded == false)))
                {
                    double MainShowPaneHeight = ((this.PaneContainer.ActualHeight - ShowTab.ActualHeight) - MAIN_LEFT_PANE_TOP_MARGIN);
                    this.MainShowPane.Height = MainShowPaneHeight;

                    if (this.ShowPropertiesPane.IsExpanded) // Properties Pane *NOT* collapsed
                    {
                        // Set the Maximum Height of the Properties Pane to the Height of the Control Pane
                        this.ShowPaneDivider.MaxHeight = (MainShowPaneHeight - this.ControlPane.ActualHeight);
                    }
                    else
                    {
                        this.ChangeShowPaneHeight((((this.PaneContainer.ActualHeight - ShowTab.ActualHeight) - COLLAPSED_EXPANDER_PLUS_BOTTOM_MARGIN_HEIGHT) - MAIN_LEFT_PANE_TOP_MARGIN));
                    }
                }
            }

            this.LayoutSettings.ApplyMainLayout = true;
        }

        //----------------------------------------------------------------------------------------------------------
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (this.LayoutSettings.ApplyMainLayout) // if the refresh was *NOT* product of a PropertiesPane Collapse
            {
                // Resize the MainScriptPane and assign the correct Height
                double MainScriptPaneHeight = ((this.PaneContainer.ActualHeight - ScriptTab.ActualHeight) - MAIN_LEFT_PANE_TOP_MARGIN);
                this.MainScriptPane.Height = MainScriptPaneHeight;

                if (this.PropertiesPane.IsExpanded) // Properties Pane *NOT* collapsed
                {
                    // Set the Maximum Height of the Properties Pane to the Height of the Control Pane
                    this.ScriptPaneDivider.MaxHeight = (MainScriptPaneHeight - this.ControlPane.ActualHeight);
                }
                else
                {
                    this.ChangeFramePaneHeight(((this.PaneContainer.ActualHeight - ScriptTab.ActualHeight) - COLLAPSED_EXPANDER_PLUS_BOTTOM_MARGIN_HEIGHT));
                }


                // Resize the MainShowPane and assign the correct Height
                double MainShowPaneHeight = ((this.PaneContainer.ActualHeight - ShowTab.ActualHeight) - MAIN_LEFT_PANE_TOP_MARGIN);
                this.MainShowPane.Height = MainShowPaneHeight;

                if (this.ShowPropertiesPane.IsExpanded) // Properties Pane *NOT* collapsed
                {
                    // Set the Maximum Height of the Properties Pane to the Height of the Control Pane
                    this.ShowPaneDivider.MaxHeight = (MainShowPaneHeight - this.ControlPane.ActualHeight);
                }
                else
                {
                    this.ChangeShowPaneHeight(((this.PaneContainer.ActualHeight - ScriptTab.ActualHeight) - COLLAPSED_EXPANDER_PLUS_BOTTOM_MARGIN_HEIGHT));
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void ChangeFramePaneHeight(double newHeight)
        {
            this.ScriptPaneDivider.MaxHeight = newHeight;
            this.ScriptPaneDivider.Height = new GridLength(newHeight);
        }

        //----------------------------------------------------------------------------------------------------------
        private void ChangeShowPaneHeight(double newHeight)
        {
            this.ShowPaneDivider.MaxHeight = newHeight;
            this.ShowPaneDivider.Height = new GridLength(newHeight);
        }

        //----------------------------------------------------------------------------------------------------------
        private void ScriptPane_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Register new data in Layout object
            if (!this.t1EventComesFromExpandOrCollapse)
            {
                if (e.NewSize.Height > 28)
                {
                    this.LayoutSettings.ScriptPaneTopPanelHeight = e.NewSize.Height;
                }
            }
            else
            {

                this.t1EventComesFromExpandOrCollapse = false;
            }

            if ((this.ScriptPaneDivider.ActualHeight >= this.ViewPaneBorder.ActualHeight) &&
                (this.ScriptPane.IsExpanded) &&
                (this.ScriptPaneDivider.Height == GridLength.Auto))
            {
                ChangeFramePaneHeight(MainScriptPane.ActualHeight - this.ControlPane.ActualHeight);
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void ScriptPane_Collapsed(object sender, RoutedEventArgs e)
        {
            this.t1EventComesFromExpandOrCollapse = true;

            if (this.PropertiesPane.IsExpanded)
            {
                this.AccordionState4ScriptPanel(enumAccordionStates.TOPCLOSED_BOTTOMOPEN);
            }
            else
            {
                this.AccordionState4ScriptPanel(enumAccordionStates.TOPCLOSED_BOTTOMCLOSED);
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void ScriptPane_Expanded(object sender, RoutedEventArgs e)
        {
            if (this.bLoaded)
            {
                this.t1EventComesFromExpandOrCollapse = true;

                if (this.PropertiesPane.IsExpanded)
                {
                    this.AccordionState4ScriptPanel(enumAccordionStates.TOPOPEN_BOTTOMOPEN);
                }
                else
                {
                    this.AccordionState4ScriptPanel(enumAccordionStates.TOPOPEN_BOTTOMCLOSED);
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void PropertiesPane_Collapsed(object sender, RoutedEventArgs e)
        {
            this.t1EventComesFromExpandOrCollapse = true;

            if (this.ScriptPane.IsExpanded)
            {
                this.AccordionState4ScriptPanel(enumAccordionStates.TOPOPEN_BOTTOMCLOSED);
            }
            else
            {
                this.AccordionState4ScriptPanel(enumAccordionStates.TOPCLOSED_BOTTOMCLOSED);
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void PropertiesPane_Expanded(object sender, RoutedEventArgs e)
        {
            if (this.bLoaded)
            {
                this.t1EventComesFromExpandOrCollapse = true;

                if (this.ScriptPane.IsExpanded)
                {
                    this.AccordionState4ScriptPanel(enumAccordionStates.TOPOPEN_BOTTOMOPEN);
                }
                else
                {
                    this.AccordionState4ScriptPanel(enumAccordionStates.TOPCLOSED_BOTTOMOPEN);
                }
            }
        }


        //----------------------------------------------------------------------------------------------------------
        // State Machine Controller
        private void AccordionState4ScriptPanel(enumAccordionStates state)
        {
            switch (state)
            {
                case enumAccordionStates.TOPCLOSED_BOTTOMCLOSED:
                    // Deactivate GridSplitter
                    this.OScriptPaneSplitter.Height = 0;

                    // Reduces size of ScriptPane to Collapsed (Properties Pane grows by default)
                    this.MainScriptPane.RowDefinitions[SCRIPT_PANE].Height =
                        new GridLength(24);

                    this.LayoutSettings.ApplyMainLayout = false;

                    break;

                case enumAccordionStates.TOPCLOSED_BOTTOMOPEN:
                    // Deactivate GridSplitter
                    this.OScriptPaneSplitter.Height = 0;

                    // Reduces size of ScriptPane to Collapsed (Properties Pane grows by default)
                    this.MainScriptPane.RowDefinitions[SCRIPT_PANE].Height =
                        new GridLength(24);

                    break;

                case enumAccordionStates.TOPOPEN_BOTTOMCLOSED:
                    // Deactivate GridSplitter
                    this.OScriptPaneSplitter.Height = 0;

                    // Takes all available space
                    this.ScriptPaneDivider.MaxHeight = (((this.PaneContainer.ActualHeight - ScriptTab.ActualHeight) - COLLAPSED_EXPANDER_PLUS_BOTTOM_MARGIN_HEIGHT) - MAIN_LEFT_PANE_TOP_MARGIN);


                    this.ScriptPaneDivider.Height =
                        new GridLength((((this.PaneContainer.ActualHeight - ScriptTab.ActualHeight) - COLLAPSED_EXPANDER_PLUS_BOTTOM_MARGIN_HEIGHT)-MAIN_LEFT_PANE_TOP_MARGIN));

                    this.LayoutSettings.ApplyMainLayout = false;

                    break;


                case enumAccordionStates.TOPOPEN_BOTTOMOPEN:
                    // Reactivate GridSplitter
                    this.OScriptPaneSplitter.Height = 4;

                    // Restores original heights
                    this.MainScriptPane.RowDefinitions[SCRIPT_PANE].Height =
                        new GridLength(this.LayoutSettings.ScriptPaneTopPanelHeight);

                    break;
            }
        }


        //----------------------------------------------------------------------------------------------------------
        // Handle Show Metadata Pane Grid/Expanders
        //----------------------------------------------------------------------------------------------------------
        private void ShowInfoPane_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Register new data in Layout object
            if (!this.t2EventComesFromExpandOrCollapse)
            {
                if (e.NewSize.Height > 29)
                {
                    this.LayoutSettings.ShowPaneTopPanelHeight = e.NewSize.Height;
                }
            }
            else
            {
                this.t2EventComesFromExpandOrCollapse = false;
            }

            if ((this.ShowPaneDivider.ActualHeight >= this.ViewPaneBorder.ActualHeight) &&
                (this.ShowInfoPane.IsExpanded) &&
                (this.ShowPaneDivider.Height == GridLength.Auto))
            {
                ChangeFramePaneHeight(MainScriptPane.ActualHeight - this.ControlPane.ActualHeight);
            }
        }


        //----------------------------------------------------------------------------------------------------------
        private void ShowInfoPane_Collapsed(object sender, RoutedEventArgs e)
        {
            this.t2EventComesFromExpandOrCollapse = true;

            if (this.ShowPropertiesPane.IsExpanded)
            {
                this.AccordionState4ShowPanel(enumAccordionStates.TOPCLOSED_BOTTOMOPEN);
            }
            else
            {
                this.AccordionState4ShowPanel(enumAccordionStates.TOPCLOSED_BOTTOMCLOSED);
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void ShowInfoPane_Expanded(object sender, RoutedEventArgs e)
        {
            if (this.bLoaded)
            {
                this.t2EventComesFromExpandOrCollapse = true;

                if (this.ShowPropertiesPane.IsExpanded)
                {
                    this.AccordionState4ShowPanel(enumAccordionStates.TOPOPEN_BOTTOMOPEN);
                }
                else
                {
                    this.AccordionState4ShowPanel(enumAccordionStates.TOPOPEN_BOTTOMCLOSED);
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void ShowPropertiesPane_Collapsed(object sender, RoutedEventArgs e)
        {
            this.t2EventComesFromExpandOrCollapse = true;

            if (this.ShowInfoPane.IsExpanded)
            {
                this.AccordionState4ShowPanel(enumAccordionStates.TOPOPEN_BOTTOMCLOSED);
            }
            else
            {
                this.AccordionState4ShowPanel(enumAccordionStates.TOPCLOSED_BOTTOMCLOSED);
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private void ShowPropertiesPane_Expanded(object sender, RoutedEventArgs e)
        {
            if (this.bLoaded)
            {
                this.t2EventComesFromExpandOrCollapse = true;

                if (this.ShowInfoPane.IsExpanded)
                {
                    this.AccordionState4ShowPanel(enumAccordionStates.TOPOPEN_BOTTOMOPEN);
                }
                else
                {
                    this.AccordionState4ShowPanel(enumAccordionStates.TOPCLOSED_BOTTOMOPEN);
                }
            }
        }


        //----------------------------------------------------------------------------------------------------------
        private void AccordionState4ShowPanel(enumAccordionStates state)
        {
            switch (state)
            {
                case enumAccordionStates.TOPCLOSED_BOTTOMCLOSED:
                    // Deactivate GridSplitter
                    this.OShowPaneSplitter.Height = 0;

                    // Reduces size of ScriptPane to Collapsed (Properties Pane grows by default)
                    this.MainShowPane.RowDefinitions[SHOWINFO_PANE].Height =
                        new GridLength(24);

                    this.LayoutSettings.ApplyMainLayout = false;

                    break;

                case enumAccordionStates.TOPCLOSED_BOTTOMOPEN:
                    // Deactivate GridSplitter
                    this.OShowPaneSplitter.Height = 0;

                    // Reduces size of ScriptPane to Collapsed (Properties Pane grows by default)
                    this.MainShowPane.RowDefinitions[SHOWINFO_PANE].Height =
                        new GridLength(24);

                    break;

                case enumAccordionStates.TOPOPEN_BOTTOMCLOSED:
                    // Deactivate GridSplitter
                    this.OShowPaneSplitter.Height = 0;

                    // Takes all available space
                    this.ShowPaneDivider.MaxHeight = (((this.PaneContainer.ActualHeight - ShowTab.ActualHeight) - COLLAPSED_EXPANDER_PLUS_BOTTOM_MARGIN_HEIGHT) - MAIN_LEFT_PANE_TOP_MARGIN);


                    this.ShowPaneDivider.Height =
                        new GridLength((((this.PaneContainer.ActualHeight - ShowTab.ActualHeight) - COLLAPSED_EXPANDER_PLUS_BOTTOM_MARGIN_HEIGHT) - MAIN_LEFT_PANE_TOP_MARGIN));

                    this.LayoutSettings.ApplyMainLayout = false;

                    break;


                case enumAccordionStates.TOPOPEN_BOTTOMOPEN:
                    // Reactivate GridSplitter
                    this.OShowPaneSplitter.Height = 4;

                    // Restores original heights
                    this.MainShowPane.RowDefinitions[SHOWINFO_PANE].Height =
                        new GridLength(this.LayoutSettings.ShowPaneTopPanelHeight);

                    break;
            }
        }

        //----------------------------------------------------------------------------------------------------------
        // Handle Show Pane Grid/Expanders
        //----------------------------------------------------------------------------------------------------------
        private void OShowPropertyGrid_Collapsed(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }


        //----------------------------------------------------------------------------------------------------------
        // ViewPane Private Methods
        //----------------------------------------------------------------------------------------------------------
        private void ResetViewPane()
        {
            this.WebChannelLogo.Source =
                new BitmapImage(new Uri(@"\Assets\Bitmaps\ViewPane\ChannelPlaceHolder.png", UriKind.Relative));

            this.OFrameBackground.Source =
                new BitmapImage(new Uri(@"\Assets\Bitmaps\ViewPane\FramePlaceHolder.png", UriKind.Relative));
        }

        //----------------------------------------------------------------------------------------------------------
        // Curent Mode
        //----------------------------------------------------------------------------------------------------------
        private void setMode(enumMainAppModes pMode)
        {
            this.previousMode = this.currentMode;
            this.currentMode = pMode;

            switch (this.currentMode)
            {
                case enumMainAppModes.START:
                    this.Title = this.GetAppTitleVersion();

                    break;


                case enumMainAppModes.WELCOME:
                    this.BlockingLens.Visibility = Visibility.Visible;
                    this.NewDialog.Visibility = Visibility.Hidden;
                    this.BlockingLens.Opacity = 0.7;
                    this.Welcome.Visibility = Visibility.Visible;

                    break;


                case enumMainAppModes.SHOW_NEW:
                    this.NewDialog.ProjectLocation = this.ProjectDirectory;
                    this.NewDialog.txtProjectName.Text = WayvFS.GetNextProjectName();
                    this.Welcome.Visibility = Visibility.Hidden;
                    this.NewDialog.Visibility = Visibility.Visible;
                    this.NewDialog.SetFocusOnProjectName();
                    break;

                case enumMainAppModes.CREATED:
                    this.ResetViewPane();
                    break;

                case enumMainAppModes.IDLE:
                    // Main App
                    this.Title = this.GetAppTitleVersion() + " - " + this.ProjectFile;
                    this.BlockingLens.Visibility = Visibility.Hidden;
                    this.Welcome.Visibility = Visibility.Hidden;
                    this.NewDialog.Visibility = Visibility.Hidden;
                    this.FrameContainer.Visibility = Visibility.Visible;
                    this.PropertiesPane.IsExpanded = false;
                    
                    // Side Pane
                    this.BlockingLensForFrame.Visibility = Visibility.Collapsed;
                    this.BlockingLensForShow.Visibility = Visibility.Collapsed;

                    // Default Memorized Script Pane Height
                    this.LayoutSettings.ScriptPaneTopPanelHeight = DEFAULT_MEMORIZED_SCRIPT_PANE_HEIGHT;
                    
                    // Default Color for ViewPane Border
                    this.ViewPaneBorder.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));

                    // Control Pane
                    this.BlockingLensForTimeTab.Visibility = Visibility.Collapsed;
                    this.BlockingLensForConsoleTab.Visibility = Visibility.Collapsed;

                    // ViewPane Toolbox
                    this.BlockingLensForInsertShowAssets.Visibility = Visibility.Collapsed;
                    this.BlockingLensForInsertFrameAssets1.Visibility = Visibility.Collapsed;
                    this.BlockingLensForInsertFrameAssets2.Visibility = Visibility.Collapsed;
                    this.BlockingLensForControlSection.Visibility = Visibility.Collapsed;

                    // Recording Feature
                    this.toggleRecording.IsEnabled = true;

                    // Encoding Dialog
                    this.WaitForPublish.Visibility = Visibility.Hidden;

                    // Select WebChannel Dialog
                    this.WebChannelSelectorDialog.Visibility = Visibility.Hidden;

                    break;

                case enumMainAppModes.RECORDING_ANIMATION:
                    // Side Pane
                    this.TimeTab.IsSelected = true;
                    this.TabAnimation.IsSelected = true;
                    this.ViewPaneBorder.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));
                    this.BlockingLensForFrame.Visibility = Visibility.Visible;
                    this.BlockingLensForShow.Visibility = Visibility.Visible;

                    // Control Pane
                    this.BlockingLensForTimeTab.Visibility = Visibility.Collapsed;
                    this.BlockingLensForConsoleTab.Visibility = Visibility.Collapsed;

                    // ViewPane Toolbox
                    this.BlockingLensForInsertShowAssets.Visibility = Visibility.Visible;
                    this.BlockingLensForInsertFrameAssets1.Visibility = Visibility.Visible;
                    this.BlockingLensForInsertFrameAssets2.Visibility = Visibility.Collapsed;
                    this.BlockingLensForControlSection.Visibility = Visibility.Visible;

                    break;

                case enumMainAppModes.PLAYING_ANIMATION:
                    this.ViewPaneBorder.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 255, 0));
                    
                    // Side Pane
                    this.BlockingLensForFrame.Visibility = Visibility.Visible;
                    this.BlockingLensForShow.Visibility = Visibility.Visible;

                    // Control Pane
                    this.BlockingLensForTimeTab.Visibility = Visibility.Visible;
                    this.BlockingLensForConsoleTab.Visibility = Visibility.Visible;

                    // ViewPane Toolbox
                    this.BlockingLensForInsertShowAssets.Visibility = Visibility.Visible;
                    this.BlockingLensForInsertFrameAssets1.Visibility = Visibility.Visible;
                    this.BlockingLensForInsertFrameAssets2.Visibility = Visibility.Visible;
                    this.BlockingLensForControlSection.Visibility = Visibility.Visible;

                    // Recording Feature
                    this.toggleRecording.IsEnabled = false;

                    break;

                case enumMainAppModes.XAML_ERROR_SHOW:
                    this.OErrorReporter.Visibility = Visibility.Visible;

                    // Side Pane
                    this.BlockingLensForFrame.Visibility = Visibility.Visible;
                    this.BlockingLensForShow.Visibility = Visibility.Visible;

                    // Control Pane
                    this.BlockingLensForTimeTab.Visibility = Visibility.Visible;
                    this.BlockingLensForConsoleTab.Visibility = Visibility.Visible;

                    // ViewPane Toolbox
                    this.BlockingLensForInsertShowAssets.Visibility = Visibility.Visible;
                    this.BlockingLensForInsertFrameAssets1.Visibility = Visibility.Visible;
                    this.BlockingLensForInsertFrameAssets2.Visibility = Visibility.Visible;
                    this.BlockingLensForControlSection.Visibility = Visibility.Visible;

                    // Recording Feature
                    this.toggleRecording.IsEnabled = false;

                    break;

                case enumMainAppModes.XAML_ERROR_HIDE:
                    this.OErrorReporter.Visibility = Visibility.Collapsed;

                    // Side Pane
                    this.BlockingLensForFrame.Visibility = Visibility.Collapsed;
                    this.BlockingLensForShow.Visibility = Visibility.Collapsed;

                    // Default Memorized Script Pane Height
                    this.LayoutSettings.ScriptPaneTopPanelHeight = DEFAULT_MEMORIZED_SCRIPT_PANE_HEIGHT;

                    // Default Color for ViewPane Border
                    this.ViewPaneBorder.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));

                    // Control Pane
                    this.BlockingLensForTimeTab.Visibility = Visibility.Collapsed;
                    this.BlockingLensForConsoleTab.Visibility = Visibility.Collapsed;

                    // ViewPane Toolbox
                    this.BlockingLensForInsertShowAssets.Visibility = Visibility.Collapsed;
                    this.BlockingLensForInsertFrameAssets1.Visibility = Visibility.Collapsed;
                    this.BlockingLensForInsertFrameAssets2.Visibility = Visibility.Collapsed;
                    this.BlockingLensForControlSection.Visibility = Visibility.Collapsed;

                    // Recording Feature
                    this.toggleRecording.IsEnabled = true;

                    break;


                case enumMainAppModes.BUSY_PUBLISHING:
                    // Console Tab
                    this.TabConsole.IsSelected = true;
                
                    // Side Pane
                    this.BlockingLensForFrame.Visibility = Visibility.Visible;
                    this.BlockingLensForShow.Visibility = Visibility.Visible;

                    // Control Pane
                    this.BlockingLensForTimeTab.Visibility = Visibility.Visible;
                    this.BlockingLensForConsoleTab.Visibility = Visibility.Visible;

                    // ViewPane Toolbox
                    this.BlockingLensForInsertShowAssets.Visibility = Visibility.Visible;
                    this.BlockingLensForInsertFrameAssets1.Visibility = Visibility.Visible;
                    this.BlockingLensForInsertFrameAssets2.Visibility = Visibility.Visible;
                    this.BlockingLensForControlSection.Visibility = Visibility.Visible;

                    // Show Wait for Publish Component
                    this.WaitForPublish.Visibility = Visibility.Visible;

                    break;

                case enumMainAppModes.SELECTING_WEBCHANNEL:
                    this.BlockingLens.Visibility = Visibility.Visible;

                    this.NewDialog.Visibility = Visibility.Hidden;
                    this.Welcome.Visibility = Visibility.Hidden;
                    this.BlockingLens.Opacity = 0.2;
                    this.WebChannelSelectorDialog.Visibility = Visibility.Visible;

                    break;
            }

        }

        //----------------------------------------------------------------------------------------------------------
        // Private Methods Section [Miscelaneous]
        //----------------------------------------------------------------------------------------------------------
        private string GetAppTitleVersion()
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return "Wayv Producer Alpha " + string.Format(CultureInfo.CurrentCulture,
                         "{0}.{1}", version.Major, version.Minor);
        }

        //----------------------------------------------------------------------------------------------------------
        private void FixProjectPath()
        {
            // Verifies Path Locations
            string sCurrentUser = this.GetUserFromProjectPath(this.ProjectDirectory);
            string sUserInProject = this.GetUserFromProjectPath(this.currentShow.FullyQualifiedFileName);
            if (sCurrentUser != sUserInProject)
            {
                MessageBox.Show("It seems like this project comes from another computer." + 
                                Convert.ToChar(13) + 
                                Convert.ToChar(10) + 
                                "File Paths will be updated",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                // Fix Paths
                this.currentShow.FullyQualifiedFileName = 
                    this.currentShow.FullyQualifiedFileName.Replace(sUserInProject, sCurrentUser);


                this.currentShow.ChannelLogoPath =
                    this.currentShow.ChannelLogoPath.Replace(sUserInProject, sCurrentUser);

                foreach (Frame f in this.currentShow.Frames)
                {
                    if ((f is IUsesAssets) && ((f as IUsesAssets).Assets != null))
                    {
                        foreach (Asset a in (f as IUsesAssets).Assets)
                        {
                            a.FileName = a.FileName.Replace(sUserInProject, sCurrentUser);
                            a.SubPath = a.SubPath.Replace(sUserInProject, sCurrentUser);
                        }
                    }

                    if (f is IAcceptsBackgroundImage)
                    {
                        (f as IAcceptsBackgroundImage).BackgroundImagePath = 
                            (f as IAcceptsBackgroundImage).BackgroundImagePath.Replace(sUserInProject, sCurrentUser);
                    }

                    f.DesignTimeHelper.XamlInfo.XamlPath = f.DesignTimeHelper.XamlInfo.XamlPath.Replace(sUserInProject, sCurrentUser);

                }

                //this.SaveShow(false);
            }
        }

        //----------------------------------------------------------------------------------------------------------
        private string GetUserFromProjectPath(string pUserDirectory)
        {
            string sRetVal = "";
            
            sRetVal = pUserDirectory.Substring(pUserDirectory.IndexOf(@"\", 3) + 1);
            sRetVal = sRetVal.Substring(0, sRetVal.IndexOf(@"\"));

            return sRetVal;
        }

        //----------------------------------------------------------------------------------------------------------
        // Private Methods Section: Data Filtering
        //----------------------------------------------------------------------------------------------------------
        public bool FilterFramesWithXamlImported(object item)
        {
            Boolean bRetVal = false;

            Frame fr = (item as Frame);

            if (fr.DesignTimeHelper.XamlContent != null)
            {
                bRetVal = true;
            }

            return bRetVal;
        }

        //----------------------------------------------------------------------------------------------------------
        // Private Methods Section: Publishing
        //----------------------------------------------------------------------------------------------------------
        private bool canPublish()
        {
            return true;
        }


        //----------------------------------------------------------------------------------------------------------
        private void startPublishThread()
        {
            // Preliminary Actions to Encode Video            
            SetValue(IsEncodingIdleProperty, false);
            this.cancelEncode = false;
            
            // EncodeVideo called in secondary thread
            AsyncDelegate acb = new AsyncDelegate(encodeVideo);
            IAsyncResult iar = acb.BeginInvoke(new AsyncCallback(encodeCompleted),this);

        }

        //----------------------------------------------------------------------------------------------------------
        private void encodeCompleted(IAsyncResult iar)
        {
            // Create a thread to do the encode.
            this.publishThread = new Thread(new ThreadStart(PublishThread));
            this.publishThread.Start();

        }

        //----------------------------------------------------------------------------------------------------------
        private void PublishThread()
        {
            if (this.buildManifest())
            {
                if (this.exportPackage())
                {
                    IndicatePublishIsFinished("");
                }
            }
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
            {
                this.setMode(enumMainAppModes.IDLE);
            });
        }

        //----------------------------------------------------------------------------------------------------------
        private void encodeVideo()
        {

            this.showTraceInDispatcherThread("Publishing Process Initiated.");

            // Indicate in the UI that we're about to analyze the file and show the progress bar.
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
            {
                this.WaitForPublish.Status = "Analyzing...";
            });


            MediaItem mediaItem;
            try
            {
                mediaItem = new MediaItem(this.ProjectDirectory + @"\Resources\" + VideoPath);
            }
            catch (InvalidMediaFileException exp)
            {
                this.showTraceInDispatcherThread("Error creating Media Item Object, Call Tech Support.");

                IndicatePublishIsFinished(exp.Message);
                return;
            }


            // Create Job and Media Item for Video to Encode
            Job job = new Job();
            job.MediaItems.Add(mediaItem);

            // Set Output Directory
            job.OutputDirectory = this.ProjectDirectory + @"\VideoOutput\";


            this.showTraceInDispatcherThread("Output Directory set to: " + this.ProjectDirectory + @"\VideoOutput\");


            // Setup Progress Callback Function
            job.EncodeProgress += new EventHandler<EncodeProgressEventArgs>(job_EncodeProgress);
            
            Microsoft.Expression.Encoder.Marker OShowMarker = 
                new Microsoft.Expression.Encoder.Marker(TimeSpan.FromMilliseconds(2500), "ShowVideoPlayer-0001");

            OShowMarker.GenerateKeyFrame = true;        
            mediaItem.Markers.Add(OShowMarker);

            // Add Markers
            int markerNumber = 1;
            foreach (TimelineController.Components.Marker TLMarker in this.OTLC.Markers)
            {
                Microsoft.Expression.Encoder.Marker OPlayMarker = 
                    new Microsoft.Expression.Encoder.Marker(TLMarker.Value, "PlayWebSlide-" + markerNumber++.ToString().PadLeft(4, '0'));

                OPlayMarker.GenerateKeyFrame = true;
                mediaItem.Markers.Add(OPlayMarker);

                if (markerNumber <= this.OTLC.Markers.Count)
                {
                    Microsoft.Expression.Encoder.Marker OLoadMarker = 
                        new Microsoft.Expression.Encoder.Marker((TLMarker.Value + TimeSpan.FromSeconds(1)), "LoadWebSlide-" + (markerNumber).ToString().PadLeft(4, '0'));

                    OLoadMarker.GenerateKeyFrame = true;
                    mediaItem.Markers.Add(OLoadMarker);
                }
            }

            this.showTraceInDispatcherThread("Markers Created..");
            this.showTraceInDispatcherThread("Encoding started.");                


            // Start Encoding
            job.Encode();
            this.WaitForPublish.Status = "Finished Encoding..";
            this.showTraceInDispatcherThread("Encoding Completed.");

        }

        //----------------------------------------------------------------------------------------------------------
        void job_EncodeProgress(object sender, EncodeProgressEventArgs e)
        {
            if (this.cancelEncode)
            {
                e.Cancel = true;
                return;
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
            {
                this.WaitForPublish.Status = "Encoding: Pass" + " " + e.CurrentPass.ToString() + " of " + e.TotalPasses.ToString();
                this.showTrace("Encoding: Pass" + " " + e.CurrentPass.ToString() + " of " + e.TotalPasses.ToString() + ": " + e.Progress.ToString() + "%");
            });

            this.WaitForPublish.Progress = e.Progress;

        }

        //----------------------------------------------------------------------------------------------------------
        private void IndicatePublishIsFinished(string message)
        {
            // Indicate in the UI that we're finished.
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
            {
                this.WaitForPublish.Status = message;
                this.showTrace(message);
                SetValue(IsEncodingIdleProperty, true);

                // Now we can hide the Wait for Publish Dialog
                Thread.Sleep(5000);
                this.setMode(enumMainAppModes.IDLE);
            });
        }

        //----------------------------------------------------------------------------------------------------------
        private bool buildManifest()
        {
            // Return Value
            bool bRetVal = false;

            // Declare Local Vars
            int newShowId = 0;
            int existingShowVersion = 1;

            // Sends Feedback to UI
            this.WaitForPublish.Status = "Publishing Manifest..";
            this.showTraceInDispatcherThread("Starting Manifest creation..");

            // Get Show ID and Version
            if (this.currentShow.Id == 0)
            {
                this.showTraceInDispatcherThread("A new Show will be Published.");

                newShowId = 0;

                try
                {

                    newShowId = this.ODirectoryServer.GetNewShowID();
                    this.showTraceInDispatcherThread("New Show Id is: " + newShowId.ToString().PadLeft(10, '0'));

                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
                    {
                        this.OExceptionSink.Manage(ex);
                    });
                }

                // Store in Cookies Collection
                Cookie OShowIdCookie = new Cookie();
                OShowIdCookie.Domain = "localhost";
                OShowIdCookie.Name = "ShowId";
                OShowIdCookie.Value = (newShowId.ToString());

                Cookie OShowVersionCookie = new Cookie();
                OShowVersionCookie.Domain = "localhost";
                OShowVersionCookie.Name = "ShowVersion";
                OShowVersionCookie.Value = "1";

                this.Cookies.Add(OShowIdCookie);
                this.Cookies.Add(OShowVersionCookie);

            }
            else
            {
                existingShowVersion = 1;
                this.showTraceInDispatcherThread("A new Version of this Show will be Published..");

                try
                {

                    this.showTraceInDispatcherThread("Show Id is: " + this.currentShow.Id.ToString().PadLeft(10, '0') + " - " +
                        "New Vresion is: " + existingShowVersion.ToString());

                    existingShowVersion = this.ODirectoryServer.GetNewVersionID(this.currentShow.Id);
                    
                    // Error: Cannot Publish, Somebody has the Show Version and is about to publish.
                    //        This may also indicate that there was some corruption in the data,
                    //        this condition, therefore, should be carefully monitored..
                    if (existingShowVersion == 0)
                    {
                        MessageBox.Show(
                            "The Publishing Process Cannot Continue\n\n Another user has locked this Show for Publishing",
                            "Producer Alpha", 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Error
                        );

                        throw new Exception("Publishing cannot be done, Another user has locked this Show for Publishing it.");
                    }
                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
                    {
                        this.OExceptionSink.Manage(ex);
                    });
                    return bRetVal;
                }

                // Store in Cookies Collection
                Cookie OShowIdCookie = new Cookie();
                OShowIdCookie.Domain = "localhost";
                OShowIdCookie.Name = "ShowId";
                OShowIdCookie.Value = (this.currentShow.Id.ToString());

                Cookie OShowVersionCookie = new Cookie();
                OShowVersionCookie.Domain = "localhost";
                OShowVersionCookie.Name = "ShowVersion";
                OShowVersionCookie.Value = existingShowVersion.ToString();

                this.Cookies.Add(OShowIdCookie);
                this.Cookies.Add(OShowVersionCookie);

            }

            // Creates Manifest

            try
            {
                this.showTraceInDispatcherThread("Writing Attributes to Manifest:");
                XmlDocument xmlDoc = new XmlDocument();

                String filename = this.ProjectDirectory + @"\Release\" + "Manifest.xml";
                XmlTextWriter xmlWriter = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                xmlWriter.WriteStartElement("WebShowManifest");

                xmlWriter.WriteAttributeString("xsi", "noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance", @"..\..\Specs\WebShowSpec.xsd");
                xmlWriter.WriteAttributeString("WebShowName", this.currentShow.Title);
                this.showTraceInDispatcherThread("WebShowName: " + this.currentShow.Title);

                xmlWriter.WriteAttributeString("WebShowAuthor", this.currentShow.Credits.Producers[0].Name);
                this.showTraceInDispatcherThread("WebShowAuthor: " + this.currentShow.Credits.Producers[0].Name);
                
                xmlWriter.WriteAttributeString("WebShowDate", this.currentShow.Date.ToString());
                this.showTraceInDispatcherThread("WebShowDate: " + this.currentShow.Date.ToString());
                
                xmlWriter.WriteAttributeString("WebShowVersion", existingShowVersion.ToString());
                this.showTraceInDispatcherThread("WebShowVersion: " + existingShowVersion.ToString());
                
                xmlWriter.WriteAttributeString("InitialVideoWindowX", this.currentShow.InitialVideoWindowX.ToString());
                this.showTraceInDispatcherThread("InitialVideoWindowX: " + this.currentShow.InitialVideoWindowX.ToString());
                
                xmlWriter.WriteAttributeString("InitialVideoWindowY", this.currentShow.InitialVideoWindowY.ToString());
                this.showTraceInDispatcherThread("InitialVideoWindowY: " + this.currentShow.InitialVideoWindowY.ToString());
                
                xmlWriter.WriteAttributeString("InitialVideoWindowHeight", "160");
                this.showTraceInDispatcherThread("InitialVideoWindowHeight: " + "160");
                
                xmlWriter.WriteAttributeString("InitialVideoWindowWidth", "140");
                this.showTraceInDispatcherThread("InitialVideoWindowWidth: " + "140");
                
                xmlWriter.WriteAttributeString("InitialVideoWindowSource", "/WebShows/" +
                                                newShowId.ToString().PadLeft(10, '0') +
                                                "/Resources/" + this.VideoPath);
                this.showTraceInDispatcherThread("InitialVideoWindowSource: " + "/WebShows/" +
                                                newShowId.ToString().PadLeft(10, '0') +
                                                "/Resources/" + this.VideoPath);

                xmlWriter.WriteAttributeString("BasePath", "/WebShows/" +
                                                newShowId.ToString().PadLeft(10, '0') + "/");
                this.showTraceInDispatcherThread("BasePath: " + "/WebShows/" +
                                                newShowId.ToString().PadLeft(10, '0') + "/");

                xmlWriter.WriteAttributeString("BottomBarDefaultColor", "#" +
                                                                        String.Format("{0:x2}", this.currentShow.BottomBarDefaultColorUI.A) +
                                                                        String.Format("{0:x2}", this.currentShow.BottomBarDefaultColorUI.R) +
                                                                        String.Format("{0:x2}", this.currentShow.BottomBarDefaultColorUI.G) +
                                                                        String.Format("{0:x2}", this.currentShow.BottomBarDefaultColorUI.B));
                this.showTraceInDispatcherThread("BottomBarDefaultColor: " + "#" +
                                                                        String.Format("{0:x2}", this.currentShow.BottomBarDefaultColorUI.A) +
                                                                        String.Format("{0:x2}", this.currentShow.BottomBarDefaultColorUI.R) +
                                                                        String.Format("{0:x2}", this.currentShow.BottomBarDefaultColorUI.G) +
                                                                        String.Format("{0:x2}", this.currentShow.BottomBarDefaultColorUI.B));
                
                xmlWriter.WriteAttributeString("BottomBarDefaultOpacity", this.currentShow.BottomBarDefaultOpacity.ToString());
                this.showTraceInDispatcherThread("BottomBarDefaultOpacity:" + this.currentShow.BottomBarDefaultOpacity.ToString());

                xmlWriter.Close();

                xmlDoc.Load(filename);

                XmlNode WebShowManifest = xmlDoc.DocumentElement;
                XmlElement WebSlidesCollectionNode = xmlDoc.CreateElement("WebSlides");
                WebShowManifest.AppendChild(WebSlidesCollectionNode);

                XmlElement WebSlideNode;
                XmlElement ThumbnailNode;
                XmlElement AssetsNode;
                XmlElement AssetNode;

                this.showTraceInDispatcherThread("Manifest Header completed, processing frame data:");

                int iCounter = 1;
                foreach (Frame F in this.currentShow.Frames)
                {
                    if (!(F is ConversationalPoll) &&
                        !(F is PageFrame))
                    {
                        this.showTraceInDispatcherThread("Processing Frame: " + iCounter.ToString());

                        WebSlideNode = xmlDoc.CreateElement("WebSlide");
                        ThumbnailNode = xmlDoc.CreateElement("Thumbnail");
                        ThumbnailNode.SetAttribute("FrameID", F.Id.ToString());

                        ThumbnailNode.SetAttribute("ImageSource",
                        "/WebShows/" + newShowId.ToString().PadLeft(10, '0') +
                        "/Resources/Thumbnails/Default.jpg");

                        ThumbnailNode.SetAttribute("FrameText", F.Script.Title);
                        WebSlideNode.AppendChild(ThumbnailNode);
                        WebSlidesCollectionNode.AppendChild(WebSlideNode);
                        this.showTraceInDispatcherThread("Frame Title is: " + F.Script.Title);

                        AssetsNode = xmlDoc.CreateElement("Assets");

                        foreach (Asset A in (F as IUsesAssets).Assets)
                        {
                            AssetNode = xmlDoc.CreateElement("Asset");
                            AssetNode.SetAttribute("ID", A.FileID);
                            AssetNode.SetAttribute("Value", A.FileName);
                            AssetsNode.AppendChild(AssetNode);
                            this.showTraceInDispatcherThread("Writing Asset: " + A.FileID + " with FileName: " + A.FileName);
                        }
                        WebSlideNode.AppendChild(AssetsNode);

                        iCounter++;
                    }

                }

                xmlDoc.Save(filename);
            }
            catch (System.Exception ex)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    this.OExceptionSink.Manage(ex);
                });

                bRetVal = false;
                return bRetVal;
            }

            // Sends Feedback to the UI
            this.WaitForPublish.Status = "Manifest Completed..";
            this.showTraceInDispatcherThread("Manifest Completed\n");
            Thread.Sleep(1200);

            // Returns Success
            bRetVal = true;
            return bRetVal;

        }

        //----------------------------------------------------------------------------------------------------------
        private bool exportPackage()
        {
            // Return Value
            bool bRetVal = false;

            // Executes Export
            this.showTraceInDispatcherThread("Starting Export Package Process..");

            this.WaitForPublish.Status = "Exporting Package..";
            this.WaitForPublish.SetStandByMessage();

            // Vars
            String VideoOutputPath      = this.ProjectDirectory + @"\VideoOutput\";
            String FrameTempPath        = this.ProjectDirectory + @"\Temp\";
            String ReleaseFolder        = this.ProjectDirectory + @"\Release\";
            String WebSlidesPath        = this.ProjectDirectory + @"\Release\WebSlides\";
            String ResourcesPath        = this.ProjectDirectory + @"\Release\Resources\";
            String BaseThumbnailsPath   = this.ProjectDirectory + @"\Release\Resources\Thumbnails\";
            String ColorThumbnailsPath  = this.ProjectDirectory + @"\Release\Resources\Thumbnails\Color\";
            String BWThumbnailsPath     = this.ProjectDirectory + @"\Release\Resources\Thumbnails\BW\";
            String ModelFolder          = this.SettingsDirectory + @"\Model\";


            // Copy Video
            var directory = new DirectoryInfo(VideoOutputPath);
            var myFile = (from d in directory.GetDirectories()
                          orderby d.Name descending
                          select d).First();

            FileInfo f = (myFile as DirectoryInfo).GetFiles()[0];
            String SourceVideoPath = f.FullName;

            this.showTraceInDispatcherThread("Starting copy of Narrative Video..");
            try
            {
                if (!File.Exists(ResourcesPath + "WebShowVideo.wmv"))
                {
                    File.Copy(SourceVideoPath, ResourcesPath + "WebShowVideo.wmv", false);
                }
            }
            catch (IOException ex)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    this.OExceptionSink.Manage(ex);
                });

                return bRetVal;
            }
            this.showTraceInDispatcherThread("Copy of Narrative Video Completed.");


            // Copy Generic Thumbnail
            this.showTraceInDispatcherThread("Starting copy o Default Thumbnail..");
            try
            {
                if (!File.Exists(BaseThumbnailsPath + "Default.jpg"))
                {
                    File.Copy(ModelFolder + "Default.jpg", BaseThumbnailsPath + "Default.jpg", false);
                }
            }
            catch (IOException ex)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    this.OExceptionSink.Manage(ex);
                });

                return bRetVal;
            }
            this.showTraceInDispatcherThread("Copy o Default Thumbnail Completed.");

            // Initialize Vars to Process Thumbnails
            this.showTraceInDispatcherThread("Processing Thumbnails..");

            System.Drawing.Image OImage = new System.Drawing.Bitmap(10,10);
            System.Drawing.Image OColorThumb;
            System.Drawing.Bitmap OBWThumb;

            // Process all Frames
            foreach (Frame OFrame in this.currentShow.Frames)
            {
                // Sends Message to the UI
                this.WaitForPublish.Status = "Processing Frame " + OFrame.Id.ToString();
                this.showTraceInDispatcherThread("Starting to Process Frame " + OFrame.Id.ToString());
                Thread.Sleep(500);


                // Process Frame
                if (!(OFrame is ConversationalPoll) && !(OFrame is PageFrame))
                {
                    // Clean Temp Folder to Process Frames
                    WayvFS.DeleteFilesInFolder(FrameTempPath);

                    // Process Thumbnail in Color and BW
                    // Assign to Assets Collection
                    OImage = ImageFast.FastFromFile(
                        (OFrame as IUsesAssets).Assets[0].SubPath + (OFrame as IUsesAssets).Assets[0].FileName
                    );

                }
                else
                {
                    // Whatever Processing we do for types that don't support IUsesAssets

                    switch (OFrame.GetType().ToString())
                    {
                        case "ConversationalPoll":
                            break;

                        case "PageFrame":
                            break;
                    }
                }

                // Saves Thumbnails to Release\Resources\Thumbnails\Color / BW
                OColorThumb = JPEGHelper.resizeImage(OImage, new System.Drawing.Size(84, 47));
                OBWThumb = JPEGHelper.ChangeIntoGrayscale(new System.Drawing.Bitmap(OColorThumb));
                JPEGHelper OJpegHelper = new JPEGHelper();
                
                
                OJpegHelper.saveJpeg(ColorThumbnailsPath + OFrame.Id.ToString().PadLeft(4,'0') + ".jpg", new System.Drawing.Bitmap(OColorThumb), 100);
                OJpegHelper.saveJpeg(BWThumbnailsPath + OFrame.Id.ToString().PadLeft(4, '0') + ".jpg", OBWThumb, 100);

                this.showTraceInDispatcherThread("Processing Thumbnails Completed.");


                // Copy WebSlide.xaml to Temp Folder
                this.showTraceInDispatcherThread("Starting Processing of Webslide.xaml..");
                FileStream stream = null;
                string xamlDocument = ModelFolder + "Webslide.xaml";

                try
                {

                    // Opens Destination (WebSlide Model)
                    XmlDocument xmlDestDoc = new XmlDocument();
                    xmlDestDoc.Load(xamlDocument);

                    // Prepares Destination Document
                    XmlNode CanvasNode = xmlDestDoc.DocumentElement;
                    CanvasNode.Attributes["x:Name"].Value = "OWebSlide_" + OFrame.Id.ToString().PadLeft(4, '0');



                    if (OFrame is IUsesAssets)
                    {
                        // Opens Source Document (Xaml coming from Blend)
                        XmlDocument xmlSourceDoc = new XmlDocument();

                        this.Dispatcher.Invoke((Action)delegate()
                        {
                            lock (this.currentXamlContent)
                            {
                                if (OFrame.DesignTimeHelper.XamlContent != null)
                                {
                                    this.currentXamlContent = OFrame.DesignTimeHelper.XamlContent;
                                }
                                else
                                {
                                    this.currentXamlContent = "";
                                }
                            }

                        });

                        if (this.currentXamlContent != "")
                        {
                            xmlSourceDoc.Load(new MemoryStream(
                                        ASCIIEncoding.Default.GetBytes(
                                            this.currentXamlContent
                                        )
                                 )
                            );

                            // Get the OSBMain Storyboard Node for the Destination
                            XmlNodeList OSBMainDestList = xmlDestDoc.GetElementsByTagName("Storyboard");
                            XmlNode OSBMainDest = null;

                            foreach (XmlNode xNode in OSBMainDestList)
                            {
                                if (xNode.Attributes["x:Name"].Value == "OSBMain")
                                {
                                    OSBMainDest = xNode;
                                    break;
                                }
                            }


                            // Get the OSBMain Storyboard Node for the Source
                            XmlNodeList OSBMainSourceList = xmlSourceDoc.GetElementsByTagName("Storyboard");
                            XmlNode ONL = null;

                            foreach (XmlNode xmlNode in OSBMainSourceList)
                            {
                                if (xmlNode.Attributes["Name"].Value == "OSBMain")
                                {
                                    ONL = xmlNode;
                                    break;
                                }
                            }

                            // Extracts Animations from Source Xaml
                            if (ONL != null)
                            {
                                // Extracts from Storyboard.Children Node (if Present?)
                                XmlNode newNode = null;
                                XmlNode SBChildren = ONL.ChildNodes[0];
                                if (SBChildren.Name == "Storyboard.Children")
                                {
                                    ONL = SBChildren;
                                }

                                // Extracts DoubleAnimations from Source / Inserts DoubleAnimations into Destination Document
                                foreach (XmlNode xn in ONL.ChildNodes)
                                {
                                    if (
                                        (xn.Name == "DoubleAnimationUsingKeyFrames")
                                       )
                                    {
                                        // Move the Node into  Destination
                                        if (OSBMainDest != null)
                                        {
                                            newNode = this.RenameNode(xn, "DoubleAnimationUsingKeyFrames", xmlDestDoc, "http://schemas.microsoft.com/client/2007");
                                            
                                            newNode.Attributes["Storyboard.TargetProperty"].Value = 
                                                newNode.Attributes["Storyboard.TargetProperty"].Value.Replace(".[", "[");

                                            OSBMainDest.AppendChild(xmlDestDoc.ImportNode(newNode, true));
                                        }

                                    }
                                }

                            }
                            else
                            {
                                throw new Exception("Error Publishing Package");
                            }

                            //--------------------------------------------------------------------------------------
                            // Process all other elements
                            XmlNodeList AllObjectsSourceList = xmlSourceDoc.GetElementsByTagName("Canvas");
                            XmlNode AllObjectsSource = AllObjectsSourceList[0];


                            XmlNodeList AllObjectsDestList = xmlDestDoc.GetElementsByTagName("Canvas");
                            XmlNode AllObjectsDest = AllObjectsDestList[0];

                            XmlNode insertNode = null;

                            foreach (XmlNode xNode in AllObjectsSource.ChildNodes)
                            {
                                if (
                                    (xNode.Name != "Canvas.Resources")
                                   )
                                {
                                    // Copies the Element
                                    insertNode = this.RenameNode(xNode, xNode.Name, xmlDestDoc, "http://schemas.microsoft.com/client/2007");
                                    AllObjectsDest.AppendChild(xmlDestDoc.ImportNode(insertNode, true));
                                }
                            }


                        }
                        else
                        { 
                            // Check if WebSlide is IAcceptsBackgroundImage, 
                            // if so Is there a Background defined?

                            if (OFrame is IAcceptsBackgroundImage)
                            {
                                if ((OFrame as IAcceptsBackgroundImage).BackgroundImagePath != null)
                                {       
                                    // Opens Source Document
                                    string xamlSourceDocumentPath = ModelFolder + "IAcceptsBackgroundImage.xaml";
                                    XmlNode OImageNode = null;

                                    try
                                    {
                                        // Opens Destination (WebSlide Model)
                                        XmlDocument xamlSourceDocument = new XmlDocument();
                                        xamlSourceDocument.Load(xamlSourceDocumentPath);

                                        // Prepares Source node
                                        XmlNode ImageDestinationNode = xamlSourceDocument.DocumentElement;

                                        // Copies the Element
                                        OImageNode = this.RenameNode(ImageDestinationNode, ImageDestinationNode.Name, xmlDestDoc, "http://schemas.microsoft.com/client/2007");
                                        CanvasNode.AppendChild(xmlDestDoc.ImportNode(OImageNode, true));

                                    }
                                    catch (System.Exception ex)
                                    {
                                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
                                        {
                                            this.OExceptionSink.Manage(ex);
                                        }); 
                                        
                                        return bRetVal;
                                    }
                                }
                            }
                        }
                    }

                    // Saves End Result
                    xmlDestDoc.Save(FrameTempPath + "Webslide.xaml"); 
                }

                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
                    {
                        this.OExceptionSink.Manage(ex);
                    }); 
                    
                    return bRetVal;
                }

                finally
                {
                    if (stream != null) stream.Close();
                }

                this.showTraceInDispatcherThread("Webslide.xaml created");

                // Copy Assets and/or BackgroundImage into Temp Folder
                this.showTraceInDispatcherThread("Copying Assets to Temp Folder..");
                try
                {
                    // Background Image
                    if (OFrame is IAcceptsBackgroundImage)
                    {
                        String BackgroundImagePath = (OFrame as IAcceptsBackgroundImage).BackgroundImagePath;
                        if (File.Exists(BackgroundImagePath))
                        {
                            File.Copy(BackgroundImagePath, FrameTempPath + System.IO.Path.GetFileName(BackgroundImagePath), false);
                        }
                    }

                    // Additional Assets
                    if (OFrame is IUsesAssets)
                    {
                        String AssetPath = "";
                        foreach (Asset aFile in (OFrame as IUsesAssets).Assets)
                        {
                            AssetPath = aFile.FileName;
                            if (File.Exists(AssetPath))
                            {
                                File.Copy(AssetPath, FrameTempPath + System.IO.Path.GetFileName(AssetPath), false);
                            }
                        }
                    }

                }
                catch (IOException ex)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
                    {
                        this.OExceptionSink.Manage(ex);
                    }); 
                    
                    return bRetVal;
                }
                this.showTraceInDispatcherThread("Completed");

                // Zip the whole thing with the right number and save to the right folder under Release\WebSlides\00#
                this.showTraceInDispatcherThread("Writing Zip File");
                try
                {
                     using (ZipFile zip = new ZipFile())
                     {
                         if (OFrame is IUsesAssets)
                         {
                             foreach (Asset a in (OFrame as IUsesAssets).Assets)
                             {
                                 zip.AddFile(FrameTempPath + System.IO.Path.GetFileName(a.FileName), @"\");
                             }
                         }

                        zip.AddFile(FrameTempPath        + "WebSlide.xaml",@"\");
                        zip.Save(WebSlidesPath           + OFrame.Id.ToString().PadLeft(4,'0') + ".zip");
                     }
                }
                catch (System.Exception ex)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
                    {
                        this.OExceptionSink.Manage(ex);
                    });

                    return bRetVal;
                }

                this.showTraceInDispatcherThread("Zip File Created for Frame.");
 
            }



            // Sends Message to the UI
            this.WaitForPublish.Status = "Creating Package..";
            this.showTraceInDispatcherThread("Creating Main Package");
            Thread.Sleep(500);

            // Clean Temp Folder
            WayvFS.DeleteFilesInFolder(FrameTempPath);
            this.showTraceInDispatcherThread("Cleanning Temp Folder.");
            this.showTraceInDispatcherThread("Creating Deployment Package..");

            // Zip the Release Folder into the Release\bin folder
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddDirectory(ReleaseFolder);
                    zip.Save(this.PublishDirectory + @"\" + this.PublishFileName);
                    File.Copy(this.PublishDirectory + @"\" + this.PublishFileName, FrameTempPath + this.PublishFileName, true);
                }
            }
            catch (System.Exception ex)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    this.OExceptionSink.Manage(ex);
                });
                return bRetVal;
            }
            this.showTraceInDispatcherThread("Package Created Successfully");

            // Sends Feedback to the UI
            this.WaitForPublish.Status = "Submitting to Channel Server";
            this.showTraceInDispatcherThread("Submitting to Channel Server: http://localhost");

            // Publish Package to Wayv Server
            FileStream OFStream = new FileStream(FrameTempPath + this.PublishFileName, System.IO.FileMode.Open);
            byte[] aBytes = new byte[(int)OFStream.Length];
            OFStream.Read(aBytes, 0, (int)OFStream.Length);
            OFStream.Close();

            DirectoryServer.WebShowMetadata OMetadata = new DirectoryServer.WebShowMetadata();

            OMetadata.ShowId = Int32.Parse(Cookies.GetCookies(new Uri("http://localhost"))["ShowId"].Value);
            OMetadata.Version = Int32.Parse(Cookies.GetCookies(new Uri("http://localhost"))["ShowVersion"].Value);
            OMetadata.ShowTitle = this.currentShow.Title;
            OMetadata.Description = this.currentShow.Synopsis;
            OMetadata.ChannelId = this.currentShow.Metadata.WebChannelId;

            this.showTraceInDispatcherThread("Uploading Data, please wait..");
            String Result = ODirectoryServer.SubmitShow( OMetadata, aBytes );

            if (!(Result == "Success"))
            {
                // Send Feedback to the UI
                this.showTraceInDispatcherThread("Error: " + Result);
                Thread.Sleep(5000);
                return bRetVal;
            }


            // Save New Show Data
            this.showTraceInDispatcherThread("Upload Completed\n");
            this.showTraceInDispatcherThread("Saving Show...");

            // Save Show Id and Version to Object Model
            this.currentShow.Id = 
                Int32.Parse(Cookies.GetCookies(new Uri("http://localhost"))["ShowId"].Value);
            
            this.currentShow.Metadata.Version = 
                Int32.Parse(Cookies.GetCookies(new Uri("http://localhost"))["ShowVersion"].Value);

            // Save Show Automatically after Publishing
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
            {
                this.currentShow.Save(this.ProjectFullPath);
            });


            // Send feedback to the User
            this.WaitForPublish.Status = "Publishing Completed..\n";
            this.showTraceInDispatcherThread("Publish Completed, if needed, right click this window to save log to a text file\n");

            Thread.Sleep(700);

            bRetVal = true;
            return bRetVal;
        }

        //---------------------------------------------------------------------------------------------------------
        private XmlNode RenameNode(XmlNode node, string qualifiedName, XmlDocument creatorDoc, string xmlns)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                XmlElement oldElement = (XmlElement)node;

                XmlElement newElement =
                    creatorDoc.CreateElement(qualifiedName, xmlns);

                while (oldElement.HasAttributes)
                {
                    newElement.SetAttributeNode(oldElement.RemoveAttributeNode(oldElement.Attributes[0]));
                }

                while (oldElement.HasChildNodes)
                {
                    if (oldElement.FirstChild.Name != "#text")
                    {
                        newElement.AppendChild(
                            this.RenameNode(oldElement.FirstChild, oldElement.FirstChild.Name, creatorDoc, xmlns)
                        );
                    }
                    else
                    { 
                        // What we do here?
                        XmlNode OTextElement = creatorDoc.CreateTextNode(oldElement.FirstChild.Value);
                        newElement.AppendChild(OTextElement);
                           
                    }
                    oldElement.RemoveChild(oldElement.FirstChild);
                }

                return newElement;
            }
            else
            {
                return null;
            }
        }

        //---------------------------------------------------------------------------------------------------------
        private void showTrace(string pTraceMessage)
        {
            this.txtConsole.Text += "[" + DateTime.Now.ToString() + "]" + " - " + pTraceMessage + Environment.NewLine;
            this.txtConsole.PageDown();
        }

        //---------------------------------------------------------------------------------------------------------
        private void showTraceInDispatcherThread(string pTraceMessage)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
            {
                this.txtConsole.Text += "[" + DateTime.Now.ToString() + "]" + " - " + pTraceMessage + Environment.NewLine;
                this.txtConsole.PageDown();
            });
        }

        //---------------------------------------------------------------------------------------------------------
        // Private Methods Section: Console
        //---------------------------------------------------------------------------------------------------------
        private void clearConsole()
        {
            this.txtConsole.Text = "";
        }

        //---------------------------------------------------------------------------------------------------------
        private void saveConsole()
        {
            String LogSavePath = "";
            String LogSaveDirectory = "";
            String LogSaveFileName = "";

            VistaSaveFileDialog dialog = new VistaSaveFileDialog();
            dialog.Filter = "Log Files (*.txt)|*.txt";
            dialog.FileName = "ProducerAlphaLog.txt";
            dialog.InitialDirectory = App.BaseProjectPath;

            if ((bool)dialog.ShowDialog(this))
            {
                // Get Path Info From Save Dialog
                LogSavePath = dialog.FileName;
                LogSaveDirectory = System.IO.Path.GetDirectoryName(dialog.FileName);
                LogSaveFileName = System.IO.Path.GetFileName(dialog.FileName);

                TextWriter OTextWriter;
                OTextWriter = new StreamWriter(LogSavePath);

                try
                {
                    // Save Console Log:
                    OTextWriter.Write(this.txtConsole.Text);
                }
                catch (Exception ex)
                {
                    this.OExceptionSink.Manage(ex);
                }
                finally
                {
                    // close the stream
                    OTextWriter.Close();
                }
            }
        }

    }
}
