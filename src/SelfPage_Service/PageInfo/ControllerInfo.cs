using System.Collections.Generic;

namespace SelfPage_Service.PageInfo
{
    /// <summary>
    /// 控制器信息
    /// </summary>
    public class ControllerInfo
    {
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string ControllerName = "";
        /// <summary>
        /// 控制器注释-描述信息
        /// </summary>
        public string Discribetion = "";
        /// <summary>
        /// 控制器的路由
        /// </summary>
        public string ControllerRoute = "";
        /// <summary>
        /// 控制器中所有Action
        /// </summary>
        public List<ActionInfo> Actions = new List<ActionInfo>();
    }
}
