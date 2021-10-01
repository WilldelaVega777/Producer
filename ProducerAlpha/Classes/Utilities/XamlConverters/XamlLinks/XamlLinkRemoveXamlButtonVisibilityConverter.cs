using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;

namespace ProducerAlpha
{
    public class XamlLinkRemoveXamlButtonVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean sRetVal = false;

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
                    sRetVal = true;
                    return sRetVal;
                }

                // Start Analysis (Use WayvFS for IO Operations..

                // File refered in the XamlInfo Path does not exist in Disk
                if (!WayvFS.FindXamlFile(IXI.XamlPath))
                {
                    sRetVal = true;
                }

                // Date in XamlInfo is different from date in File
                if (!WayvFS.CompareXamlFileDatesTimes(IXI.DateOfLastModification, IXI.XamlPath))
                {
                    sRetVal = true;
                }
                else
                {
                    // Date in XamlInfo is the same as the date in File
                    sRetVal = true;
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