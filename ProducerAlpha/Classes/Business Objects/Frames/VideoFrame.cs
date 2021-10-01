using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace ProducerAlpha
{
    public class VideoFrame : Frame, IAcceptsControls, IUsesAssets, IAcceptsBackgroundImage
    {

        //-------------------------------------------------------------------------------------------------
        // Private Fields Section
        //-------------------------------------------------------------------------------------------------
        private string filePath;
        private bool xamlContent;


        //-------------------------------------------------------------------------------------------------
        // Public Properties Section
        //-------------------------------------------------------------------------------------------------
        public string FilePath
        {
            get
            {
                return this.filePath;
            }
            set
            {
                this.filePath = value;
            }
        }


        //-------------------------------------------------------------------------------------------------
        // Constructor Method Section
        //-------------------------------------------------------------------------------------------------
        public VideoFrame(Script pScript) : base(pScript)
        {
            this.assets = new System.Collections.ObjectModel.ObservableCollection<Asset>();
            this.controls = new System.Collections.ObjectModel.ObservableCollection<Control>();
        }

        //------------------------------------------------------------------------------------
        public VideoFrame() : base() { }


        //-------------------------------------------------------------------------------------------------
        // IUsesAssets Interface Implementation Section
        //-------------------------------------------------------------------------------------------------
        public bool XamlContent
        {
            get
            {
                return this.xamlContent;
            }
            set
            {
                this.xamlContent = value;
            }
        }
        //-------------------------------------------------------------------------------------------------
        // Private Member
        private System.Collections.ObjectModel.ObservableCollection<Asset> assets;

        //-------------------------------------------------------------------------------------------------
        // Public Property
        public System.Collections.ObjectModel.ObservableCollection<Asset> Assets
        {
            get
            {
                return this.assets;
            }

            set
            {
                this.assets = value;
            }
        }



        //-------------------------------------------------------------------------------------------------
        // IAcceptsControls Interface Implementation Section
        //-------------------------------------------------------------------------------------------------
        // Private Member
        private System.Collections.ObjectModel.ObservableCollection<Control> controls;

        //-------------------------------------------------------------------------------------------------
        // Public Property
        public System.Collections.ObjectModel.ObservableCollection<Control> Controls
        {
            get
            {
                return this.controls;
            }
        }

        //------------------------------------------------------------------------------------
        // IAcceptsBackgroundImage Interface Implementation Section
        //------------------------------------------------------------------------------------
        // Private Member
        private String backgroundImageUri;
        private String inactiveBackgroundImageUri;

        //------------------------------------------------------------------------------------
        // Public Property
        [Browsable(false)]
        public String BackgroundImagePath
        {
            get
            {
                return this.backgroundImageUri;
            }
            set
            {
                this.backgroundImageUri = value;
                this.Script.ImageUrl = value;
            }
        }

        //------------------------------------------------------------------------------------
        [Browsable(false)]
        public String InactiveBackgroundImagePath
        {
            get
            {
                return this.inactiveBackgroundImageUri;
            }
            set
            {
                this.inactiveBackgroundImageUri = value;
            }
        }

    }
}
