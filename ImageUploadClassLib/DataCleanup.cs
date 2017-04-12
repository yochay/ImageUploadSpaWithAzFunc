using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;

namespace ImageUploadClassLib
{
    class DataCleanup
    {
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, CloudBlobContainer inputContainer,
                                                          CloudTable inputTable, TraceWriter log)
        {
            //delete all blobs in the container
            log.Info($" Deleting all blobs (images)");
            Parallel.ForEach(inputContainer.ListBlobs(), x => ((CloudBlob)x).Delete());

            //delete table is the fastet way to delete all data
            log.Info($" Deleting image text table ");
            await inputTable.DeleteAsync();

            return req.CreateResponse(HttpStatusCode.OK, "Data cleanup complete!");
        }
    } 
}