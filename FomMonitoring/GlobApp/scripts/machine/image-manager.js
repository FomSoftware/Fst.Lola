function ImageManager () {

    var vmImageMachine;
    var vmButtonsMenu;
    var vmPanelsMachine;
    var checkModello;

    this.mixinDetictingMobile = {
        methods: {
            isMobile: function() {
                var check = false;
                (function(a) {

                    /*if (/Android|webOS|iPhone|iPod|BlackBerry/i.test(a) ||
                        (/iPad/i.test(a) && $(window).width() < 1200))*/
                    if ($(window).width() <= 992)
                        check = true;

                })(navigator.userAgent || navigator.vendor || window.opera);
                return check;
            },
            isTablet: function() {
                var check = false;
                (function(a) {

                    /*if (/Android|webOS|iPhone|iPod|BlackBerry/i.test(a) ||
                        (/iPad/i.test(a) && $(window).width() < 1200))*/
                    if ($(window).width() > 992 && $(window).width() <= 1200)
                        check = true;

                })(navigator.userAgent || navigator.vendor || window.opera);
                return check;
            },
            isLargeTablet: function() {
                var check = false;
                (function(a) {

                    /*if (/Android|webOS|iPhone|iPod|BlackBerry/i.test(a) ||
                        (/iPad/i.test(a) && $(window).width() < 1200))*/
                    if ($(window).width() > 1200 && $(window).width() <= 1400)
                        check = true;

                })(navigator.userAgent || navigator.vendor || window.opera);
                return check;
            }
        }
    };

    checkVisibility = function (checkModello) {
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
            if (Jobs != null)
                Jobs.show();
            if (OtherMachinesJobs != null)
                OtherMachinesJobs.show();
        } else {
            if (Jobs != null)
                Jobs.hide();
            if (OtherMachinesJobs != null)
                OtherMachinesJobs.hide();
        }

        checkModello(vmImageMachine.machineGroupSelected);


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

        setTimeout(function() {

                MachineManager.initFlipAndSwipMenu();
            },
            1000);


    }
    this.selectedGroup = function () {
        if (vmImageMachine)
            return vmImageMachine.machineGroupSelected;
        else
            return null;
    }
   

    selectMachineGroup = function (element) {
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

    selectPanel = function (element) {
        var panel = $(element).data('panel');

        $("[data-panel]").removeClass("selected");
        $(element).addClass("selected");

        vmImageMachine.machinePanelSelected = panel;
        vmImageMachine.machineGroupSelected = null;
        $("g[data-highlighted] path").css("fill", "transparent");
    };


    var overMachineGroup = function (element) {
        var group = $(element).data('group');
        if (vmImageMachine.machineGroupSelected == group) return;
        $("g[data-highlighted='" + group + "'] path").css("fill", "#A61646");
    };
    var outMachineGroup = function (element) {
        var group = $(element).data('group');
        if (vmImageMachine.machineGroupSelected == group) return;
        $("g[data-highlighted='" + group + "'] path").css("fill", "transparent");
    };



    initMachineImage = function (checkModello) {
        if (vmImageMachine.isMobile() || vmImageMachine.isTablet()) {
            $("#panels-area").hide();
        }
        else {
            $("#panels-area").show();
        }
        $("g[data-group]").off("click");
        $(".machine-group-selection button").off("click");
        $("button[data-panel]").off("click");
        $("#button-back-machine").off("click");


        $("g[data-group]").click(function (e) {
            e.preventDefault();
            selectMachineGroup(this);

            checkVisibility(checkModello);
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

            checkVisibility(checkModello);
        });

        $("button[data-panel]").click(function (e) {
            e.preventDefault();
            selectPanel(this);

            checkVisibility(checkModello);
        });

        $("#button-back-machine").click(function (e) {
            $("[data-panel]").removeClass("selected");
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

                if (vmImageMachine.isLargeTablet() || vmImageMachine.isTablet() || vmImageMachine.isMobile()) {
                    $("#panels-area").hide();
                } else {
                    $("#panels-area").show();
                }
                checkVisibility(checkModello);
            }
        });

        this.checkVisibility(checkModello);
    };

    checkVisibilityImageMachine = function (checkModello) {
       
            if (vmImageMachine.isLargeTablet() || vmImageMachine.isTablet() || vmImageMachine.isMobile()) {
                $("#image-machine-sm").show();
                $("#image-machine-lg").hide();
            } else {
                $("#image-machine-sm").hide();
                $("#image-machine-lg").show();
            }
        initMachineImage(checkModello);
      
    }

    this.init = function (checkModello) {
        vmImageMachine = new Vue({
            el: '#panels-box',
            data: {
                machineGroupSelected: null,
                machinePanelSelected: null
            },
            mixins: [this.mixinDetictingMobile],
            watch: {

                machineGroupSelected: function (val, oldVal) {
                    console.log("old al " + oldVal + "new Val " + val);
                    if (val != null && (this.isMobile() || this.isTablet())) {
                        $("#panels-area").show();
                    }
                },

                machinePanelSelected: function (val, oldVal) {
                    if (val != null && (this.isMobile() || this.isTablet())) {
                        $("#panels-area").show();
                    }
                }

            }
        });


        vmButtonsMenu = new Vue({
            el: "#buttons-bar",
            mixins: [this.mixinDetictingMobile]
        });

        checkVisibilityImageMachine(checkModello);
        $(window).resize(function () {
            checkVisibilityImageMachine(checkModello);
        });

        initMachineImage(checkModello);
        return this;
    };

    return {
        init: init,
        selectedGroup: this.selectedGroup,
        checkVisibility: this.checkVisibility
    };

};



