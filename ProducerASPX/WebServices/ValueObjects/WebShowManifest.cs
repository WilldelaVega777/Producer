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
    public class WebShowManifest
    {

        #region Public Properties Section
        //-----------------------------------------------------------------------------
        // Public Properties Section
        //-----------------------------------------------------------------------------
        [XmlAttribute]
        public String WebShowVersion { get; set; }

        [XmlAttribute]
        public String WebShowName { get; set; }

        [XmlAttribute]
        public String WebShowDate { get; set; }

        [XmlAttribute]
        public String WebShowAuthor { get; set; }

        [XmlAttribute]
        public double InitialVideoWindowY { get; set; }

        [XmlAttribute]
        public double InitialVideoWindowX { get; set; }

        [XmlAttribute]
        public double InitialVideoWindowWidth { get; set; }

        [XmlAttribute]
        public double InitialVideoWindowHeight { get; set; }

        [XmlAttribute]
        public String InitialVideoWindowSource { get; set; }

        [XmlAttribute]
        public String BasePath { get; set; }

        [XmlAttribute]
        public String BottomBarDefaultColor { get; set; }

        [XmlAttribute]
        public double BottomBarDefaultOpacity { get; set; }

        [XmlArray]
        public WebSlide[] WebSlides { get; set; }

        #endregion

        #region Constructor Method Section
        //-----------------------------------------------------------------------------
        // Constructor Method Section
        //-----------------------------------------------------------------------------
        public WebShowManifest() { }

        //-----------------------------------------------------------------------------
        public WebShowManifest(String pWebShowVersion, 
                                String pWebShowName,
                                String pWebShowDate,
                                String pWebShowAuthor,
                                double pInitialVideoWindowY,
                                double pInitialVideoWindowX,
                                double pInitialVideoWindowWidth,
                                double pInitialVideoWindowHeight,
                                String pInitialVideoWindowSource,
                                String pBasePath,
                                String pBottomBarDefaultColor,
                                double pBottomBarDefaultOpacity,
                                WebSlide[] pWebSlides)
        {
            this.WebShowVersion             = pWebShowVersion;
            this.WebShowName                = pWebShowName;
            this.WebShowDate                = pWebShowDate;
            this.WebShowAuthor              = pWebShowAuthor;
            this.InitialVideoWindowY        = pInitialVideoWindowY;
            this.InitialVideoWindowX        = pInitialVideoWindowX;
            this.InitialVideoWindowWidth    = pInitialVideoWindowWidth;
            this.InitialVideoWindowHeight   = pInitialVideoWindowHeight;
            this.InitialVideoWindowSource   = pInitialVideoWindowSource;
            this.BasePath                   = pBasePath;
            this.BottomBarDefaultColor      = pBottomBarDefaultColor;
            this.BottomBarDefaultOpacity    = pBottomBarDefaultOpacity;
            this.WebSlides                  = pWebSlides;
        }

        #endregion

    }
}
