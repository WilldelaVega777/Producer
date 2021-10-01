using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ProducerAlpha
{
    [Serializable]
    public abstract class Frame : INotifyPropertyChanged
    {
        //----------------------------------------------------------------------------------------------------------
        // Private Fields Section
        //----------------------------------------------------------------------------------------------------------
        private int                     id;
        private string                  title;
        private int                     sortOrder;
        private string                  thumbnailPath;
        private Script                  script;
        private DesignTimeFrameHelper   designTimeHelper;

        //----------------------------------------------------------------------------------------------------------
        // Public Properties Section
        //----------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.OnPropertyChanged("Id");
                this.id = value;
                this.Script.Id = value;
            }
        }

        //----------------------------------------------------------------------------------------------------------
        [Category("General")]
        [DisplayName("Frame Title:")]
        [Description("The Title for this Frame")]        
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

        //----------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public int SortOrder
        {
            get
            {
                return this.sortOrder;
            }
            set
            {
                this.sortOrder = value;
                this.OnPropertyChanged("SortOrder");
            }
        }

        //----------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public string ThumbnailPath
        {
            get
            {
                return this.thumbnailPath;
            }
            set
            {
                this.thumbnailPath = value;
                this.OnPropertyChanged("ThumbnailPath");
            }
        }

        //----------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public ProducerAlpha.Script Script
        {
            get
            {
                return this.script;
            }
            set
            {
                this.script = value;
                this.OnPropertyChanged("Script");
            }
        }

        //----------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public ProducerAlpha.DesignTimeFrameHelper DesignTimeHelper
        {
            get
            {
                return this.designTimeHelper;
            }
            set
            {
                this.designTimeHelper = value;
                this.OnPropertyChanged("DesignTimeHelper");
            }
        }

        //----------------------------------------------------------------------------------------------------------
        // Constructors Method Section
        //----------------------------------------------------------------------------------------------------------
        public Frame()
        {
            this.script = new Script();
            this.designTimeHelper = new DesignTimeFrameHelper();
        }

        //----------------------------------------------------------------------------------------------------------
        public Frame(Script pScript)
        {
            this.script = pScript;
            (this.script as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(Frame_PropertyChanged);
            this.designTimeHelper = new DesignTimeFrameHelper();
            (this.designTimeHelper as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(Frame_PropertyChanged);

            this.id = pScript.Id;
        }

        //----------------------------------------------------------------------------------------------------------
        // Event Handler Methods Section
        //----------------------------------------------------------------------------------------------------------
        void Frame_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Script)
            {
                this.OnPropertyChanged("Script");
            }
            else if (sender is DesignTimeFrameHelper)
            {
                this.OnPropertyChanged("DesignTimeHelper");
            }

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
