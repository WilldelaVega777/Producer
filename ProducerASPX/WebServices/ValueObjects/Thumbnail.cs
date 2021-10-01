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
using System.Xml.Serialization;

namespace Wayv.Framework
{
    [Serializable]
    public class Thumbnail
    {
        #region Public Properties Section
        //-----------------------------------------------------------------------------
        // Public Properties Section
        //-----------------------------------------------------------------------------
        [XmlAttribute]
        public String ImageSource { get; set; }

        [XmlAttribute]
        public String FrameText { get; set; }

        [XmlAttribute]
        public Int32 FrameID { get; set; }

        #endregion

        #region Constructor Method Section
        //-----------------------------------------------------------------------------
        // Constructor Method Section
        //-----------------------------------------------------------------------------
        public Thumbnail() { }

        //-----------------------------------------------------------------------------
        public Thumbnail(String pImageSource, String pFrameText, Int32 pFrameID)
        {
            this.ImageSource    = pImageSource;
            this.FrameText      = pFrameText;
            this.FrameID        = pFrameID;
        }

        #endregion

    }
}
