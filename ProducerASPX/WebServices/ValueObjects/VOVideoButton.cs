using System;

namespace WayvASPX
{
    public class VOVideoButton
    {
        //--------------------------------------------------------------------------------------------
        // Public Fields Section
        //--------------------------------------------------------------------------------------------        
        public Int32  ButtonID;
        public String ImageSource;
        public String ButtonText;

        //--------------------------------------------------------------------------------------------
        // Public Properties Section
        //--------------------------------------------------------------------------------------------
        public Uri ImageUri
        {
            get
            {
                return new Uri(this.ImageSource, UriKind.Relative);
            }
        }

        //--------------------------------------------------------------------------------------------
        // Constructor Method Section
        //--------------------------------------------------------------------------------------------        
        public VOVideoButton(Int32 pButtonID, String pImageSource, string pText)
        {
            this.ButtonID       = pButtonID;
            this.ImageSource    = pImageSource;
            this.ButtonText     = pText;
        }

        //--------------------------------------------------------------------------------------------   
        public VOVideoButton() {}        
    }
}
