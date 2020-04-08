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
                        "~/Content/bootstrap.css",
                        "~/Content/google-font-lato.css",
                        "~/Content/google-font-roboto-mono.css",
                        "~/Content/icofom.css",
                        "~/Content/daterangepicker.css",
                        "~/Content/font-awesome.min.css",
                        "~/Content/simple-line-icons.css",
                        "~/Scripts/DataTables/dataTables.bootstrap4.css",
                        "~/Scripts/DataTables/responsive.bootstrap4.css",
                        "~/Content/bootstrap-select.css",
                        "~/Content/toastr.css"
                      ));


            bundles.Add(new ScriptBundle("~/Scripts/FomMonitoringScriptsLIB").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/hammer.js",
                        "~/Scripts/bootstrap.bundle.js",
                        "~/Scripts/bootstrap-select.js",
                        "~/Scripts/vue.js",
                        "~/Scripts/highcharts/5.0.14/highcharts.js",
                        "~/Scripts/highcharts/5.0.14/highcharts-more.js",
                        "~/Scripts/highcharts/5.0.14/modules/solid-gauge.js",
                        //"~/Scripts/highcharts/5.0.14/themes/dark-unica.js",
                        "~/Scripts/DataTables/jquery.dataTables.js",
                        "~/Scripts/DataTables/dataTables.bootstrap4.js",
                        "~/Scripts/DataTables/dataTables.responsive.js",
                        "~/Scripts/DataTables/responsive.bootstrap4.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/moment-with-locales.js",
                        "~/Scripts/daterangepicker.js",
                        "~/Scripts/underscore.js",
                        "~/Scripts/toastr.js"
                      ));

            #endregion

            #region GlobApp 

            bundles.Add(new StyleBundle("~/GlobApp/styles/FomMonitoringStylesAPP").Include(
                        "~/GlobApp/plugins/swiper/css/swiper.css",
                        "~/GlobApp/plugins/waitMe/waitMe.css",
                        "~/GlobApp/grey/plant-manager.css",
                        "~/GlobApp/grey/user-manager.css",
                        "~/GlobApp/grey/layout.css",
                        "~/GlobApp/grey/toolbar.css",
                        "~/GlobApp/grey/machine.css",
                        "~/GlobApp/grey/mes.css",
                        "~/GlobApp/grey/user-manager.css",
                        "~/GlobApp/grey/utilities.css"
                     ));


            bundles.Add(new ScriptBundle("~/GlobApp/scripts/FomMonitoringScriptsAPP").Include(                
                        "~/GlobApp/plugins/jquery-flip/jquery.flip.min.js",
                        "~/GlobApp/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                        "~/GlobApp/plugins/swiper/js/swiper.js",
                        "~/GlobApp/plugins/waitMe/waitMe.js",
                        "~/GlobApp/plugins/sweet-alert/sweetalert.min.js",
                        "~/GlobApp/plugins/jStorage/jstorage.js",
                        "~/GlobApp/scripts/vue-manager.js",
                        "~/GlobApp/scripts/shared-manager.js",
                        "~/GlobApp/scripts/user-manager.js",
                        "~/GlobApp/scripts/plant-manager.js",
                        "~/GlobApp/scripts/machine-manager.js",
                        "~/GlobApp/scripts/mes-manager.js",
                        "~/GlobApp/scripts/chart-manager.js",
                        //"~/GlobApp/scripts/common-manager.js",
                        "~/GlobApp/scripts/waitme-manager.js",
                        "~/GlobApp/scripts/machine/image-manager.js",
                        "~/GlobApp/scripts/machine/efficiency.js",
                        "~/GlobApp/scripts/machine/productivity.js",
                        //"~/GlobApp/scripts/machine/alarms.js",
                        "~/GlobApp/scripts/machine/messages.js",
                        "~/GlobApp/scripts/machine/spindles.js",
                        "~/GlobApp/scripts/machine/tools.js",
                        "~/GlobApp/scripts/machine/xtools.js",
                        "~/GlobApp/scripts/machine/jobs/fmc-lmx.js",
                        "~/GlobApp/scripts/machine/maintenance.js",
                        "~/GlobApp/scripts/plantmessages.js",
                        "~/GlobApp/scripts/machine/tools/blitz.js",
                        "~/GlobApp/scripts/machine/motoraxes/blitz.js",
                        "~/GlobApp/scripts/machine/motor/keope.js",
                        "~/GlobApp/scripts/machine/axes/keope.js",
                        "~/GlobApp/scripts/machine/axes/lmx.js",
                        "~/GlobApp/scripts/machine/electrospindle/electrospindle.js",
                        "~/GlobApp/scripts/machine/otherdata/otherdataFMC.js",
                        "~/GlobApp/scripts/machine/tools/fmcLmx.js",
                        "~/GlobApp/scripts/machine/multispindles/multispindles.js",
                        "~/GlobApp/scripts/notification-manager.js",
                        "~/GlobApp/scripts/machine/tiltingaxes/tiltingaxes.js",
                        "~/GlobApp/scripts/machine/rotaryaxes/rotaryaxes.js",
                        "~/GlobApp/scripts/machine/sensorspindles/sensorspindles.js",
                        "~/GlobApp/scripts/machine/motor/motorbladelmx.js",
                        "~/GlobApp/scripts/machine/otherdata/otherdataLMX.js",
                        "~/GlobApp/scripts/machine/jobs/other-machines.js",
                        "~/GlobApp/scripts/machine/xtools/lmx.js"

                      ));

            #endregion

            #region Login 

            bundles.Add(new StyleBundle("~/Content/FomMonitoringStylesLOGIN").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/google-font-lato.css",
                      "~/Content/google-font-roboto-mono.css",
                      "~/Content/icomoon-icons.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/simple-line-icons.css",
                      "~/GlobApp/grey/login.css"
                    ));



            bundles.Add(new ScriptBundle("~/Scripts/FomMonitoringScriptsLOGIN").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.bundle.js"
                      ));

            #endregion

        }

    }
}