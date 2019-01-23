var Efficiency = function ()
{
    var vmEfficiency;
    var circle_kpi;


    var init = function (data)
    {
        /*var data = {
            vm_efficiency: {
                kpi: {
                    value: 93,
                    threshold: {
                        green: 80,
                        yellow: 60
                    }
                },
                total: {
                    on: {
                        days: null,
                        hours: '01',
                        minutes: '23',
                        seconds: null
                    },
                    off: {
                        days: null,
                        hours: '00',
                        minutes: '10',
                        seconds: null
                    }
                },
                overfeed: {
                    value: 88,
                    threshold: {
                        green: 80,
                        yellow: 60
                    }
                },
                states: [{
                    code: 'prod',
                    text: 'Produzione',
                    perc: 30,
                    time: {
                        days: null,
                        hours: '01',
                        minutes: '23',
                        seconds: null
                    },
                    active: true
                },
                {
                    code: 'pause',
                    text: 'Pausa',
                    perc: 25,
                    time: {
                        days: null,
                        hours: '01',
                        minutes: '23',
                        seconds: null
                    },
                    active: false
                }, {
                    code: 'manual',
                    text: 'Manuale',
                    perc: 20,
                    time: {
                        days: null,
                        hours: '01',
                        minutes: '23',
                        seconds: null
                    },
                    active: false
                }, {
                    code: 'error',
                    text: 'Guasto',
                    perc: 25,
                    time: {
                        days: null,
                        hours: '01',
                        minutes: '23',
                        seconds: null
                    },
                    active: false
                }]
            }
        }*/
        
        initVueModel(data.vm_efficiency);

        if (data.opt_historical != null)
        {
            vmEfficiency.show.historical = true;
            ChartManager.lineChart('efc_historical_chart', data.opt_historical);
        }

        if (data.opt_operators != null)
        {
            vmEfficiency.show.operators = true;
            ChartManager.stackedBarChart('efc_operators_chart', data.opt_operators);
        }

        if (data.opt_shifts != null)
        {
            vmEfficiency.show.shifts = true;
            ChartManager.stackedBarChart('efc_shifts_chart', data.opt_shifts);
        }
    }


    var initVueModel = function (data)
    {
        vmEfficiency = new Vue({
            el: '#CardEfficiency',
            data: {
                kpi: data.kpi,
                total: data.total,
                overfeed: data.overfeed,
                states: data.states,
                show: {
                    historical: false,
                    operators: false,
                    shifts: false
                }
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
                    if (this.show.historical || this.show.operators || this.show.shifts)
                        return true;
                    else
                        return false;
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
        vmEfficiency.kpi = efficiency.kpi;
        vmEfficiency.total = efficiency.total;
        vmEfficiency.overfeed = efficiency.overfeed;
        vmEfficiency.states = efficiency.states;

        // chart historical
        if (data.opt_historical != null)
        {
            vmEfficiency.show.historical = true;
            ChartManager.lineChart('efc_historical_chart', data.opt_historical);
        }
        else
        {
            vmEfficiency.show.historical = false;
            ChartManager.destroyChart('efc_historical_chart');
        }

        // chart operators
        if (data.opt_operators != null)
        {
            vmEfficiency.show.operators = true;
            ChartManager.stackedBarChart('efc_operators_chart', data.opt_operators);
        }
        else
        {
            vmEfficiency.show.operators = false;
            ChartManager.destroyChart('efc_operators_chart');
        }

        // chart shifts
        if (data.opt_shifts != null)
        {
            vmEfficiency.show.shifts = true;
            ChartManager.stackedBarChart('efc_shifts_chart', data.opt_shifts);
        }
        else
        {
            vmEfficiency.show.shifts = false;
            ChartManager.destroyChart('efc_shifts_chart');
        }
    }

    return {
        init: init,
        update: update
    }

}();