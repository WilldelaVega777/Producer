using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace ProducerAlpha
{
    public class WebChannelInfo
    {
        //-------------------------------------------------------------------------------------------------
        // Private Fields Section
        //-------------------------------------------------------------------------------------------------
        private int             webChannelId;
        private string          webChannelName;
        private string          webChannelDescription;
        private BitmapSource    webChannelImage;
        private Bitmap          webChannelLogo;

        //-------------------------------------------------------------------------------------------------
        // Public Properties Section
        //-------------------------------------------------------------------------------------------------
        public int WebChannelId
        {
            get
            {
                return this.webChannelId;
            }
            set
            {
                this.webChannelId = value;
            }
        }

        //-------------------------------------------------------------------------------------------------
        public string WebChannelName
        {
            get
            {
                return this.webChannelName;
            }
            set
            {
                this.webChannelName = value;
            }
        }

        //-------------------------------------------------------------------------------------------------
        public String WebChannelDescription
        {
            get
            {
                return this.webChannelDescription;
            }
            set
            {
                this.webChannelDescription = value;
            }
        }

        //-------------------------------------------------------------------------------------------------
        public BitmapSource WebChannelImage
        {
            get
            {
                return this.webChannelImage;
            }
            set
            {
                this.webChannelImage = value;
            }
        }

        //-------------------------------------------------------------------------------------------------
        public Bitmap WebChannelLogo
        {
            get
            {
                return this.webChannelLogo;
            }
            set
            {
                this.webChannelLogo = value;
            }
        }


        //-------------------------------------------------------------------------------------------------
        // Constructor Methods Section
        //-------------------------------------------------------------------------------------------------
        public WebChannelInfo()
        { }

        //-------------------------------------------------------------------------------------------------
        public WebChannelInfo
        (
            int pWebChannelId, 
            string pWebChannelName, 
            string pWebChannelDescription, 
            BitmapSource pWebChannelImage, 
            Bitmap pWebChannelLogo
        )
        {
            this.webChannelId           = pWebChannelId;
            this.webChannelName         = pWebChannelName;
            this.webChannelDescription  = pWebChannelDescription;
            this.webChannelImage        = pWebChannelImage;
            this.webChannelLogo         = pWebChannelLogo;
        }
    }
}
