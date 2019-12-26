using System.Collections.Generic;

namespace SelfPage_Service.PageInfo
{
    /// <summary>
    /// 接口文档描述页面-参数配置
    /// </summary>
    public class SelfPageInfo
    {
        /// <summary>
        /// 设置接口文档描述的根节点。需以/开头
        /// http://localhost:port/selfpage
        /// </summary>
        public string EndPointPath = "/selfpage";
        /// <summary>
        /// 预计把接口文档描述分为几组。
        /// </summary>
        public List<string> Groups = new List<string> { "default" };
        /// <summary>
        /// 是否使用 Authorization 请求头。
        /// </summary>
        public bool AddAuthorizationHeader = false;
        /// <summary>
        /// 是否使用xml注释信息。 | 需要项目生成xml文档在bin\Debug\netcoreapp2.2，提供注释信息。若无则不提供。
        /// </summary>
        public bool IfUseXmlInfo = false;
    }
}
