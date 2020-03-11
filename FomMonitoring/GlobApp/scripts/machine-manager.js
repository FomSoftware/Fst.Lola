var MachineManager = function ()
{
    var urlMachineAPI;
    var urls;

    var init = function (url)
    {
        initFlipAndSwipMenu();

        urlMachineAPI = url.urlMachine;
        urls = url;
        // DOM updated
        Vue.nextTick(function ()
        {
            $('#MachineLevel').addClass('show');
            initFlipCard();
            initProgressBar();
            initScrollBar();             
        });

        
    };

    var buildRequest = function (url, filters, panelId) {
        console.log(url);
        return $.ajax({
            type: "POST",
            url: url,
            contentType: 'application/json',
            data: JSON.stringify(filters),
            beforeSend: function () {
                if(panelId != null)
                    WaitmeManager.start(panelId);
            },
            completed: function() {
                if (panelId != null)
                    WaitmeManager.end(panelId);
            }
        });
    };

    var callAjaxMachineMessageViewModelData = function(filters) {
        WaitmeManager.start("#CardMessages");
        $.when(
                buildRequest(urls.urlMessages, filters))
            .done(function (messages) {
                $(".slimscroll").slimScroll({ destroy: true });

                Messages.update(messages);

                Vue.nextTick(function () {
                    initProgressBar();
                    initFlipCard();
                    initScrollBar();
                });


            }).always(function () {
                WaitmeManager.end("#CardMessages");
            });
    }

    var callAjaxMachineViewModelData = function (filters) {
        console.log(JSON.stringify(urls));
        WaitmeManager.start("body");
        $.when(buildRequest(urls.urlEfficiency, filters),
            buildRequest(urls.urlProductivity, filters),
            buildRequest(urls.urlJob, filters),
            buildRequest(urls.urlMaintenance, filters),
            buildRequest(urls.urlMessages, filters),
            buildRequest(urls.urlParameters, filters),
            buildRequest(urls.urlXTools, filters),
            buildRequest(urls.urlTools, filters),
            buildRequest(urls.urlToolsBlitz, filters))
            .done(function(efficiency, productivity, job, maintenance, messages, parameters, xTools, tools, toolsBlitz) {
                $(".slimscroll").slimScroll({ destroy: true });

                Efficiency.update(efficiency[0]);
                Productivity.update(productivity[0]);
                ToolsBlitz.update(toolsBlitz[0]);
                Tools.update(tools[0]);
                XTools.update(xTools[0]);
                Jobs.update(job[0]);
                OtherMachinesJobs.update(job[0]);
                Messages.update(messages[0]);
                Maintenance.update(maintenance[0]);
                MotorAxesBlitz.update(parameters[0]);
                MotorKeope.update(parameters[0]);
                AxesKeope.update(parameters[0]);
                ElectroSpindle.update(parameters[0]);
                OtherData.update(parameters[0]);
                MultiSpindles.update(parameters[0]);

                Vue.nextTick(function () {
                    initProgressBar();
                    initFlipCard();
                    initScrollBar();
                });


            }).always(function() {
                WaitmeManager.end("body");
            });

    };

    var initProgressBar = function ()
    {
        $('.progress .progress-bar').css("width",
           function () { return $(this).attr("aria-valuenow") + "%"; });


        $('.overfeed .overfeed-bar').css("height",
            function () { return $(this).attr("aria-valuenow") + "%"; });
    };

    var initScrollBar = function ()
    {
        $('.slimscroll').each(function () {

            var hh = 244;

            

            // se sono in una tabella con il titolo fisso lo slimscroll contiene solo il tbody e non il theader
            var tableContainer = $(this).closest('.table-container');
            if (tableContainer.length > 0) {             
                hh = hh - tableContainer.first().children('.table-header').height();
            }

            if ($(this).attr('id') == "maintenanceSlimscroll" )
                hh = '95%';
            else if ($(this).attr('id') == "sensorSlimscroll")
                hh = '100%';
            else
                hh = hh + 'px';
            
            $(this).slimScroll({
                size: '5px',
                height: hh,
                alwaysVisible: false,
                //wheelStep: 10,
                touchScrollStep: 35,
                color: '#999',
                allowPageScroll: true
            });
        });
    };


    var initFlipCard = function ()
    {
        $(".card").flip({
            axis: "y",
            reverse: true,
            trigger: "click"
        });
    };


    var setGroupActive = function(group, itemActive) {
        $("[data-group='" + group + "']").removeClass("active");
        $(itemActive + "[data-group='" + group + "']").addClass("active");
    }




    var initFlipAndSwipMenu = function ()
    {
        $(".card-portlet").flip({
            axis: "y",
            reverse: true,
            trigger: "manual"
        });

        $(".js-flip").click(function (e)
        {
            $(this).parent().find('.active').removeClass('active');
            $(this).addClass('active');
            
            var card_class = '.card-' + $(this).data('group');
            $(card_class + " .front").children().hide();
            $(card_class).flip(true);
            $(card_class + " .back").children().show();
        });

        $(".js-unflip").click(function (e)
        {
            $(this).parent().find('.active').removeClass('active');
            $(this).addClass('active');

            var card_class = '.card-' + $(this).data('group');


            $(card_class + " .back").children().hide();
            $(card_class).flip(false);
            $(card_class + " .front").children().show();
        });

        var swiper_efficiency = new Swiper('.swiper-container.efficiency', {
            direction: 'horizontal',
            loop: false
        });

        var swiper_productivity = new Swiper('.swiper-container.productivity', {
            direction: 'horizontal',
            loop: false
        });


        var swiper_message = new Swiper('.swiper-container.messages', {
            direction: 'horizontal',
            loop: false
        });

        $('.slide-dashboard').click(function (e) {
            e.preventDefault();
            var group = $(this).data('group');
            if (group == 'messages') {
                showStandardPeriod("#CardMessages");
                swiper_message.slideTo(0, 300);
            }
            if (group == 'efficiency') {
                showStandardPeriod("#CardEfficiency");
                swiper_efficiency.slideTo(0, 300);
            }
            if (group == 'productivity') {
                showStandardPeriod("#CardProductivity");
                swiper_productivity.slideTo(0, 300);
            }

            setGroupActive(group, ".slide-dashboard");

        });

        $('.slide-summary').click(function (e) {
            e.preventDefault();
            var group = $(this).data('group');
            if (group == 'messages') {
                showStandardPeriod("#CardMessages");
                swiper_message.slideTo(2, 300);
            }

            setGroupActive(group, ".slide-summary");
        });


        $('.slide-history').click(function (e)
        {
            e.preventDefault();
            var group = $(this).data('group');

            if (group == 'efficiency') {
                showHistoricalPeriod("#CardEfficiency");
                swiper_efficiency.slideTo(1, 300);
            }

            if (group == 'productivity') {
                showHistoricalPeriod("#CardProductivity");
                swiper_productivity.slideTo(1, 300);
            }

            if (group == 'messages') {
                showHistoricalPeriod("#CardMessages");
                swiper_message.slideTo(1, 300);
            }


            setGroupActive(group, ".slide-history");
        });

        $('.slide-operator').click(function (e)
        {
            e.preventDefault();
            var group = $(this).data('group');

            if (group == 'efficiency') {
                showStandardPeriod("#CardEfficiency");
                swiper_efficiency.slideTo(2, 300);
            }

            if (group == 'productivity') {
                showStandardPeriod("#CardProductivity");
                swiper_productivity.slideTo(2, 300);
            }

            setGroupActive(group, ".slide-operator");
        });

        $('.slide-turni').click(function (e)
        {
            e.preventDefault();
            var group = $(this).data('group');

            if (group == 'efficiency') {
                showStandardPeriod("#CardEfficiency");
                swiper_efficiency.slideTo(3, 300);
            }

            if (group == 'productivity') {
                showStandardPeriod("#CardProductivity");
                swiper_productivity.slideTo(3, 300);
            }

            setGroupActive(group, ".slide-turni");
        });
        

    };





    var showHistoricalPeriod = function(idCard) {
        $(idCard).find(".js-historical-period").show();
        $(idCard).find(".js-period").hide();
    }

    var showStandardPeriod = function (idCard) {
        $(idCard).find(".js-historical-period").hide();
        $(idCard).find(".js-period").show();
    }

    var initVueComponents = function ()
    {
        Vue.component('no-data', {
            props: ['show'],
            template: '#no-data'
        });

        Vue.component('modal-tool', {
            props: ['tool'],
            template: '#modal-tool'
        });

    };

    var getColorKPI = function (color_name)
    {
        var color;

        switch (color_name)
        {
            case 'color-red':
                color = '#D32337';
                break;

            case 'color-yellow':
                color = '#F9ED4B';
                break;

            case 'color-green':
                color = '#8FBF36';
                break;
        }

        return color;
    };

    return {
        init: init,
        callAjaxMachineViewModelData: callAjaxMachineViewModelData,
        callAjaxMachineMessageViewModelData: callAjaxMachineMessageViewModelData,
        initVueComponents: initVueComponents,
        getColorKPI: getColorKPI,
        initScrollBar: initScrollBar,
        initFlipAndSwipMenu: initFlipAndSwipMenu
    };

}();