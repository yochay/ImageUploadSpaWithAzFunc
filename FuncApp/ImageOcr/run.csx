/*
    Blob trigger, read single (image) file, and sends it to MS Cognative services to perform OCR
*/

#r "System.IO"
#r "System.Runtime"
#r "System.Threading.Tasks"
#r "Microsoft.WindowsAzure.Storage"

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.WindowsAzure.Storage.Table;


// Simple POC class to save image text and Uri (to blob) to Azure Table Store  
public class ImageText : TableEntity
{
    public string Text { get; set; }
    public string Uri {get; set; }
} 

//  ICloudBlob as input to the image
//  ICollector<ImageText> to add data entry to Azure Table Store
public static void Run( ICloudBlob inputBlob, ICollector<ImageText> outputTable, TraceWriter log) 
{

    try  
    {
        using (Stream imageFileStream = new MemoryStream())
        {

            // read the image stream to memory 
            inputBlob.DownloadToStream(imageFileStream); 
            log.Info($"stream length = {imageFileStream.Length}"); // just to verify

            // Init MS Cognative Services
            //var visionClient = new VisionServiceClient("cac68b20bbfa40c4bd7860a3f639c201");
            //var visionClient = new VisionServiceClient("d22e09b5466e4db39568dd02cab3e71c");
            var visionClient = new VisionServiceClient(System.Environment.GetEnvironmentVariable("OCR_KRY") );
            

            // reset stream position to begining 
            imageFileStream.Position = 0;
            // Use MS Cognative Services to perform OCR
            var ocrResult = visionClient.RecognizeTextAsync(imageFileStream, "en");
            
            // parse the results 
            string OCRText = LogOcrResults(ocrResult.Result);
            log.Info($"image text = {OCRText}");

            // add results to Azure Table store
            outputTable.Add(new ImageText()
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
        // TBD - better error handling 
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