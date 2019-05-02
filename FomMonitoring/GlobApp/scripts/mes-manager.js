var MesManager = function ()
{
    var urlMachine;
    var urlMesAPI;
    var vmMes;

    var init = function(url, urlAPI, data)
    {
        urlMachine = url;
        urlMesAPI = urlAPI;

        //var data = {
        //    machines: [{
        //        info: {
        //            id: 1,
        //            description: 'description',
        //            model: 'model',
        //            icon: 'centrolavoro'
        //        },
        //        state: {
        //            code: 'error',
        //            text: 'error'
        //        },
        //        error: 2999,
        //        job: {
        //            code: '2017-01',
        //            perc: 71
        //        },
        //        operator: 'Giovanni',
        //        efficiency: {
        //            kpi: {
        //                value: 55,
        //                threshold: {
        //                    green: 80,
        //                    yellow: 60
        //                }
        //            },
        //            overfeed: {
        //                value: 45
        //            }
        //        },
        //        productivity: {
        //            kpi: {
        //                value: 67,
        //                threshold: {
        //                    green: 80,
        //                    yellow: 60
        //                }
        //            },
        //            pieces: {
        //                total: 33
        //            }
        //        },
        //        alarms: {
        //            quantity: 11,
        //            time: {
        //                days: null,
        //                hours: '00',
        //                minutes: '10',
        //                seconds: null
        //            }
        //        }
        //    },
        //    {
        //        info: {
        //            id: 2,
        //            description: 'description',
        //            model: 'model',
        //            icon: 'lineataglio'
        //        },
        //        state: {
        //            code: 'manual',
        //            text: 'manual'
        //        },
        //        error: 2999,
        //        job: {
        //            code: '2017-01',
        //            perc: 71
        //        },
        //        operator: 'Giovanni',
        //        efficiency: {
        //            kpi: {
        //                value: 55,
        //                threshold: {
        //                    green: 80,
        //                    yellow: 60
        //                }
        //            },
        //            overfeed: {
        //                value: 45
        //            }
        //        },
        //        productivity: {
        //            kpi: {
        //                value: 67,
        //                threshold: {
        //                    green: 80,
        //                    yellow: 60
        //                }
        //            },
        //            pieces: {
        //                total: 33
        //            }
        //        },
        //        alarms: {
        //            quantity: 11,
        //            time: {
        //                days: null,
        //                hours: '00',
        //                minutes: '10',
        //                seconds: null
        //            }
        //        }
        //    },
        //    {
        //        info: {
        //            id: 3,
        //            description: 'description',
        //            model: 'model',
        //            icon: 'lineatagliolavoro'
        //        },
        //        state: {
        //            code: 'prod',
        //            text: 'production'
        //        },
        //        error: 2999,
        //        job: {
        //            code: '2017-01',
        //            perc: 71
        //        },
        //        operator: 'Giovanni',
        //        efficiency: {
        //            kpi: {
        //                value: 55,
        //                threshold: {
        //                    green: 80,
        //                    yellow: 60
        //                }
        //            },
        //            overfeed: {
        //                value: 45
        //            }
        //        },
        //        productivity: {
        //            kpi: {
        //                value: 67,
        //                threshold: {
        //                    green: 80,
        //                    yellow: 60
        //                }
        //            },
        //            pieces: {
        //                total: 33
        //            }
        //        },
        //        alarms: {
        //            quantity: 11,
        //            time: {
        //                days: null,
        //                hours: '00',
        //                minutes: '10',
        //                seconds: null
        //            }
        //        }
        //    },
        //    {
        //        info: {
        //            id: 4,
        //            description: 'description',
        //            model: 'model',
        //            icon: 'troncatrice'
        //        },
        //        state: {
        //            code: 'pause',
        //            text: 'pause'
        //        },
        //        error: 2999,
        //        job: {
        //            code: '2017-01',
        //            perc: 71
        //        },
        //        operator: 'Giovanni',
        //        efficiency: {
        //            kpi: {
        //                value: 55,
        //                threshold: {
        //                    green: 80,
        //                    yellow: 60
        //                }
        //            },
        //            overfeed: {
        //                value: 45
        //            }
        //        },
        //        productivity: {
        //            kpi: {
        //                value: 67,
        //                threshold: {
        //                    green: 80,
        //                    yellow: 60
        //                }
        //            },
        //            pieces: {
        //                total: 33
        //            }
        //        },
        //        alarms: {
        //            quantity: 11,
        //            time: {
        //                days: null,
        //                hours: '00',
        //                minutes: '10',
        //                seconds: null
        //            }
        //        }
        //    }]
        //};

        initVueModel(data);

        Vue.nextTick(function ()
        {
            initFlipCard();
            $('#MesLevel').addClass('show');
        });
    }

    var initFlipCard = function()
    {
        $(".card-portlet").flip({
            axis: "y",
            reverse: true,
            trigger: "manual"
        });

        $(".js-toggle-card").click(function (event)
        {
            event.cancelBubble = true;
            var cardID = '.' + $(this).data('cardid');
            $(cardID).flip('toggle');
        });
    }

    var initVueModel = function (data)
    {
        vmMes = new Vue({
            el: '#MesLevel',
            data: {
                machines: data.machines
            },
            filters: {
                round: function (value)
                {
                    return Math.round(value);
                }
            },
            methods: {
                locationMachine: function(machineID)
                {
                    var url = urlMachine + '/' + machineID;
                    location.href = url;
                },
                isOffline: function (machine) {
                    if (machine.state == null &&
                        machine.job == null &&
                        machine.operator == null &&
                        machine.efficiency == null &&
                        machine.productivity == null &&
                        machine.alarms == null)
                        return true;
                },
                showOverlay: function(machine)
                {
                    if (machine.state == null &&
                        machine.job == null &&
                        machine.operator == null &&
                        machine.efficiency == null &&
                        machine.productivity == null &&
                        machine.alarms == null)
                        return false;//return true;
                },
                iconState: function (state)
                {
                    return {
                        'fa-play': state.code == 'prod',
                        'icofom-pause': state.code == 'pause',
                        'icofom-manual': state.code == 'manual',
                        'fa-exclamation-triangle': state.code == 'error'
                    }
                },
                iconMachine: function(machine)
                {
                    return 'icofom-' + machine.info.icon /*+ ' cr-' + machine.state.code*/;
                },
                colorJob: function(perc)
                {
                    return {
                        'color-progress': perc < 100,
                        'color-green': perc == 100,
                    }
                },
                iconJob: function (perc)
                {
                    return {
                        'icofom-jobs': perc < 100,
                        'fa-check': perc == 100,
                    }
                },
                colorKPI: function (kpi)
                {
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
                },
                colorAlarms: function(number)
                {
                    return {
                        'color-green': number == 0,
                        'color-red': number > 0
                    }
                }
            }
        });
    }

    var initVueComponents = function () {
        Vue.component('no-data', {
            props: ['show'],
            template: '#no-data'
        });

        Vue.component('offline-machine', {
            props: ['show'],
            template: '#offline-machine'
        });

        Vue.component('modal-tool', {
            props: ['tool'],
            template: '#modal-tool'
        });

        Vue.component('modal-spindle', {
            props: ['spindle'],
            template: '#modal-spindle'
        });
    }

    var callAjaxMesViewModelData = function (plantID)
    {
        var request = $.ajax({
            type: "POST",
            url: urlMesAPI,
            contentType: 'application/json',
            data: plantID,
            beforeSend: function ()
            {
                WaitmeManager.start('body');
            },
            complete: function ()
            {
                WaitmeManager.end('body');
            }
        });

        request.done(function (data)
        {
            vmMes.machines = data.machines;
            Vue.nextTick(function ()
            {
                initFlipCard();
                $('#MesLevel').addClass('show');
            });
        });

        request.fail(function (jqXHR, textStatus, errorThrown)
        {
            location.reload();
        });
    }

    return {
        init: init,
        callAjaxMesViewModelData: callAjaxMesViewModelData,
        initVueComponents: initVueComponents
    }
}();