# Simple SPA with Azure Functions

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fyochay%2FImageUploadSpaWithAzFunc%2Fmaster%2Fazuredeploy.json" target="_blank">![Deploy to Azure](http://azuredeploy.net/deploybutton.png)</a>

## About this demo
* this demo is based of on [Get some hands-on time with Serverless development right now, for free](https://blogs.msdn.microsoft.com/appserviceteam/2016/10/04/get-some-hands-on-time-with-serverless-development-right-now-for-free/) blog.
* This demo includes a Functions exposing few APIs to upload images to blob, run OCR on the images, and get list of images.
* This demo is .NET (C#) based. The repo includes two versions of the same demo. A C# scripting version (edit code in the Azure Functions Portal) and a pre-compiled assembly version, which you can run locally (on Windows machine) using Visual Studio and deploy to the cloud. The ImageUploadClassLib folder includes a pre-compiled .net version of the demo.
* Function uses Azure Cognative Services (vision) to perform the OCR
* This demo includes static HTML (SPA) that can be used with the Functions API to upload and view images. Static HTML can be run locally (for testing) and can also be hosted on Azure blob.
* Using Azure Function proxies, you can create a simple URL for the static HTML page (note, initially, you will need to override the proxy settings)
* You can findseveral test images in Sample-Images.

## Running the demo

### Demo Setup for C# Scripting 

1. Fork the repo into your own GitHub

2. Ensure that you've authorized at least one Azure Web App on your subscription to connect to your GitHub account. To learn more, see [Continuous Deployment to Azure App Service](https://azure.microsoft.com/en-us/documentation/articles/app-service-continuous-deployment/).

3. Click the Deploy to Azure button above. 
  
  * Enter a sitename **with no dashes**. 
  
  * A storage account will be created with the same site name (and Storage does not allow dashes in account names).
  
  * Enter the OCR key from the Cognitive Services [Computer Vision API](https://www.microsoft.com/cognitive-services/en-us/https://www.microsoft.com/cognitive-services/en-us/computer-vision-api)

  * Enter a your repo URL (the one from step #1)

4. In [Azure Storage Explorer](http://storageexplorer.com/), navigate to the storage account with the same name as your Function App.
   
   * Create the blob container `outcontainer`. Uploaded images will be sotred in this container.

### Demo Testing
1. Open HTML page (test-try-functions2.html)
* You will see empty text box, to which you will need to copy the specific URL of your Functions
* Copy ImageUpload Function's URL from the Azure Functions Portal to the ImageUpload Function URL text box
* Copy ImageVewText Function's URL from the Azure Functions Portal to the ImageView Text Function URL text box

2. upload an image with some text on it by draging one of the sample images from the SampleImages folder in the repo
3. press the Get Images button. You should see the a small thumbprint of the image and the text the OCR extracted


### notes
Deploying from Source Control (like this git repo), locks the portal for any editing expereince. To reenable portal edting 
* Open the Function App you just deployed. Go to Function App settings -> Configure Continuous Integration. In the command bar, select **Disconnect**.
* Close and reopen the Function App. Verify that you can edit code in CardGenerator -> Develop.