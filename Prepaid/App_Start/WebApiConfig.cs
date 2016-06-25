using Microsoft.Practices.Unity;
using Prepaid.ExceptionHandling;
using Prepaid.Filters;
using Prepaid.Models;
using Prepaid.Repositories;
using Prepaid.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Prepaid
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            config.Filters.Add(new ValidateModelAttribute()); // 添加模型验证过滤器

            var container = new UnityContainer();
            container.RegisterType<IAdminRepository, AdminRespository>(new HierarchicalLifetimeManager());
            container.RegisterType<IPointRepository, PointRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<string, CreditLevel>, CreditLevelRespository>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<string, Recharge>, RechargeRespository>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);

            // Web API 路由
            config.MapHttpAttributeRoutes();

            // There can be multiple exception loggers. (By default, no exception loggers are registered.)
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());

            // There must be exactly one exception handler. (There is a default one that may be replaced.)
            // To make this sample easier to run in a browser, replace the default exception handler with one that sends
            // back text/plain content for all errors.
            config.Services.Replace(typeof(IExceptionHandler), new GenericTextExceptionHandler());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{uuid}",
                defaults: new { uuid = RouteParameter.Optional }
            );
        }
    }
}
