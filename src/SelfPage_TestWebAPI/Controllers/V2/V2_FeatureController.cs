using Microsoft.AspNetCore.Mvc;
using SelfPage_TestWebAPI.Attribute;
using SelfPage_TestWebAPI.Mode;

namespace SelfPage_TestWebAPI.Controllers.V2
{
    /// <summary>
    /// 功能操作控制器
    /// </summary>
    [Route("v2/feature")]
    [ApiController]
    public class V2_FeatureController : BaseController
    {
        /// <summary>
        /// 是否为自己的名称
        /// </summary>
        /// <param name="selfName"></param>
        /// <returns></returns>
        [HttpGet("ifself")]
        [DenyAnonymous]
        public bool IfSelf([FromQuery]string selfName)
        {
            return ToKen.SelfUserId.Equals(selfName);
        }

        /// <summary>
        /// 账号是否是自己
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [HttpPost("iflogok")]
        [DenyAnonymous]
        public bool IfLogOk([FromBody]ReqInfo mode)
        {
            return ToKen.SelfUserId == mode.SelfUserId && ToKen.SelfUserName.Equals(mode.SelfUserName);
        }

        /// <summary>
        /// 账号是否是自己2
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="randomnum"></param>
        /// <returns></returns>
        [HttpPost("iflogok2")]
        [DenyAnonymous]
        public ResResult<ResInfo> IfLogOk2([FromBody]ReqInfo mode, [FromQuery]long randomnum)
        {
            return new ResResult<ResInfo>
            {
                Code = 1,
                IsSuccess = true,
                Msg = "验证结果",
                Data = new ResInfo
                {
                    IsOk = ToKen.SelfUserId == mode.SelfUserId && ToKen.SelfUserName.Equals(mode.SelfUserName),
                    RandomNum = randomnum
                }
            };
        }
    }
}