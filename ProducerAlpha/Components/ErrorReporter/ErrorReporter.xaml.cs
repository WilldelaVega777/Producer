using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ProducerAlpha
{
    public partial class ErrorReporter : UserControl
    {
        //------------------------------------------------------------------------
        // Public Events Section
        //------------------------------------------------------------------------
        public event EventHandler Close;
        public event ExceptionEventHandler Save;

        //------------------------------------------------------------------------
        // Private Fields Section
        //------------------------------------------------------------------------
        private string errorText;
        private string errorStackTrace;

        //------------------------------------------------------------------------
        // Public Properties Section
        //------------------------------------------------------------------------
        public string ErrorText
        {
            get
            {
                return this.errorText;
            }

            set 
            {
                this.errorText = value;
                this.txtErrorDetails.Text = value;
            }
        }

        //------------------------------------------------------------------------
        public string ErrorStackTrace
        {
            get
            {
                return this.errorStackTrace;
            }

            set
            {
                this.errorStackTrace = value;
                this.txtErrorStackTrace.Text = value;
            }
        }
        
        //------------------------------------------------------------------------
        // Constructor Method Section
        //------------------------------------------------------------------------
        public ErrorReporter()
        {
            InitializeComponent();
        }


        //------------------------------------------------------------------------
        // Event Handler Methods Section
        //------------------------------------------------------------------------
        private void cmdClose_Click(object sender, RoutedEventArgs e)
        {
            this.ErrorText          = "";
            this.ErrorStackTrace    = "";
            this.Visibility = Visibility.Collapsed;
            this.OnClose();
        }

        //------------------------------------------------------------------------
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            this.OnSave(this.errorText, this.errorStackTrace);
        }


        //------------------------------------------------------------------------
        // Event Trigger Methods Section
        //------------------------------------------------------------------------
        protected void OnClose()
        {
            if (this.Close != null)
            {
                this.Close(this, new EventArgs());
            }
        }

        //------------------------------------------------------------------------
        protected void OnSave(string pErrorMessage, string pErrorStackTrace)
        {
            if (this.Save != null)
            {
                this.Save(this, new ExceptionEventArgs(pErrorMessage, pErrorStackTrace));
            }
        }

    }
}
