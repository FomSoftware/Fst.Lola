var AxesKeope = function () {
    var vmAxes;

    var init = function (data) {
        //var data = {
        //    vm_tools: {
        //        tools: [{
        //            code: 'P164',
        //            description: 'Punte arrotondate',
        //            perc: 99,
        //            changes: {
        //                total: 5,
        //                breaking: 3,
        //                replacement: 2,
        //                historical: [{
        //                    date: '10/12/2017',
        //                    type: 'breaking',
        //                    duration: {
        //                        days: null,
        //                        hours: '01',
        //                        minutes: '55',
        //                        seconds: null
        //                    }
        //                }]
        //            },
        //            time: {
        //                days: null,
        //                hours: null,
        //                minutes: '10',
        //                seconds: null,
        //                elapsed: 2000
        //            }
        //        }, {
        //            code: 'P165',
        //            description: 'Punte arrotondate',
        //            perc: 70,
        //            changes: {
        //                total: 5,
        //                breaking: 3,
        //                replacement: 2,
        //                historical: [{
        //                    date: '10/12/2017',
        //                    type: 'breaking',
        //                    duration: {
        //                        days: null,
        //                        hours: '01',
        //                        minutes: '55',
        //                        seconds: null
        //                    }
        //                }]
        //            },
        //            time: {
        //                days: null,
        //                hours: '10',
        //                minutes: '20',
        //                seconds: null,
        //                elapsed: 100
        //            }
        //        }],
        //        sorting: {
        //            code: null,
        //            time: 'asc'
        //        }
        //    }
        //};

        initVueModel(data.vm_axes_keope);
    }

    var initVueModel = function (data) {
        vmAxes = new Vue({
            el: '#CardAxesKeope',
            data: {
                axes: data.axes,
            },
            computed: {
                colorKPI: function () {
                    if ( this.axes == null || this.axes.length == 0)
                        return 'color-no-data';

                    var color = 'color-darkgreen';

                    return color;
                },
            },
        });
    }

    var update = function (data) {
        // update vue model

        var vm_axes = data.vm_axes_keope;
        if (vm_axes != null) {
            vmAxes.axes = vm_axes.axes;
        }

    }

    return {
        init: init,
        update: update
    }

}();