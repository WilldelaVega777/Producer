using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProducerAlpha
{
    public class ExceptionEventArgs
    {
        //-------------------------------------------------------------------------
        // Private Fields Section
        //-------------------------------------------------------------------------
        private string exceptionMessage;
        private string exceptionStackTrace;

        //-------------------------------------------------------------------------
        // Public Properties Section
        //-------------------------------------------------------------------------
        public string ExceptionMessage
        {
            get
            {
                return this.exceptionMessage;
            }

            set
            {
                this.exceptionMessage = value;
            }
        }

        //-------------------------------------------------------------------------
        public string ExceptionStackTrace
        {
            get
            {
                return this.exceptionStackTrace;
            }

            set
            {
                this.exceptionStackTrace = value;
            }
        }

        //-------------------------------------------------------------------------
        // Constructor Method Section
        //-------------------------------------------------------------------------
        public ExceptionEventArgs(string pExceptionMessage, string pExceptionStackTrace)
        {
            this.exceptionMessage = pExceptionMessage;
            this.exceptionStackTrace = pExceptionStackTrace;
        }
    }
}
