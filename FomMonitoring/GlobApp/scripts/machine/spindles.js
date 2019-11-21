var Spindles = function ()
{
    var vmSpindles;

    var init = function (data)
    {
        initVueModel(data);
    }

    var initVueModel = function (data)
    {
        vmSpindles = new Vue({
            el: '#CardSpindles',
            data: {
                spindles: data.vm_spindles.spindles,
                sorting: data.vm_spindles.sorting,
                type: data.vm_machine_info.mtype
            },
            computed: {
                sortingCode: function ()
                {
                    if (this.sorting.code != null)
                        return 'active ' + this.sorting.code;
                },
                sortingTTL: function ()
                {
                    if (this.sorting.time != null)
                        return 'active ' + this.sorting.time;
                },
                colorKPI: function ()
                {
                    if (this.spindles == null)
                        return 'color-no-data';

                    var color = 'color-green';

                    var spindleRed = _.filter(this.spindles, function (spindle)
                    {
                        if (spindle.perc > 0)
                            return spindle.time.residual.hours == null && spindle.time.residual.minutes < 60
                    });

                    if (spindleRed.length > 0)
                        color = 'color-red';

                    return color;
                }
            },
            methods: {
                refreshProgress: function ()
                {
                    $('.progress-spindle .progress-bar').css("width",
                          function () { return $(this).attr("aria-valuenow") + "%"; });
                },
                sortCode: function()
                {
                    this.sorting.time = null;

                    if (this.sorting.code == 'desc')
                    {
                        this.spindles = _.sortBy(this.spindles, 'code');
                        this.sorting.code = 'asc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }

                    if (this.sorting.code == 'asc' || this.sorting.code == null)
                    {
                        this.spindles = _.sortBy(this.spindles, 'code').reverse();
                        this.sorting.code = 'desc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }
                },
                sortTTL: function ()
                {
                    this.sorting.code = null;

                    if (this.sorting.time == 'desc')
                    {
                        this.spindles = _.sortBy(this.spindles, function (spin) { return spin.time.residual.elapsed; });
                        this.sorting.time = 'asc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }

                    if (this.sorting.time == 'asc' || this.sorting.time == null)
                    {
                        this.spindles = _.sortBy(this.spindles, function (spin) { return spin.time.residual.elapsed; }).reverse();
                        this.sorting.time = 'desc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }
                },
                borderColor: function (time, perc)
                {
                    var color = 'border-green';

                    if (perc > 0 && time.residual.hours == null && time.residual.minutes < 60)
                        color = 'border-red';

                    return color;
                },
                bgColor: function (perc)
                {
                    return {
                        'gradient-until-70': perc <= 70,
                        'gradient-after-70': perc > 70
                    }
                },
                showModal: function (spindle, event)
                {
                    event.cancelBubble = true;

                    $.jStorage.set('spindle_data', spindle);

                    var modalID = '#modal-' + spindle.code;
                    $(modalID).modal('show');

                    $(modalID).on('shown.bs.modal', function (e)
                    {
                        var jspindle = $.jStorage.get('spindle_data');

                        var chartID = 'chart_bands_' + jspindle.code;
                        ChartManager.columnChart(chartID, jspindle.opt_bands);

                        $.jStorage.deleteKey('spindle_data');

                        $(this).unbind('shown.bs.modal');
                    });
                }
            }
        });
    }

    var update = function (data)
    {
        // update vue model
        var vm_spindles = data ? data.vm_spindles : null;
        if (vm_spindles != null && vmSpindles != null) {
            vmSpindles.spindles = vm_spindles.spindles;
            vmSpindles.sorting = vm_spindles.sorting;
            vmSpindles.type = data.vm_machine_info.mtype;
        }
       
        
        //if (vmSpindles.spindles != null)
        //    $('.slimscroll').slimscroll({ scrollBy: '0px' });
        //else
        //    $(".slimscroll").slimScroll({ destroy: true });

    }

    return {
        init: init,
        update: update
    }

}();