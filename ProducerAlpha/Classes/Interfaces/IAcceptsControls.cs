using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProducerAlpha
{
    public interface IAcceptsControls
    {
        System.Collections.ObjectModel.ObservableCollection<ProducerAlpha.Control> Controls
        {
            get;
        }
    }
}
