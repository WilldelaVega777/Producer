using System;

namespace WayvASPX
{
    public class WebChannelInfo
    {
        //--------------------------------------------------------------------------------------------
        // Public Fields Section
        //--------------------------------------------------------------------------------------------        
        public Int32  ChannelId;
        public String ChannelName;
        public String ChannelDescription;
        public Byte[] ChannelImage;
        public Byte[] ChannelLogo;

        //--------------------------------------------------------------------------------------------
        // Constructor Method Section
        //--------------------------------------------------------------------------------------------        
        public WebChannelInfo(Int32 pChannelId, String pChannelName, string pChannelDescription, Byte[] pChannelImage, Byte[] pChannelLogo)
        {
            this.ChannelId              = pChannelId;
            this.ChannelName            = pChannelName;
            this.ChannelDescription     = pChannelDescription;
            this.ChannelImage           = pChannelImage;
            this.ChannelLogo            = pChannelLogo;
        }

        //--------------------------------------------------------------------------------------------   
        public WebChannelInfo() { }
    }
}