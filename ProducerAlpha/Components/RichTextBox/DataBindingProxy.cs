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
using System.Windows.Threading;

namespace ProducerAlpha
{
    public delegate void Operation();

    public class DataBindingProxy : FrameworkElement
    {
        public static readonly DependencyProperty InProperty;
        public static readonly DependencyProperty OutProperty;

        public DataBindingProxy()
        {
            Visibility = Visibility.Collapsed;
        }

        static DataBindingProxy()
        {
            FrameworkPropertyMetadata inMetadata = new FrameworkPropertyMetadata(
                delegate(DependencyObject p, DependencyPropertyChangedEventArgs args)
                {
                    if (null != BindingOperations.GetBinding(p, OutProperty))
                        (p as DataBindingProxy).Out = args.NewValue;
                });

            inMetadata.BindsTwoWayByDefault = false;
            inMetadata.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            InProperty = DependencyProperty.Register("In",
                typeof(object),
                typeof(DataBindingProxy),
                inMetadata);

            FrameworkPropertyMetadata outMetadata = new FrameworkPropertyMetadata(
                delegate(DependencyObject p, DependencyPropertyChangedEventArgs args)
                {
                    ValueSource source = DependencyPropertyHelper.GetValueSource(p, args.Property);

                    if (source.BaseValueSource != BaseValueSource.Local)
                    {
                        DataBindingProxy OProxy = p as DataBindingProxy;
                        object expected = OProxy.In;
                        if (!object.ReferenceEquals(args.NewValue, expected))
                        {
                            Dispatcher.CurrentDispatcher.BeginInvoke(
                                DispatcherPriority.DataBind, new Operation(delegate
                                {
                                    OProxy.Out = OProxy.In;
                                }));
                        }
                    }
                });

            outMetadata.BindsTwoWayByDefault = true;
            outMetadata.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            OutProperty = DependencyProperty.Register("Out",
                typeof(object),
                typeof(DataBindingProxy),
                outMetadata);
        }

        public object In
        {
            get { return this.GetValue(InProperty); }
            set { this.SetValue(InProperty, value); }
        }

        public object Out
        {
            get { return this.GetValue(OutProperty); }
            set { this.SetValue(OutProperty, value); }
        }
    }
}
