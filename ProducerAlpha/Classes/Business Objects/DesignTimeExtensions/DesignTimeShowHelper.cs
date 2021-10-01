using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace ProducerAlpha
{
    public class DesignTimeShowHelper : DependencyObject, INotifyPropertyChanged
    {
        //----------------------------------------------------------------------------------------------------------
        // Private Fields Section
        //----------------------------------------------------------------------------------------------------------
        private double videoVolume = .80;
        private bool videoMuteVolume = false;

        //-------------------------------------------------------------------------------------------------------
        // Dependency Properties
        //-------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public double VideoVolume
        {
            get { return (double)GetValue(VideoVolumeProperty); }

            set
            {
                this.videoVolume = value;
                SetValue(VideoVolumeProperty, value);
                this.OnPropertyChanged("VideoVolume");
            }

        }
        public static readonly DependencyProperty VideoVolumeProperty =
            DependencyProperty.Register("VideoVolume",
                typeof(double),
                typeof(DesignTimeShowHelper),
                new FrameworkPropertyMetadata()
            );

        //-------------------------------------------------------------------------------------------------------
        [Browsable(false)]
        public bool VideoMuteVolume
        {
            get { return (bool)GetValue(VideoMuteVolumeProperty); }

            set
            {
                this.videoMuteVolume = value;
                SetValue(VideoMuteVolumeProperty, value);
                this.OnPropertyChanged("VideoMuteVolume");
            }

        }
        public static readonly DependencyProperty VideoMuteVolumeProperty =
            DependencyProperty.Register("VideoMuteVolume",
                typeof(bool),
                typeof(DesignTimeShowHelper),
                new FrameworkPropertyMetadata()
            );


        //----------------------------------------------------------------------------------------------------------
        // Constructors Method Section
        //----------------------------------------------------------------------------------------------------------
        public DesignTimeShowHelper()
        {
            this.videoMuteVolume = false;
            this.videoVolume = .80;
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
