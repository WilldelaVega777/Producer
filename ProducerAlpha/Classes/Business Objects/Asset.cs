using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProducerAlpha
{
    public class Asset
    {
        //-------------------------------------------------------------------------------------------------
        // Private Fields Section
        //-------------------------------------------------------------------------------------------------
        private int frameId;
        private string fileId;
        private enumAssetTypes description;
        private string fileName;
        private string subPath;

        //-------------------------------------------------------------------------------------------------
        // Public Properties Section
        //-------------------------------------------------------------------------------------------------
        public int FrameID
        {
            get
            {
                return this.frameId;
            }
            set
            {
                this.frameId = value;
            }
        }

        //-------------------------------------------------------------------------------------------------
        public string FileID
        {
            get
            {
                return this.fileId;
            }
            set
            {
                this.fileId = value;
            }
        }

        //-------------------------------------------------------------------------------------------------
        public enumAssetTypes Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }



        //-------------------------------------------------------------------------------------------------
        public string FileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                this.fileName = value;
            }
        }

        //-------------------------------------------------------------------------------------------------
        public string SubPath
        {
            get
            {
                return this.subPath;
            }
            set
            {
                this.subPath = value;
            }
        }

        //-------------------------------------------------------------------------------------------------
        public string Type
        {
            get
            {
                return this.fileName.Substring((this.fileName.IndexOf(".") + 1), 3).ToUpper();
            }
        }

        //-------------------------------------------------------------------------------------------------
        public string FrameIDTitle
        {
            get
            {
                return Convert.ToString(this.frameId);
            }
            set
            {
                this.frameId = Int32.Parse(value);
            }
        }

        //-------------------------------------------------------------------------------------------------
        public string UNCSubPath
        {
            get
            {
                return this.subPath.Replace('/','\\');
            }
        }

        //-------------------------------------------------------------------------------------------------
        // Constructor Methods Section
        //-------------------------------------------------------------------------------------------------
        public Asset()
        { }

        //-------------------------------------------------------------------------------------------------
        public Asset(int pFrameID, string pFileID, enumAssetTypes pDescription, string pFileName, string pSubPath)
        {
            this.frameId        = pFrameID;
            this.fileId         = pFileID;
            this.description    = pDescription;
            this.fileName       = pFileName;
            this.subPath        = pSubPath;
        }


    }
}
