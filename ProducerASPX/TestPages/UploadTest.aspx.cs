using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WayvSL2ASPX.DirectoryServer;

namespace WayvSL2ASPX.TestPages
{
    public partial class UploadTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void OSubmitButton_Click(object sender, EventArgs e)
        {
            String sReturn = "";

            byte[] FileInBytes = this.OFileUpload.FileBytes;
            
            DirectoryServer.DirectorySuscriptions ODirectoryServer = 
                new DirectoryServer.DirectorySuscriptions();

            try
            {
            DirectoryServer.WebShowMetadata OMetadata = new DirectoryServer.WebShowMetadata();

            OMetadata.ShowId = 10;
            OMetadata.Version = 1;
            OMetadata.ShowTitle = "The singing bee";
            OMetadata.Description = "The funky fox jumped over the green grass of happiness";
            OMetadata.ChannelId = 1;

               sReturn = ODirectoryServer.SubmitShow(OMetadata, FileInBytes);
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message + " - " + sReturn;
            }
        }
    }
}
