using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ProducerAlpha
{
    class Project
    {
        //----------------------------------------------------------------------------------------------------------
        // Private Fields Section
        //----------------------------------------------------------------------------------------------------------
        private Settings settings;
        private Show show;

        //-------------------------------------------------------------------------------------------------
        // Public Properties Section
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Settings Class (VO for Preferences Window)
        /// </summary>
        [Browsable(false)]
        public Settings Settings
        {
            get
            {
                return this.settings;
            }
            set
            {
                this.settings = value;
            }
        }

        //-------------------------------------------------------------------------------------------------        
        /// <summary>
        /// Main Show Class
        /// </summary>
        [Browsable(true)] 
        public Show Show
        {
            get
            {
                return this.show;
            }
            set
            {
                this.show = value;
            }
        }

        //----------------------------------------------------------------------------------------------------------
        // Constructor Section
        //----------------------------------------------------------------------------------------------------------
        public Project()
        {
            this.settings = new Settings();
        }

    }
}
