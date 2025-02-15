using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Circular.Core.DTOs;
using System.Security.Claims;
using TimeZoneConverter;

namespace Circular.Filters
{
    public class TimeZoneFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var ActionInfo = context.ActionDescriptor;
            var pars = ActionInfo.Parameters;
			//var UserTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Static.AppSettings.DefaultUserTimeZone);
			//var ServerTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Static.AppSettings.ServerTimeZone);

			var UserTimeZone = TZConvert.GetTimeZoneInfo(Static.AppSettings.DefaultUserTimeZone);
			var ServerTimeZone = TZConvert.GetTimeZoneInfo(Static.AppSettings.ServerTimeZone);



			//var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
			//if (identity != null)
			//{
			//    UserTimeZone = identity.Claims.Where(c => c.Type == "ThumbPrint")
			//                   .Select(c => TimeZoneInfo.FindSystemTimeZoneById(c.Value)).SingleOrDefault();
			//}
			if (context.HttpContext.Items != null && context.HttpContext.Items["UserTimeZone"] != null)
            {
               // UserTimeZone = TimeZoneInfo.FindSystemTimeZoneById(context.HttpContext.Items["UserTimeZone"].ToString());
				UserTimeZone = TZConvert.GetTimeZoneInfo(context.HttpContext.Items["UserTimeZone"].ToString());
			}


            foreach (var p in pars)
            {
                if (p.ParameterType.FullName.StartsWith("Circular"))
                {
                    var actionArgument = context.ActionArguments[p.Name];
                    changeClassDateTime(actionArgument, UserTimeZone, ServerTimeZone);
                }
                else if (p.ParameterType == typeof(DateTime) || p.ParameterType == typeof(DateTime?))
                {
                    TimeZoneInfo.ConvertTime(Convert.ToDateTime(context.ActionArguments[p.Name]), UserTimeZone, ServerTimeZone);
                }
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var result = context.Result;

            if (result != null)
            {
                if (result is OkObjectResult okResult)
                {
                    if (okResult.Value is APIResponse apiResponse)
                    {
                        var UserTimeZone = TZConvert.GetTimeZoneInfo(Static.AppSettings.DefaultUserTimeZone);
                        var ServerTimeZone = TZConvert.GetTimeZoneInfo(Static.AppSettings.ServerTimeZone);

                        if (context.HttpContext.Items != null && context.HttpContext.Items["UserTimeZone"] != null)
                        {
                            UserTimeZone = TZConvert.GetTimeZoneInfo(context.HttpContext.Items["UserTimeZone"].ToString());
                        }

                        // Perform the datetime adjustment on the APIResponse
                        changeClassDateTime(apiResponse, ServerTimeZone, UserTimeZone);
                    }
                    else if (okResult.Value is List<string> stringList)
                    {
                        
                    }
                }
            }
        }

        //public void OnActionExecuted(ActionExecutedContext context)
        //{
        //    var result = context.Result;
        //    if (result != null)
        //    {
        //        if (result.GetType() == typeof(OkObjectResult))
        //        {

        //            //  var ServerTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Static.AppSettings.ServerTimeZone);
        //            //  var UserTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Static.AppSettings.DefaultUserTimeZone);

        //            var UserTimeZone = TZConvert.GetTimeZoneInfo(Static.AppSettings.DefaultUserTimeZone);
        //            var ServerTimeZone = TZConvert.GetTimeZoneInfo(Static.AppSettings.ServerTimeZone);

        //            //var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        //            //if (identity != null)
        //            //{
        //            //    UserTimeZone = identity.Claims.Where(c => c.Type == "ThumbPrint")
        //            //                   .Select(c => TimeZoneInfo.FindSystemTimeZoneById(c.Value)).SingleOrDefault();
        //            //}
        //            if (context.HttpContext.Items != null && context.HttpContext.Items["UserTimeZone"] != null)
        //            {
        //                // UserTimeZone = TimeZoneInfo.FindSystemTimeZoneById(context.HttpContext.Items["UserTimeZone"].ToString());
        //                UserTimeZone = TZConvert.GetTimeZoneInfo(context.HttpContext.Items["UserTimeZone"].ToString());

        //            }

        //            var viewResult = (OkObjectResult)result;
        //            changeClassDateTime((APIResponse)viewResult.Value, ServerTimeZone, UserTimeZone);
        //        }
        //    }

        //}


        object changeClassDateTime(object obj, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
        {
            if (obj != null)
            {
                var enumerableResult = obj as IEnumerable;

                if (enumerableResult != null)
                {
                    foreach (var item in enumerableResult)
                    {
                        changeClassDateTime(item, sourceTimeZone, destinationTimeZone);
                    }
                }

                else
                {
                    var properties = obj.GetType().GetProperties().Where(p =>
                                p.PropertyType == typeof(DateTime) ||
                                p.PropertyType == typeof(DateTime?)
                            )
                            .ToList();

                    if (properties != null)
                    {
                        foreach (var property in properties)
                        {
                            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                            {
                                if (property.GetSetMethod() != null)
                                {
                                    var DateTimeValue = DateTime.SpecifyKind(Convert.ToDateTime(property.GetValue(obj)), DateTimeKind.Unspecified);
                                    property.SetValue(obj, TimeZoneInfo.ConvertTime(DateTimeValue, sourceTimeZone, destinationTimeZone));
                                }
                            }
                            else if (property.GetType().FullName.StartsWith("Circular"))
                            {
                                changeClassDateTime(property.GetValue(obj), sourceTimeZone, destinationTimeZone);
                            }
                        }
                    }
                    var EnumerableProperties = obj.GetType().GetProperties()
                    .Where(p =>
                             (p.PropertyType != typeof(string) && p.GetValue(obj) != null &&
                            (p.GetValue(obj) as ICollection) != null && (p.GetValue(obj) as ICollection).Count > 0)
                            || (p.GetValue(obj) != null && (p.GetValue(obj).GetType().FullName.StartsWith("Circular")))
                          ).ToList();

                    if (EnumerableProperties != null)
                    {
                        foreach (var property in EnumerableProperties)
                        {
                            changeClassDateTime(property.GetValue(obj), sourceTimeZone, destinationTimeZone);
                        }
                    }
                }
            }
            return obj;
        }
    }
}
