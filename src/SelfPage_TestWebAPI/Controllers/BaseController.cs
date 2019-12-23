using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SelfPage_TestWebAPI.Filter;
using SelfPage_TestWebAPI.Mode;
using SelfPage_TestWebAPI.Utilty;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SelfPage_TestWebAPI.Controllers
{
    [EnableCors("AllowAllOrigin")]
    [TypeFilter(typeof(DenyAnonymousFilter))]
    public class BaseController : ControllerBase
    {
        public TokenInfo ToKen
        {
            get
            {
                try
                {
                    var headers = Request.Headers;
                    var user = User.Identities as ClaimsIdentity;
                    var token = new TokenInfo();
                    if (user != null)
                    {
                        token.SelfUserId = user.FindInt64(nameof(TokenInfo.SelfUserId));
                        token.SelfUserName = user.FindFistVlaue(nameof(TokenInfo.SelfUserName));
                        token.SelfCreatTime = user.FindFirstTime(nameof(TokenInfo.SelfCreatTime));
                    }
                    return token;
                }
                catch (Exception ex)
                {
                    return new TokenInfo();
                }
            }
        }

        protected string CreatToken(System.Security.Claims.Claim[] claim)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("selfpage_key"));
            var credis = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "SelfPage"
                , audience: "SelfPage"
                , claims: claim
                , notBefore: DateTime.Now
                , expires: DateTime.Now.AddDays(1)
                , signingCredentials: credis
                );
            return $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}";
        }
    }
}