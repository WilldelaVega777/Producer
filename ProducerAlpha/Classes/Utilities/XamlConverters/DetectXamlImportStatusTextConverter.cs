using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;

namespace ProducerAlpha
{
    public class DetectXamlImportStatusTextConverter : IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String sRetVal = "Error: Internal object corruption detected";
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
                    sRetVal = "Information: This Frame contains Xaml but a Xaml File Link was not established";
                    return sRetVal;
                }

                // Start Analysis (Use WayvFS for IO Operations..

                // File refered in the XamlInfo Path does not exist in Disk
                if (!WayvFS.FindXamlFile(IXI.XamlPath))
                {
                    sRetVal = "Warning: Link appears to be broken";
                }

                // Date in XamlInfo is different from date in File
                if (!WayvFS.CompareXamlFileDatesTimes(IXI.DateOfLastModification, IXI.XamlPath))
                {
                    sRetVal = "Warning: File is not current, please Update the Link";
                }
                else
                {
                    // Date in XamlInfo is the same as the date in File
                    sRetVal = "Information: The Xaml File Link to this Frame exists and appears to be Valid";
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