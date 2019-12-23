using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SelfPage_Service.PageInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SelfPage_Service.Service
{
    public static class SelfPageService
    {
        public static SelfPageInfo pageInfo = null;
        //分组策略
        public static event Func<string, ControllerInfo, bool> groupPolicy = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="action">配置信息</param>
        /// <param name="_groupPolicy">分组策略 (groupName,controllerInfo)=>{return true;}</param>
        public static void UseSelfPage(this IApplicationBuilder app, Action<SelfPageInfo> action = null, Func<string, ControllerInfo, bool> _groupPolicy = null)
        {
            try
            {
                pageInfo = new SelfPageInfo();
                action?.Invoke(pageInfo);
                if (pageInfo.Groups == null)
                {
                    pageInfo.Groups = new List<string> { "default" };
                }
                if (!pageInfo.Groups[0].Equals("default")) { pageInfo.Groups.Insert(0, "default"); }
                groupPolicy = _groupPolicy;

                GetEnableControllerTypes(out List<Type> controllerTypes);
                GetGroupsPageInfo(controllerTypes, out List<ResPageInfo> groupsPageInfo);
            }
            catch (Exception ex)
            {
                app.Run(async (context) =>
                {
                    context.Response.ContentType = "text/html; charset=UTF-8";
                    await context.Response.WriteAsync($"使用SelfPage,遇到一个问题。{ex.Message}");
                });
            }
        }

        /// <summary>
        /// 根据分组提取所有控制器和Action的信息
        /// </summary>
        /// <param name="controllerTypes"></param>
        /// <param name="list"></param>
        private static void GetGroupsPageInfo(List<Type> controllerTypes, out List<ResPageInfo> groupsPageInfo)
        {
            groupsPageInfo = new List<ResPageInfo>();
            List<ControllerInfo> controllerInfos = new List<ControllerInfo>();
            foreach (Type item in controllerTypes)
            {
                if (item.IsDefined(typeof(RouteAttribute)))
                {
                    ControllerInfo controller = new ControllerInfo();
                    List<ActionInfo> actionInfos = new List<ActionInfo>();
                    controller.ControllerName = item.Name;
                    controller.Discribetion = ""; //控制器的描述信息 todo待扩展
                    string controllerRoute = item.GetCustomAttribute<RouteAttribute>().Template;
                    controller.ControllerRoute = controllerRoute;
                    var methods = item.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var method in methods)
                    {
                        ActionInfo action = new ActionInfo();
                        action.DescribeTion = ""; //Action的描述信息 todo待扩展
                        //请求参数信息 todo待扩展 fromquey/frombody...
                        if (method.IsDefined(typeof(HttpGetAttribute)))
                        {
                            string actionRoute = method.GetCustomAttribute<HttpGetAttribute>().Template;
                            action.RequestType = RequestType.HttpGet;
                            action.RequestPath = $"/{controllerRoute}/{actionRoute}";
                            actionInfos.Add(action);
                        }
                        else if (method.IsDefined(typeof(HttpPostAttribute)))
                        {
                            string actionRoute = method.GetCustomAttribute<HttpPostAttribute>().Template;
                            action.RequestType = RequestType.HttpPost;
                            action.RequestPath = $"/{controllerRoute}/{actionRoute}";
                            actionInfos.Add(action);
                        }
                    }
                    controller.Actions = actionInfos;
                    controllerInfos.Add(controller);
                }
            }
            //根据分组策略配置不同分组
            foreach (string group in pageInfo.Groups)
            {
                if (groupPolicy != null)
                {
                    groupsPageInfo.Add(new ResPageInfo
                    {
                        GroupName = group,
                        Controllers = (from item in controllerInfos
                                       where groupPolicy(@group, item)
                                       select item)?.ToList()
                    });
                }
                else
                {
                    groupsPageInfo.Add(new ResPageInfo
                    {
                        GroupName = "default",
                        Controllers = controllerInfos
                    });
                    break;
                }
            }
        }

        /// <summary>
        /// 获取所有用于显示的Controller-Type
        /// </summary>
        /// <returns></returns>
        private static void GetEnableControllerTypes(out List<Type> controllerTypes)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string dllName = $"{AppDomain.CurrentDomain.FriendlyName}.dll";
            Assembly assembly = Assembly.LoadFrom($"{appPath}/{dllName}");
            var types = assembly.ExportedTypes;
            controllerTypes = new List<Type>();
            foreach (Type item in types)
            {
                List<Type> baseTypes = new List<Type>();
                if (item.IsClass)
                {
                    Type baseType = item.BaseType;
                    while (baseType != typeof(object))
                    {
                        baseTypes.Add(baseType);
                        baseType = baseType.BaseType;
                    }
                    if (baseTypes.Contains(typeof(ControllerBase)))
                    {
                        if (item.IsDefined(typeof(ApiControllerAttribute)))
                        {
                            controllerTypes.Add(item);
                        }
                    }
                }
            }
        }


    }
}
