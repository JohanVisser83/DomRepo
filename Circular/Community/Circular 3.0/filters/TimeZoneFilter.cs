using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenIddict.Abstractions;
using System.Collections;
using System.Security.Claims;
using TimeZoneConverter;

namespace CircularWeb.filters
{
    public class TimeZoneFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Do something before the action executes.
            var ActionInfo = context.ActionDescriptor;
            var pars = ActionInfo.Parameters;

			//var UserTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Static.AppSettings.DefaultUserTimeZone);
			//var ServerTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Static.AppSettings.ServerTimeZone);
			var UserTimeZone = TZConvert.GetTimeZoneInfo(Static.AppSettings.DefaultUserTimeZone);
			var ServerTimeZone = TZConvert.GetTimeZoneInfo(Static.AppSettings.ServerTimeZone);

			HttpContextAccessor access = new HttpContextAccessor();
            string timeZone = access.HttpContext.User.HasClaim(ClaimTypes.Thumbprint) ? access.HttpContext.User.FindFirst(ClaimTypes.Thumbprint).Value : null ;
            if (!string.IsNullOrEmpty(timeZone))
            {
				// UserTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
				UserTimeZone = TZConvert.GetTimeZoneInfo(timeZone);
			}

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            if (identity != null)
            {
                UserTimeZone = identity.Claims.Where(c => c.Type == "Thumbprint")
					  .Select(c => TZConvert.GetTimeZoneInfo(c.Value)).SingleOrDefault();
				// .Select(c => TimeZoneInfo.FindSystemTimeZoneById(c.Value)).SingleOrDefault();

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
                if (result.GetType() == typeof(ViewResult) || result.GetType() == typeof(PartialViewResult) || result.GetType() == typeof(JsonResult))
                {
					//var ServerTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Static.AppSettings.ServerTimeZone);
					//var UserTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Static.AppSettings.DefaultUserTimeZone);
					var UserTimeZone = TZConvert.GetTimeZoneInfo(Static.AppSettings.DefaultUserTimeZone);
					var ServerTimeZone = TZConvert.GetTimeZoneInfo(Static.AppSettings.ServerTimeZone);

					//var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
					//if (identity != null)
					//{
					//    ServerTimeZone = identity.Claims.Where(c => c.Type == "Thumbprint")
					//                   .Select(c => TimeZoneInfo.FindSystemTimeZoneById(c.Value)).SingleOrDefault();
					//}

					HttpContextAccessor access = new HttpContextAccessor();
                    string timeZone = access.HttpContext.User.HasClaim(ClaimTypes.Thumbprint) ? access.HttpContext.User.FindFirst(ClaimTypes.Thumbprint).Value : null;
                    if (!string.IsNullOrEmpty(timeZone))
                    {
						// UserTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
						UserTimeZone = TZConvert.GetTimeZoneInfo(timeZone);

					}
					dynamic viewResult =  null;
                    if (result.GetType() == typeof(ViewResult))
                    {
                        viewResult = (ViewResult)result;
                        if (viewResult != null)
                            changeClassDateTime(viewResult.Model, ServerTimeZone, UserTimeZone);
                    }
                    else if (result.GetType() == typeof(PartialViewResult))
                    {
                        viewResult = (PartialViewResult)result;
                        if (viewResult != null)
                            changeClassDateTime(viewResult.Model, ServerTimeZone, UserTimeZone);
                    }
                    else if (result.GetType() == typeof(JsonResult))
                    {
                        viewResult = (JsonResult)result;
                        if (viewResult != null && viewResult != null)
                            changeClassDateTime(viewResult.Value, ServerTimeZone, UserTimeZone);
                    }

                }
            }
        }


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

                    if (properties != null && properties.Count > 0)
                    {
                        foreach (var property in properties)
                        {
                            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                            {
                                if (property.GetSetMethod() != null)
                                {
                                    var DateTimeValue = DateTime.SpecifyKind(Convert.ToDateTime(property.GetValue(obj)), DateTimeKind.Unspecified);
                                    property.SetValue(obj, TimeZoneInfo.ConvertTime(DateTimeValue, sourceTimeZone, destinationTimeZone));
                                    //property.SetValue(obj, TimeZoneInfo.ConvertTime(Convert.ToDateTime(property.GetValue(obj)), sourceTimeZone, destinationTimeZone));
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

                            || (p.GetValue(obj) != null && p.GetValue(obj).GetType().FullName.StartsWith("Circular"))

                          ).ToList();

                    if (EnumerableProperties != null && EnumerableProperties.Count > 0)
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
