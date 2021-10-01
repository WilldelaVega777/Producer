using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProducerAlpha
{
    public class LayoutSettings
    {
        //--------------------------------------------------------------------------
        // Private Fields Section
        //--------------------------------------------------------------------------
        private double scriptPaneTopPanelHeight;
        private double showPaneTopPanelHeight;

        private bool isToogleModeActive = false;
        private bool applyMainLayout    = true;

        //--------------------------------------------------------------------------
        // Public Properties Section
        //--------------------------------------------------------------------------
        public double ScriptPaneTopPanelHeight
        {
            get { return this.scriptPaneTopPanelHeight; }
            set { this.scriptPaneTopPanelHeight = value;}
        }

        //--------------------------------------------------------------------------
        public double ShowPaneTopPanelHeight
        {
            get { return this.showPaneTopPanelHeight; }
            set { this.showPaneTopPanelHeight = value;}
        }

        //--------------------------------------------------------------------------
        public bool IsToogleModeActive
        {
            get { return this.isToogleModeActive; }
            set { this.isToogleModeActive = value; }
        }

        //--------------------------------------------------------------------------
        public bool ApplyMainLayout
        {
            get { return this.applyMainLayout; }
            set { this.applyMainLayout = value; }
        }

        //--------------------------------------------------------------------------
        // Constructor Method Section
        //--------------------------------------------------------------------------
        public LayoutSettings()
        { 
        
        }
    }
}
