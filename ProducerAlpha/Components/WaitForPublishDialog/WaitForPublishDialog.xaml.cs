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
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;

namespace ProducerAlpha
{
    public partial class WaitForPublishDialog : UserControl, INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------
        // Private Fields Section
        //-------------------------------------------------------------------------------------
        private string status;

        //-------------------------------------------------------------------------------------
        // Public Properties Section
        //-------------------------------------------------------------------------------------
        public double Progress
        {
            get
            {
                return this.pbPublish.Value;
            }

            set
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    this.pbPublish.Value = value;
                    this.lblPercentProgress.Text = string.Format("{0:F1}%", value);
                });
            }
        }

        //-------------------------------------------------------------------------------------
        public String Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    this.status = value;
                    this.OnPropertyChanged("Status");
                });
            }
        }

        //-------------------------------------------------------------------------------------
        // Constructor Method Section
        //-------------------------------------------------------------------------------------
        public WaitForPublishDialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        //-------------------------------------------------------------------------------------
        // Event Handler Methods Section
        //-------------------------------------------------------------------------------------


        //-------------------------------------------------------------------------------------
        // Public Methods Section
        //-------------------------------------------------------------------------------------
        public void SetStandByMessage()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
            {
                this.lblPercentProgress.Text = "Please stand by..";
            });        
        }

        //-------------------------------------------------------------------------------------
        // INotifyPropertyChanged Members
        //-------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------
        private void OnPropertyChanged(string pProperty)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(pProperty));
            }
        }        

    }
}
