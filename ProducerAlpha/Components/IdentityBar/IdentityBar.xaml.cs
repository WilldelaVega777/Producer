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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace ProducerAlpha
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class IdentityBar : UserControl, INotifyPropertyChanged
    {
        //-----------------------------------------------------------------------------------------
        // Private Fields Section
        //-----------------------------------------------------------------------------------------
        private string webShowAuthor;
        private string webShowName;

        private Storyboard OSBMouseEnter;
        private Storyboard OSBMouseLeave;


        //-----------------------------------------------------------------------------------------
        // Public Events Section
        //-----------------------------------------------------------------------------------------
        public event EventHandler onMouseOver;
        public event EventHandler onMouseOut;

        //-------------------------------------------------------------------------------------------------------
        // Dependency Properties
        //-------------------------------------------------------------------------------------------------------
        public string ShowName
        {
            get { return (string)GetValue(ShowNameProperty); }

            set
            {
                this.webShowName = value;
                SetValue(ShowNameProperty, value);
                this.OnPropertyChanged("ShowName");
            }

        }
        public static readonly DependencyProperty ShowNameProperty =
            DependencyProperty.Register("ShowName",
                typeof(string),
                typeof(IdentityBar),
                new FrameworkPropertyMetadata()
            );


        //-------------------------------------------------------------------------------------------------------
        public string ShowAuthor
        {
            get { return (string)GetValue(ShowAuthorProperty); }

            set
            {
                this.webShowAuthor = value;
                SetValue(ShowAuthorProperty, value);
                this.OnPropertyChanged("ShowAuthor");
            }

        }
        public static readonly DependencyProperty ShowAuthorProperty =
            DependencyProperty.Register("ShowAuthor",
                typeof(string),
                typeof(IdentityBar),
                new FrameworkPropertyMetadata()
            );

        //-----------------------------------------------------------------------------------------
        // Constructor Method Section
        //-----------------------------------------------------------------------------------------            
        public IdentityBar()
        {
            // Initialize Components
            InitializeComponent();

            // Event Registration
            this.MouseEnter += new MouseEventHandler(IdentityBar_MouseEnter);
            this.MouseLeave += new MouseEventHandler(IdentityBar_MouseLeave);

            this.OSBMouseEnter  = (this.OMainCanvas.Resources["OSBMouseEnter"] as Storyboard);
            this.OSBMouseLeave = (this.OMainCanvas.Resources["OSBMouseLeave"] as Storyboard);

            this.OWebShowName.DataContext = this;
            this.OWebShowAuthor.DataContext = this;
            this.OMainCanvas.DataContext = this;
        }


        //-----------------------------------------------------------------------------------------
        // Event Handler Methods Section
        //-----------------------------------------------------------------------------------------
        private void IdentityBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.OnMouseOver();
            //this.increaseOpacity();
        }


        //-----------------------------------------------------------------------------------------
        private void IdentityBar_MouseLeave(object sender, EventArgs e)
        {
            this.OnMouseOut();
            //this.decreaseOpacity();
        }

        //-----------------------------------------------------------------------------------------
        // Public Methods Section
        //-----------------------------------------------------------------------------------------
        public void increaseOpacity()
        {
            if (this.OSBMouseEnter != null)
            {
                this.OSBMouseEnter.Begin();
            }
        }

        //-----------------------------------------------------------------------------------------
        public void decreaseOpacity()
        {
            if (this.OSBMouseLeave != null)
            {
                this.OSBMouseLeave.Begin();
            }
        }

        //-----------------------------------------------------------------------------------------
        // Event Triggers Section
        //-----------------------------------------------------------------------------------------
        protected void OnMouseOver()
        {
            if (this.onMouseOver != null)
            {
                this.onMouseOver(this, new EventArgs());
            }
        }

        //-----------------------------------------------------------------------------------------
        protected void OnMouseOut()
        {
            if (this.onMouseOut != null)
            {
                this.onMouseOut(this, new EventArgs());
            }
        }

        //----------------------------------------------------------------------------------------------------------
        // INotifyPropertyChanged Implementation Section
        //----------------------------------------------------------------------------------------------------------
        private PropertyChangedEventHandler propertyChanged;

        //-------------------------------------------------------------------------------------------------
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { this.propertyChanged += value; }
            remove { this.propertyChanged -= value; }
        }

        //-------------------------------------------------------------------------------------------------
        private void OnPropertyChanged(string propertyName)
        {
            if (this.propertyChanged != null)
            {
                this.propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
