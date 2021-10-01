using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;

namespace ProducerAlpha
{
    public class ExistsTextToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (!String.IsNullOrEmpty(value.ToString()))
                {
                    return true;
                }
            }

            return false;
        }



        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TypeConverter objTypeConverter = TypeDescriptor.GetConverter(targetType);
            object objReturnValue = null;

            if (objTypeConverter.CanConvertFrom(value.GetType()))
            {
                objReturnValue = objTypeConverter.ConvertFrom(value);
            }

            return objReturnValue;
        }

        #endregion
    }
}
