using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SelfPage_TestWebAPI.Mode;

namespace SelfPage_TestWebAPI.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        public TokenInfo ToKen
        {
            get
            {
                return new TokenInfo();
            }
        }
    }
}