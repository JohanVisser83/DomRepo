using Circular.Framework.Logger;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CircularWeb.Middleware
{
    /// <summary>
    /// Middleware for Logging Request and Responses.
    /// Remarks: Original code taken from https://exceptionnotfound.net/using-middleware-to-log-requests-and-responses-in-asp-net-core/
    /// </summary>
    public class HttpRequestResponseMiddleware
    {
        private readonly ILoggerManager _logger;
        private readonly RequestDelegate _next;

        public HttpRequestResponseMiddleware(RequestDelegate next, ILoggerManager logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request != null && context.Request.ContentType != null && !context.Request.ContentType.ToLower().StartsWith("multipart/form-data"))
            {
                if (context != null && context.Response != null && context.Response.Body != null)
                {
                    string RequestId = Guid.NewGuid().ToString();

                    var responseBody = new MemoryStream();
                    Action act;
                    Task task;
                    var request = await GetRequestAsTextAsync(context.Request);
                    act = new Action(async () =>
                    {
                        _logger.LogInfo($"{{ReqId:\"{RequestId}\", Message:\"{request}\"}}");
                    });

                    task = new Task(act);
                    task.Start();
                    await _next(context);

                }
            }
            else
            {
                await _next(context);
            }
        }
         


        private async Task<string> GetRequestAsTextAsync(HttpRequest request)
        {
            //Set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();

            //Read request stream
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            //Copy into  buffer.
            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            //Convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            //Assign the read body back to the request body
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(bodyAsText));

            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        
    }
}
