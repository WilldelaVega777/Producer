using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace ProducerAlpha
{
    public class ExceptionSink
    {
        //-----------------------------------------------------------------------------------------------------
        // Public Events Section
        //-----------------------------------------------------------------------------------------------------
        public event ExceptionEventHandler UIManaging;
        public event ExceptionEventHandler TraceReporting;

        //-----------------------------------------------------------------------------------------------------
        // Public Methods Section
        //-----------------------------------------------------------------------------------------------------
        public void Manage(Exception ex)
        { 
            if (ex is XamlParseException)
            {
                this.OnUIManaging(ex.Message, ex.StackTrace);           
            }
            else
            {
                // Temporary, Replace with Event Log
                this.OnTraceReporting(ex.Message, ex.StackTrace);
            }
        }

        //-----------------------------------------------------------------------------------------------------
        private void OnUIManaging(string pMessage, string pStackTrace)
        {
            if (this.UIManaging != null)
            {
                this.UIManaging(this, new ExceptionEventArgs(pMessage, pStackTrace));
            }
        }

        //-----------------------------------------------------------------------------------------------------
        private void OnTraceReporting(string pMessage, string pStackTrace)
        {
            if (this.TraceReporting != null)
            { 
                this.TraceReporting(this, new ExceptionEventArgs(pMessage, pStackTrace));
            }
        }
    }
}
