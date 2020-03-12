var Productivity = function ()
{
    var vmProductivity;

    var init = function (data)
    {
        initVueModel(data.vm_productivity, data.vm_machine_info);

        if (data.opt_historical != null)
        {
            vmProductivity.show.historical = true;
            ChartManager.dualAxesColumnChart('prd_historical_chart', data.opt_historical);
        }

        if (data.opt_operators != null)
        {
            vmProductivity.show.operators = true;
            ChartManager.dualAxesBarChart('prd_operators_chart', data.opt_operators);
        }

    }

    var show = function () {
        vmProductivity.showed = true;
    }

    var hide = function () {
        vmProductivity.showed = false;
    }

    var initVueModel = function (data, infoMachine)
    {
        vmProductivity = new Vue({
            el: '#CardProductivity',
            data: {
                type: infoMachine.mtype,
                machineModel: infoMachine.model,
                kpi: data.kpi,
                piece: data.piece,
                material: data.material,
                phases: data.phases,
                operators: data.operators,
                time: data.time,
                productionVariables: data.productionVariables,
                currentState: data.currentState,
                show: {
                    historical: false,
                    operators: false
                },
                showed: true
            },
            filters: {
                round: function (value)
                {
                    return Math.round(value);
                }
            },
            computed: {
                showDashboard: function ()
                {
                    if (this.piece == null && this.material == null
                        && this.phases == null && this.operators == null)
                        return false;
                    else
                        return true;
                },
                showDetails: function()
                {
                    if (this.show.historical || this.show.operators)
                        return true;
                    else
                        return false;
                },
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
                }
            },
            methods: {
                bgPhase: function (index)
                {
                    return {
                        'bg-first-phase': index == 0,
                        'bg-second-phase': index == 1,
                        'bg-third-phase': index == 2,
                    }
                },
                bgOperator: function (index)
                {
                    return {
                        'bg-first-oper': index == 0,
                        'bg-second-oper': index == 1,
                        'bg-third-oper': index == 2,
                    }
                }
            }
        });
    }

    var update = function (data)
    {
        // update vue model
        var vm_prod = data.vm_productivity;
        vmProductivity.type = data.vm_machine_info.mtype;
        vmProductivity.machineModel = data.vm_machine_info.model;
        vmProductivity.kpi = vm_prod.kpi;
        vmProductivity.piece = vm_prod.piece;
        vmProductivity.material = vm_prod.material;
        vmProductivity.phases = vm_prod.phases;
        vmProductivity.operators = vm_prod.operators;
        vmProductivity.time = vm_prod.time;
        vmProductivity.productionVariables = vm_prod.productionVariables;
        vmProductivity.currentState = vm_prod.currentState;

        // chart historical
        if (data.opt_historical != null)
        {
            vmProductivity.show.historical = true;
            ChartManager.dualAxesColumnChart('prd_historical_chart', data.opt_historical);
        }
        else
        {
            vmProductivity.show.historical = false;
            ChartManager.destroyChart('prd_historical_chart');
        }

        // chart operators
        if (data.opt_operators != null)
        {
            vmProductivity.show.operators = true;
            ChartManager.dualAxesBarChart('prd_operators_chart', data.opt_operators);
        }
        else
        {
            vmProductivity.show.operators = false;
            ChartManager.destroyChart('prd_operators_chart');
        }

    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();