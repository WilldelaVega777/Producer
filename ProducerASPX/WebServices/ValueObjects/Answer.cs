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
    public class Answer
    {
        #region Public Properties Section
        //-----------------------------------------------------------------------------
        // Public Properties Section
        //-----------------------------------------------------------------------------
        [XmlAttribute]
        public int Id { get; set; }
        
        [XmlAttribute]
        public int NextQuestionID { get; set; }

        [XmlAttribute]
        public String Text { get; set; }

        #endregion

        #region Constructor Method Section
        //-----------------------------------------------------------------------------
        // Constructor Method Section
        //-----------------------------------------------------------------------------
        public Answer() { }

        //-----------------------------------------------------------------------------
        public Answer(int pId, int pNextQuestionID, String pText)
        {
            this.Id             = pId;
            this.NextQuestionID = pNextQuestionID;
            this.Text           = pText;
        }

        #endregion
    }
}
