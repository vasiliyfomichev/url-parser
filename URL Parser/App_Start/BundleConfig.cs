#region

using System.Web.Optimization;

#endregion

namespace URL_Parser
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts")
                .Include("~/scripts/chart.min.js",
                "~/scripts/jquery-2.1.4.min.js", 
                         "~/Scripts/angular.min.js",
                         "~/Scripts/angular-animate.min.js",
                         "~/scripts/twinmax.min.js",
                         "~/scripts/tc-angular-chartjs.min.js",
                         "~/scripts/angular-touch.min.js")
                .IncludeDirectory("~/scripts/app", "*.js"));

            bundles.Add(new StyleBundle("~/styles")
                .Include("~/content/bootstrap.min.css",
                    "~/content/app/main.css"));
        }
    }
}