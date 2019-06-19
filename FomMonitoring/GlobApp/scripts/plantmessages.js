var PlantMessages = function ()
{
    var vmMessages;
    var urlPlantMessagesAPI;

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
        initScrollBar();
    }

    var initVueModel = function (data)
    {
        Vue.component('no-data', {
            props: ['show'],
            template: '#no-data'
        });

        vmMessages = new Vue({
            el: '#CardPlantMessages',
            data: {
                messages: data.messages,          
                sorting: data.sorting,
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
                        return message.message.type == 'error'
                    });

                    var n_operator = _.filter(this.messages, function (message)
                    {
                        return message.message.type == 'operator'
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
                sortTimestamp: function () {
                    this.sorting.group = null;
                    this.sorting.model = null;
                    this.sorting.serial = null;
                    this.sorting.duration = null;

                    if (this.sorting.timestamp == 'desc') {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.message.timestamp; });
                        this.sorting.timestamp = 'asc';
                        return;
                    }

                    if (this.sorting.timestamp == 'asc' || this.sorting.timestamp == null) {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.message.timestamp; }).reverse();
                        this.sorting.timestamp = 'desc';
                        return;
                    }
                },
                sortDuration: function () {
                    this.sorting.group = null;
                    this.sorting.model = null;
                    this.sorting.serial = null;
                    this.sorting.timestamp = null;
                   
                    if (this.sorting.duration == 'desc') {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.message.time.elapsed; });
                        this.sorting.duration = 'asc';
                        return;
                    }

                    if (this.sorting.duration == 'asc' || this.sorting.duration == null) {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.message.time.elapsed; }).reverse();
                        this.sorting.duration = 'desc';
                        return;
                    }
                },
                sortGroup: function () {
                    this.sorting.timestamp = null;
                    this.sorting.model = null;
                    this.sorting.serial = null;
                    this.sorting.duration = null;
                    this.sorting.timestamp = null;

                    if (this.sorting.group == 'desc') {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.message.group; });
                        this.sorting.group = 'asc';
                        return;
                    }

                    if (this.sorting.group == 'asc' || this.sorting.group == null) {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.message.group; }).reverse();
                        this.sorting.group = 'desc';
                        return;
                    }
                },
                sortSerialMachine: function () {
                    this.sorting.group = null;
                    this.sorting.timestamp = null;
                    this.sorting.model = null;
                    this.sorting.duration = null;
                    this.sorting.timestamp = null;

                    if (this.sorting.serial == 'desc') {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.message.serial; });
                        this.sorting.serial = 'asc';
                        return;
                    }

                    if (this.sorting.serial == 'asc' || this.sorting.serial == null) {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.message.serial; }).reverse();
                        this.sorting.serial = 'desc';
                        return;
                    }
                },
                sortModelMachine: function () {
                    this.sorting.group = null;
                    this.sorting.timestamp = null;
                    this.sorting.serial = null;
                    this.sorting.duration = null;
                    this.sorting.timestamp = null;

                    if (this.sorting.model == 'desc') {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.message.model; });
                        this.sorting.model = 'asc';
                        return;
                    }

                    if (this.sorting.model == 'asc' || this.sorting.model == null) {
                        this.$data.messages = _.sortBy(this.$data.messages, function (message) { return message.message.model; }).reverse();
                        this.sorting.model = 'desc';
                        return;
                    }
                },
                showDescription: function (message, event)
                {
                    if (message.message.description != null && message.message.description != "")
                    {
                        var $this = $(event.currentTarget);
                        $this.next().toggle();
                        $this.find('.click-more > .icon-plus').toggleClass('icon-minus');
                    }
                },               
                isThereDesc: function (message)
                {
                    var desc = false;

                    if (message.message.description != null && message.message.description != "")
                        desc = true;

                    return desc;
                }
            }
        });
    }

    var update = function (data)
    {
        // update vue model
        var vm_messages = data.messages;
        if (vm_messages) {
            vmMessages.messages = vm_messages;
            vmMessages.sorting = data.sorting;
        }
        
       if (vmMessages)
        {
            vmMessages.show.historical = false;
            ChartManager.destroyChart('msg_historical_chart');
        }
       
       
    }

    var callAjaxPlantMessagesViewModelData = function (filters) {
        var request = $.ajax({
            type: "POST",
            url: PlantMessages.urlPlantMessagesAPI,
            contentType: 'application/json',
            data: JSON.stringify(filters),
            beforeSend: function () {
                WaitmeManager.start('body');
            },
            complete: function () {
                WaitmeManager.end('body');
            }
        });

        request.done(function (data) {
            $(".slimscroll").slimScroll({ destroy: true });
            PlantMessages.update(data);
            Vue.nextTick(function () {               
                initScrollBar();
            });

        });

        request.fail(function (jqXHR, textStatus, errorThrown) {
            console.debug(jqXHR);
            console.debug(textStatus);
            console.debug(errorThrown);
            location.reload();
        });
    }

    var initScrollBar = function () {
        var TableWidth = $($('#slimscroll-plant-msg-wrapper .table-header tr')[0]).outerWidth();
        var FrontWidth = $($('.front')[0]).outerWidth();
        var delta = TableWidth - FrontWidth;
        //caso mobile:
        if (delta > 0)
            TableWidth = TableWidth + 12;

        $('.slimscroll').slimScroll({
            size: '5px',
            height: '82vh',
            alwaysVisible: false,
            //wheelStep: 10,
            touchScrollStep: 35,
            color: '#999',
            allowPageScroll: true,
            width: TableWidth
        });

        //caso IPad e PC o schermi più larghi
        if (delta <= 0)
        {
            $($('.front')[0]).css("overflow-x", "hidden");
        }
    }

    return {
        init: init,
        callAjaxPlantMessagesViewModelData: callAjaxPlantMessagesViewModelData,
        update: update,
 
    }

}();