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

    }

}
