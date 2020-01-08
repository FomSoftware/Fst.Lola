var SharedManager = function ()
{
    var vmLastUpdate;

    var enPage = {
        machine: 2,
        mes: 3,
        messagesMachine: 4
    }

    var filters = {
        machine: null,
        period: null
    };

    var initHeader = function (data)
    {
        initHeaderMenu();
        initMobileMenuEventHandler();
        vmLastUpdate = new Vue({
            el: '#last-update',
            data: {
                date: data.Date,//moment.utc(data.DateTime).local().format('L'),
                time: data.Time //moment.utc(data.DateTime).local().format('LTS')
            }
        });
    }

    var initToolbar = function (data)
    {
        switch (data.page)
        {
            case enPage.mes:
                initPlantsFilter(data);
                if (/Android|webOS|iPhone|iPod|BlackBerry/i.test(navigator.userAgent)) {
                    $('#plantMessagesLabel').css('display', 'none')
                }
                break;

            case enPage.messagesMachine:
                initPlantsFilter(data);
                initCalendar(data.period, data.language, data.page);
                break;

            case enPage.machine:
                initMachinesFilter(data);
                initCalendar(data.period, data.language, data.page);
                break;
        }

        Vue.nextTick(function ()
        {
            initBootstrapSelect();
        });
    }

    var viewPlantMessagesBtn = function () {
        if (/Android|webOS|iPhone|iPod|BlackBerry/i.test(navigator.userAgent)) {
            if ($('#plantMessagesLabel').css('display') == 'none') {
                $('#plantMessagesLabel').css('display', 'inline');
                $('#plants-filter').css('display', 'none');
                $('#plantIcon').css('display', 'inline');

            }
            else {
                $('#plantMessagesLabel').css('display', 'none');
                $('#plants-filter').css('display', 'inline');
                $('#plantIcon').css('display', 'none');
            }
        }
        else {
            location.href = "PlantMessages";
        }
    }

    var initPlantsFilter = function (data)
    {
        var vmPlantsFilter = new Vue({
            el: '#plants-filter',
            data: {
                selected: {
                    plant: data.selected_plant
                },
                plants: data.plants
            },
            mounted: function () {
                $('#plant_select').selectpicker(); 
                $('#machine_select').selectpicker();
            }
        });

        $('#plant_select').on('changed.bs.select', function (e)
        {
            var plantID = $(this).val();
            MesManager.callAjaxMesViewModelData(plantID);
        });
    }

    var initMachinesFilter = function (data)
    {
        var vmMachinesFilter = new Vue({
            el: '#machines-filter',
            data: {
                selected: {
                    machine: data.selected_machine
                },
                machines: data.machines,
                empty: '-'
            }
        });

        $('#machine_select').on('changed.bs.select', function (e)
        {
            var machineID = parseInt($(this).val());
            var machine = _.where(vmMachinesFilter.machines, { id: machineID })[0];

            if (machine != null)
            {
                vmMachinesFilter.selected.machine = machine;
                filters.machine = machine;
                              
                var url = machineID;
                location.href = url;
                //MachineManager.callAjaxMachineViewModelData(filters);
            }
            
        });
    }

    var initCalendar = function (period, language, page)
    {
        moment.locale(language.initial);
        var formatLabel = moment.localeData().longDateFormat('L');
        var formatCalendar = moment.localeData().longDateFormat('ll');



        

        var start = moment(period.start);
        var end = moment(period.end);


        var ranges = getRangesCalendar(language);
        var range = _.map(ranges, function (val, key)
        {
            if (val[0].format() == start.format() && val[1].format() == end.format())
                return key;
        });



        var label = _.find(range, function(el) { return el != null });

        if (_.isEmpty(label))
            label = getLabelCalendar(start, end, formatLabel);

        $('.js-period').text(label);

        setHistoricalRange(start, end, label, formatLabel);
        // first load
        $('#calendar .text-period').html(getLabelCalendar(start, end, formatCalendar));

        // init calendar
        $('#calendar').daterangepicker({
            startDate: start,
            endDate: end,
            ranges: ranges,
            locale: getLocaleCalendar(language)

        }, function (start, end, label)
        {
            // callback change period
            filters.period = {
                start: start.toDate(),
                end: end.toDate()
            };

            if (this.locale.customRangeLabel == label)
                label = getLabelCalendar(this.startDate, this.endDate, formatLabel);
            if (page == enPage.machine) {
                MachineManager.callAjaxMachineViewModelData(filters);
            }
            else {
                PlantMessages.callAjaxPlantMessagesViewModelData(filters);
            }
            

            $('#calendar .text-period').html(getLabelCalendar(start, end, formatCalendar));
            $('.js-period').text(label);
            setHistoricalRange(this.startDate, this.endDate, label, formatLabel);
        });
    }

    var setHistoricalRange = function(start, end, defaultLabel, formatLabel) {
        var hstart = moment(start);
        var hend = moment(end);
        var diff = hend.diff(hstart, 'days');
        if (diff <= 7) {
            var historicalStart = moment(hend).subtract(6, 'days');

            var historicalLabel = getLabelCalendar(historicalStart, hend, formatLabel);
            $('.js-historical-period').text(historicalLabel);
        } else {
            $('.js-historical-period').text(defaultLabel);
        }
    }

    var getRangesCalendar = function (language)
    {
        var labels = language.labels;

        var ranges = _.object([
            [labels.Today, [moment(), moment()]],
            [labels.Yesterday, [moment().subtract(1, 'days'), moment().subtract(1, 'days')]],
            [labels.Last7Days, [moment().subtract(6, 'days'), moment()]],
            [labels.Last30Days, [moment().subtract(29, 'days'), moment()]],
            [labels.ThisMonth, [moment().startOf('month'), moment().endOf('month')]],
            [labels.LastMonth, [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]],
        ]);

        return ranges;
    }

    var getLocaleCalendar = function (language)
    {

        var moment_locale = moment().locale(language.initial).localeData();

        var daysOfWeek = moment_locale._weekdaysMin;
        var monthNames = moment_locale._monthsShort;

        var labels = language.labels;

        var locale = {
            applyLabel: labels.ApplyLabel,
            cancelLabel: labels.CancelLabel,
            customRangeLabel: labels.CustomRangeLabel,
            daysOfWeek: daysOfWeek,
            monthNames: monthNames,
            firstDay: 1
        };

        return locale;
    }

    var getLabelCalendar = function (start, end, format)
    {
        var startFormat = start.format(format);
        var endFormat = end.format(format);
        var label;

        if (startFormat == endFormat)
            label = startFormat;
        else
            label = startFormat + ' - ' + endFormat;

        return label
    }

    var initMobileMenuEventHandler = function ()
    {
        $('.js-open-menu').click(function (e)
        {
            e.preventDefault();
            $('#mobile-menu').css('right', '0');
        });

        $('.js-close-menu').click(function (e)
        {
            e.preventDefault();
            var dim = '-' + $('#mobile-menu').width() + 'px';
            $('#mobile-menu').css('right', dim);
        });
    }

    var updateLastUpdate = function (data)
    {
        if (data) {
            vmLastUpdate.date = data.Date;
            vmLastUpdate.time = data.Time;
        }
       
    }

    var handlerDropdown = function (dropdown)
    {
        if ($(dropdown).find('.dropdown-content').hasClass('show'))
            $('.dropdown-content.show').removeClass('show');
        else
        {
            $('.dropdown-content.show').removeClass('show');
            $(dropdown).find('.dropdown-content').addClass('show');
        }
    }

    var initBootstrapSelect = function ()
    {
        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent))
        {
            $('.bs-select').selectpicker({
                mobile: true,
                iconBase: 'fa',
                tickIcon: 'fa-check'
            });
        }
        else
        {
            $('.bs-select').selectpicker({
                iconBase: 'fa',
                tickIcon: 'fa-check'
            });
        }
    }

    var initHeaderMenu = function ()
    {
        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent))
        {
            $('.btn-mobile').show();
            $('.desktop-menu').hide();
        }
        else
        {
            $('.btn-mobile').hide();
            $('.desktop-menu').show();
        }
    }

    return {
        initHeader: initHeader,
        initToolbar: initToolbar,
        handlerDropdown: handlerDropdown,
        updateLastUpdate: updateLastUpdate,
        initBootstrapSelect: initBootstrapSelect,
        viewPlantMessagesBtn: viewPlantMessagesBtn
    }

}();