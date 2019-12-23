using System;
using System.Collections.Generic;
using System.Text;

namespace SelfPage_Service.PageInfo
{
    /// <summary>
    /// 返回的接口文档描述-分组页面信息
    /// </summary>
    public class ResPageInfo
    {
        /// <summary>
        /// 分组接口文档描述页面中所有控制器
        /// </summary>
        public List<ControllerInfo> Controllers = new List<ControllerInfo>();
        /// <summary>
        /// 所在分组
        /// </summary>
        public string GroupName = "default";
    }
}
