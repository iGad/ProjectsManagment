using System.Web.Mvc;
using System.Web.Routing;

namespace ProjectsManagment
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute("GetStagesByProjectId",
                            "dependency/getstagesbyprojectid/",
                            new { controller = "Dependency", action = "GetStagesByProjectId" },
                            new[] { "ProjectsManagment.Controllers" });
            routes.MapRoute("GetPartitionsByStageId",
                            "dependency/getpartitionsbystageid/",
                            new { controller = "Dependency", action = "GetPartitionsByStageId" },
                            new[] { "ProjectsManagment.Controllers" });
            routes.MapRoute("GetTasksByPartitionId",
                            "dependency/gettasksbypartitionid/",
                            new { controller = "Dependency", action = "GetTasksByPartitionId" },
                            new[] { "ProjectsManagment.Controllers" });
        }
    }
}