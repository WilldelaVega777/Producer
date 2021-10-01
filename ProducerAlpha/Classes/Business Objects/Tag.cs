using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;


namespace ProducerAlpha
{
    [Serializable]
    public class Tag
    {
        //-----------------------------------------------------------------------------------
        // Private Fields Section
        //-----------------------------------------------------------------------------------
        private string content;

        //-----------------------------------------------------------------------------------
        // Public Properties Section
        //-----------------------------------------------------------------------------------
        public string Content
        {
            get
            {
                return this.content;
            }
            set
            {
                this.content = value;
            }
        }

        //-----------------------------------------------------------------------------------
        // Constructor Method Section
        //-----------------------------------------------------------------------------------
        public Tag() { }

    }
}
