using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FunctionApp.Examples;
using FunctionApp.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace FunctionApp
{
    public static class BinaryDataHttpTrigger
    {
        [FunctionName(nameof(BinaryDataHttpTrigger.RunByteArray))]
        [OpenApiOperation(operationId: "run.bytearray", tags: new[] { "bytearray" }, Summary = "Transfer image in the binary format", Description = "This transfers an image in the binary format.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "text/plain", bodyType: typeof(byte[]), Example = typeof(ByteArrayExample), Required = true, Description = "Image data")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "image/png", bodyType: typeof(byte[]), Summary = "Image data", Description = "This returns the image", Deprecated = false)]
        public static async Task<IActionResult> RunByteArray(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "bytearray")] HttpRequest req,
            ILogger log)
        {
            var payload = default(string);
            using (var reader = new StreamReader(req.Body))
            {
                payload = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            var content = Convert.FromBase64String(payload);

            var result = new FileContentResult(content, "image/png");

            return result;
        }

        [FunctionName(nameof(BinaryDataHttpTrigger.RunMultiPartFormData))]
        [OpenApiOperation(operationId: "run.multipart.formdata", tags: new[] { "multipartformdata" }, Summary = "Transfer image through multipart/formdata", Description = "This transfers an image through multipart/formdata.", Visibility = OpenApiVisibilityType.Advanced)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "multipart/form-data", bodyType: typeof(MultiPartFormDataModel), Required = true, Description = "Image data")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "image/png", bodyType: typeof(byte[]), Summary = "Image data", Description = "This returns the image", Deprecated = false)]
        public static async Task<IActionResult> RunMultiPartFormData(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "form/multipart")] HttpRequest req,
            ILogger log)
        {
            var files = req.Form.Files;
            var file = files[0];

            var content = default(byte[]);
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms).ConfigureAwait(false);
                content = ms.ToArray();
            }

            var result = new FileContentResult(content, "image/png");

            return result;
        }
   }
}