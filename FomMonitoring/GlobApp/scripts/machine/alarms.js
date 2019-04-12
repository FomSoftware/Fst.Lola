var Alarms = function ()
{
    var vmAlarms;

    var init = function (data)
    {
        //var data = {
        //    vm_alarms: {
        //        alarms: [{
        //            code: '2001',
        //            type: 'error',
        //            time: {
        //                days: null,
        //                hours: '01',
        //                minutes: '23',
        //                seconds: null,
        //                elapsed: 3000
        //            },
        //            quantity: 1,
        //            description: 'bla bla bla 1'
        //        }, {
        //            code: '2002',
        //            type: 'operator',
        //            time: {
        //                days: null,
        //                hours: '01',
        //                minutes: '22',
        //                seconds: null,
        //                elapsed: 2000
        //            },
        //            quantity: 2,
        //            description: 'bla bla bla 2'
        //        }, {
        //            code: '2003',
        //            type: 'operator',
        //            time: {
        //                days: null,
        //                hours: '01',
        //                minutes: '21',
        //                seconds: null,
        //                elapsed: 1000
        //            },
        //            quantity: 3,
        //            description: 'bla bla bla 3'
        //        }],
        //        sorting: {
        //            duration: 'desc',
        //            quantity: null
        //        }
        //    }
        //}

        initVueModel(data);

        if (data.opt_historical != null)
        {
            vmAlarms.show.historical = true;
            ChartManager.lineChart('alm_historical_chart', data.opt_historical);
        }

    }

    var initVueModel = function (data)
    {
        vmAlarms = new Vue({
            el: '#CardAlarms',
            data: {
                alarms: data.vm_alarms.alarms,
                details: data.vm_details.alarms,
                sortingDet: data.vm_details.sorting,
                sorting: data.vm_alarms.sorting,
                show: {
                    historical: false
                }
            },
            computed: {
                colorAlarms: function ()
                {
                    if (this.alarms == null)
                        return 'color-no-data';

                    var color = 'color-red';

                    var n_error = _.filter(this.alarms, function (alarm)
                    {
                        return alarm.type == 'error'
                    });

                    var n_operator = _.filter(this.alarms, function (alarm)
                    {
                        return alarm.type == 'operator'
                    });

                    if (n_operator > n_error)
                        color = 'color-yellow';

                    return color;
                }
            },
            methods: {
                colorClass: function (type)
                {
                    return {
                        'color-red': type == 'error',
                        'color-yellow': type == 'operator',
                        'color-orange': type == 'warning'
                    }
                },
                iconClass: function (type)
                {
                    return {
                        'fa-exclamation-triangle color-red': type == 'error',
                        'fa-exclamation-triangle color-yellow': type == 'operator',
                        'fa-exclamation-circle color-orange': type == 'warning'
                    }
                },
                sortQuantity: function ()
                {
                    this.sorting.duration = null;

                    if (this.sorting.quantity == 'desc')
                    {
                        this.$data.alarms = _.sortBy(this.$data.alarms, 'quantity');
                        this.sorting.quantity = 'asc';
                        return;
                    }

                    if (this.sorting.quantity == 'asc' || this.sorting.quantity == null)
                    {
                        this.$data.alarms = _.sortBy(this.$data.alarms, 'quantity').reverse();
                        this.sorting.quantity = 'desc';
                        return;
                    }
                },
                sortDuration: function ()
                {
                    this.sorting.quantity = null;

                    if (this.sorting.duration == 'desc')
                    {
                        this.$data.alarms = _.sortBy(this.$data.alarms, function (alarm) { return alarm.time.elapsed; });
                        this.sorting.duration = 'asc';
                        return;
                    }

                    if (this.sorting.duration == 'asc' || this.sorting.duration == null)
                    {
                        this.$data.alarms = _.sortBy(this.$data.alarms, function (alarm) { return alarm.time.elapsed; }).reverse();
                        this.sorting.duration = 'desc';
                        return;
                    }
                },
                sortTimestamp: function () {
                    
                    if (this.sortingDet.timestamp == 'desc') {
                        this.$data.details = _.sortBy(this.$data.details, function (alarm) { return alarm.timestamp; });
                        this.sortingDet.timestamp = 'asc';
                        return;
                    }

                    if (this.sortingDet.timestamp == 'asc' || this.sortingDet.timestamp == null) {
                        this.$data.details = _.sortBy(this.$data.details, function (alarm) { return alarm.timestamp; }).reverse();
                        this.sortingDet.timestamp = 'desc';
                        return;
                    }
                },
                showDescription: function (alarm, event)
                {
                    if (alarm.description != null && alarm.description != "")
                    {
                        var $this = $(event.currentTarget);
                        $this.next().toggle();
                        $this.find('.click-more > .icon-plus').toggleClass('icon-minus');
                    }
                },
                isThereDesc: function (alarm)
                {
                    var desc = false;

                    if (alarm.description != null && alarm.description != "")
                        desc = true;

                    return desc;
                }
            }
        });
    }

    var update = function (data)
    {
        // update vue model
        var vm_alarms = data.vm_alarms;
        if (vm_alarms) {
            vmAlarms.alarms = vm_alarms.alarms;
            vmAlarms.sorting = vm_alarms.sorting;
        }
        vmAlarms.details = data.vm_details.alarms;
        vmAlarms.sortingDet = data.vm_details.sorting;

        // update historical chart
        if (data.opt_historical != null)
        {
            vmAlarms.show.historical = true;
            ChartManager.lineChart('alm_historical_chart', data.opt_historical);
        }
        else
        {
            vmAlarms.show.historical = false;
            ChartManager.destroyChart('alm_historical_chart');
        }
    }

    return {
        init: init,
        update: update
    }

}();