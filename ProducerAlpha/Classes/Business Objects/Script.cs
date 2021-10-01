using System;
using System.Collections.Generic;
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
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;


namespace ProducerAlpha
{
    [Serializable]
    public class Script : DependencyObject, INotifyPropertyChanged
    {
        //---------------------------------------------------------
        // Private Fields 
        //---------------------------------------------------------
        private int id;
        private string title;
        private string imageUrl;

        [NonSerialized]
        private FlowDocument scriptFlowDocument;       
        private string scriptText;

        private string expanderForAnimationText;
        private string expanderForOverlaysText;
        private string expanderForControlsText;
        private string expanderForImageText;
        private string expanderForSoundtrackText;
        private string expanderForVideoText;

        //-------------------------------------------------------------------------------------------------------
        // Public Properties
        //-------------------------------------------------------------------------------------------------------
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
        //-------------------------------------------------------------------------------------------------------
        public string Title 
        {
            get
            { 
                return this.title; 
            }
            set
            { 
                this.title = value;
                this.OnPropertyChanged("Title");
            } 
        }

        //-------------------------------------------------------------------------------------------------------
        public string ImageUrl 
        {
            get
            { 
                return this.imageUrl; 
            }
            set
            { 
                this.imageUrl = value;
                this.OnPropertyChanged("ImageUrl"); 
            }
        }
        
        //-------------------------------------------------------------------------------------------------------        
        [XmlIgnore]
        public FlowDocument ScriptFlowDocument
        {
            get
            { 
                return this.scriptFlowDocument; 
            }
            set
            { 
                this.scriptFlowDocument = value;
            }
        }

        //-------------------------------------------------------------------------------------------------------
        // Sets on DeSerialize
        public string ScriptText
        {
            get
            {
                return this.scriptText;
            }
            set
            {
                this.scriptText = value;

                if (this.scriptFlowDocument == null)
                {
                    this.scriptFlowDocument = new FlowDocument();
                }

                FlowDocumentConverter.AssignXamlToFlowDocument(ref this.scriptFlowDocument, value);
                this.OnPropertyChanged("ScriptFlowDocument");

            }
        }

        //-------------------------------------------------------------------------------------------------------
        // Dependency Properties
        //-------------------------------------------------------------------------------------------------------
        public string ExpanderForAnimationText
        {

            get { return (string)GetValue(ExpanderForAnimationTextProperty); }

            set 
            {
                this.expanderForAnimationText = value;
                SetValue(ExpanderForAnimationTextProperty, value);
                this.OnPropertyChanged("ExpanderForAnimationText");
            }

        }
        public static readonly DependencyProperty ExpanderForAnimationTextProperty =
            DependencyProperty.Register("ExpanderForAnimationText",
                typeof(string),
                typeof(Script),
                new FrameworkPropertyMetadata()
            );

        //----------------------------------------------------------------------------------------------------------
        public string ExpanderForOverlaysText
        {

            get { return (string)GetValue(ExpanderForOverlaysTextProperty); }

            set 
            {
                this.expanderForOverlaysText = value;
                SetValue(ExpanderForOverlaysTextProperty, value);
                this.OnPropertyChanged("ExpanderForOverlaysText");
            }

        }
        public static readonly DependencyProperty ExpanderForOverlaysTextProperty =
            DependencyProperty.Register("ExpanderForOverlaysText",
                typeof(string),
                typeof(Script),
                new FrameworkPropertyMetadata()
            );

        //----------------------------------------------------------------------------------------------------------
        public string ExpanderForControlsText
        {

            get { return (string)GetValue(ExpanderForControlsTextProperty); }

            set 
            {
                this.expanderForControlsText = value;
                SetValue(ExpanderForControlsTextProperty, value);
                this.OnPropertyChanged("ExpanderForControlsText");
            }

        }
        public static readonly DependencyProperty ExpanderForControlsTextProperty =
            DependencyProperty.Register("ExpanderForControlsText",
                typeof(string),
                typeof(Script),
                new FrameworkPropertyMetadata()
            );

        //----------------------------------------------------------------------------------------------------------
        public string ExpanderForImageText
        {

            get { return (string)GetValue(ExpanderForImageTextProperty); }

            set 
            {
                this.expanderForImageText = value;
                SetValue(ExpanderForImageTextProperty, value);
                this.OnPropertyChanged("ExpanderForImageText");
            }

        }
        public static readonly DependencyProperty ExpanderForImageTextProperty =
            DependencyProperty.Register("ExpanderForImageText",
                typeof(string),
                typeof(Script),
                new FrameworkPropertyMetadata()
            );

        //----------------------------------------------------------------------------------------------------------
        public string ExpanderForSoundtrackText
        {

            get { return (string)GetValue(ExpanderForSoundtrackTextProperty); }

            set 
            {
                this.expanderForSoundtrackText = value;
                SetValue(ExpanderForSoundtrackTextProperty, value);
                this.OnPropertyChanged("ExpanderForSoundtrackText");
            }

        }
        public static readonly DependencyProperty ExpanderForSoundtrackTextProperty =
            DependencyProperty.Register("ExpanderForSoundtrackText",
                typeof(string),
                typeof(Script),
                new FrameworkPropertyMetadata()
            );

        //----------------------------------------------------------------------------------------------------------
        public string ExpanderForVideoText
        {

            get { return (string)GetValue(ExpanderForVideoTextProperty); }

            set 
            {
                this.expanderForVideoText = value;
                SetValue(ExpanderForVideoTextProperty, value);
                this.OnPropertyChanged("ExpanderForVideoText");
            }

        }
        public static readonly DependencyProperty ExpanderForVideoTextProperty =
            DependencyProperty.Register("ExpanderForVideoText",
                typeof(string),
                typeof(Script),
                new FrameworkPropertyMetadata()
            );


        //---------------------------------------------------------
        // Constructor Methods Section
        //---------------------------------------------------------
        public Script(int pId, string pTitle, string pImageUrl, FlowDocument pScriptText)
        {
            this.id                     = pId;
            this.Title                  = pTitle;
            this.ImageUrl               = pImageUrl;
            this.ScriptFlowDocument     = pScriptText;
        }

        //---------------------------------------------------------
        public Script()
        {
            this.id = 0;
            this.Title = "Test";
            this.ImageUrl = "";
            this.ScriptFlowDocument = null;
        }


        //----------------------------------------------------------------------------------------------------------
        // Public Methods Section
        //----------------------------------------------------------------------------------------------------------
        public void UpdateScriptText()
        {
            if (this.ScriptFlowDocument != null)
            {
                this.scriptText = FlowDocumentConverter.ConvertFlowDocumentToText(this.scriptFlowDocument);
            }

            this.OnPropertyChanged("ScriptText");
        
        }
        
        //----------------------------------------------------------------------------------------------------------
        // INotifyPropertyChanged Implementation Section
        //----------------------------------------------------------------------------------------------------------
        private PropertyChangedEventHandler propertyChanged;

        //-------------------------------------------------------------------------------------------------
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { this.propertyChanged += value; }
            remove { this.propertyChanged -= value; }
        }

        //-------------------------------------------------------------------------------------------------
        private void OnPropertyChanged(string propertyName)
        {
            if (this.propertyChanged != null)
            {
                this.propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
