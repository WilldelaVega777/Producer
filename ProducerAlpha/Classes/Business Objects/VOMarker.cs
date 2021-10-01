using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProducerAlpha
{
    [Serializable]
    public class VOMarker
    {
        //--------------------------------------------------------------------
        // Private Fields Section
        //--------------------------------------------------------------------
        private long m_id;
        private TimeSpan m_value;


        //--------------------------------------------------------------------
        // Public Properties Section
        //--------------------------------------------------------------------
        public long Id
        {
            get
            {
                return this.m_id;
            }

            set
            {
                this.m_id = value;
            }
        }

        //--------------------------------------------------------------------
        [XmlElement("Value")]
        public string XmlValue
        {
            get { return this.m_value.ToString(); }
            set { this.m_value = TimeSpan.Parse(value); }
        }

        [XmlIgnore]
        public TimeSpan Value
        {
            get 
            {
                return this.m_value;
            }
            set
            {
                m_value = value;
            }
        }

        //-----------------------------------------------------------------------------------
        // Constructor Method Section
        //-----------------------------------------------------------------------------------
        public VOMarker() { }
    }
}
