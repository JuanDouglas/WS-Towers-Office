using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using WSTowersOffice.Api.Properties;

namespace WSTowersOffice.Api
{
    public static class WebApiConfig
    {
        public static string EntityFreameworkConnectionString { get { return $"metadata=res://*/Models.WSTowersOfficeModel.csdl|res://*/Models.WSTowersOfficeModel.ssdl|res://*/Models.WSTowersOfficeModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;{ConnectionString}&quot;"; } }
        public static string ConnectionString { get { return ConfigurationManager.ConnectionStrings["WSTowersOfficeEntities"].ConnectionString; } }
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
