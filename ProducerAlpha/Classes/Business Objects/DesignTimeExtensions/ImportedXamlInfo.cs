using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Xml.Serialization;


namespace ProducerAlpha
{
    [Serializable]
    public class ImportedXamlInfo : DependencyObject, INotifyPropertyChanged
    {
        //-----------------------------------------------------------
        // Private Fields Section
        //-----------------------------------------------------------
        private string      xamlPath;
        private DateTime    dateOfLastModification;

        //-------------------------------------------------------------------------------------------------------
        // Dependency Properties
        //-------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public String XamlPath
        {
            get { return (String)GetValue(XamlPathProperty); }

            set
            {
                this.xamlPath = value;
                SetValue(XamlPathProperty, value);
                this.OnPropertyChanged("XamlPath");
            }
        }
        public static readonly DependencyProperty XamlPathProperty =
            DependencyProperty.Register("XamlPath",
                typeof(String),
                typeof(ImportedXamlInfo),
                new FrameworkPropertyMetadata()
            );

        //-------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public DateTime DateOfLastModification
        {
            get { return (DateTime)GetValue(DateOfLastModificationProperty); }

            set
            {
                this.dateOfLastModification = value;
                SetValue(DateOfLastModificationProperty, value);
                this.OnPropertyChanged("DateOfLastModification");
            }

        }
        public static readonly DependencyProperty DateOfLastModificationProperty =
            DependencyProperty.Register("DateOfLastModification",
                typeof(DateTime),
                typeof(ImportedXamlInfo),
                new FrameworkPropertyMetadata()
            );

        //-----------------------------------------------------------
        // Constructor Methods Section
        //-----------------------------------------------------------
        public ImportedXamlInfo(string pXamlPath, DateTime pDateOfLastModification)
        {
            this.XamlPath               = pXamlPath;
            this.DateOfLastModification = pDateOfLastModification;
        }

        //-----------------------------------------------------------
        public ImportedXamlInfo()
        {
            this.XamlPath = "Empty";
            this.DateOfLastModification = DateTime.Now;
        }

        //----------------------------------------------------------------------------------------------------------
        // INotifyPropertyChanged Implementation Section
        //----------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //----------------------------------------------------------------------------------------------------------
        protected void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
