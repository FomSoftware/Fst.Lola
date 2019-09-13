using System.Web.Optimization;

namespace FomMonitoring.App_Start
{
    public class BundleConfig
    {
        // For more information on bundling
        // visit http://go.microsoft.com/fwlink/?LinkId=301862

        public static void RegisterBundles(BundleCollection bundles)
        {

            #region LIBS NuGet

            // non cambiare ordine css
            bundles.Add(new StyleBundle("~/Content/FomMonitoringStylesLIB").Include(
                        "~/Content/bootstrap.min.css",
                        "~/Content/google-font-lato.css",
                        "~/Content/google-font-roboto-mono.css",
                        "~/Content/icofom.css",
                        "~/Content/font-awesome.min.css",
                        "~/Content/simple-line-icons.css",
                        "~/Content/DataTables/css/jquery.dataTables.css"
                      ));


            bundles.Add(new ScriptBundle("~/Scripts/FomMonitoringScriptsLIB").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/vue.min.js",
                        "~/Scripts/highcharts/5.0.14/highcharts.js",
                        "~/Scripts/highcharts/5.0.14/highcharts-more.js",
                        "~/Scripts/highcharts/5.0.14/modules/solid-gauge.js",
                        //"~/Scripts/highcharts/5.0.14/themes/dark-unica.js",
                        "~/Scripts/DataTables/jquery.dataTables.js",
                        "~/Scripts/DataTables/dataTables.bootstrap.js"
                      ));

            #endregion

            #region GlobApp 

            bundles.Add(new StyleBundle("~/GlobApp/styles/FomMonitoringStylesAPP").Include(
                        "~/GlobApp/plugins/bootstrap-select/css/bootstrap-select.min.css",
                        "~/GlobApp/plugins/swiper/css/swiper.css",
                        "~/GlobApp/plugins/daterangepicker/daterangepicker.css",
                        "~/GlobApp/plugins/waitMe/waitMe.css",
                        "~/GlobApp/plugins/bootstrap-select.css",
                        //"~/GlobApp/styles/layout.css",
                        //"~/GlobApp/styles/toolbar.css",
                        //"~/GlobApp/styles/machine.css",
                        //"~/GlobApp/styles/mes.css",
                        //"~/GlobApp/styles/user-manager.css",
                        //"~/GlobApp/styles/utilities.css",
                        "~/GlobApp/grey/user-manager.css",
                        "~/GlobApp/grey/layout.css",
                        "~/GlobApp/grey/toolbar.css",
                        "~/GlobApp/grey/machine.css",
                        "~/GlobApp/grey/mes.css",
                        "~/GlobApp/grey/user-manager.css",
                        "~/GlobApp/grey/utilities.css"
                     ));


            bundles.Add(new ScriptBundle("~/GlobApp/scripts/FomMonitoringScriptsAPP").Include(
                        "~/GlobApp/plugins/bootstrap-select/js/bootstrap-select.min.js",                        
                        "~/GlobApp/plugins/jquery-flip/jquery.flip.min.js",
                        "~/GlobApp/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                        "~/GlobApp/plugins/bootstrap-select.js",
                        "~/GlobApp/plugins/underscore/underscore-min.js",
                        "~/GlobApp/plugins/swiper/js/swiper.js",
                        "~/GlobApp/plugins/daterangepicker/moment.js",
                        "~/GlobApp/plugins/daterangepicker/moment-with-locales.js",
                        "~/GlobApp/plugins/daterangepicker/daterangepicker.js",
                        "~/GlobApp/plugins/waitMe/waitMe.js",
                        "~/GlobApp/plugins/sweet-alert/sweetalert.min.js",
                        "~/GlobApp/plugins/jStorage/jstorage.js",
                        "~/GlobApp/scripts/vue-manager.js",
                        "~/GlobApp/scripts/shared-manager.js",
                        "~/GlobApp/scripts/user-manager.js",
                        "~/GlobApp/scripts/machine-manager.js",
                        "~/GlobApp/scripts/mes-manager.js",
                        "~/GlobApp/scripts/chart-manager.js",
                        //"~/GlobApp/scripts/common-manager.js",
                        "~/GlobApp/scripts/waitme-manager.js",
                        "~/GlobApp/scripts/machine/efficiency.js",
                        "~/GlobApp/scripts/machine/productivity.js",
                        //"~/GlobApp/scripts/machine/alarms.js",
                        "~/GlobApp/scripts/machine/messages.js",
                        "~/GlobApp/scripts/machine/spindles.js",
                        "~/GlobApp/scripts/machine/xspindles.js",
                        "~/GlobApp/scripts/machine/tools.js",
                        "~/GlobApp/scripts/machine/xtools.js",
                        "~/GlobApp/scripts/machine/jobs.js",
                        "~/GlobApp/scripts/machine/maintenance.js",
                        "~/GlobApp/scripts/plantmessages.js",
                        "~/GlobApp/scripts/machine/tools/blitz.js",
                        "~/GlobApp/scripts/machine/motoraxes/blitz.js",
                        "~/GlobApp/scripts/machine/motor/keope.js"
                      ));

            #endregion

            #region Login 

            bundles.Add(new StyleBundle("~/Content/FomMonitoringStylesLOGIN").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/google-font-lato.css",
                      "~/Content/google-font-roboto-mono.css",
                      "~/Content/icomoon-icons.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/simple-line-icons.css",
                      "~/GlobApp/grey/login.css"
                    ));


            bundles.Add(new ScriptBundle("~/Scripts/FomMonitoringScriptsLOGIN").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.js"
                      ));

            #endregion

        }

    }
}