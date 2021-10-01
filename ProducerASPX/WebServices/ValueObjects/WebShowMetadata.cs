using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WayvSL2ASPX
{
    public class WebShowMetadata
    {
        #region Public Properties Section
        //-----------------------------------------------------------------------------
        // Public Properties Section
        //-----------------------------------------------------------------------------
        [XmlAttribute]
        public int ShowId { get; set; }

        [XmlAttribute]
        public int Version { get; set; }
        
        [XmlAttribute]
        public string ShowTitle { get; set; }

        [XmlAttribute]
        public String Description { get; set; }

        [XmlAttribute]
        public int ChannelId { get; set; }

        #endregion

        #region Constructor Method Section
        //-----------------------------------------------------------------------------
        // Constructor Method Section
        //-----------------------------------------------------------------------------
        public WebShowMetadata() { }

        //-----------------------------------------------------------------------------
        public WebShowMetadata(int pShowId, 
                                int pVersion, 
                                String pShowTitle,
                                String pDescription,
                                int pChannelId
        )
        {
            this.ShowId         = pShowId;
            this.Version        = pVersion;
            this.ShowTitle      = pShowTitle;
            this.Description    = pDescription;
            this.ChannelId      = pChannelId;
        }

        #endregion
    }
}
