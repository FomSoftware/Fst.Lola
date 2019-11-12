var ElectroSpindle = function () {
    var vmElectroSpindle;

    var init = function (data) {
        initVueModel(data.vm_electro_spindle);
    }

    var initVueModel = function (data) {
        vmElectroSpindle = new Vue({
            el: '#CardElectroSpindle',
            data: {
                values: data
            },
            computed: {
                colorKPI: function () {
                    if (this == null)
                        return 'color-no-data';

                    var color = 'color-darkgreen';

                    return color;
                },
            },
        });
    }

    var update = function (data) {
        // update vue model

        var vm_electro_spindle = data.vm_electro_spindle;
        if (vm_electro_spindle != null) {
            vmElectroSpindle.values = vm_electro_spindle;
        }

    }

    return {
        init: init,
        update: update
    }

}();