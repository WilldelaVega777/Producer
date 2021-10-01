using System;

namespace WayvASPX
{
    public class WebChannelButton : VOVideoButton
    {
        //--------------------------------------------------------------------------------------------
        // Public Fields Section
        //--------------------------------------------------------------------------------------------        
        public string WebChannelImageSource;

        //--------------------------------------------------------------------------------------------
        // Public Properties Section
        //--------------------------------------------------------------------------------------------
        public Uri WebChannelImageUri
        {
            get
            {
                return new Uri(this.WebChannelImageSource, UriKind.Relative);
            }
        }

        //--------------------------------------------------------------------------------------------
        // Constructor Method Section
        //--------------------------------------------------------------------------------------------        
        public WebChannelButton(Int32 pButtonID, String pImageSource, string pText, String pWebChannelImageSource)
        {
            base.ButtonID = pButtonID;
            base.ImageSource = pImageSource;
            base.ButtonText = pText;
            this.WebChannelImageSource = pWebChannelImageSource;
        }

        //--------------------------------------------------------------------------------------------   
        public WebChannelButton() { }
    }
}