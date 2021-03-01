var SensorsSpindleAxel = function () {
    var vmSensorSpindles;

    var init = function (data) {
        initVueModel(data.vm_sensors_axel);
    }

    var show = function () {
        vmSensorSpindles.showed = true;
    }

    var hide = function () {
        vmSensorSpindles.showed = false;
    }

    var initVueModel = function (data) {
        vmSensorSpindles = new Vue({
            el: '#CardSensorsSpindleAxel',
            data: {
                values: data,
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
                    if (this.values == null || (
                        this.values.TempoSovraAssorbimento == null &&
                        this.values.QtaSovrassorbimento == null &&
                        this.values.TempoSovraTemperatura == null &&
                        this.values.QtaSovraTemperatura == null &&
                        this.values.NumSblocchiPinza == null 
                       )) {
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

        var vm_sensor_spindles = data.vm_sensors_axel;
        vmSensorSpindles.values = vm_sensor_spindles;
        

    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();