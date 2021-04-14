using System.Collections.Generic;
using System;

namespace JoVei.Base.Backend
{
    public enum ResulTypes { Error, Success }

    /// <summary>
    /// Base class for requests to the backend
    /// </summary>
    public abstract class BaseRequest
    {
        public DateTime RequestTime { get; set; }
    }

    /// <summary>
    /// Base class for results from backend
    /// </summary>
    public abstract class BaseResult
    {
        public ResulTypes ResultType { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}