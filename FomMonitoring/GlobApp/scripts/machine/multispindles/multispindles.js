var MultiSpindles = function ()
{
    var vmMultiSpindles;
    var selectedPosition = 11;

    var cp = function changePosition(oldPos, pos) {
        $("g[data-MMspindle-index = '" + oldPos + "'] circle").css({ 'fill': 'white' });
        $("g[data-MMspindle-index = '" + oldPos + "'] text").css({ 'fill': 'black' });
        $("g[data-MMspindle-index = '" + oldPos + "']").css({ 'transform': 'scale(1)' });
        $("g[data-MMspindle-index = '" + pos + "'] circle").css({ 'fill': 'black' });
        $("g[data-MMspindle-index = '" + pos + "'] text").css({ 'fill': 'white' });
        /*$("g[data-MMspindle-index = '" + pos + "']").css({ 'transform': 'scale(1.2)', 'transform-origin': '100% 55%' });*/
        $("text[data-mmspindle-selected = '11']").text(pos);
        selectedPosition = pos;
    };

    var init = function (data)
    {
        initVueModel(data.vm_multi_spindle);

        //inizializzo con la posizione minima selezionata es. 11
        $("g[data-MMspindle-index = '" + selectedPosition + "'] circle").css({ 'fill': 'black' });
        $("g[data-MMspindle-index = '" + selectedPosition + "'] text").css({ 'fill': 'white' });

        $("g[data-MMspindle-index]").click(function (e) {
            console.log(e.target.textContent);
            //chiamata ajax per caricare le variabili della posizione selezionata
            if (selectedPosition !== parseInt(e.target.textContent))
            {
                //qui va la chiamata ajax per caricare le variabili della posizione selezionata
                cp(selectedPosition, e.target.textContent);
            }
            
        });
    }

    var initVueModel = function (data)
    {
        vmMultiSpindles = new Vue({
            el: '#CardMultiSpindles',
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
            methods: {
                noData: function () {
                    if (this.values.RpmRange1500 == null &&
                        this.values.RpmRange3999 == null &&
                        this.values.RpmRange7999 == null &&
                        this.values.RpmRange11500 == null &&
                        this.values.RpmRange14500 == null &&
                        this.values.RpmRange20000 == null)
                    {
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

        var vm_multi_spindle = data.vm_multi_spindle;
        if (vm_multi_spindle != null) {
            vmMultiSpindle.values = vm_multi_spindle;
        }

    }

    return {
        init: init,
        update: update
    }

}();