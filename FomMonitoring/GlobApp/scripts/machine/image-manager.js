var ImageManager = function () {

    var vmImageMachine;
    var vmButtonsMenu;

    var mixinDetictingMobile = {
        methods: {
            isMobile: function () {
                var check = false;
                (function (a) {

                    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(a))
                        check = true;
                    
                })(navigator.userAgent || navigator.vendor || window.opera);
                return check;
            }
        }
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
        vmImageMachine.machinePanelSelected = panel;
        vmImageMachine.machineGroupSelected = null;
        $("g[data-highlighted] path").css("fill", "transparent");
    };

    var initMachineImage = function () {
        $("g[data-group]").click(function (e) {
            e.preventDefault();
            selectMachineGroup(this);
        });

        $(".machine-group-selection button").click(function (e) {
            e.preventDefault();
            selectMachineGroup(this);
        });

        $("button[data-panel]").click(function (e) {
            e.preventDefault();
            selectPanel(this);
        });

        $("#button-back-machine").click(function (e) {
            if (vmImageMachine.machineGroupSelected != null || vmImageMachine.machinePanelSelected != null) {
                e.preventDefault();
                $("g[data-highlighted] path").css("fill", "transparent");
                vmImageMachine.machineGroupSelected = null;
                vmImageMachine.machinePanelSelected = null;
            }
        });
    };

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
        
        initMachineImage();
    };
    
    return {
        init: init
    };

}();