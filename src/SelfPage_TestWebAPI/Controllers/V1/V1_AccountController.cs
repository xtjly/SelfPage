using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfPage_TestWebAPI.Attribute;
using SelfPage_TestWebAPI.Mode;
using System;
using System.Security.Claims;

namespace SelfPage_TestWebAPI.Controllers.V1
{
    [Route("v1/account")]
    [ApiController]
    public class V1_AccountController : BaseController
    {
        [HttpGet("testmsg")]
        [AllowAnonymous]
        public string GetTestToken()
        {
            return "this is a test msg,but is not used!";
        }

        [HttpGet("token")]
        [AllowAnonymous]
        public ResResult<string> Token()
        {
            return new ResResult<string>
            {
                Code = 1,
                IsSuccess = true,
                Data = CreatToken(new Claim[] {
                    new Claim(nameof(TokenInfo.SelfUserId) , "2020"),
                    new Claim(nameof(TokenInfo.SelfUserName) , "SelfPage"),
                    new Claim(nameof(TokenInfo.SelfCreatTime) , DateTime.Now.ToString("YYYY-MM-DD HH:mm:ss:ms"))
                }),
                Msg = "这是返回的token"
            };
        }

        [HttpPost("jiexi")]
        [DenyAnonymous]
        public TokenInfo GetTokenInfo()
        {
            return ToKen;
        }

    }
}