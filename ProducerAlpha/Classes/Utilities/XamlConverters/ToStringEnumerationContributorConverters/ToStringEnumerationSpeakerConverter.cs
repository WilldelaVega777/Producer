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
    public class ToStringEnumerationSpeakerConverter : IValueConverter
    {
        //------------------------------------------------------------------------------------------------------------------
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String sRetVal = "";

            if (!(value is ObservableCollection<Speaker>))
            {
                throw new ArgumentException("Parameter must be an ObservableCollection of Speaker");
            }

            if ((value == null) || ((string)parameter == ""))
            {
                return "";
            }

            foreach (object o in (value as ObservableCollection<Speaker>))
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
            ObservableCollection<Speaker> ocpRetVal = new ObservableCollection<Speaker>();

            if (!(value is string))
            {
                throw new ArgumentException("Parameter must be a Comma Separated String");
            }

            if (value == null)
            {
                return "";
            }

            string[] aSpeakers = (value as string).Split(',');

            foreach (string s in aSpeakers)
            {
                Speaker OSpeaker = new Speaker();
                OSpeaker.Name = s;
                ocpRetVal.Add(OSpeaker);
            }

            return ocpRetVal;

        }
    }
}
