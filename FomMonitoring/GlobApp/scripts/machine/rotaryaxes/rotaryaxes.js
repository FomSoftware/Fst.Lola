var RotaryAxes = function () {
    var vmRotaryAxes;

    var init = function (data) {
        initVueModel(data.vm_rotary_axes);
    }

    var show = function () {
        vmRotaryAxes.showed = true;
    }

    var hide = function () {
        vmRotaryAxes.showed = false;
    }

    var initVueModel = function (data) {
        vmRotaryAxes = new Vue({
            el: '#CardRotaryAxes',
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
                    if (this.values.NrotazioniAsse3C1 == null &&
                        this.values.NrotazioniAsse3C2 == null &&
                        this.values.NsblocchiForc1 == null &&
                        this.values.NsblocchiForc2 == null &&
                        this.values.NsblocchiForc3 == null) {
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

        var vm_rotary_axes = data.vm_rotary_axes;
        vmRotaryAxes.values = vm_rotary_axes;
        

    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();