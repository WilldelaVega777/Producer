using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProducerAlpha
{
    [Serializable]
    public enum enumAssetTypes
    {
        NONE,
        NVIDEO,
        CHANNEL_LOGO,
        DEFAULT_THUMBNAIL_PATH,
        FRAME_BACKGROUND,
        ASSET_FOR_XAML_ANIMATION,
        UNDEFINED
    }
}
