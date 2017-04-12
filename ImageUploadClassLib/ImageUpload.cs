/*
    multiple images upload 
    upload directly through the Function (API) to blob 
    optimal way is to upload multiple iamges directly to Blob using a Function to get a SAS token and then the client will updload via SAS
*/

using System.Net;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.WebJobs.Host;

namespace ImageUploadClassLib
{
    class ImageUpload
    {
        //Using CloudBlobContainer to upload multiple images vs. single blob
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, CloudBlobContainer inputContainer, TraceWriter log)
        {
            log.Info($"ImageUploadClassLib was triggered!");

            HttpResponseMessage result = null;

            if (req.Content.IsMimeMultipartContent())
            {
                // memory stream of the incomping request 
                var streamProvider = new MultipartMemoryStreamProvider();

                log.Info($" ***\t reading input data into stream...");
                await req.Content.ReadAsMultipartAsync(streamProvider);
                log.Info($" ***\t after await on ReadMultpart...");

                foreach (var content in streamProvider.Contents)
                {
                    // You would get hold of the inner memory stream here
                    Stream stream = await content.ReadAsStreamAsync();
                    log.Info($"stream length = {stream.Length}"); // just to verify

                    //random GUID to 'overcome' name colisions 
                    var blob = inputContainer.GetBlockBlobReference(Guid.NewGuid().ToString() + ".jpg");

                    log.Info($"uploading {blob.Name}");
                    blob.UploadFromStream(stream);
                }

                result = req.CreateResponse(HttpStatusCode.OK, $"Successfull uploaded images ");
            }
            else
            {
                log.Info($" ***\t ERROR!!! bad format request ");
                result = req.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted");
            }

            return result;
        }
    }
}