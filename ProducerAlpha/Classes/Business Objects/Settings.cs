using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProducerAlpha
{
    public class Settings
    {
        //-------------------------------------------------------------------------------------------------
        // Private Fields Section
        //-------------------------------------------------------------------------------------------------
        private LayoutSettings layoutSettings;

        //-------------------------------------------------------------------------------------------------
        // Public Properties Section
        //-------------------------------------------------------------------------------------------------
        public LayoutSettings LayoutSettings
        {
            get
            {
                return this.layoutSettings;
            }
            set
            {
                this.layoutSettings = value;
            }
        }


        //-------------------------------------------------------------------------------------------------
        // Constructor Method Section
        //-------------------------------------------------------------------------------------------------
        public Settings() 
        {
            this.layoutSettings = new LayoutSettings();
        }

    }
}
