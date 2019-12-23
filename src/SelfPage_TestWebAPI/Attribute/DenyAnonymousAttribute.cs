using Microsoft.AspNetCore.Authorization;
using System;

namespace SelfPage_TestWebAPI.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class DenyAnonymousAttribute : AuthorizeAttribute
    {

    }
}
