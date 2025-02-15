using Circular.Framework.Logger;
using Newtonsoft.Json;
using System.Text;

namespace Circular.Middleware
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
            if (context != null && context.Response != null && context.Response.Body != null)
            {
                using (Stream originalBodyStream = context.Response.Body)
                {
                    string RequestId = Guid.NewGuid().ToString();
                    //Create a new memory stream and use it for the temp reponse body
                    //Copy  pointer to the original response body stream
                    var responseBody = new MemoryStream();

                    Action act;
                    Task task;


                    if (context.Request != null && context.Request.ContentType != null && context.Request.ContentType.ToLower().StartsWith("multipart/form-data"))
                    {
                        act = new Action(async () =>
                        {
                            _logger.LogInfo($"{{ReqId:\"{RequestId}\", Message:File received to upload\"}}");
                        });
                        task = new Task(act);
                        task.Start();
                        await _next(context);
                    }
                    else
                    {
                        var request = await GetRequestAsTextAsync(context.Request);
                        act = new Action(async () =>
                        {
                            _logger.LogInfo($"{{ReqId:\"{RequestId}\", Message:\"{request}\"}}");
                        });



                        task = new Task(act);
                        task.Start();



                        context.Response.Body = responseBody;


                        //Continue down the Middleware pipeline
                        await _next(context);

                        //Format the response from the server
                        var response = await GetResponseAsTextAsync(context.Response, RequestId);

                        responseBody = new MemoryStream(Encoding.UTF8.GetBytes(response.Item2));

                        //Log it
                        act = new Action(async () =>
                        {
                            _logger.LogInfo($"{{ReqId:\"{RequestId}\", Message:\"{response.Item1}\"}}");
                        });
                        task = new Task(act);
                        task.Start();

                        //Copy the contents of the new memory stream, which contains the response to the original stream, which is then returned to the client.
                        await responseBody.CopyToAsync(originalBodyStream);

                        responseBody.FlushAsync();
                        responseBody.DisposeAsync();
                        originalBodyStream.FlushAsync();
                        originalBodyStream.DisposeAsync();
                        context.Request.Body.DisposeAsync();
                    }

                }
            }
        }
        //private async Task HandleExceptionAsync(HttpResponse responseHTTP, Exception exception, string requestId, MemoryStream responseBody, Stream originalBodyStream)
        //{
        //    responseHTTP.ContentType = "application/json";

        //    Debug.Print("Status Code:" + responseHTTP.StatusCode.ToString());

        //    _logger.LogError($"{{ReqId:\"{requestId}\", Message:\"{JsonConvert.SerializeObject(exception)}\"}}");

        //    APIResponse errorResponse = new APIResponse();
        //    errorResponse.requestId = requestId;

        //    switch (exception)
        //    {
        //        case ApplicationException ex:
        //            if (ex.Message.Contains("Invalid Token"))
        //            {
        //                responseHTTP.StatusCode = (int)HttpStatusCode.Forbidden;
        //                errorResponse.statusCode = (int)APIResponseCode.Failure;
        //                errorResponse.message = ex.Message;
        //                break;
        //            }
        //            responseHTTP.StatusCode = (int)HttpStatusCode.BadRequest;
        //            errorResponse.statusCode = (int)APIResponseCode.Failure;
        //            errorResponse.message = ex.Message;
        //            break;
        //        default:
        //            responseHTTP.StatusCode = (int)HttpStatusCode.OK;
        //            errorResponse.statusCode = (int)APIResponseCode.Failure;
        //            errorResponse.message = exception.StackTrace;
        //            break;
        //    }

        //    var result = JsonConvert.SerializeObject(errorResponse);
        //    responseBody = new MemoryStream(Encoding.UTF8.GetBytes(result));
        //    await responseBody.CopyToAsync(originalBodyStream);


        //}


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

        private async Task<Tuple<string, string>> GetResponseAsTextAsync(HttpResponse response, string RequestId)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            //Create stream reader to write entire stream
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            var obj = JsonConvert.DeserializeObject(text);

            //APIResponse apiRes = new APIResponse()
            //{
            //    requestId = RequestId,
            //    statusCode = (int)APIResponseCode.Success,
            //    data = obj
            //};

            JsonSerializerSettings sett = new JsonSerializerSettings();
            sett.Formatting = Formatting.Indented;
            sett.CheckAdditionalContent = true;
            return new Tuple<string, string>(text, JsonConvert.SerializeObject(obj, Formatting.Indented, sett));
        }
    }
}
