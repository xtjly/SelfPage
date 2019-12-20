using System;

namespace SelfPage_TestWebAPI
{
    [Serializable]
    public class ResResult<T>
    {
        public bool IsSuccess { get; set; }
        public int Code { get; set; }
        public string Msg { get; set; }
        public T Data { get; set; }
    }
}
