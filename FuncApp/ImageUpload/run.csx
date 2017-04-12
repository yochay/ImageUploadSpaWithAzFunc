/*
    multiple images upload 
    upload directly through the Function (API) to blob 
    optimal way is to upload multiple iamges directly to Blob using a Function to get a SAS token and then the client will updload via SAS
*/

#r "Microsoft.WindowsAzure.Storage"

using System.Net;
using Microsoft.WindowsAzure.Storage.Blob;

//
//Using CloudBlobContainer to upload multiple images vs. single blob
public static HttpResponseMessage Run(HttpRequestMessage req, CloudBlobContainer  inputContainer, TraceWriter log) 
{ 
    System.Diagnostics.Debugger.Launch();
    log.Info($"Webhook was triggered!");  

    HttpResponseMessage result = null; 
    
    if (req.Content.IsMimeMultipartContent()) 
    {
            // memory stream of the incomping request 
            var streamProvider = new MultipartMemoryStreamProvider (); 

            log.Info($" ***\t reading input data into stream...");
            req.Content.ReadAsMultipartAsync(streamProvider); 
            log.Info($" ***\t after await on ReadMultpart...");
            
            foreach (HttpContent ctnt in streamProvider.Contents)
            {
                // You would get hold of the inner memory stream here
                Stream stream = ctnt.ReadAsStreamAsync().Result;
                log.Info($"stream length = {stream.Length}"); // just to verify
                
                //random GUID to 'overcome' name colisions 
                var blob = inputContainer.GetBlockBlobReference(Guid.NewGuid().ToString()+".jpg");

                log.Info($"uploading {blob.Name}");
                blob.UploadFromStream(stream);
            }

            result = req.CreateResponse(HttpStatusCode.OK, $"Successfull uploaded images ");             
        }
        else
        {
            log.Info($" ***\t ERROR!!! bad format request ");
            result = req.CreateResponse(HttpStatusCode.NotAcceptable,"This request is not properly formatted");
        }

    return result;
}