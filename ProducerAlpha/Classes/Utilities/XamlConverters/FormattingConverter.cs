using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;

namespace ProducerAlpha
{
    class FormattingConverter: IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                string strFormatString = parameter.ToString();

                if (!String.IsNullOrEmpty(strFormatString))
                {
                    return String.Format(culture, strFormatString, value);
                }
            }

            return value.ToString();
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
