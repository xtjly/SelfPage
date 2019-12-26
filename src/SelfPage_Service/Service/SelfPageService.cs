using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SelfPage_Service.PageInfo;
using SelfPage_Service.PageSrc;
using SelfPage_Service.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                var realyGroups = groupsPageInfo.GroupBy(item => item.GroupName).Select(group => group.Key).ToList();
                foreach (ResPageInfo resPageInfo in groupsPageInfo)
                {
                    app.MapWhen(
                        httpContext =>
                        {
                            return httpContext.Request.Path.Value.Equals($"{pageInfo.EndPointPath}/{resPageInfo.GroupName}", StringComparison.CurrentCultureIgnoreCase) ||
                                   httpContext.Request.Path.Value.Equals($"{pageInfo.EndPointPath}/", StringComparison.CurrentCultureIgnoreCase) ||
                                   httpContext.Request.Path.Value.Equals($"{pageInfo.EndPointPath}", StringComparison.CurrentCultureIgnoreCase);
                        }
                        , appBuilder =>
                        {
                            appBuilder.Run(async context =>
                            {
                                string resStr = HtmlInfo.GetHtmlPageInfo(resPageInfo, realyGroups);
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
                        if (method.IsDefined(typeof(HttpGetAttribute)))
                        {
                            ActionInfo action = new ActionInfo();
                            action.RequestParameters = GetMethodParameters(method);
                            action.ReturnJsonStr = GetMethodReturnObjStr(method);
                            action.DescribeTion = GetActionDisCribeTionFromXmlInfo(item.Name, method.Name, xmlInfo);
                            string actionRoute = method.GetCustomAttribute<HttpGetAttribute>().Template ?? method.GetCustomAttribute<RouteAttribute>().Template;
                            action.RequestType = RequestType.HttpGet;
                            action.RequestPath = $"/{controllerRoute}/{actionRoute}";
                            actionInfos.Add(action);
                        }
                        else if (method.IsDefined(typeof(HttpPostAttribute)))
                        {
                            ActionInfo action = new ActionInfo();
                            action.RequestParameters = GetMethodParameters(method);
                            action.ReturnJsonStr = GetMethodReturnObjStr(method);
                            action.DescribeTion = GetActionDisCribeTionFromXmlInfo(item.Name, method.Name, xmlInfo);
                            string actionRoute = method.GetCustomAttribute<HttpPostAttribute>().Template ?? method.GetCustomAttribute<RouteAttribute>().Template;
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
        /// 获取方法的返回参数信息
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static string GetMethodReturnObjStr(MethodInfo method)
        {
            var type = method.ReturnType;
            var iso = new IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss:ms";
            if (type.Name.Equals("Void", StringComparison.CurrentCultureIgnoreCase) || type.Name.Equals("Task", StringComparison.CurrentCultureIgnoreCase))
            {
                return "";
            }
            else if (type.Name.StartsWith("Task`", StringComparison.CurrentCultureIgnoreCase) ||
                type.Name.StartsWith("ActionResult`", StringComparison.CurrentCultureIgnoreCase))
            {
                var realyType = type.GenericTypeArguments[0];
                if (realyType == typeof(string))
                {
                    return "";
                }
                try
                {
                    string resJsonStr = JsonConvert.SerializeObject(Activator.CreateInstance(realyType), iso);
                    return resJsonStr;
                }
                catch
                {
                    return "";
                }
            }
            else
            {
                if (type == typeof(string))
                {
                    return "";
                }
                try
                {
                    string resJsonStr = JsonConvert.SerializeObject(Activator.CreateInstance(type), iso);
                    return resJsonStr;
                }
                catch
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 获取方法的请求参数信息
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns></returns>
        private static List<PageInfo.ParameterInfo> GetMethodParameters(MethodInfo method)
        {
            List<PageInfo.ParameterInfo> parameterInfos = new List<PageInfo.ParameterInfo>();
            var parameters = method.GetParameters();
            foreach (System.Reflection.ParameterInfo parameter in parameters)
            {
                GetPropertityInfos(parameter, parameterInfos);
            }
            return parameterInfos;
        }

        /// <summary>
        /// 找到最终的参数信息
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterInfos"></param>
        private static void GetPropertityInfos(System.Reflection.ParameterInfo parameter, List<PageInfo.ParameterInfo> parameterInfos)
        {
            if (parameter.IsDefined(typeof(FromBodyAttribute)))
            {
                GetPropertitys(parameter, parameterInfos, FromEnumType.FromBody);
            }
            else
            {
                GetPropertitys(parameter, parameterInfos, FromEnumType.FromQuery);
            }
        }

        /// <summary>
        /// 终止条件
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterInfos"></param>
        private static void GetPropertitys(System.Reflection.ParameterInfo parameter, List<PageInfo.ParameterInfo> parameterInfos, FromEnumType fromEnumType)
        {
            if (parameter.ParameterType != typeof(string) && parameter.ParameterType != typeof(int) && parameter.ParameterType != typeof(uint) &&
                parameter.ParameterType != typeof(long) && parameter.ParameterType != typeof(ulong) && parameter.ParameterType != typeof(byte) &&
                parameter.ParameterType != typeof(ushort) && parameter.ParameterType != typeof(short) && parameter.ParameterType != typeof(DateTime) &&
                parameter.ParameterType != typeof(char) && parameter.ParameterType != typeof(bool))
            {
                GetTypePropertitys(parameter.ParameterType, parameterInfos, fromEnumType);
            }
            else
            {
                PageInfo.ParameterInfo parameterInfo = new PageInfo.ParameterInfo();
                parameterInfo.FromType = fromEnumType;
                parameterInfo.DataName = parameter.Name.Substring(0, 1).ToLower() + parameter.Name.Substring(1);
                if (parameter.ParameterType == typeof(string) || parameter.ParameterType == typeof(DateTime))
                {
                    parameterInfo.DefaultValue = string.IsNullOrWhiteSpace(parameter.DefaultValue?.ToString()) ? "" : parameter.DefaultValue;
                }
                else if (parameter.ParameterType == typeof(bool))
                {
                    parameterInfo.DefaultValue = string.IsNullOrWhiteSpace(parameter.DefaultValue?.ToString()) ? false : parameter.DefaultValue;
                    parameterInfo.DataType = ParameterDataType.Bool;
                }
                else
                {
                    parameterInfo.DefaultValue = string.IsNullOrWhiteSpace(parameter.DefaultValue?.ToString()) ? 0 : parameter.DefaultValue;
                    parameterInfo.DataType = ParameterDataType.Int;
                }
                parameterInfos.Add(parameterInfo);
            }
        }

        /// <summary>
        /// 递归获取某个ClassType的所有属性
        /// </summary>
        /// <param name="parameterType"></param>
        /// <param name="parameterInfos"></param>
        /// <param name="fromEnumType"></param>
        private static void GetTypePropertitys(Type parameterType, List<PageInfo.ParameterInfo> parameterInfos, FromEnumType fromEnumType)
        {
            foreach (PropertyInfo propertityType in parameterType.GetProperties())
            {
                if (propertityType.PropertyType != typeof(string) && propertityType.PropertyType != typeof(int) && propertityType.PropertyType != typeof(uint) &&
                    propertityType.PropertyType != typeof(byte) && propertityType.PropertyType != typeof(long) && propertityType.PropertyType != typeof(ulong) &&
                    propertityType.PropertyType != typeof(short) && propertityType.PropertyType != typeof(ushort) && propertityType.PropertyType != typeof(DateTime) &&
                    propertityType.PropertyType != typeof(char) && propertityType.PropertyType != typeof(bool))
                {
                    GetTypePropertitys(propertityType.PropertyType, parameterInfos, fromEnumType);
                }
                else
                {
                    PageInfo.ParameterInfo parameterInfo = new PageInfo.ParameterInfo();
                    parameterInfo.FromType = fromEnumType;
                    parameterInfo.DataName = propertityType.Name.Substring(0, 1).ToLower() + propertityType.Name.Substring(1);
                    if (propertityType.PropertyType == typeof(string) || propertityType.PropertyType == typeof(DateTime))
                    {
                        //System.Reflection.Assembly.GetExecutingAssembly().CreateInstance("typestring");
                        //Activator.CreateInstance(typeof());
                        parameterInfo.DefaultValue = "";
                        parameterInfo.DataType = ParameterDataType.String;
                    }
                    else if (propertityType.PropertyType == typeof(bool))
                    {
                        parameterInfo.DefaultValue = false;
                        parameterInfo.DataType = ParameterDataType.Bool;
                    }
                    else
                    {
                        parameterInfo.DefaultValue = 0;
                        parameterInfo.DataType = ParameterDataType.Int;
                    }
                    parameterInfos.Add(parameterInfo);
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
