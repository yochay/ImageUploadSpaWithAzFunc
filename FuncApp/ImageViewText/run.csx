/*
    Simple GET REST API to retrieve images and the text output
*/

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

// IQueryable return list of image text objects
// CloudBlobContainer used to generate SAS token to allow secure access to image file
public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, IQueryable<ImageText> inputTable, CloudBlobContainer inputContainer,  TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

    //get container sas token
    var st = GetContainerSasToken(inputContainer);
    //log.Info($"token --> {st}");
    var result = new List<SimpleImageText>();

    //TODO: handle paging 
    var query = from ImageText in inputTable select ImageText;
    //log.Info($"original query --> {JsonConvert.SerializeObject(query)}");

    foreach (ImageText imageText in query)
    {
        result.Add( new SimpleImageText(){Text = imageText.Text, Uri = imageText.Uri + st});
        //log.Info($"{JsonConvert.SerializeObject()}");
    }

    return req.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(result));
}

// used to get rows from table
public class ImageText : TableEntity
{
    public string Text { get; set; }
    public string Uri {get; set; }
}

public class SimpleImageText
{
    public string Text { get; set; }
    public string Uri {get; set; }
}

// generate 24 hour SAS token for the container. Will allow read for all images
// TBD -  shoudl be done once every 24 hours via timer, rather than each time in the funciton 
static string GetContainerSasToken(CloudBlobContainer container)
{
    //Set the expiry time and permissions for the container.
    //In this case no start time is specified, so the shared access signature becomes valid immediately.
    SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
    sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24);
    sasConstraints.Permissions = SharedAccessBlobPermissions.Read;

    //Generate the shared access signature on the container, setting the constraints directly on the signature.
    string sasContainerToken = container.GetSharedAccessSignature(sasConstraints);

    //Return the URI string for the container, including the SAS token.
    return sasContainerToken;
}