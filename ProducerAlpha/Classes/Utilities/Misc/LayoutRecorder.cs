using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProducerAlpha
{
    class LayoutRecorder
    {
        //--------------------------------------------------------------------------
        // Private Fields Section
        //--------------------------------------------------------------------------
        private double topPaneHeight;

        //--------------------------------------------------------------------------
        // Public Properties Section
        //--------------------------------------------------------------------------
        public double TopPaneHeight
        {
            get { return this.topPaneHeight; }
            set { this.topPaneHeight = value; }
        }

        //--------------------------------------------------------------------------
        // Constructor Method Section
        //--------------------------------------------------------------------------
        public LayoutRecorder()
        { 
        
        }
    }
}
