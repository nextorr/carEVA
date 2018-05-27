using System.Web;
using System.Web.Optimization;

namespace carEVA
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //*******************Study Area bundles****************************
            //include here the slick slider code
            bundles.Add(new ScriptBundle("~/bundles/ControlPanel").Include(
                        "~/Scripts/Slick/slick.js",
                        "~/Areas/StudyArea/Scripts/ControlPanel.js"));

            bundles.Add(new StyleBundle("~/Content/css/ControlPanel").Include(
                      "~/Areas/StudyArea/Content/ControlPanel.css"));

            //*******************Lesson Area bundles****************************
            bundles.Add(new ScriptBundle("~/bundles/LessonPanel").Include(
                        "~/Areas/StudyArea/Scripts/LessonPanel.js"));

            bundles.Add(new StyleBundle("~/Content/css/LessonPanel").Include(
                      "~/Areas/StudyArea/Content/LessonPanel.css"));
            //*******************slick slider bundles****************************
            bundles.Add(new StyleBundle("~/Content/Slick/css").Include(
                     "~/Content/Slick/slick.css",
                     "~/Content/Slick/slick-theme.css"));

            //*******************JavaScript bundles****************************
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryUI").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            //adding the kendo scripts
            bundles.Add(new ScriptBundle("~/bundles/kendoui").Include(
                "~/Scripts/kendo/2018.2.516/kendo.ui.core.min.js",
                "~/Scripts/kendo/2018.2.516/cultures/kendo.culture.es-CO.min.js"));

            //bundle the eva upload video helper
            bundles.Add(new ScriptBundle("~/bundles/evaVideoUpload").Include(
                        "~/Scripts/eva/evaVideoUploader.js"));

            //bundle for the jquery file upload plugin
            bundles.Add(new ScriptBundle("~/bundles/fileUpload").Include(
                "~/Scripts/jQuery.FileUpload/jquery.iframe-transport.js",
                "~/Scripts/jQuery.FileUpload/jquery.fileupload.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            //*******************style bundles***********************************

            //adding the kendo styles.
            bundles.Add(new StyleBundle("~/Content/kendo/2018.2.516/css").Include(
                     "~/Content/kendo/2018.2.516/kendo.common-material.min.css",
                     "~/Content/kendo/2018.2.516/kendo.material.min.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //bundle the required styles for the jquery file upload plugin
            bundles.Add(new StyleBundle("~/Content/fileUpload").Include(
                      "~/Content/jQuery.FileUpload/css/jquery.fileupload.css"));

            //bundle custom styles for the file upload plugin
            bundles.Add(new StyleBundle("~/Content/evaFileUpload").Include(
                      "~/Content/eva/evaFileUpload.css"));

            //bundle custom styles for the jquery UI plugin
            bundles.Add(new StyleBundle("~/Content/jqueryUI").Include(
                      "~/Content/themes/base/jquery-ui.css"));

        }
    }
}
