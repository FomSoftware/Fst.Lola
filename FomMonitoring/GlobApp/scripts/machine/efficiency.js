﻿var Efficiency = function ()
{
    var vmEfficiency;
    var circle_kpi;
    var localizations;

    var init = function (data, loc)
    {
        localizations = loc;
        initVueModel(data);
        if (data.opt_states != null && data.opt_states.series.length > 0) {
            ChartManager.stateMachinePieChart("efc_pie_chart", data.opt_states);
        }

        if (data.opt_kpis != null && data.opt_states.series.length > 0) {
            ChartManager.productivityMachineSolidGaugeChart("efc_kpi_chart", data.opt_kpis, localizations);
        }

        if (data.opt_historical != null && data.opt_historical.series.length > 0) {
            vmEfficiency.show.historical = true;
            ChartManager.lineChart('efc_historical_chart', data.opt_historical);
        }

        if (data.opt_operators != null && data.opt_operators.series.length > 0) {
            vmEfficiency.show.operators = true;
            ChartManager.stackedBarChart('efc_operators_chart', data.opt_operators);
        }


    }

    var show = function () {
        vmEfficiency.showed = true;
    }

    var hide = function () {
        vmEfficiency.showed = false;
    }

    var initVueModel = function (data)
    {
        vmEfficiency = new Vue({
            el: '#CardEfficiency',
            data: {
                type: data.vm_machine_info.mtype,
                kpi: data.vm_efficiency.kpi,
                total: data.vm_efficiency.total,
                overfeed: data.vm_efficiency.overfeed,
                states: data.vm_efficiency.states,
                show: {
                    historical: false,
                    operators: false
                },
                showed: true
            },
            filters: {
                round: function(value)
                {
                    return Math.round(value);
                }
            },
            computed: {
                colorKPI: function ()
                {
                    var kpi = this.kpi;

                    if (kpi == null)
                        return 'color-no-data';

                    var color = '';

                    if (kpi.value < kpi.threshold.yellow)
                        color = 'color-red';

                    if (kpi.value >= kpi.threshold.yellow && kpi.value < kpi.threshold.green)
                        color = 'color-yellow';

                    if (kpi.value >= kpi.threshold.green)
                        color = 'color-green';

                    return color;
                },
                bgOverfeed: function ()
                {
                    var overfeed = this.overfeed.value;
                    var threshold = this.overfeed.threshold;

                    return {
                        'bg-red': overfeed < threshold.yellow,
                        'bg-yellow': overfeed >= threshold.yellow && overfeed < threshold.green,
                        'bg-green': overfeed >= threshold.green
                    }
                },
                showDetails: function()
                {
                    if (this.show.historical || this.show.operators)
                        return true;
                    else
                        return false;
                },
                enableSwiper: function() {
                    if (this.show.historical || this.show.operators)
                        return "";
                    else return "swiper-disabled";
                },
                getCol: function () {
                    if (this.type != 'Troncatrice')
                        return "col-11 col-sm-11 col-md-5 col-lg-5 col-xl-6 larger";
                    else
                        return "col-12 col-sm-12 col-md-6 col-lg-6";
                }
            },
            methods: {
                icon: function (state)
                {
                    return {
                        'fa-play': state.code == 'prod',
                        'icofom-pause': state.code == 'pause',
                        'icofom-manual': state.code == 'manual',
                        'fa-exclamation-triangle': state.code == 'error'
                        //'icofom-alarms': state.code == 'error'
                    }
                }
            }
        });
    }


    var update = function(data)
    {
        // update vue model
        var efficiency = data.vm_efficiency;
        vmEfficiency.type = data.vm_machine_info.mtype;
        vmEfficiency.kpi = efficiency.kpi;
        vmEfficiency.total = efficiency.total;
        vmEfficiency.overfeed = efficiency.overfeed;
        vmEfficiency.states = efficiency.states;
        if (vmEfficiency.states != null) {
            Vue.nextTick(function () {
                // container IS finished rendering to the DOM

                if (data.opt_states != null && data.opt_states.series.length > 0) {
                    ChartManager.stateMachinePieChart("efc_pie_chart", data.opt_states);
                }
                else {
                    ChartManager.destroyChart('efc_pie_chart');
                }

                if (data.opt_kpis != null) {
                    ChartManager.productivityMachineSolidGaugeChart("efc_kpi_chart", data.opt_kpis, localizations);
                }
                else {
                    ChartManager.destroyChart('efc_kpi_chart');
                }

                
                
            });
        }

        // chart historical
        if (data.opt_historical != null) {
            vmEfficiency.show.historical = true;
            ChartManager.lineChart('efc_historical_chart', data.opt_historical);
        }
        else {
            vmEfficiency.show.historical = false;
            ChartManager.destroyChart('efc_historical_chart');
        }

        // chart operators
        if (data.opt_operators != null) {
            vmEfficiency.show.operators = true;
            ChartManager.stackedBarChart('efc_operators_chart', data.opt_operators);
        }
        else {
            vmEfficiency.show.operators = false;
            ChartManager.destroyChart('efc_operators_chart');
        }


    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();