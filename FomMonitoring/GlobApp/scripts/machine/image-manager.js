var ImageManager = function () {

    var vmImageMachine;
    var vmButtonsMenu;

    var mixinDetictingMobile = {
        methods: {
            isMobile: function () {
                var check = false;
                (function (a) {

                    /*if (/Android|webOS|iPhone|iPod|BlackBerry/i.test(a) ||
                        (/iPad/i.test(a) && $(window).width() < 1200))*/
                    if ($(window).width() <= 992)
                        check = true;
                    
                })(navigator.userAgent || navigator.vendor || window.opera);
                return check;
            },
            isTablet: function () {
                var check = false;
                (function (a) {

                    /*if (/Android|webOS|iPhone|iPod|BlackBerry/i.test(a) ||
                        (/iPad/i.test(a) && $(window).width() < 1200))*/
                    if ($(window).width() > 992 && $(window).width() <= 1200)
                        check = true;

                })(navigator.userAgent || navigator.vendor || window.opera);
                return check;
            },
            isLargeTablet: function () {
                var check = false;
                (function (a) {

                    /*if (/Android|webOS|iPhone|iPod|BlackBerry/i.test(a) ||
                        (/iPad/i.test(a) && $(window).width() < 1200))*/
                    if ($(window).width() > 1200 && $(window).width() <= 1400)
                        check = true;

                })(navigator.userAgent || navigator.vendor || window.opera);
                return check;
            }
        }
    }

    var checkVisibility = function() {
        if (vmImageMachine.machinePanelSelected == 'maintenance' ||
            (!(vmImageMachine.isLargeTablet() || vmImageMachine.isTablet() || vmImageMachine.isMobile()) &&
                (vmImageMachine.machineGroupSelected == null && vmImageMachine.machinePanelSelected == null))) {
            Maintenance.show();
        } else {
            Maintenance.hide();
        }

        if (vmImageMachine.machinePanelSelected == 'efficiency' ||
            (!(vmImageMachine.isLargeTablet() || vmImageMachine.isTablet() || vmImageMachine.isMobile()) &&
                (vmImageMachine.machineGroupSelected == null && vmImageMachine.machinePanelSelected == null))) {
            Efficiency.show();
        } else {
            Efficiency.hide();
        }

        if (vmImageMachine.machinePanelSelected == 'production' ||
            (!(vmImageMachine.isLargeTablet() || vmImageMachine.isTablet() || vmImageMachine.isMobile()) &&
                (vmImageMachine.machineGroupSelected == null && vmImageMachine.machinePanelSelected == null))) {
            Productivity.show();
        } else {
            Productivity.hide();
        }

        if (vmImageMachine.machinePanelSelected == 'ordersStandard' ||
            (!(vmImageMachine.isLargeTablet() || vmImageMachine.isTablet() || vmImageMachine.isMobile()) &&
                (vmImageMachine.machineGroupSelected == null && vmImageMachine.machinePanelSelected == null))) {
            Jobs.show();
        } else {
            Jobs.hide();
        }

        if (vmImageMachine.machineGroupSelected == 'FMC3-4_axes') {
            OtherData.show();
        } else {
            OtherData.hide();
        }

        if (vmImageMachine.machineGroupSelected == 'FMC3-4_spindles') {
            ElectroSpindle.show();
        } else {
            ElectroSpindle.hide();
        }

        if (vmImageMachine.machineGroupSelected == 'FMC3-4_tools') {
            ToolsFmcLmx.show();
        } else {
            ToolsFmcLmx.hide();
        }

        if ((!(vmImageMachine.isLargeTablet() || vmImageMachine.isTablet()) &&
            (vmImageMachine.machineGroupSelected == null && vmImageMachine.machinePanelSelected == null))) {

            $(".placeholder-panel-full").show();
        } else {
            $(".placeholder-panel-full").hide();
        }

        if (((vmImageMachine.isLargeTablet() || vmImageMachine.isTablet()) &&
            (vmImageMachine.machineGroupSelected != null || vmImageMachine.machinePanelSelected != null))) {

            $(".placeholder-panel-mobile").show();
        } else {
            $(".placeholder-panel-mobile").hide();
        }


        MachineManager.initFlipAndSwipMenu();
    }


    var selectMachineGroup = function (element) {
        var group = $(element).data('group');
        vmImageMachine.machineGroupSelected = group;
        vmImageMachine.machinePanelSelected = null;
        $("g[data-highlighted] path").css("fill", "transparent");
        $("g[data-highlighted='" + group + "'] path").css("fill", "pink");

    };

    var selectPanel = function (element) {
        var panel = $(element).data('panel');

        $("[data-panel]").removeClass("selected");
        $(element).addClass("selected");

        vmImageMachine.machinePanelSelected = panel;
        vmImageMachine.machineGroupSelected = null;
        $("g[data-highlighted] path").css("fill", "transparent");
    };

    var initMachineImage = function () {
        $("g[data-group]").click(function (e) {
            e.preventDefault();
            selectMachineGroup(this);

            checkVisibility();
        });

        $(".machine-group-selection button").click(function (e) {
            e.preventDefault();
            selectMachineGroup(this);

            checkVisibility();
        });

        $("button[data-panel]").click(function (e) {
            e.preventDefault();
            selectPanel(this);

            checkVisibility();
        });

        $("#button-back-machine").click(function (e) {
            if (vmImageMachine.machineGroupSelected != null || vmImageMachine.machinePanelSelected != null) {
                e.preventDefault();
                $("g[data-highlighted] path").css("fill", "transparent");
                vmImageMachine.machineGroupSelected = null;
                vmImageMachine.machinePanelSelected = null;

                checkVisibility();
            }
        });

        checkVisibility();
    };

    var checkVisibilityImageMachine = function() {
        if (vmImageMachine.isLargeTablet()) {
            $("#image-machine-sm").show();
            $("#image-machine-lg").hide();
        } else {
            $("#image-machine-sm").hide();
            $("#image-machine-lg").show();
        }

        initMachineImage();
    }

    var init = function () {

        vmImageMachine = new Vue({
            el: '#panels-box',
            data: {
                machineGroupSelected: null,
                machinePanelSelected: null
            },
            mixins: [mixinDetictingMobile]
        });

        vmButtonsMenu = new Vue({
            el: "#buttons-bar",
            mixins: [mixinDetictingMobile]
        });

        checkVisibilityImageMachine();
        $(window).resize(function() {
            checkVisibilityImageMachine();
        });

        initMachineImage();
    };
    
    return {
        init: init
    };

}();