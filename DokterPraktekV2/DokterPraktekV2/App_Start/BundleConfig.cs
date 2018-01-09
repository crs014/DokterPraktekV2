using System.Web;
using System.Web.Optimization;

namespace DokterPraktekV2
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Plugins/bower_components/jquery/dist/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Plugins/bower_components/jquery-ui/jquery-ui.min.js",
                         "~/Plugins/DataTables-1.10.16/js/jquery.dataTables.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Plugins/bower_components/bootstrap/dist/js/bootstrap.min.js",
                      "~/Plugins/dist/js/adminlte.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Plugins/bower_components/bootstrap/dist/css/bootstrap.min.css",
                      "~/Content/Site.css",
                      "~/Plugins/bower_components/font-awesome/css/font-awesome.min.css",
                      "~/Plugins/bower_components/Ionicons/css/ionicons.min.css",
                      "~/Plugins/dist/css/AdminLTE.min.css",
                      "~/Plugins/dist/css/skins/_all-skins.min.css",
                      "~/Plugins/DataTables-1.10.16/css/jquery.dataTables.min.css"));
        }
    }
}
