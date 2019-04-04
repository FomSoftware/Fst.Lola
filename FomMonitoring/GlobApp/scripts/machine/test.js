var Test = function ()
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

        

    }

    var initVueModel = function (data)
    {
        vmAlarms = new Vue({
            el: '#CardListTest',
            data: {
                alarms: data.vm_errori,
                show: {
                    historical: false
                }
            },
            computed: {
                colorAlarms: function ()
                {
                    if (this.alarms == null)
                        return 'color-no-data';


                }
            }
        });
    }

    var update = function (data)
    {
        // update vue model
        var vm_alarms = data.vm_errori;
        vmAlarms.alarms = vm_alarms;
 
    }

    return {
        init: init,
        update: update
    }

}();