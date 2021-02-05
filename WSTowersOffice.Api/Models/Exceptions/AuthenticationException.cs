using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web;

namespace WSTowersOffice.Api.Models.Exceptions
{
    [Serializable]
    public class AuthenticationException : APIException
    {
        public AuthenticationException(Exception inner,
    HttpContext context) : base(inner.Message, inner, null, context) { }
        public AuthenticationException(HttpResponseMessage response,
            HttpContext context) : base(response, context) { }
        public AuthenticationException(string message,
            HttpResponseMessage response,
            HttpContext context) : base(message, response, context) { }
        public AuthenticationException(string message,
            Exception inner,
            HttpResponseMessage response,
            HttpContext context) : base(message, inner, response, context) { }
        protected AuthenticationException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}