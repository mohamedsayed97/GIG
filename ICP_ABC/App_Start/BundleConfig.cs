using System.Web;
using System.Web.Optimization;

namespace ICP_ABC
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                       "~/Scripts/toastr.js*",
                   "~/Scripts/toastrImp.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                       "~/Content/toastr.css",
                      "~/Content/site.css"));

            //bundles.Add(new ScriptBundle("~/bundles/toastr").Include("~/Scripts/toastr.js"));
            //bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/toastr.css"));

        
            
            
            //bundles.Add(new ScriptBundle("~/bundles/toastr").Include(
            //       "~/Scripts/toastr.js*",
            //       "~/Scripts/toastrImp.js"));
            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css",
                     
            //          "~/Content/datatables/css/datatables.bootstrap.css"
            //          /*"~/Content/DataTables/css/dataTables.bootstrap.css"*/));
        }
    }
}
