using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Globalization;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Windows;
using TimelineController.Components;


namespace ProducerAlpha
{

    [System.Xml.Serialization.XmlInclude(typeof(Tag))]
    [System.Xml.Serialization.XmlInclude(typeof(Script))]
    [System.Xml.Serialization.XmlInclude(typeof(WebSlide))]
    [System.Xml.Serialization.XmlInclude(typeof(VideoFrame))]
    [System.Xml.Serialization.XmlInclude(typeof(ConversationalPoll))]
    [System.Xml.Serialization.XmlInclude(typeof(PageShot))]
    [System.Xml.Serialization.XmlInclude(typeof(PageFrame))]
    [System.Xml.Serialization.XmlInclude(typeof(Script))]
    [System.Xml.Serialization.XmlInclude(typeof(DesignTimeFrameHelper))]
    [System.Xml.Serialization.XmlInclude(typeof(ImportedXamlInfo))]
    [System.Xml.Serialization.XmlInclude(typeof(Credits))]
    [System.Xml.Serialization.XmlInclude(typeof(Producer))]
    [System.Xml.Serialization.XmlInclude(typeof(Director))]
    [System.Xml.Serialization.XmlInclude(typeof(Designer))]
    [System.Xml.Serialization.XmlInclude(typeof(Speaker))]
    [System.Xml.Serialization.XmlInclude(typeof(Composer))]
    [System.Xml.Serialization.XmlInclude(typeof(Developer))]
    [System.Xml.Serialization.XmlInclude(typeof(VOMarker))]
    [System.Xml.Serialization.XmlInclude(typeof(ShowMetadata))]

    [Serializable]
    public partial class Show : Observable
    {

        //-------------------------------------------------------------------------------------------------
        // Private Fields Section
        //-------------------------------------------------------------------------------------------------
        private int id;
        private string title;
        private DateTime date;
        private double initialVideoWindowX;
        private double initialVideoWindowY;
        private string nVideoWindowPath;
        private string channelLogoPath;

        private System.Windows.Media.Color bottomBarDefaultColorUI;
        private System.Windows.Media.Color identityBarFontColorUI;

        private double bottomBarDefaultOpacity;
        private string defaultThumbnailPath;
        private string synopsis;
        private ObservableCollection<ProducerAlpha.Tag> tags;
        private ObservableCollection<ProducerAlpha.Frame> frames;
        private Credits credits;
        private string fullyQualifiedFileName;
        private DesignTimeShowHelper designTimeHelper;
        private ObservableCollection<VOMarker> markers;
        private ShowMetadata metadata;

