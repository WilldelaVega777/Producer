using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ProducerAlpha
{
    public class WebSlide : Frame, IAcceptsControls, IUsesAssets, IAcceptsBackgroundImage
    {

        //-------------------------------------------------------------------------------------------------
        // Private Fields Section
        //-------------------------------------------------------------------------------------------------
        private bool xamlContent;

        //-------------------------------------------------------------------------------------------------
        // Public Properties Section
        //-------------------------------------------------------------------------------------------------


        //-------------------------------------------------------------------------------------------------
        // Constructor Method Section
        //-------------------------------------------------------------------------------------------------
        public WebSlide(Script pScript) : base(pScript)
        {
            this.assets = new System.Collections.ObjectModel.ObservableCollection<Asset>();
            this.controls = new System.Collections.ObjectModel.ObservableCollection<Control>();
        }

        //-------------------------------------------------------------------------------------------------
        public WebSlide() : base() 
        {
            this.assets = new System.Collections.ObjectModel.ObservableCollection<Asset>();
            this.controls = new System.Collections.ObjectModel.ObservableCollection<Control>();        
        }


        //-------------------------------------------------------------------------------------------------
        // IUsesAssets Interface Implementation Section
        //-------------------------------------------------------------------------------------------------
        [Browsable(false)]
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
        [Browsable(false)]
        public System.Collections.ObjectModel.ObservableCollection<Asset> Assets
        {
            set
            {
                this.assets = value;
            }

            get
            {
                return this.assets;
            }
        }

        //-------------------------------------------------------------------------------------------------
        // IAcceptsControls Interface Implementation Section
        //-------------------------------------------------------------------------------------------------
        // Private Member
        private System.Collections.ObjectModel.ObservableCollection<Control> controls;

        //-------------------------------------------------------------------------------------------------
        // Public Property
        [Browsable(false)]
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
