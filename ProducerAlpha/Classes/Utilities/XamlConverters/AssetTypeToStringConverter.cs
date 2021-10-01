using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;


namespace ProducerAlpha
{
    class AssetTypeToStringConverter : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String sRetVal = "No Additional Information is Available";
            if (value != null)
            {
                enumAssetTypes eat = ((enumAssetTypes)value);

                // PreConditions Validation
                if (eat == null)
                {
                    return sRetVal;
                }

                switch (eat)
                { 
                    case enumAssetTypes.NVIDEO:
                        sRetVal = "Narrative Video";
                        break;

                    case enumAssetTypes.CHANNEL_LOGO:
                        sRetVal = "Channel Logo";
                        break;

                    case enumAssetTypes.DEFAULT_THUMBNAIL_PATH:
                        sRetVal = "Default Thumbnail Path";
                        break;

                    case enumAssetTypes.FRAME_BACKGROUND:
                        sRetVal = "Frame Background";
                        break;

                    case enumAssetTypes.ASSET_FOR_XAML_ANIMATION:
                        sRetVal = "Asset for Xaml Animation";
                        break;

                }

            }

            return sRetVal;
        }



        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
