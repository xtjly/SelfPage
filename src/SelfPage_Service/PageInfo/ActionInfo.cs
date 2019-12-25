using System.Collections.Generic;

namespace SelfPage_Service.PageInfo
{
    /// <summary>
    /// Action信息
    /// </summary>
    public class ActionInfo
    {
        /// <summary>
        /// Action请求路径
        /// </summary>
        public string RequestPath = "";
        /// <summary>
        /// Http请求方式
        /// </summary>
        public RequestType RequestType = RequestType.HttpGet;
        /// <summary>
        /// Action注释-描述信息
        /// </summary>
        public string DescribeTion = "";
        /// <summary>
        /// 方法的请求参数信息
        /// </summary>
        public List<ParameterInfo> RequestParameters = new List<ParameterInfo>();
        /// <summary>
        /// 方法的return 对象的json格式字符串
        /// </summary>
        public string ReturnJsonStr { get; set; }
    }

}
