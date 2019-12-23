using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SelfPage_TestWebAPI.Attribute;
using System.Linq;
using System.Text;

namespace SelfPage_TestWebAPI.Filter
{
    public class DenyAnonymousFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                //有验证，且通过，（只对Authoriz才有这种情况，且token填写正确）
            }
            else
            {
                //无验证，（两种情况：①AllowAnon ②Authoriz，但token错误），这两种情况通过Action的CustomAttribute区分
                //②
                if (((System.Collections.ObjectModel.ReadOnlyCollection<System.Reflection.CustomAttributeData>)((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.CustomAttributes).Any(item => item.AttributeType == typeof(DenyAnonymousAttribute)))
                {
                    //context.HttpContext.Response.Body.Write(Encoding.UTF8.GetBytes("token错误/未填写token"));
                    //context.Result = new StatusCodeResult(401);
                    if (string.IsNullOrWhiteSpace(context.HttpContext.Request.Headers["Authorization"]))
                    {
                        context.Result = new JsonResult("未填写token");
                    }
                    else
                    {
                        context.Result = new JsonResult("token解析错误");
                    }
                }
            }

            return;
        }
    }
}
