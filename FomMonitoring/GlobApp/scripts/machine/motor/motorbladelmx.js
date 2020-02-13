var MotorBladeLMX = function () {
    var vmMotorBladeLMX;

    var init = function (data) {
        initVueModel(data.vm_motor_blade);
    }

    var show = function () {
        vmMotorBladeLMX.showed = true;
    }

    var hide = function () {
        vmMotorBladeLMX.showed = false;
    }

    var initVueModel = function (data) {
        vmMotorBladeLMX = new Vue({
            el: '#CardMotorBladeLMX',
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
                    if (this.values.RpmRange1500 == null &&
                        this.values.RpmRange2500 == null &&
                        this.values.RpmRange3000 == null &&
                        this.values.TempoSovraAss == null &&
                        this.values.QtaSovraAss == null &&
                        this.values.TempoTot == null &&
                        this.values.TagliLamaTot == null &&
                        this.values.TagliLamaPar == null) {
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

        var vm_motor_blade = data.vm_motor_blade;
        vmMotorBladeLMX.values = vm_motor_blade;
        

    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();