using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Collections.ObjectModel;

namespace ProducerAlpha
{
    public class HasXamlLoadedToBooleanConverter : IValueConverter
    {
        //------------------------------------------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Boolean bRetVal = true;

            if (value is String)
            {
                if ((value == null) || ((value as String) == String.Empty))
                {
                    bRetVal = false;
                }
                else
                {
                    if ((value as String) == null)
                    {
                        bRetVal = false;
                    }
                }

            }
            else
            {
                bRetVal = false;
            }

            return bRetVal;

        }

        //------------------------------------------------------------------------------------------------------------------
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}