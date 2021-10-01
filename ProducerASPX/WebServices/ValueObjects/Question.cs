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
    public class Question
    {
        #region Public Properties Section
        //-----------------------------------------------------------------------------
        // Public Properties Section
        //-----------------------------------------------------------------------------
        [XmlAttribute]
        public int ID { get; set; }

        [XmlAttribute]
        public double FontSize { get; set; }

        [XmlAttribute]
        public double QuestionMargin { get; set; }

        [XmlAttribute]
        public double AnswerMargin { get; set; }

        [XmlArray]
        public QuestionText[] QuestionTexts { get; set; }

        [XmlArray]
        public Answer[] Answers { get; set; }

        #endregion

        #region Constructor Method Section
        //-----------------------------------------------------------------------------
        // Constructor Method Section
        //-----------------------------------------------------------------------------
        public Question() { }

        //-----------------------------------------------------------------------------
        public Question(int pID, 
                        double pFontSize, 
                        double pQuestionMargin, 
                        double pAnswerMargin
                       )
        {
            this.ID             = pID;
            this.FontSize       = pFontSize;
            this.QuestionMargin = pQuestionMargin;
            this.AnswerMargin   = pAnswerMargin;
        }

        #endregion
    }
}
