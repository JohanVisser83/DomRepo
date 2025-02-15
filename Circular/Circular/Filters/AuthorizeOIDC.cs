using Circular.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Circular.Filters
{
    public class AuthorizeOIDC : Attribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                if (context != null)
                {
                    var header = AuthenticationHeaderValue.Parse(context.HttpContext.Request.Headers["Authorization"]);
                    var token = header.Parameter;
                    using var client = new HttpClient();
                    using var request = new HttpRequestMessage(HttpMethod.Get, Static.AppSettings.ChallengeUrl);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    using var response = client.Send(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        context.Result = new UnauthorizedResult();
                    }
                    else if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Task<string> json = response.Content.ReadAsStringAsync();
                        APIResponse oResponse = JsonConvert.DeserializeObject<APIResponse>(json.Result);
                        dynamic data = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(oResponse.Data));
                        context.HttpContext.Items["UserId"] = data.id;

                        context.HttpContext.Items["UserTimeZone"] = data.timezone;
                        if (context.HttpContext.Items["UserTimeZone"] == "")
                            context.HttpContext.Items["UserTimeZone"] = "South Africa Standard Time";

                    }
                }
            }
            catch (Exception ex)
            {
                context.Result = new UnauthorizedResult();
            }
           
        }
    }
}
