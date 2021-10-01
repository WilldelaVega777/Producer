using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace ProducerAlpha
{
    [Serializable]
    public class DesignTimeFrameHelper : DependencyObject, INotifyPropertyChanged
    {
        //----------------------------------------------------------------------------------------------------------
        // Private Fields Section
        //----------------------------------------------------------------------------------------------------------
        private bool lockFrame;
        private bool renderPreview;
        private bool hasSyncLink;
        private string xamlContent;
        private ImportedXamlInfo xamlInfo;


        //-------------------------------------------------------------------------------------------------------
        // Dependency Properties
        //-------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public bool RenderPreview
        {
            get { return (bool)GetValue(RenderPreviewProperty); }

            set
            {
                this.renderPreview = value;
                SetValue(RenderPreviewProperty, value);
                this.OnPropertyChanged("RenderPreview");
            }

        }
        public static readonly DependencyProperty RenderPreviewProperty =
            DependencyProperty.Register("RenderPreview",
                typeof(bool),
                typeof(DesignTimeFrameHelper),
                new FrameworkPropertyMetadata()
            );

        //----------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public bool LockFrame
        {

            get { return (bool)GetValue(LockFrameProperty); }

            set
            {
                this.lockFrame = value;
                SetValue(LockFrameProperty, value);
                this.OnPropertyChanged("LockFrame");
            }

        }
        public static readonly DependencyProperty LockFrameProperty =
            DependencyProperty.Register("LockFrame",
                typeof(bool),
                typeof(DesignTimeFrameHelper),
                new FrameworkPropertyMetadata()
            );


        //----------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public bool HasSyncLink
        {

            get { return (bool)GetValue(HasSyncLinkProperty); }

            set
            {
                this.hasSyncLink = value;
                SetValue(HasSyncLinkProperty, value);
                this.OnPropertyChanged("HasSyncLink");
            }

        }
        public static readonly DependencyProperty HasSyncLinkProperty =
            DependencyProperty.Register("HasSyncLink",
                typeof(bool),
                typeof(DesignTimeFrameHelper),
                new FrameworkPropertyMetadata()
            );


        //----------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public string XamlContent
        {

            get { return (string)GetValue(XamlContentProperty); }

            set
            {
                this.xamlContent = value;
                SetValue(XamlContentProperty, value);
                this.OnPropertyChanged("XamlContent");
            }

        }
        public static readonly DependencyProperty XamlContentProperty =
            DependencyProperty.Register("XamlContent",
                typeof(string),
                typeof(DesignTimeFrameHelper),
                new FrameworkPropertyMetadata()
            );


        //----------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public ImportedXamlInfo XamlInfo
        {

            get { return (ImportedXamlInfo)GetValue(XamlInfoProperty); }

            set
            {
                this.xamlInfo = value;
                this.xamlInfo.PropertyChanged += new PropertyChangedEventHandler(XamlInfo_PropertyChanged);
                SetValue(XamlInfoProperty, value);
                this.OnPropertyChanged("XamlInfo");
            }

        }
        public static readonly DependencyProperty XamlInfoProperty =
            DependencyProperty.Register("XamlInfo",
                typeof(ImportedXamlInfo),
                typeof(DesignTimeFrameHelper),
                new FrameworkPropertyMetadata()
            );


        //----------------------------------------------------------------------------------------------------------
        // Constructors Method Section
        //----------------------------------------------------------------------------------------------------------
        public DesignTimeFrameHelper() 
        {
            this.RenderPreview = true;
            this.XamlInfo = new ImportedXamlInfo();
        }

        //----------------------------------------------------------------------------------------------------------
        // Event Handler Method Section (PropertyChanged)
        //----------------------------------------------------------------------------------------------------------
        void XamlInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged("XamlInfo");
        }


        //----------------------------------------------------------------------------------------------------------
        // INotifyPropertyChanged Implementation Section
        //----------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
