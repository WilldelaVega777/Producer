using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProducerAlpha
{
    public interface IUsesAssets
    {
        bool XamlContent
        {
            get;
            set;
        }

        System.Collections.ObjectModel.ObservableCollection<ProducerAlpha.Asset> Assets
        {
            get;
        }
    }
}
