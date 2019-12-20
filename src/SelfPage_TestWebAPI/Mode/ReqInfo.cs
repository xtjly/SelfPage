using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SelfPage_TestWebAPI.Mode
{
    [Serializable]
    public class ReqInfo
    {
        public long SelfUserId { get; set; }
        public string SelfUserName { get; set; }
    }
}
