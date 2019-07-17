var ToolsBlitz = function ()
{
    var vmTools;

    var init = function (data)
    {
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

        initVueModel(data.vm_tools_blitz);
    }

    var initVueModel = function (data)
    {
        vmTools = new Vue({
            el: '#CardTools',
            data: {
                toolsTm: data.toolsTm,
                toolsTf: data.toolsTf,
            },
            computed: {
                colorKPI: function ()
                {
                    if (this.tools == null)
                        return 'color-no-data';

                    var color = 'color-green';                   

                    return color;
                },               
            },            
        });
    }

    var update = function (data)
    {
        // update vue model
       
        var vm_tools = data.vm_tools_blitz;
        if (vm_tools != null) {
            vmTools.toolsTm = vm_tools.toolsTm;
            vmTools.toolsTf = vm_tools.toolsTf;
        }
        
    }

    return {
        init: init,
        update: update
    }

}();