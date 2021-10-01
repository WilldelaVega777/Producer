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
    public class QuestionText
    {
        #region Public Properties Section
        //-----------------------------------------------------------------------------
        // Public Properties Section
        //-----------------------------------------------------------------------------
        [XmlAttribute]
        public String Text { get; set; }

        [XmlAttribute]
        public int Order { get; set; }

        #endregion

        #region Constructor Method Section
        //-----------------------------------------------------------------------------
        // Constructor Method Section
        //-----------------------------------------------------------------------------
        public QuestionText() { }

        //-----------------------------------------------------------------------------
        public QuestionText(String pText, int pOrder)
        {
            this.Text = pText;
            this.Order = pOrder;
        }

        #endregion
    }
}
