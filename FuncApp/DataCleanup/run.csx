#r "System.IO"
#r "System.Runtime"
#r "System.Threading.Tasks"
#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"

using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

// 
public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, CloudBlobContainer  inputContainer, CloudTable inputTable, TraceWriter log)
{
    //log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

    //delete all blobs in the container
    log.Info($" Deleting all blobs (images)"); 
    Parallel.ForEach(inputContainer.ListBlobs(), x => ((CloudBlob) x).Delete());

    //delete table is the fastet way to delete all data
    log.Info($" Deleting image text table ");
    await inputTable.DeleteAsync();

    //create table again (empty) --> need to wait for delete to complete as it can take 40 sec 
    //await inputTable.CreateIfNotExistsAsync();

    return req.CreateResponse(HttpStatusCode.OK, "Data cleanup complete!");
}