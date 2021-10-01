using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Documents;
using System.Windows;


namespace ProducerAlpha
{
    public class FlowDocumentConverter
    {
        //-------------------------------------------------------------------------------------------------------        
        // Public Static Methods Section
        //-------------------------------------------------------------------------------------------------------                
        public static string ConvertFlowDocumentToText(FlowDocument pFlowDocument)
        {
            string sRetVal = "";

            TextRange sourceDocument = new TextRange(pFlowDocument.ContentStart, pFlowDocument.ContentEnd);
            using (MemoryStream stream = new MemoryStream())
            {
                sourceDocument.Save(stream, DataFormats.Rtf);

                stream.Seek(0, SeekOrigin.Begin);

                using (StreamReader reader = new StreamReader(stream))
                {
                    sRetVal = reader.ReadToEnd();
                }
            }

            return sRetVal;
        }

        //-------------------------------------------------------------------------------------------------------        
        internal static void AssignXamlToFlowDocument(ref FlowDocument FDoc, string sXaml)
        {
            if (string.IsNullOrEmpty(sXaml)) 
            {
                return;
            }



            TextRange textRange = new TextRange(FDoc.ContentStart, FDoc.ContentEnd);

            //Create a MemoryStream of the Rtf content
            using (MemoryStream XamlMemoryStream = new MemoryStream())
            {
                using (StreamWriter XamlStreamWriter = new StreamWriter(XamlMemoryStream))
                {
                    XamlStreamWriter.Write(sXaml);
                    XamlStreamWriter.Flush();
                    XamlMemoryStream.Seek(0, SeekOrigin.Begin);

                    //Load the MemoryStream into TextRange ranging from start to end of RichTextBox.
                    textRange.Load(XamlMemoryStream, DataFormats.Rtf);
                }
            }
        }
    }
}
