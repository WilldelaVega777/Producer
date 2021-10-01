using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace ProducerAlpha
{
    public class XamlLinkCreateLinkButtonVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility sRetVal = Visibility.Hidden;
            if (value != null)
            {
                ImportedXamlInfo IXI = (value as ImportedXamlInfo);

                // PreConditions Validation
                if (IXI == null)
                {
                    return sRetVal;
                }

                // No Xaml Info
                if (IXI.XamlPath == "Empty")
                {
                    sRetVal = Visibility.Visible;
                    return sRetVal;
                }

                // File refered in the XamlInfo Path does not exist in Disk
                if (!WayvFS.FindXamlFile(IXI.XamlPath))
                {
                    sRetVal = Visibility.Hidden;
                }

                // Date in XamlInfo is different from date in File
                if (!WayvFS.CompareXamlFileDatesTimes(IXI.DateOfLastModification, IXI.XamlPath))
                {
                    sRetVal = Visibility.Hidden;
                }
                else
                {
                    // Date in XamlInfo is the same as the date in File
                    sRetVal = Visibility.Hidden;
                }


            }

            return sRetVal;
        }



        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}