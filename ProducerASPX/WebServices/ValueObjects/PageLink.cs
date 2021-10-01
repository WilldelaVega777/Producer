﻿using System;
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
using System.Xml.Serialization;

namespace Wayv.Framework
{
    [Serializable]
    public class PageLink
    {
        #region Public Properties Section
        //-----------------------------------------------------------------------------
        // Public Properties Section
        //-----------------------------------------------------------------------------
        [XmlAttribute]
        public String Url { get; set; }

        [XmlAttribute]
        public String InitialButtonDelayTime { get; set; }

        [XmlAttribute]
        public double Left { get; set; }

        [XmlAttribute]
        public double Top { get; set; }

        #endregion

        #region Constructor Method Section

        //-----------------------------------------------------------------------------
        // Constructor Method Section
        //-----------------------------------------------------------------------------
        public PageLink() { }

        //-----------------------------------------------------------------------------
        public PageLink(String pUrl, String pInitialButtonDelayTime)
        {
            this.Url = pUrl;
            this.InitialButtonDelayTime = pInitialButtonDelayTime;
        }

        #endregion
    }
}
