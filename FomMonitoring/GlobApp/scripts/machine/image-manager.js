var ImageManager = function () {

    var vmImageMachine;

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
            selectMachineGroup(this);
        });

        $(".machine-group-selection button").click(function (e) {
            selectMachineGroup(this);
        });

        $("button[data-panel]").click(function (e) {
            selectPanel(this);
        });
    };

    var init = function () {

        vmImageMachine = new Vue({
            el: '#panels-box',
            data: {
                machineGroupSelected: null,
                machinePanelSelected: null
            }
        });
        initMachineImage();
    };
    
    return {
        init: init
    };

}();