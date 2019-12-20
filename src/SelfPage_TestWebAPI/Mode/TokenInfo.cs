using System;

namespace SelfPage_TestWebAPI.Mode
{
    [Serializable]
    public class TokenInfo
    {
        public DateTime SelfCreatTime { get; set; }
        public string SelfUserName { get; set; }
        public long SelfUserId { get; set; }
    }
}
