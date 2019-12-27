using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfPage_TestWebAPI.Attribute;
using SelfPage_TestWebAPI.Mode;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SelfPage_TestWebAPI.Controllers.V1
{
    /// <summary>
    /// 账号信息控制器
    /// </summary>
    [Route("v1/account")]
    [ApiController]
    public class V1_AccountController : BaseController
    {
        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet("testmsg")]
        [AllowAnonymous]
        public async Task<string> GetTestToken()
        {
            return "123";
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 解析token,并返回信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("jiexi")]
        [DenyAnonymous]
        public TokenInfo GetTokenInfo()
        {
            return ToKen;
        }

    }
}