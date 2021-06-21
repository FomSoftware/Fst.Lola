var SensorSpindles = function () {
    var vmSensorSpindles;

    var init = function (data) {
        initVueModel(data.vm_sensor_spindles);
    }

    var show = function () {
        vmSensorSpindles.showed = true;
    }

    var hide = function () {
        vmSensorSpindles.showed = false;
    }

    var initVueModel = function (data) {
        vmSensorSpindles = new Vue({
            el: '#CardSensorSpindles',
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
                noData: function () {
                    if (this.values == null) return true;
                    if (this.values.SoglieAmpereMandrini == null &&
                        this.values.SoglieAmpereContatore == null &&
                        this.values.AccelerometroINT_1 == null &&
                        this.values.AccelerometroINT_2 == null &&
                        this.values.AccelerometroINT_3 == null &&
                        this.values.HSD_NumCollRilevate == null &&
                        this.values.AccelContatoreINT_2 == null &&
                        this.values.AccelContatoreINT_3 == null &&
                        this.values.TemperSchedaMinutiINT == null &&
                        this.values.TemperSchedaContatoreINT == null &&
                        this.values.TemperStatoreMinutiINT == null &&
                        this.values.TemperStatoreContatoreINT == null &&
                        this.values.TemperCuscinettiMinutiINT == null &&
                        this.values.TemperCuscinettiContatoreINT == null) {
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

        var vm_sensor_spindles = data.vm_sensor_spindles;
        vmSensorSpindles.values = vm_sensor_spindles;
        

    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();