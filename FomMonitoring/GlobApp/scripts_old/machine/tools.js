var Tools = function ()
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

        initVueModel(data.vm_tools);
    }

    var initVueModel = function (data)
    {
        vmTools = new Vue({
            el: '#CardTools',
            data: {
                tools: data.tools,
                sorting: data.sorting
            },
            computed: {
                colorKPI: function ()
                {
                    if (this.tools == null)
                        return 'color-no-data';

                    var color = 'color-green';

                    var toolRed = _.filter(this.tools, function (tool)
                    {
                        return tool.perc > 70
                    });

                    if (toolRed.length > 0)
                        color = 'color-red';

                    return color;
                },
                sortingCode: function ()
                {
                    if (this.sorting.code != null)
                        return 'active ' + this.sorting.code;
                },
                sortingTTL: function ()
                {
                    if (this.sorting.time != null)
                        return 'active ' + this.sorting.time;
                }
            },
            methods: {
                bgColor: function (perc)
                {
                    return {
                        'gradient-until-70': perc <= 70,
                        'gradient-after-70': perc > 70
                    }
                },
                borderColor: function (perc)
                {
                    return {
                        'border-green': perc <= 70,
                        'border-red': perc > 70
                    }
                },
                refreshProgress: function ()
                {
                    $('.progress-tool .progress-bar').css("width",
                          function () { return $(this).attr("aria-valuenow") + "%"; });
                },
                sortCode: function ()
                {
                    this.sorting.time = null;

                    if (this.sorting.code == 'desc')
                    {
                        this.tools = _.sortBy(this.tools, 'code');
                        this.sorting.code = 'asc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }

                    if (this.sorting.code == 'asc' || this.sorting.code == null)
                    {
                        this.tools = _.sortBy(this.tools, 'code').reverse();
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
                        this.tools = _.sortBy(this.tools, function (tool) { return tool.time.elapsed; });
                        this.sorting.time = 'asc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }

                    if (this.sorting.time == 'asc' || this.sorting.time == null)
                    {
                        this.tools = _.sortBy(this.tools, function (tool) { return tool.time.elapsed; }).reverse();
                        this.sorting.time = 'desc';

                        this.$nextTick(function ()
                        {
                            this.refreshProgress();
                        });

                        return;
                    }
                },
                showModal: function (tool, event)
                {
                    if (tool.changes.historical.length > 0)
                    {
                        event.cancelBubble = true;
                        var modalID = '#modal-' + tool.code;
                        $(modalID).modal('show');
                    }
                }
            }
        });
    }

    var update = function (data)
    {
        // update vue model
        var vm_tools = data.vm_tools;
        vmTools.tools = vm_tools.tools;
        vmTools.sorting = vm_tools.sorting;
    }

    return {
        init: init,
        update: update
    }

}();