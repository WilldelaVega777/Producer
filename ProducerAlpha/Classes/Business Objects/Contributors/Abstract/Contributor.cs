using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace ProducerAlpha
{
    [Serializable]
    public abstract class Contributor
    {

        //-----------------------------------------------------------------------------------
        // Private Fields Section
        //-----------------------------------------------------------------------------------
        private string name;
        private string info;


        //-----------------------------------------------------------------------------------
        // Public Properties Section
        //-----------------------------------------------------------------------------------
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        //-----------------------------------------------------------------------------------
        public string Info
        {
            get
            {
                return this.info;
            }
            set
            {
                this.info = value;
            }
        }

        //-----------------------------------------------------------------------------------
        // Constructor Method Section
        //-----------------------------------------------------------------------------------
        public Contributor() { }


    }
}
