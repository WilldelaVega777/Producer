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
    [ValueConversion(typeof(IEnumerable), typeof(string))]
    public class ToStringEnumerationProducerConverter : IValueConverter
    {
        //------------------------------------------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String sRetVal = "";

            if (!(value is ObservableCollection<Producer>))
            {
                throw new ArgumentException("Parameter must be an ObservableCollection of Producer");
            }

            if ((value == null) || ((string)parameter == ""))
            {
                return "";
            }

            foreach (object o in (value as ObservableCollection<Producer>))
            {
                sRetVal += (o as Contributor).Name.Trim() + ", ";
            }

            if (sRetVal.Length > 0)
            {
                sRetVal = sRetVal.Substring(0, sRetVal.Length - 2);
            }

            return sRetVal;

        }

        //------------------------------------------------------------------------------------------------------------------
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<Producer> ocpRetVal = new ObservableCollection<Producer>();

            if (!(value is string))
            {
                throw new ArgumentException("Parameter must be a Comma Separated String");
            }

            if (value == null)
            {
                return "";
            }

            string[] aProducers = (value as string).Split(',');

            foreach (string s in aProducers)
            {
                Producer OProducer = new Producer();
                OProducer.Name = s;
                ocpRetVal.Add(OProducer);
            }

            return ocpRetVal;

        }
    }
}
