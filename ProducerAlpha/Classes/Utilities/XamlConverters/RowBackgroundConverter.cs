using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProducerAlpha
{
    public sealed class RowBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;

            // Get the index of a ListViewItem

            int index = listView.ItemContainerGenerator.IndexFromContainer(item);
            if (index % 2 == 0)
            {
                return new SolidColorBrush(Color.FromArgb(255,40,40,40));
            }

            else
            {
                return new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}