using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SelfPage_Service.PageInfo;
using SelfPage_Service.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SelfPage_Service.Service
{
    public static class SelfPageService
    {
        public static SelfPageInfo pageInfo = null;
        //分组策略
        public static event Func<string, ControllerInfo, bool> groupPolicy = null;

        /// <summary>
        /// 使用SelfPage服务
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
                if (groupsPageInfo == null || groupsPageInfo.Count == 0)
                {
                    AdvanceHttpReturn(app, "未找到符合要求的控制器和Action，请检查代码！");
                }

                if (groupsPageInfo[0].GroupName.Equals("default") && groupsPageInfo[0].Controllers.Count == 0)
                {
                    groupsPageInfo.RemoveAt(0);
                }
                foreach (ResPageInfo resPageInfo in groupsPageInfo)
                {
                    app.MapWhen(
                        httpContext =>
                        {
                            return httpContext.Request.Path.Value.Equals($"{pageInfo.EndPointPath}/{resPageInfo.GroupName}") ||
                                   httpContext.Request.Path.Value.Equals($"{pageInfo.EndPointPath}/") ||
                                   httpContext.Request.Path.Value.Equals($"{pageInfo.EndPointPath}");
                        }
                        , appBuilder =>
                        {
                            appBuilder.Run(async context =>
                            {
                                string resStr = $"{resPageInfo.GroupName}"; //todo待完善
                                await ResponseStr(context.Response, resStr);
                            });
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                AdvanceHttpReturn(app, $"使用SelfPage,遇到一个问题。<br/>{ex.Message}");
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

            XmlDocInfo xmlInfo = GetXmlInfo();
            foreach (Type item in controllerTypes)
            {
                if (item.IsDefined(typeof(RouteAttribute)))
                {
                    ControllerInfo controller = new ControllerInfo();
                    List<ActionInfo> actionInfos = new List<ActionInfo>();
                    controller.ControllerName = item.Name;
                    controller.Discribetion = GetActionDisCribeTionFromXmlInfo(item.Name, null, xmlInfo, true);
                    string controllerRoute = item.GetCustomAttribute<RouteAttribute>().Template;
                    controller.ControllerRoute = controllerRoute;
                    var methods = item.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var method in methods)
                    {
                        //请求参数信息 todo待扩展 fromquey/frombody...
                        if (method.IsDefined(typeof(HttpGetAttribute)))
                        {
                            ActionInfo action = new ActionInfo();
                            action.DescribeTion = GetActionDisCribeTionFromXmlInfo(item.Name, method.Name, xmlInfo);
                            string actionRoute = method.GetCustomAttribute<HttpGetAttribute>().Template;
                            action.RequestType = RequestType.HttpGet;
                            action.RequestPath = $"/{controllerRoute}/{actionRoute}";
                            actionInfos.Add(action);
                        }
                        else if (method.IsDefined(typeof(HttpPostAttribute)))
                        {
                            ActionInfo action = new ActionInfo();
                            action.DescribeTion = GetActionDisCribeTionFromXmlInfo(item.Name, method.Name, xmlInfo);
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
        /// 一次性加载控制器和Action注释。| 需要项目生成xml文档在bin\Debug\netcoreapp2.2，提供注释信息。若无则不提供。
        /// </summary>
        /// <returns></returns>
        private static XmlDocInfo GetXmlInfo()
        {
            if (pageInfo.IfUseXmlInfo)
            {
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string xmlName = $"{AppDomain.CurrentDomain.FriendlyName}.xml";
                XElement xe = XElement.Load($"{appPath}/{xmlName}");
                if (xe == null) { return null; }
                XmlDocInfo xmlDocInfo = new XmlDocInfo();
                xmlDocInfo.Members = (from member in xe.Element("members").Elements("member")
                                      where member.Attribute("name").Value.Contains("Controller")
                                      select new Xml.MemberInfo
                                      {
                                          MemberName = member.Attribute("name").Value.Trim(),
                                          Summary = member.Element("summary").Value.Trim()
                                      })?.ToList();
                if (xmlDocInfo.Members == null || xmlDocInfo.Members.Count == 0)
                {
                    return null;
                }
                return xmlDocInfo;
            }
            return null;
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

        /// <summary>
        /// 提前结束请求
        /// </summary>
        /// <param name="app"></param>
        private static void AdvanceHttpReturn(IApplicationBuilder app, string str)
        {
            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html; charset=UTF-8";
                await context.Response.WriteAsync(str);
            });
        }

        /// <summary>
        /// 返回文档信息
        /// </summary>
        /// <param name="res"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private static async Task ResponseStr(HttpResponse res, string str)
        {
            res.ContentType = "text/html; charset=utf-8";
            await res.WriteAsync(str);
        }

        /// <summary>
        /// 从内存查询注释信息
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="xmlDocInfo"></param>
        /// <returns></returns>
        private static string GetActionDisCribeTionFromXmlInfo(string controllerName, string actionName, XmlDocInfo xmlInfo, bool isController = false)
        {
            if (xmlInfo != null)
            {
                try
                {
                    if (isController)
                    {
                        return (from member in xmlInfo.Members
                                where member?.MemberName?.EndsWith(controllerName) ?? false
                                select member?.Summary)?.First();
                    }
                    else
                    {
                        return (from member in xmlInfo.Members
                                where member?.MemberName?.Contains($"{controllerName}.{actionName}") ?? false
                                select member?.Summary)?.First();
                    }
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }


    }
}
