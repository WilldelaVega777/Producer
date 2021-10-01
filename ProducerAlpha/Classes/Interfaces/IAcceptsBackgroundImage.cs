using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProducerAlpha
{
    public interface IAcceptsBackgroundImage
    {
        String BackgroundImagePath
        {
            get;
            set;
        }

        String InactiveBackgroundImagePath
        {
            get;
            set;
        }

    }
}