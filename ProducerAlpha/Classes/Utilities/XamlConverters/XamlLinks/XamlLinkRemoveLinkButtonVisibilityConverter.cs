using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;

namespace ProducerAlpha
{
    public class XamlLinkRemoveLinkButtonVisibilityConverter : IValueConverter
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
                    return sRetVal;
                }

                // Start Analysis (Use WayvFS for IO Operations..

                // File refered in the XamlInfo Path does not exist in Disk
                if (!WayvFS.FindXamlFile(IXI.XamlPath))
                {
                    sRetVal = Visibility.Visible;
                }

                // Date in XamlInfo is different from date in File
                if (!WayvFS.CompareXamlFileDatesTimes(IXI.DateOfLastModification, IXI.XamlPath))
                {
                    sRetVal = Visibility.Visible;
                }
                else
                {
                    // If all is ok with this link, still the user can delete the link
                    sRetVal = Visibility.Visible;                
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