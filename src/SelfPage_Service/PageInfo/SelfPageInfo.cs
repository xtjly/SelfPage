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
        /// 是否使用xml注释信息
        /// </summary>
        public bool IfUseXmlInfo = false;
        /// <summary>
        /// 配置引用的Jquery-Url
        /// </summary>
        public string JqueryUrl { get { return "https://cdn.staticfile.org/jquery/2.2.4/jquery.min.js"; } private set { } }
        /// <summary>
        /// 配置引用的Js-Url
        /// </summary>
        public string JsUrl = "";
        /// <summary>
        /// 配置引用的CSS-Url
        /// </summary>
        public string CssUrl = "";
    }
}
