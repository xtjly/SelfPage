using System.Collections.Generic;

namespace SelfPage_Service.Xml
{
    /// <summary>
    /// xml注释信息
    /// </summary>
    public class XmlDocInfo
    {
        public List<MemberInfo> Members = new List<MemberInfo>();
    }

    public class MemberInfo
    {
        /// <summary>
        /// 控制器|Action 的注释name。
        /// 例如：T:SelfPage_TestWebAPI.Controllers.Manage.UserInfoController
        ///       M:SelfPage_TestWebAPI.Controllers.Manage.UserInfoController.GetHelp
        /// </summary>
        public string MemberName { get; set; }
        /// <summary>
        /// 注释信息
        /// </summary>
        public string Summary { get; set; }
    }
}
