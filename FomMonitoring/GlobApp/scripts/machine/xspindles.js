var XSpindles = function ()
{
    var vmSpindles;

    var init = function (data)
    {
        //var data = {
        //    vm_spindles: {
        //        spindles: [{
        //            code: 'M366',
        //            perc: 69,
        //            velocity: 17.000,
        //            workover: {
        //                power: {
        //                    days: null,
        //                    hours: null,
        //                    minutes: '10',
        //                    seconds: null
        //                },
        //                vibration: 2,
        //                heating: 3
        //            },
        //            info: {
        //                install: '01/12/2017',
        //                maintenance: '03/01/2018',
        //                change: 2
        //            },
        //            time: {
        //                work: {
        //                    days: null,
        //                    hours: null,
        //                    minutes: '10',
        //                    seconds: null
        //                },
        //                residual: {
        //                    days: null,
        //                    hours: null,
        //                    minutes: '10',
        //                    seconds: null,
        //                    elapsed: 1000,
        //                }
        //            },
        //            bands: [
        //                ['1K', 237],
        //                ['2K', 161],
        //                ['3K', 142],
        //                ['4K', 140],
        //                ['5K', 125],
        //                ['6K', 121],
        //                ['7K', 118],
        //                ['8K', 117],
        //                ['9K', 111],
        //                ['10K', 111],
        //                ['11K', 111],
        //                ['12K', 131],
        //                ['13K', 131],
        //                ['14K', 131],
        //                ['15K', 131],
        //                ['16K', 131],
        //                ['17K', 131],
        //                ['18K', 131],
        //                ['19K', 131],
        //                ['20K', 131]
        //            ]
        //        }],
        //        sorting: {
        //            code: null,
        //            time: 'asc'
        //        }
        //    }
        //}

        initVueModel(data.vm_spindles);
    }

    var initVueModel = function (data)
    {
        vmSpindles = new Vue({
            el: '#CardXSpindles',
            data: {
                spindles: data.spindles,
                sorting: data.sorting
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
        var vm_spindles = data.vm_spindles;
        vmSpindles.spindles = vm_spindles.spindles;
        vmSpindles.sorting = vm_spindles.sorting;

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