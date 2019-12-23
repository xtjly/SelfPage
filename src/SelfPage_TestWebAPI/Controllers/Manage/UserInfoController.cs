using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SelfPage_TestWebAPI.Controllers.Manage
{
    [Route("manage/userinfo")]
    [ApiController]
    public class UserInfoController : BaseController
    {
        [HttpGet("help")]
        [AllowAnonymous]
        public string GetHelp()
        {
            return "this is origin/manage/userinfo api's help return string!";
        }

        [HttpGet("alluser")]
        [AllowAnonymous]
        public ResResult<List<string>> GetAllUserInfo()
        {
            return new ResResult<List<string>>
            {
                Code = 1,
                IsSuccess = true,
                Msg = "这里包含所有User",
                Data = new List<string> { "Aken", "Blusure", "Caker", "Deghature" }
            };
        }
    }
}