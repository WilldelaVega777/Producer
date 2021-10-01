using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProducerAlpha
{
    public class PageShot: Frame, IUsesAssets, IAcceptsControls
    {
        //-------------------------------------------------------------------------------------------------
        // Private Fields Section
        //-------------------------------------------------------------------------------------------------
        private string url;
        private bool xamlContent;
        
        
        //-------------------------------------------------------------------------------------------------
        // Public Properties Section
        //-------------------------------------------------------------------------------------------------
        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
            }
        }


        //-------------------------------------------------------------------------------------------------
        // Constructor Method Section
        //-------------------------------------------------------------------------------------------------
        public PageShot(Script pScript)
            : base(pScript)
        {
            this.assets = new System.Collections.ObjectModel.ObservableCollection<Asset>();
            this.controls = new System.Collections.ObjectModel.ObservableCollection<Control>();
        }

        //-------------------------------------------------------------------------------------------------
        public PageShot() : base() { }


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
    }
}
