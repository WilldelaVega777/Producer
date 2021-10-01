using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;

namespace ProducerAlpha
{
    public class PositionConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            ItemsControl itemsControl   = value[0] as ItemsControl;
            UIElement templateRoot      = value[1] as UIElement;
            ObservableCollection<Frame> frames = ((itemsControl as ListBox).DataContext as Show).Frames;

            if (templateRoot != null)
            {
                UIElement container = ItemsControl.ContainerFromElement(itemsControl, templateRoot) as UIElement;
                if (container != null)
                {
                    int iCurrentFrame = (itemsControl.ItemContainerGenerator.IndexFromContainer(container)+1);
                    (frames[iCurrentFrame-1] as Frame).Id = iCurrentFrame;
                    return (iCurrentFrame);
                }
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return new object[] {value};
        }
    }

}
