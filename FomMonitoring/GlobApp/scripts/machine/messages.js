var Messages = function ()
{
    var vmMessages;

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
            vmMessages.show.historical = true;
            ChartManager.lineChart('msg_historical_chart', data.opt_historical);
        }

    }

    var initVueModel = function (data)
    {
        vmMessages = new Vue({
            el: '#CardMessages',
            data: {
                messages: data.vm_messages.messages,
                details: data.vm_details.messages,
                sortingDet: data.vm_details.sorting,
                sorting: data.vm_messages.sorting,
                show: {
                    historical: false
                }
            },
            computed: {
                colorAlarms: function ()
                {
                    if (this.messages == null)
                        return 'color-no-data';

                    var color = 'color-red';

                    var n_error = _.filter(this.messages, function (message)
                    {
                        return message.type == 'error'
                    });

                    var n_operator = _.filter(this.messages, function (message)
                    {
                        return message.type == 'operator'
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
                        this.$data.messages = _.sortBy(this.$data.messages, 'quantity');
                        this.sorting.quantity = 'asc';
                        return;
                    }

                    if (this.sorting.quantity == 'asc' || this.sorting.quantity == null)
                    {
                        this.$data.messages = _.sortBy(this.$data.messages, 'quantity').reverse();
                        this.sorting.quantity = 'desc';
                        return;
                    }
                },
                sortDuration: function ()
                {
                    this.sorting.quantity = null;

                    if (this.sorting.duration == 'desc')
                    {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.time.elapsed; });
                        this.sorting.duration = 'asc';
                        return;
                    }

                    if (this.sorting.duration == 'asc' || this.sorting.duration == null)
                    {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.time.elapsed; }).reverse();
                        this.sorting.duration = 'desc';
                        return;
                    }
                },
                sortTimestamp: function () {
                    
                    if (this.sortingDet.timestamp == 'desc') {
                        this.$data.details = _.sortBy(this.$data.details, function (message) { return message.timestamp; });
                        this.sortingDet.timestamp = 'asc';
                        return;
                    }

                    if (this.sortingDet.timestamp == 'asc' || this.sortingDet.timestamp == null) {
                        this.$data.details = _.sortBy(this.$data.details, function (message) { return message.timestamp; }).reverse();
                        this.sortingDet.timestamp = 'desc';
                        return;
                    }
                },
                sortGroup: function () {

                    if (this.sortingDet.group == 'desc') {
                        this.$data.details = _.sortBy(this.$data.details, function (message) { return message.group; });
                        this.sortingDet.group = 'asc';
                        return;
                    }

                    if (this.sortingDet.group == 'asc' || this.sortingDet.group == null) {
                        this.$data.details = _.sortBy(this.$data.details, function (message) { return message.group; }).reverse();
                        this.sortingDet.group = 'desc';
                        return;
                    }
                },
                showDescription: function (message, event)
                {
                    if (message.description != null && message.description != "")
                    {
                        var $this = $(event.currentTarget);
                        $this.next().toggle();
                        $this.find('.click-more > .icon-plus').toggleClass('icon-minus');
                    }
                },
                isThereDesc: function (message)
                {
                    var desc = false;

                    if (message.description != null && message.description != "")
                        desc = true;

                    return desc;
                }
            }
        });
    }

    var update = function (data)
    {
        // update vue model
        var vm_messages = data.vm_messages;
        if (vm_messages) {
            vmMessages.messages = vm_messages.messages;
            vmMessages.sorting = vm_messages.sorting;
        }
        vmMessages.details = data.vm_details.messages;
        vmMessages.sortingDet = data.vm_details.sorting;

        // update historical chart
        if (data.opt_historical != null)
        {
            vmMessages.show.historical = true;
            ChartManager.lineChart('msg_historical_chart', data.opt_historical);
        }
        else
        {
            vmMessages.show.historical = false;
            ChartManager.destroyChart('msg_historical_chart');
        }
    }

    return {
        init: init,
        update: update
    }

}();