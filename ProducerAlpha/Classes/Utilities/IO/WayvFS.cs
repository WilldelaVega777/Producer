using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace ProducerAlpha
{
    class WayvFS
    {
        //-------------------------------------------------------------------------------------------
        // Public Static Methods Section
        //-------------------------------------------------------------------------------------------
        public static bool CheckIfProjectExists(string sPath)
        {
            bool bRetVal = false;

            if (Directory.Exists(Path.GetDirectoryName(sPath)))
            {
                bRetVal = true;
            }

            return bRetVal;
        }

        //-------------------------------------------------------------------------------------------
        public static bool CheckIfProjectFileExists(string sPath)
        {
            Boolean bRetVal = false;

            DirectoryInfo d = new DirectoryInfo(System.IO.Path.GetDirectoryName(sPath));

            if (d.Exists)
            {
                foreach (FileInfo f in d.GetFiles())
                {
                    if (f.Extension == ".wayvproj")
                    {
                        bRetVal = true;
                    }
                }
            }

            return bRetVal;
        }

        //-------------------------------------------------------------------------------------------
        public static void CreateEmptyProject(string sPath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(sPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(sPath));
                Directory.CreateDirectory(Path.GetDirectoryName(sPath) + @"\" + "Resources");
                Directory.CreateDirectory(Path.GetDirectoryName(sPath) + @"\" + "VideoOutput");
                Directory.CreateDirectory(Path.GetDirectoryName(sPath) + @"\" + "XamlErrLogs");
                Directory.CreateDirectory(Path.GetDirectoryName(sPath) + @"\" + "Temp");
            }

            if (!File.Exists(sPath))
            {
                File.Create(sPath);
            }

        }

        //-------------------------------------------------------------------------------------------
        public static void CreateReleaseFolders(string sPath)
        {
            // Vars
            String ReleaseFolder        = Path.GetDirectoryName(sPath) + @"\Release\";
            String WebSlidesPath        = Path.GetDirectoryName(sPath) + @"\Release\WebSlides\";
            String ResourcesPath        = Path.GetDirectoryName(sPath) + @"\Release\Resources\";
            String BaseThumbnailsPath   = Path.GetDirectoryName(sPath)   + @"\Release\Resources\Thumbnails\";
            String ColorThumbnailsPath  = Path.GetDirectoryName(sPath) + @"\Release\Resources\Thumbnails\Color\";
            String BWThumbnailsPath     = Path.GetDirectoryName(sPath) + @"\Release\Resources\Thumbnails\BW\";
            
            // Delete Previous Contents
            if (Directory.Exists(BWThumbnailsPath))
            {
                DeleteFilesInFolder(BWThumbnailsPath);
                Directory.Delete(BWThumbnailsPath);
            }

            if (Directory.Exists(ColorThumbnailsPath))
            {
                DeleteFilesInFolder(ColorThumbnailsPath);
                Directory.Delete(ColorThumbnailsPath);
            }

            if (Directory.Exists(BaseThumbnailsPath))
            {
                DeleteFilesInFolder(BaseThumbnailsPath);
                Directory.Delete(BaseThumbnailsPath);
            }


            if (Directory.Exists(ResourcesPath))
            {
                DeleteFilesInFolder(ResourcesPath);
                Directory.Delete(ResourcesPath);
            }

            if (Directory.Exists(WebSlidesPath))
            {
                DeleteFilesInFolder(WebSlidesPath);
                Directory.Delete(WebSlidesPath);
            }

            if (Directory.Exists(ReleaseFolder))
            {
                DeleteFilesInFolder(ReleaseFolder);
                Directory.Delete(ReleaseFolder);
            }


            // Create new Folders
            if (!Directory.Exists(Path.GetDirectoryName(sPath) + @"\" + "Release"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(sPath) + @"\" + "Release");
                Directory.CreateDirectory(Path.GetDirectoryName(sPath) + @"\" + "Release" + @"\" + "WebSlides");
                Directory.CreateDirectory(Path.GetDirectoryName(sPath) + @"\" + "Release" + @"\" + "Resources");
                Directory.CreateDirectory(Path.GetDirectoryName(sPath) + @"\" + "Release" + @"\" + "Resources" + @"\" + "Thumbnails");
                Directory.CreateDirectory(Path.GetDirectoryName(sPath) + @"\" + "Release" + @"\" + "Resources" + @"\" + "Thumbnails" + @"\" + "Color");
                Directory.CreateDirectory(Path.GetDirectoryName(sPath) + @"\" + "Release" + @"\" + "Resources" + @"\" + "Thumbnails" + @"\" + "BW");
            }
        }

        //-------------------------------------------------------------------------------------------
        public static void DeleteFilesInFolder(string BWThumbnailsPath)
        {
            string[] files = Directory.GetFiles(BWThumbnailsPath);
            foreach (string file in files)
            {
                File.Delete(file);
            }            
        }

        //-------------------------------------------------------------------------------------------
        public static void SaveShow(Show pShow)
        {
            XmlSerializer xml = new XmlSerializer(typeof(Show));
            using (Stream stream = new FileStream(pShow.FullyQualifiedFileName,
                   FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xml.Serialize(stream, pShow);
            }
        }

        //-------------------------------------------------------------------------------------------
        public static Show OpenShow(string sPath)
        {
            Show shRetVal = null;

            XmlSerializer serializer = new XmlSerializer(typeof(Show));
            TextReader tr = new StreamReader(sPath);
            shRetVal = (Show)serializer.Deserialize(tr);
            tr.Close();

            return shRetVal;
        }

        //-------------------------------------------------------------------------------------------
        public static void SaveXamlError(string sErrFilePath, string sText)
        {
            StreamWriter SW = null;

            try
            {
                SW = File.CreateText(sErrFilePath);
                SW.Write(sText);
            }
            catch (System.IO.IOException ex)
            {
                /// TODO: Move OExceptionSink to App Global Object
                // App.OExceptionSink.Manage(ex);
            }
            finally
            {
                SW.Close();
            }

       
        }


        //-------------------------------------------------------------------------------------------
        // Xaml Link Verification Methods
        //-------------------------------------------------------------------------------------------
        public static bool FindXamlFile(string sXamlFilePath)
        {
            bool sRetVal = false;

            if (File.Exists(sXamlFilePath))
            {
                sRetVal = true;
            }

            return sRetVal;
        }

        //-------------------------------------------------------------------------------------------
        public static bool CompareXamlFileDatesTimes(DateTime dtLoadedTime, string sXamlFilePath)
        {
            bool sRetVal = false;

            if (DateTime.Compare(dtLoadedTime, File.GetLastWriteTime(sXamlFilePath)) == 0)
            {
                sRetVal = true;
            }

            return sRetVal;
        }

        //-------------------------------------------------------------------------------------------
        // Publish Functionality Methods
        //-------------------------------------------------------------------------------------------
        public static void CreateFrame(string sPath, int pNumber)
        {
            if (!Directory.Exists(Path.GetDirectoryName(sPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(sPath) + @"\" + "Frames" + @"\" + pNumber.ToString().PadLeft(4,'0'));
            }
        }

        //-------------------------------------------------------------------------------------------
        public static void AddResourceToFrame(string sPath, string pSourcePath, int pFrameNumber, string pResourceName)
        {
            if (!Directory.Exists(Path.GetDirectoryName(sPath) + @"\" + "Frames" + @"\" + pFrameNumber.ToString().PadLeft(4, '0')))
            {
                if (File.Exists(Path.GetDirectoryName(pSourcePath) +
                                    @"\" +
                                    "Frames" +
                                    @"\" +
                                    pFrameNumber.ToString().PadLeft(4, '0') +
                                    @"\" + pResourceName)
                    )
                {

                }


            }
            else
            {
                throw new ProducerAlphaException("Resource already exists, please delete it first");
            }
        }

        //-------------------------------------------------------------------------------------------
        // Private Methods Section
        //-------------------------------------------------------------------------------------------
        internal static string GetNextProjectName()
        {
            string sRetVal = "";
            int iCounter = 1;
            while (Directory.Exists((App.BaseProjectPath + @"\" + "Project" + iCounter.ToString())))
            {
                iCounter++;
            }

            sRetVal = "Project" + iCounter.ToString();

            return sRetVal;
        }
    }
}
