var ElectroSpindle = function () {
    var vmElectroSpindle;

    var init = function (data) {
        initVueModel(data.vm_electro_spindle, data.vm_machine_info);
    }


    var show = function () {
        vmElectroSpindle.showed = true;
    }

    var hide = function () {
        vmElectroSpindle.showed = false;
    }

    var initVueModel = function (data, info) {
        vmElectroSpindle = new Vue({
            el: '#CardElectroSpindle',
            data: {
                values: data,
                isLmx: info.model.toUpperCase().indexOf('LMX'),
                showed: true
            },
            computed: {
                colorKPI: function () {
                    if (this == null)
                        return 'color-no-data';

                    var color = 'color-darkgreen';

                    return color;
                },
            },
            methods: {
                noData: function() {
                    if (this.values.RpmRange1500 == null &&
                        this.values.RpmRange8000 == null &&
                        this.values.RpmRange11500 == null &&
                        this.values.RpmRange14500 == null &&
                        this.values.RpmRange20000 == null) {
                        return true;
                    } else {
                        return false;
                    }
                }
            }
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
        update: update,
        show: show,
        hide: hide
    }

}();