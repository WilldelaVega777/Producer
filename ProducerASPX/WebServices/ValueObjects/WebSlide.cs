using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Wayv.Framework
{
    [Serializable]
    public class WebSlide
    {
        #region Public Properties Section
        //-----------------------------------------------------------------------------
        // Public Properties Section
        //-----------------------------------------------------------------------------
        [XmlAttribute]
        public String WebSlideFrameLoopColor { get; set; }

        [XmlAttribute]
        public double WebSlideFrameLoopOpacity { get; set; }
        
        [XmlElement]
        public Thumbnail Thumbnail { get; set; }

        [XmlElement]
        public OverLink OverLink { get; set; }

        [XmlElement]
        public PageLink PageLink { get; set; }

        [XmlElement]
        public ConversationalPoll ConversationalPoll { get; set; }

        [XmlElement]
        public SurfMenu SurfMenu { get; set; }

        [XmlElement]
        public WatchClip WatchClip { get; set; }

        [XmlArray]
        public Asset[] Assets { get; set; }

        #endregion

        #region Constructor Method Section
        //-----------------------------------------------------------------------------
        // Constructor Method Section
        //-----------------------------------------------------------------------------
        public WebSlide() { }

        //-----------------------------------------------------------------------------
        public WebSlide(Thumbnail pThumbnail, Asset[] pAssets)
        {
            this.Thumbnail  = pThumbnail;
            this.Assets     = pAssets;
        }

        #endregion
    }
}
