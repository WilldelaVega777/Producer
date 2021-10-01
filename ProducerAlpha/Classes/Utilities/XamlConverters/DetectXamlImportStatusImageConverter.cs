using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;

namespace ProducerAlpha
{
    public class DetectXamlImportStatusImageConverter : IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String sRetVal = @"\Assets\Icons\XamlLinks\Error.png";
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
                    sRetVal = @"\Assets\Icons\XamlLinks\Warning.png";
                    return sRetVal;
                }

                // Start Analysis (Use WayvFS for IO Operations..

                // File refered in the XamlInfo Path does not exist in Disk
                if (!WayvFS.FindXamlFile(IXI.XamlPath))
                {
                    sRetVal = @"\Assets\Icons\XamlLinks\Warning.png";
                }

                // Date in XamlInfo is different from date in File
                if (!WayvFS.CompareXamlFileDatesTimes(IXI.DateOfLastModification, IXI.XamlPath))
                {
                    sRetVal = @"\Assets\Icons\XamlLinks\Warning.png";
                }
                else
                {
                    // Date in XamlInfo is the same as the date in File
                    sRetVal = @"\Assets\Icons\XamlLinks\Info.png";
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