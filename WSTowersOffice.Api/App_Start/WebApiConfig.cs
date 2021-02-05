using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Web.Http;
using WSTowersOffice.Api.Properties;

namespace WSTowersOffice.Api
{
    public static class WebApiConfig
    {
        public static string[] KeyValueConnectionString { get => ConnectionString.Split(';'); }
        public static string EntityFreameworkConnectionString
        {
            get
            {
                var connectionBuilder = new EntityConnectionStringBuilder
                {
                    Provider = "System.Data.SqlClient",
                    ProviderConnectionString = $"{ConnectionString};MultipleActiveResultSets=True;App=EntityFramework;",
                    Metadata = @"res://*/Models.WSTowersOfficeModel.csdl|res://*/Models.WSTowersOfficeModel.ssdl|res://*/Models.WSTowersOfficeModel.msl"
                };
                string connectionString = connectionBuilder.ToString();
                return connectionString;
            }
        }
        public static string ConnectionString { get { return ConfigurationManager.ConnectionStrings["WSTowersOfficeEntitiesDefault"].ConnectionString; } }
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
