/*
    Blob trigger, read single (image) file, and sends it to MS Cognative services to perform OCR
*/

using System;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;

namespace ImageUploadClassLib
{
    class ImageOcr
    {
        // Simple POC class to save image text and Uri (to blob) to Azure Table Store  
        public class ImageText : TableEntity
        {
            public string Text { get; set; }
            public string Uri { get; set; }
        }

        //  ICloudBlob as input to the image
        //  ICollector<ImageText> to add data entry to Azure Table Store
        public static async Task Run(ICloudBlob inputBlob, IAsyncCollector<ImageText> outputTable, TraceWriter log)
        {
            try
            {
                using (Stream imageFileStream = new MemoryStream())
                {
                    // read the image stream to memory 
                    inputBlob.DownloadToStream(imageFileStream);
                    log.Info($"stream length = {imageFileStream.Length}"); // just to verify

                    var visionClient = new VisionServiceClient(Environment.GetEnvironmentVariable("OCR_KEY"));

                    // reset stream position to begining 
                    imageFileStream.Position = 0;
                    // Use MS Cognative Services to perform OCR
                    var ocrResult = await visionClient.RecognizeTextAsync(imageFileStream, "en");

                    // parse the results 
                    string OCRText = LogOcrResults(ocrResult);
                    log.Info($"image text = {OCRText}");

                    // add results to Azure Table store
                    await outputTable.AddAsync(new ImageText()
                    {
                        PartitionKey = "TryFunctions",
                        RowKey = inputBlob.Name,
                        Text = OCRText,
                        Uri = inputBlob.Uri.ToString()
                    });
                }

            }
            catch (Exception e)
            {
                log.Info(e.Message);
            }
        }

        // helper function to parse OCR results 
        static string LogOcrResults(OcrResults results)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (results != null && results.Regions != null)
            {
                stringBuilder.Append(" ");
                stringBuilder.AppendLine();
                foreach (var item in results.Regions)
                {
                    foreach (var line in item.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            stringBuilder.Append(word.Text);
                            stringBuilder.Append(" ");
                        }
                        stringBuilder.AppendLine();
                    }
                    stringBuilder.AppendLine();
                }
            }
            return stringBuilder.ToString();
        }
    }
}