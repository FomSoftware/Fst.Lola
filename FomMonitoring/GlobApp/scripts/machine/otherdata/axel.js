var OtherDataAxel = function () {
    var vmOther;

    var init = function (data) {
        initVueModel(data.vm_otherdata_axel);
    }

    var show = function () {
        vmOther.showed = true;
    }

    var hide = function () {
        vmOther.showed = false;
    }

    var initVueModel = function (data) {
        vmOther = new Vue({
            el: '#CardOtherDataMachineAxel',
            data: {
                values: data,
                kmMorse: data.KmMorse,
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
                    if (this.values == null || (
                        this.values.ClickIsolaSx == null &&
                        this.values.ClickIsolaDx == null &&
                        this.values.OreVitaMacchina == null &&
                        this.values.OreUltimoIngrassaggio == null &&
                        this.values.KmMorse == null )) {
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

        var vm_other = data.vm_otherdata_axel;
        if (vm_other != null) {
            vmOther.values = vm_other;
            kmMorse: davm_otherta.KmMorse;
        }

    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();