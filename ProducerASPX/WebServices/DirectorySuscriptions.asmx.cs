using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using WayvASPX;
using System.IO;
using System.Diagnostics;
using Ionic;
using Ionic.Zip;
using Ionic.Zlib;


namespace WayvSL2ASPX.WebServices
{
    [WebService(Namespace = "http://wayv.tv/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class DirectorySuscriptions : System.Web.Services.WebService
    {
        //--------------------------------------------------------------------------------------
        // Public WebMethods Section
        //--------------------------------------------------------------------------------------
        [WebMethod(MessageName = "ListWebChannels")]
        public WebChannelInfo[] ListWebChannels()
        {
            SqlConnection OConn = this.getConnection();

            SqlCommand OComm = new SqlCommand();
            OComm.CommandType = CommandType.StoredProcedure;
            OComm.CommandText = "ListWebChannels";
            OComm.Connection = OConn;

            SqlDataAdapter ODA = new SqlDataAdapter(OComm);
            DataSet ODS = new DataSet();
            ODA.Fill(ODS);

            WebChannelInfo[] aReturnValue = new WebChannelInfo[ODS.Tables[0].Rows.Count];

            int iCounter = 0;
            foreach (DataRow dr in ODS.Tables[0].Rows)
            {
                aReturnValue[iCounter] = new WebChannelInfo(
                    Convert.ToInt32(
                        dr.ItemArray[0].ToString()
                    ),
                    
                    dr.ItemArray[1].ToString(),
                    dr.ItemArray[2].ToString(),

                    this.GetImage(dr.ItemArray[3].ToString()),
                    this.GetImage(dr.ItemArray[4].ToString())
                    
                    );

                iCounter++;
            }

            return aReturnValue;
        }

        //--------------------------------------------------------------------------------------
        [WebMethod(MessageName = "GetNewShowID", EnableSession = true)]
        public int GetNewShowID()
        {
            if (Session["SessionID"] == null)
            {
                Session["SessionID"] = Context.Session.SessionID;
            }

            int iRetVal = 0;

            SqlConnection OConn = this.getConnection();

            SqlCommand OComm = new SqlCommand();
            OComm.CommandType = CommandType.StoredProcedure;
            OComm.CommandText = "GetNewShowID";
            OComm.Connection = OConn;

            SqlParameter ORetVal = new SqlParameter();
            ORetVal.ParameterName = "@return_value";
            ORetVal.SqlDbType = SqlDbType.BigInt;
            ORetVal.Direction = ParameterDirection.ReturnValue;


            SqlParameter OSessionID = new SqlParameter("@SessionID", Session["SessionID"]);
            OComm.Parameters.Add(ORetVal);
            OComm.Parameters.Add(OSessionID);

            try
            {
                OConn.Open();
                OComm.ExecuteNonQuery();
                iRetVal = ((int)OComm.Parameters["@return_value"].Value); 
            }
            catch (SqlException sEx)
            {

            }
            finally
            {
                OConn.Close();
            }

            return iRetVal;
        }

        //--------------------------------------------------------------------------------------
        [WebMethod(MessageName = "GetNewVersionID", EnableSession = true)]
        public int GetNewVersionID(int pShowId)
        {
            if (Session["SessionID"] == null)
            {
                Session["SessionID"] = Context.Session.SessionID;
            }

            int iRetVal = 0;

            SqlConnection OConn = this.getConnection();

            SqlCommand OComm = new SqlCommand();
            OComm.CommandType = CommandType.StoredProcedure;
            OComm.CommandText = "GetNewVersionID";
            OComm.Connection = OConn;

            SqlParameter ORetVal = new SqlParameter();
            ORetVal.ParameterName = "@return_value";
            ORetVal.SqlDbType = SqlDbType.BigInt;
            ORetVal.Direction = ParameterDirection.ReturnValue;

            SqlParameter OSessionID = new SqlParameter("@SessionID", Session["SessionID"]);

            SqlParameter OShowID = new SqlParameter("@ShowID", pShowId);

            OComm.Parameters.Add(ORetVal);
            OComm.Parameters.Add(OSessionID);
            OComm.Parameters.Add(OShowID);

            try
            {
                OConn.Open();
                OComm.ExecuteNonQuery();
                iRetVal = ((int)OComm.Parameters["@return_value"].Value);
            }
            catch (SqlException sEx)
            {

            }
            finally
            {
                OConn.Close();
            }

            return iRetVal;
        }


        //--------------------------------------------------------------------------------------
        [WebMethod(MessageName = "GetSessionID", EnableSession = true)]
        public string GetSessionID()
        {
            return Session["SessionID"].ToString();
        }


        //--------------------------------------------------------------------------------------
        [WebMethod(MessageName = "SubmitShow", EnableSession = true)]
        public string SubmitShow(WebShowMetadata pMetadata, byte[] pZipPackage)
        {
            //----------------------------------------------------------------------------------
            // Default Return Value
            string sRetVal = "No Session Available";

            //----------------------------------------------------------------------------------
            // Local Variables Declaration
            int iRetVal = 0;
            SqlConnection OConn = this.getConnection();
            SqlCommand OComm;
            SqlParameter ORetVal;
            SqlParameter OSessionID;
            SqlParameter OShowID;
            SqlParameter OShowVersion;
            SqlParameter OShowTitle;
            SqlParameter OShowDescription;
            SqlParameter OShowChannelID;
            SqlParameter OShowManifestPath;


            //----------------------------------------------------------------------------------
            // If there is no Session that is very bad..
            if (Session["SessionID"] == null)
            {
                return sRetVal;
            }

            //----------------------------------------------------------------------------------
            // Retrieve Package / Save File in Temp Folder
            String TempShowPath = Server.MapPath(@"\Temp\" + pMetadata.ShowId.ToString().PadLeft(10, '0'));
            System.IO.Directory.CreateDirectory(TempShowPath);
            String sFilePath = TempShowPath + @"\" + "ShowPackage.zip";

            FileStream aFile = new FileStream(sFilePath, FileMode.Create);

            try
            {
                aFile.Write(pZipPackage, 0, pZipPackage.Length);
            }
            catch (IOException ex)
            {
                sRetVal = ex.Message;
                return sRetVal;
            }
            finally
            {
                aFile.Close();
            }

            //----------------------------------------------------------------------------------
            // Determine if we have a new show or a new version

            OComm = new SqlCommand();
            OComm.CommandType = CommandType.StoredProcedure;
            OComm.CommandText = "DetectSubmissionType";
            OComm.Connection = OConn;

            ORetVal = new SqlParameter();
            ORetVal.ParameterName = "@return_value";
            ORetVal.SqlDbType = SqlDbType.BigInt;
            ORetVal.Direction = ParameterDirection.ReturnValue;

            OSessionID = new SqlParameter("@SessionID", Session["SessionID"]);
            OShowID = new SqlParameter("@ShowID", pMetadata.ShowId);

            OComm.Parameters.Add(ORetVal);
            OComm.Parameters.Add(OSessionID);
            OComm.Parameters.Add(OShowID);

            try
            {
                OConn.Open();
                OComm.ExecuteNonQuery();
                iRetVal = ((int)OComm.Parameters["@return_value"].Value);
            }
            catch (SqlException sEx)
            {
                sRetVal = sEx.Message;
                return sRetVal;
            }
            finally
            {
                OComm.Parameters.Clear();
                OConn.Close();
            }


            //----------------------------------------------------------------------------------
            // If we have a Show:
            if (iRetVal == 0)
            {
                this.UnzipPackage(pMetadata.ShowId, sFilePath);

                // Suscribe to Directory, Insert WebShow and Deletes ReservedShowIDs Tables (Blocks)
                int iRetValFromProc = 0;
                string sManifest = "/WebShows/" + pMetadata.ShowId.ToString().PadLeft(10, '0') + "/Manifest.xml";

                OComm.CommandType = CommandType.StoredProcedure;
                OComm.CommandText = "SubmitShow";
                OComm.Connection = OConn;

                ORetVal = new SqlParameter();
                ORetVal.ParameterName = "@return_value";
                ORetVal.SqlDbType = SqlDbType.BigInt;
                ORetVal.Direction = ParameterDirection.ReturnValue;


                // Stored Procedure Parameters
                OSessionID = new SqlParameter("@SessionID", Session["SessionID"]);
                OShowID = new SqlParameter("@ShowID", pMetadata.ShowId);
                OShowVersion = new SqlParameter("@Version", pMetadata.Version);
                OShowTitle = new SqlParameter("@ShowTitle", pMetadata.ShowTitle);
                OShowDescription = new SqlParameter("@Description", pMetadata.Description);
                OShowChannelID = new SqlParameter("@ChannelID", pMetadata.ChannelId);
                OShowManifestPath = new SqlParameter("@ManifestPath", sManifest);

                OComm.Parameters.Add(ORetVal);
                OComm.Parameters.Add(OSessionID);
                OComm.Parameters.Add(OShowID);
                OComm.Parameters.Add(OShowVersion);
                OComm.Parameters.Add(OShowTitle);
                OComm.Parameters.Add(OShowDescription);
                OComm.Parameters.Add(OShowChannelID);
                OComm.Parameters.Add(OShowManifestPath);

                try
                {
                    OConn.Open();
                    OComm.ExecuteNonQuery();
                    iRetValFromProc = ((int)OComm.Parameters["@return_value"].Value);
                }
                catch (SqlException sEx)
                {
                    sRetVal = sEx.Message;
                    return sRetVal;
                }
                finally
                {
                    OComm.Parameters.Clear();
                    OConn.Close();
                }

            }

            //----------------------------------------------------------------------------------
            // If we have a Version:
            if (iRetVal == 1)
            {
                String sSuccess = "";
                sSuccess = this.BackupExistingVersion(pMetadata.ShowId, pMetadata.Version);
                if (sSuccess != "Success")
                {
                    sRetVal = sSuccess;
                    return sRetVal;
                }

                sSuccess = this.UnzipPackage(pMetadata.ShowId, sFilePath);
                if (sSuccess != "Success")
                {
                    sRetVal = sSuccess;
                    return sRetVal;
                }

                // Updates Version in Directory, Update WebShow and Deletes ReservedShowIDs Tables (Blocks)
                int iRetValFromProc = 0;

                OComm.CommandType = CommandType.StoredProcedure;
                OComm.CommandText = "SubmitVersion";
                OComm.Connection = OConn;

                ORetVal = new SqlParameter();
                ORetVal.ParameterName = "@return_value";
                ORetVal.SqlDbType = SqlDbType.BigInt;
                ORetVal.Direction = ParameterDirection.ReturnValue;

                // Stored Procedure Parameters
                OSessionID = new SqlParameter("@SessionID", Session["SessionID"]);
                OShowID = new SqlParameter("@ShowID", pMetadata.ShowId);
                OShowVersion = new SqlParameter("@Version", pMetadata.Version);

                OComm.Parameters.Add(ORetVal);
                OComm.Parameters.Add(OSessionID);
                OComm.Parameters.Add(OShowID);
                OComm.Parameters.Add(OShowVersion);

                try
                {
                    OConn.Open();
                    OComm.ExecuteNonQuery();
                    iRetValFromProc = ((int)OComm.Parameters["@return_value"].Value);
                }
                catch (SqlException sEx)
                {
                    sRetVal = sEx.Message;
                    return sRetVal;
                }
                finally
                {
                    OComm.Parameters.Clear();
                    OConn.Close();
                }
            }
           

            // Deletes Temp Show Folder
            try
            {
                System.IO.Directory.Delete(TempShowPath, true);
            }
            catch (Exception ex)
            {
                sRetVal = ex.Message;
                return sRetVal;
            }

            // Return Value
            sRetVal = "Success";
            return sRetVal;
        }

        //--------------------------------------------------------------------------------------
        [WebMethod(MessageName = "UnZipTest")]
        public string UnZipTest()
        {
            // Return Value
            string sRetVal = "";

            // Unzip Package
            try
            {

                // Unzip to Production Folder
                String pFilePath = Server.MapPath(@"\Temp\ShowPackage.zip");
                String ProductionFolder = Server.MapPath(@"\WebShows\" + 14.ToString().PadLeft(10, '0'));
                System.IO.Directory.CreateDirectory(ProductionFolder);

                using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(pFilePath))
                {
                    foreach (Ionic.Zip.ZipEntry e in zip)
                    {
                        e.Extract(ProductionFolder, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            catch (Exception ex)
            {
                sRetVal = ex.Message;
                return sRetVal;
            }

            sRetVal = "Success";
            return sRetVal;
        }

        //--------------------------------------------------------------------------------------
        // Private Methods Section
        //--------------------------------------------------------------------------------------
        private string UnzipPackage(int pShowId, string pFilePath)
        {
            // Return Value
            string sRetVal = "";

            // Unzip Package
            try
            {
                // Unzip Package / Check Structure
                sRetVal = this.VerifyPackageContents(pShowId);
                if (sRetVal != "Success")
                {
                    return sRetVal;
                }

                // If Structure Ok, Unzip to Production Folder
                String ProductionFolder = Server.MapPath(@"\WebShows\" + pShowId.ToString().PadLeft(10, '0'));
                System.IO.Directory.CreateDirectory(ProductionFolder);

                using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(pFilePath))
                {
                    foreach (Ionic.Zip.ZipEntry e in zip)
                    {
                        e.Extract(ProductionFolder, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            catch (Exception ex)
            {
                sRetVal = ex.Message;
                return sRetVal;
            }

            sRetVal = "Success";
            return sRetVal;
        }

        //--------------------------------------------------------------------------------------
        private string BackupExistingVersion(int pShowId, int pVersionId)
        {
            string sReturnValue = "";

            String SourceDirectory = Server.MapPath(@"\WebShows\" + pShowId.ToString().PadLeft(10, '0'));
            String DestinationFile = Server.MapPath(
                    @"\ShowVersions\" + pShowId.ToString().PadLeft(10, '0') + @"\" +
                    "Show_" + pShowId.ToString().PadLeft(10, '0') + "_" +
                    (pVersionId-1).ToString() + ".zip"
            );


            try
            {
                // Create Folder
                if (!System.IO.Directory.Exists(Server.MapPath(@"\ShowVersions\" + pShowId.ToString().PadLeft(10, '0'))))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(@"\ShowVersions\" + pShowId.ToString().PadLeft(10, '0')));
                }

                // Create Zip Package
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddDirectory(SourceDirectory, "/");
                    zip.Save(DestinationFile);
                }

                // Deletes Production Folder
                System.IO.Directory.Delete(SourceDirectory,true);
            }
            catch (Exception ex)
            {
                sReturnValue = ex.Message;
                return sReturnValue;
            }

            sReturnValue = "Success";
            return sReturnValue;
        }


        //--------------------------------------------------------------------------------------
        // Private Methods Section (Utility Methods)
        //--------------------------------------------------------------------------------------        
        private SqlConnection getConnection()
        {
            String sConn = ConfigurationManager.ConnectionStrings["OConnMain"].ConnectionString;
            SqlConnection OConn = new SqlConnection(sConn);
            return OConn;
        }

        //--------------------------------------------------------------------------------------
        public Byte[] GetImage(string strImagePath)
        {
            FileStream fs = new FileStream(Server.MapPath(strImagePath),
                    FileMode.OpenOrCreate, FileAccess.Read);
            Byte[] img = new Byte[fs.Length];
            try
            {
                fs.Read(img, 0, Convert.ToInt32(fs.Length));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
            finally
            {
                fs.Close();
            }
            return img;
        }

        //--------------------------------------------------------------------------------------
        private string VerifyPackageContents(int pShowID)
        {
 	        return "Success";
        }

    }
}
