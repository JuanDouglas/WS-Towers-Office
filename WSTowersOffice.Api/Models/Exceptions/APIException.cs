using System;
using System.Net.Http;
using System.Web;

namespace WSTowersOffice.Api.Models.Exceptions
{
    [Serializable]
    public class APIException : Exception
    {
        public HttpResponseMessage Response { get; private set; }
        public HttpContext RequestContext { get; private set; }
        public APIException(HttpResponseMessage response, HttpContext context) { Response = response; RequestContext = context; }
        public APIException(string message, HttpResponseMessage response, HttpContext context) : base(message) { Response = response; RequestContext = context; }
        public APIException(string message, Exception inner, HttpResponseMessage response, HttpContext context) : base(message, inner) { Response = response; RequestContext = context; }
        protected APIException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}