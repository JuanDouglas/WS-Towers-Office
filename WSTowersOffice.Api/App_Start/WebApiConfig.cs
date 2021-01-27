using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WSTowersOffice.Api.Properties;

namespace WSTowersOffice.Api
{
    public static class WebApiConfig
    {
        public static string DataConnectionString { get { return Settings.Default.DataConnectionString; } }
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
