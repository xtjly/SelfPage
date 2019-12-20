using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SelfPage_TestWebAPI.Mode;

namespace SelfPage_TestWebAPI.Controllers.V1
{
    [Route("v1/account")]
    [ApiController]
    public class V1_AccountController : BaseController
    {
        [HttpGet("testtoken")]
        public string GetTestToken()
        {
            return "this is a test token,but is not used!";
        }

        [HttpGet("token")]
        public ResResult<string> Token()
        {
            return new ResResult<string>
            {
                Code = 1,
                IsSuccess = true,
                Data = "",
                Msg = "这是返回的token"
            };
        }

        [HttpPost("jiexi")]
        public TokenInfo GetTokenInfo()
        {
            return ToKen;
        }

    }
}