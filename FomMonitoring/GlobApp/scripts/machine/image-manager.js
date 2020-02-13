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
            (!(vmImageMachine.isTablet() || vmImageMachine.isMobile()) &&
                (vmImageMachine.machineGroupSelected == null && vmImageMachine.machinePanelSelected == null))) {
            Maintenance.show();
        } else {
            Maintenance.hide();
        }

        if (vmImageMachine.machinePanelSelected == 'efficiency' ||
            (!(vmImageMachine.isTablet() || vmImageMachine.isMobile()) &&
                (vmImageMachine.machineGroupSelected == null && vmImageMachine.machinePanelSelected == null))) {
            Efficiency.show();
        } else {
            Efficiency.hide();
        }

        if (vmImageMachine.machinePanelSelected == 'production' ||
            (!(vmImageMachine.isTablet() || vmImageMachine.isMobile()) &&
                (vmImageMachine.machineGroupSelected == null && vmImageMachine.machinePanelSelected == null))) {
            Productivity.show();
        } else {
            Productivity.hide();
        }

        if (vmImageMachine.machinePanelSelected == 'ordersStandard' ||
            (!(vmImageMachine.isTablet() || vmImageMachine.isMobile()) &&
                (vmImageMachine.machineGroupSelected == null && vmImageMachine.machinePanelSelected == null))) {
            Jobs.show();
        } else {
            Jobs.hide();
        }

        if (vmImageMachine.modello == 'FMC') {
            if (vmImageMachine.machineGroupSelected == 'FMC3-4_axes') {
                OtherData.show();
            } else  {
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
        }
        else if (vmImageMachine.modello == 'LMX') {
            if (vmImageMachine.machineGroupSelected == 'LMX650_Motor/Blade') {
               
            } else {
               
            }
            if (vmImageMachine.machineGroupSelected == 'LMX650_XMU') {
               
            } else {
              
            }
            if (vmImageMachine.machineGroupSelected == 'LMX650_StepIn/Out') {
              
            } else {
               
            }
            if (vmImageMachine.machineGroupSelected == 'LMX650_MM') {
               
            } else {
               
            }
        }


        if ((!(vmImageMachine.isLargeTablet() || vmImageMachine.isTablet()) &&
            (vmImageMachine.machineGroupSelected == null && vmImageMachine.machinePanelSelected == null))) {

            $(".placeholder-panel-full").show();
        } else {
            $(".placeholder-panel-full").hide();
        }

        if ((vmImageMachine.isTablet()) &&
            (vmImageMachine.machineGroupSelected != null || vmImageMachine.machinePanelSelected != null)) {
            $(".placeholder-panel-mobile").show();
        } else {
            $(".placeholder-panel-mobile").hide();
        }


        if ((vmImageMachine.isTablet() || vmImageMachine.isMobile() || vmImageMachine.isLargeTablet())) {
            $('#MsgPanel').removeClass("col-xl-6 col-lg-6");
            $('#MsgPanel').addClass("col-xl-12 col-lg-12");
        } else {
            $('#MsgPanel').removeClass("col-xl-12 col-lg-12");
            $('#MsgPanel').addClass("col-xl-6 col-lg-6");
        }

        MachineManager.initFlipAndSwipMenu();


    }
    var selectedGroup = function () {
        if (vmImageMachine)
            return vmImageMachine.machineGroupSelected;
        else
            return null;
    }

    var selectMachineGroup = function (element) {
        var group = $(element).data('group');
        $("[data-panel]").removeClass("selected");
        vmImageMachine.machineGroupSelected = group;
        vmImageMachine.machinePanelSelected = null;
        $("g[data-highlighted] path").css("fill", "transparent");
        $("g[data-highlighted='" + group + "'] path").css("fill", "#D1839A");


        var per = $('#calendar').data('daterangepicker');
        var filters = {
            period: {
                start: per.startDate.toDate(),
                end: per.endDate.toDate()
            },
            machineGroup: vmImageMachine.machineGroupSelected
        };

        MachineManager.callAjaxMachineMessageViewModelData(filters);
    };


    var overMachineGroup = function (element) {
        var group = $(element).data('group');
        if (vmImageMachine.machineGroupSelected == group) return;
        $("g[data-highlighted='" + group + "'] path").css("fill", "transparent");
    };
    var outMachineGroup = function (element) {
        var group = $(element).data('group');
        if (vmImageMachine.machineGroupSelected == group) return;
        $("g[data-highlighted='" + group + "'] path").css("fill", "transparent");
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
        $("g[data-group]").off("click");
        $(".machine-group-selection button").off("click");
        $("button[data-panel]").off("click");
        $("#button-back-machine").off("click");


        $("g[data-group]").click(function (e) {
            e.preventDefault();
            selectMachineGroup(this);

            checkVisibility();
        });

        $("g[data-group]").mouseover(function (e) {
            e.preventDefault();
            overMachineGroup(this);

        });

        $("g[data-group]").mouseout(function (e) {
            e.preventDefault();
            outMachineGroup(this);

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
                var per = $('#calendar').data('daterangepicker');
                var filters = {
                    period: {
                        start: per.startDate.toDate(),
                        end: per.endDate.toDate()
                    },
                    machineGroup: null
                };

                MachineManager.callAjaxMachineMessageViewModelData(filters);
                checkVisibility();
            }
        });

        checkVisibility();
    };

    var checkVisibilityImageMachine = function () {
        if (vmImageMachine.modello == 'FMC') {
            if (vmImageMachine.isLargeTablet() || vmImageMachine.isTablet() || vmImageMachine.isMobile()) {
                $("#image-machine-sm").show();
                $("#image-machine-lg").hide();
            } else {
                $("#image-machine-sm").hide();
                $("#image-machine-lg").show();
            }
            initMachineImage();
        }
       

       
    }

    var init = function (model) {

        vmImageMachine = new Vue({
            el: '#panels-box',
            data: {
                machineGroupSelected: null,
                machinePanelSelected: null,
                modello: model
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
        init: init,
        selectedGroup: selectedGroup
    };

}();