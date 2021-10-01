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
using System.Windows.Controls.Primitives;

namespace ProducerAlpha
{
    public class ThreeStateToggleButton : ToggleButton
    {
        //----------------------------------------------------------------------------------------------------------
        // Routed Events Section
        //----------------------------------------------------------------------------------------------------------
        public static readonly RoutedEvent MarkedEvent = 
            EventManager.RegisterRoutedEvent("Marked", RoutingStrategy.Bubble,
                                            typeof(RoutedEventHandler),
                                            typeof(ThreeStateToggleButton));

        public event RoutedEventHandler Marked
        {
            add { AddHandler(MarkedEvent, value); }
            remove { RemoveHandler(MarkedEvent, value); }
        }


        //----------------------------------------------------------------------------------------------------------
        // Dependency Properties Section
        //----------------------------------------------------------------------------------------------------------
        static DependencyProperty IsMarkedProperty;


        //----------------------------------------------------------------------------------------------------------
        // Public Properties Section
        //----------------------------------------------------------------------------------------------------------
        public bool IsMarked
        {
            get
            {
                return (bool)GetValue(IsMarkedProperty);
            }

            set
            {
                SetValue(IsMarkedProperty, value);
            }

        }


        //----------------------------------------------------------------------------------------------------------
        // Constructor Method Section
        //----------------------------------------------------------------------------------------------------------
        static ThreeStateToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ThreeStateToggleButton), new FrameworkPropertyMetadata(typeof(ThreeStateToggleButton)));


            PropertyChangedCallback countChangedCallback =
                new PropertyChangedCallback(IsMarkedChanged);

            PropertyMetadata metaData =
                new PropertyMetadata(countChangedCallback);

            IsMarkedProperty = DependencyProperty.Register("IsMarked",
                typeof(bool), typeof(ThreeStateToggleButton), metaData);

        }


        //----------------------------------------------------------------------------------------------------------
        // Callbacks (Event Handling) Section
        //----------------------------------------------------------------------------------------------------------
        static void IsMarkedChanged(DependencyObject property,
            DependencyPropertyChangedEventArgs args)
        {
            
        }

        //protected virtual void RaisePropertyChanged()
        //{
        //    this.RaiseEvent(new RoutedEventArgs(ThreeStateToggleButton.MarkedEvent, this));
        //}
    }
}
