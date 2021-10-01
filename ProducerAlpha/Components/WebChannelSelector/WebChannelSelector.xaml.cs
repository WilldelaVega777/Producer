using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;

namespace ProducerAlpha
{
    public partial class WebChannelSelector : UserControl
    {

        //---------------------------------------------------------------------------------
        // Private Fields Section
        //---------------------------------------------------------------------------------
        ObservableCollection<ProducerAlpha.WebChannelInfo> WebChannelData;

        //---------------------------------------------------------------------------------
        // Routed Events Section
        //---------------------------------------------------------------------------------
        public static readonly RoutedEvent OKButtonClickEvent = EventManager.RegisterRoutedEvent(
            "OKButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WebChannelSelector));

        //---------------------------------------------------------------------------------        
        public static readonly RoutedEvent CancelButtonClickEvent = EventManager.RegisterRoutedEvent(
            "CancelButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WebChannelSelector));


        //---------------------------------------------------------------------------------
        // Routed Methods Section
        //---------------------------------------------------------------------------------
        public event RoutedEventHandler OKButtonClick
        {
            add { AddHandler(OKButtonClickEvent, value); }
            remove { RemoveHandler(OKButtonClickEvent, value); }
        }

        //---------------------------------------------------------------------------------
        public event RoutedEventHandler CancelButtonClick
        {
            add { AddHandler(CancelButtonClickEvent, value); }
            remove { RemoveHandler(CancelButtonClickEvent, value); }
        }

        //---------------------------------------------------------------------------------
        // Public Properties Section
        //--------------------------------------------------------------------------------- 
        public ProducerAlpha.WebChannelInfo SelectedChannel
        {
            get
            {
                return (this.lvSelectWebChannel.SelectedItem as WebChannelInfo);
            }
        }

        //---------------------------------------------------------------------------------
        // Constructor Method Section
        //---------------------------------------------------------------------------------        
        public WebChannelSelector()
        {
            InitializeComponent();

            // Initialize Instance Data
            this.WebChannelData = new ObservableCollection<ProducerAlpha.WebChannelInfo>();

            // Instance Events
            this.Loaded += new RoutedEventHandler(WebChannelSelector_Loaded);
        }

        //---------------------------------------------------------------------------------
        // Event Handler Methods Section
        //---------------------------------------------------------------------------------
        void WebChannelSelector_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryServer.DirectorySuscriptions ODirectoryServer =
                new ProducerAlpha.DirectoryServer.DirectorySuscriptions();

            DirectoryServer.WebChannelInfo[] aChannelInfo = ODirectoryServer.ListWebChannels();

            
            for (int iCounter = 0; iCounter < aChannelInfo.Length; iCounter++)
            {
                this.WebChannelData.Add(new ProducerAlpha.WebChannelInfo(
                       aChannelInfo[iCounter].ChannelId,
                       aChannelInfo[iCounter].ChannelName,
                       aChannelInfo[iCounter].ChannelDescription,
                       this.BitmapToBitmapSource(this.byteArrayToBitmap(aChannelInfo[iCounter].ChannelImage)),
                       this.byteArrayToBitmap(aChannelInfo[iCounter].ChannelLogo)
                    )
                );
            }

            this.lvSelectWebChannel.ItemsSource = this.WebChannelData;
            this.lvSelectWebChannel.SelectedItem = null;

        }

        //---------------------------------------------------------------------------------
        private void cmdOk_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OKButtonClickEvent));
        }

        //---------------------------------------------------------------------------------        
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CancelButtonClickEvent));
        }

        //---------------------------------------------------------------------------------
        // Private Methods Section
        //---------------------------------------------------------------------------------
        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }

        //---------------------------------------------------------------------------------
        public System.Drawing.Bitmap byteArrayToBitmap(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image OImage = System.Drawing.Image.FromStream(ms);
            System.Drawing.Bitmap returnBitmap = new System.Drawing.Bitmap(OImage);
            return returnBitmap;
        }

        //---------------------------------------------------------------------------------
        public BitmapSource BitmapToBitmapSource(Bitmap BitmapIn)
        {
            BitmapSource returnValue;
            IntPtr hBitmap = BitmapIn.GetHbitmap();
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            returnValue = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            return returnValue;
        }


    }
}
