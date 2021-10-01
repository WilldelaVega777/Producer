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
    public class ConversationalPoll
    {
        #region Public Properties Section
        //-----------------------------------------------------------------------------
        // Public Properties Section
        //-----------------------------------------------------------------------------
        [XmlAttribute]
        public double DefaultFontSize {get; set;}

        [XmlAttribute]
        public String EndMessage { get; set; }

        [XmlAttribute]
        public double DefaultQuestionMargin { get; set; }

        [XmlAttribute]
        public double DefaultAnswerMargin { get; set; }

        [XmlArray]
        public Question[] Questions { get; set; }

        #endregion

        #region Constructor Method Section
        //-----------------------------------------------------------------------------
        // Constructor Method Section
        //-----------------------------------------------------------------------------
        public ConversationalPoll() { }

        //-----------------------------------------------------------------------------
        public ConversationalPoll(double pDefaultFontSize,  
                                  String pEndMessage, 
                                  double pDefaultQuestionMargin, 
                                  double pDefaultAnswerMargin, 
                                  Question[] pQuestions
                                )
        {
            this.DefaultFontSize           = pDefaultFontSize;
            this.EndMessage                = pEndMessage;
            this.DefaultQuestionMargin     = pDefaultQuestionMargin;
            this.DefaultAnswerMargin       = pDefaultAnswerMargin;
            this.Questions                 = pQuestions;
        }

        #endregion
    }
}