        //-------------------------------------------------------------------------------------------------
        // Public Properties Section
        //-------------------------------------------------------------------------------------------------        
        /// <summary>
        /// Unique ID for Show
        /// </summary>
        [Browsable(true)]
        [Category("General")]
        [Description("The Show Id")]
        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        //-------------------------------------------------------------------------------------------------        
        /// <summary>
        /// Track show versions
        /// </summary>
        [Category("General")]
        [DisplayName("Show Title:")]
        [Description("The Show Title")]
        public String Title
        {
            get
            {
                return this.title;
            }
            set
            {
                Set(ref title, value, "Title");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        /// <summary>
        /// Release Date
        /// </summary>
        [Category("General")]
        [DisplayName("Last Modification:")]
        [Description("The Show Date")]
        public DateTime Date
        {
            get
            {
                return this.date;
            }
            set
            {
                Set(ref date, value, "Date");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        /// <summary>
        /// Windowed NVideo Position in Player
        /// </summary>
        [Category("Layout")]
        [DisplayName("Narrative Video X:")]
        [Description("The Initial Position (X) for the Narrative Video Window when not minimized")]
        public double InitialVideoWindowX
        {
            get
            {
                return this.initialVideoWindowX;
            }
            set
            {
                Set(ref initialVideoWindowX, value, "InitialVideoWindowX");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        /// <summary>
        /// Windowed NVideo Position in Player
        /// </summary>
        [Category("Layout")]
        [DisplayName("Narrative Video Y:")]
        [Description("The Initial Position (Y) for the Narrative Video Window when not minimized")]
        public double InitialVideoWindowY
        {
            get
            {
                return this.initialVideoWindowY;
            }
            set
            {
                Set(ref initialVideoWindowY, value, "InitialVideoWindowY");
            }
        }

        //-------------------------------------------------------------------------------------------------
        [Category("Layout")]
        [DisplayName("BottomBar Color:")]
        [Description("Color for the Bottom Bar")]
        public System.Windows.Media.Color BottomBarDefaultColorUI
        {
            get
            {
                return this.bottomBarDefaultColorUI;
            }
            set
            {
                Set(ref bottomBarDefaultColorUI, value, "BottomBarDefaultColorUI");
            }
        }

        //-------------------------------------------------------------------------------------------------
        [Category("Layout")]
        [DisplayName("IDBar FontColor:")]
        [Description("Color for the Font of the Identity Bar")]
        public System.Windows.Media.Color IdentityBarFontColorUI
        {
            get
            {
                return this.identityBarFontColorUI;
            }
            set
            {
                Set(ref this.identityBarFontColorUI, value, "IdentityBarFontColorUI");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        [Category("Layout")]
        [DisplayName("BottomBar Opacity:")]
        [Description("Opacity for the Bottom Bar")]
        public double BottomBarDefaultOpacity
        {
            get
            {
                return this.bottomBarDefaultOpacity;
            }
            set
            {
                Set(ref bottomBarDefaultOpacity, value, "BottomBarDefaultOpacity");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        [Browsable(false)]
        [Category("Media")]
        [Description("The Relative Path from the Project Root to the Narrative Video File")]
        public String NVideoPath
        {
            get
            {
                return this.nVideoWindowPath;
            }
            set
            {
                Set(ref nVideoWindowPath, value, "NVideoPath");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        [Browsable(false)]
        [Category("Media")]
        [Description("The Absolute Path for the Channel Logo")]
        public String ChannelLogoPath
        {
            get
            {
                return this.channelLogoPath;
            }
            set
            {
                Set(ref channelLogoPath, value, "ChannelLogoPath");
            }
        }


        //-------------------------------------------------------------------------------------------------        
        [Browsable(false)]
        [Category("Media")]
        [Description("The Relative Path from the Project Root to the Default Thumbnail File")]
        public String DefaultThumbnailPath
        {
            get
            {
                return this.defaultThumbnailPath;
            }
            set
            {
                Set(ref defaultThumbnailPath, value, "DefaultThumbnailPath");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        [Browsable(false)]
        [Category("Metadata")]
        [Description("The Show's Synopsis")]
        public string Synopsis
        {
            get
            {
                return this.synopsis;
            }
            set
            {
                Set(ref synopsis, value, "Synopsis");
            }
        }


        //-------------------------------------------------------------------------------------------------        
        [Browsable(false)]
        public ShowMetadata Metadata
        {
            get
            {
                return this.metadata;
            }
            set
            {
                Set(ref this.metadata, value, "Metadata");
            }
        }


        //-------------------------------------------------------------------------------------------------        
        [Browsable(false)]
        [Category("Metadata")]
        [Description("Tags to clasify this Show")]
        public System.Collections.ObjectModel.ObservableCollection<Tag> Tags
        {
            get
            {
                return this.tags;
            }
            set
            {
                Set(ref this.tags, value, "Tags");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        [Browsable(false)]
        public System.Collections.ObjectModel.ObservableCollection<Frame> Frames
        {
            get
            {
                return this.frames;
            }
            set
            {
                Set(ref this.frames, value, "Frames");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        [Browsable(false)]
        public Credits Credits
        {
            get
            {
                return this.credits;
            }
            set
            {
                this.credits = value;
            }
        }

        //-------------------------------------------------------------------------------------------------        
        [Browsable(false)]
        public System.Collections.ObjectModel.ObservableCollection<VOMarker> Markers
        {
            get
            {
                return this.markers;
            }
            set
            {
                this.markers = value;
            }
        }


        //-------------------------------------------------------------------------------------------------        
        [Browsable(false)]
        public string FullyQualifiedFileName
        {
            get
            {
                return this.fullyQualifiedFileName;
            }
            set
            {
                Set(ref this.fullyQualifiedFileName, value, "FullyQualifiedFileName");
            }
        }

        //-------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public DesignTimeShowHelper DesignTimeHelper
        {
            get
            {
                return this.designTimeHelper;
            }
            set
            {
                this.designTimeHelper = value;
            }
        }

        //-------------------------------------------------------------------------------------------------
        // Constructor Methods Section
        //-------------------------------------------------------------------------------------------------
        public Show(int                             pId, 
                    string                          pVersion, 
                    DateTime                        pDate,
                    double                          pInitialVideoWindowX,
                    double                          pInitialVideoWindowY,
                    string                          pNVideoWindowPath,
                    System.Windows.Media.Color      pBottomBarDefaultColor,
                    System.Windows.Media.Color      pIdentityBarFontColor,
                    double                          pBottomBarDefaultOpacity,
                    string                          pDefaultThumbnailPath,
                    string                          pSynopsis
                   )
        { 

            // Updates Object Private Fields
            this.id                         = pId;
            this.date                       = pDate;
            this.initialVideoWindowX        = pInitialVideoWindowX;
            this.initialVideoWindowY        = pInitialVideoWindowY;
            this.nVideoWindowPath           = pNVideoWindowPath;
            this.bottomBarDefaultColorUI    = pBottomBarDefaultColor;
            this.identityBarFontColorUI     = pIdentityBarFontColor;
            this.bottomBarDefaultOpacity    = pBottomBarDefaultOpacity;
            this.defaultThumbnailPath       = pDefaultThumbnailPath;
            this.synopsis                   = pSynopsis;

            this.tags                       = new ObservableCollection<Tag>();
            this.tags.CollectionChanged     += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(tags_CollectionChanged);

            this.frames                     = new ObservableCollection<Frame>();
            this.frames.CollectionChanged   += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(frames_CollectionChanged);
            
            this.credits                    = new Credits();
            this.credits.PropertyChanged    += new PropertyChangedEventHandler(credits_PropertyChanged);

            this.markers = new ObservableCollection<VOMarker>();
            this.markers.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(markers_CollectionChanged);

            this.designTimeHelper           = new DesignTimeShowHelper();

        }


        //-------------------------------------------------------------------------------------------------
        public Show()
        { 

            // Updates Object Private Fields
            this.id                         = 0;
            this.date                       = DateTime.Now;
            this.initialVideoWindowX        = 760.0;
            this.initialVideoWindowY        = 17.0;
            this.nVideoWindowPath           = "";
            this.bottomBarDefaultColorUI    = System.Windows.Media.Color.FromArgb(0xa1, 0x00, 0x00, 0x00);
            this.identityBarFontColorUI     = System.Windows.Media.Color.FromArgb(0xff, 0xff, 0xff, 0xff);
            this.bottomBarDefaultOpacity    = .5;
            this.defaultThumbnailPath       = "Resources/Thumbnails/Default.jpg";
            this.synopsis                   = "";
            this.metadata                   = new ShowMetadata();

            this.tags                       = new ObservableCollection<Tag>();
            this.tags.CollectionChanged     += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(tags_CollectionChanged);

            this.frames                     = new ObservableCollection<Frame>();
            this.frames.CollectionChanged   += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(frames_CollectionChanged);            
            
            this.credits                    = new Credits();
            this.credits.PropertyChanged    += new PropertyChangedEventHandler(credits_PropertyChanged);

            this.markers = new ObservableCollection<VOMarker>();
            this.markers.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(markers_CollectionChanged);

            this.designTimeHelper           = new DesignTimeShowHelper();
            
        }


        //-------------------------------------------------------------------------------------------------
        // Event Handlers Section
        //-------------------------------------------------------------------------------------------------
        void credits_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Why is this not working????
            this.IsDirty = true;
        }



        //-------------------------------------------------------------------------------------------------
        void tags_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.IsDirty = true;
        }

        //-------------------------------------------------------------------------------------------------
        void frames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.IsDirty = true;
        }

        //-------------------------------------------------------------------------------------------------
        void markers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.IsDirty = true;
        }


        //-------------------------------------------------------------------------------------------------
        // Public Methods Section
        //-------------------------------------------------------------------------------------------------
        public void Save(string pFullPath)
        {
            foreach (Frame f in this.Frames)
            {
                f.Script.UpdateScriptText();
            }

            this.fullyQualifiedFileName = pFullPath;
            WayvFS.SaveShow(this);
            this.setDirty(false);
        }

        //-------------------------------------------------------------------------------------------------
        public void Open(string pFullPath)
        { 
        
        }

        //-------------------------------------------------------------------------------------------------
        public void setDirty(bool pIsDirty)
        {
            this.IsDirty = pIsDirty;
        }
    }

}
