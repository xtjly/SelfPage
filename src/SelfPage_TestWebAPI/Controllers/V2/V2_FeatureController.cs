using Microsoft.AspNetCore.Mvc;
using SelfPage_TestWebAPI.Attribute;
using SelfPage_TestWebAPI.Mode;

namespace SelfPage_TestWebAPI.Controllers.V2
{
    [Route("v2/feature")]
    [ApiController]
    public class V2_FeatureController : BaseController
    {
        [HttpGet("ifself")]
        [DenyAnonymous]
        public bool IfSelf([FromQuery]string selfName)
        {
            return ToKen.SelfUserId.Equals(selfName);
        }

        [HttpPost("iflogok")]
        [DenyAnonymous]
        public bool IfLogOk([FromBody]ReqInfo mode)
        {
            return ToKen.SelfUserId == mode.SelfUserId && ToKen.SelfUserName.Equals(mode.SelfUserName);
        }

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