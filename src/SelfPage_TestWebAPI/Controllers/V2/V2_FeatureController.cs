using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SelfPage_TestWebAPI.Mode;

namespace SelfPage_TestWebAPI.Controllers.V2
{
    [Route("v2/feature")]
    [ApiController]
    public class V2_FeatureController : BaseController
    {
        [HttpGet("ifself")]
        public bool IfSelf([FromQuery]string selfName)
        {
            return ToKen?.SelfUserId.Equals(selfName) ?? false;
        }

        [HttpPost("iflogok")]
        public bool IfLogOk([FromBody]long selfUserId, [FromBody]string selfUserName)
        {
            return ToKen.SelfUserId == selfUserId && ToKen.SelfUserName.Equals(selfUserName);
        }

        [HttpPost("iflogok2")]
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