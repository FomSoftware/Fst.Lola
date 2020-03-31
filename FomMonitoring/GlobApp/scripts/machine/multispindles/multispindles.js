var MultiSpindles = function ()
{
    var vmMultiSpindles;
    var urlParametersApi;
    var selectedPosition = 11;

    var cp = function changePosition(oldPos, pos) {
        $("g[data-mmspindle-index = '" + oldPos + "'] circle").css({ 'fill': 'white' });
        $("g[data-mmspindle-index = '" + oldPos + "'] circle").attr("r", "9");
        $("g[data-mmspindle-index = '" + oldPos + "'] text").css({ 'fill': 'black' });
        $("g[data-mmspindle-index = '" + oldPos + "']").css({ 'transform': 'scale(1)' });
        $("g[data-mmspindle-index = '" + pos + "'] circle").css({ 'fill': 'black' });
        $("g[data-mmspindle-index = '" + pos + "'] text").css({ 'fill': 'white' });
        $("g[data-mmspindle-index = '" + pos + "'] circle").attr("r", "11");
        $("text[data-mmspindle-selected = '11']").text(pos);
        $("g[data-mmspindle-index]").css({ "cursor": "pointer" });
        selectedPosition = pos;

        var request = $.ajax({
            type: "POST",
            url: urlParametersApi,
            contentType: 'application/json',
            data: JSON.stringify({
                panelId: 3,
                cluster: pos
            }),
            beforeSend: function () {
                WaitmeManager.start('#bodyMultiSpindles');
                

            },
            complete: function () {
                WaitmeManager.end('#bodyMultiSpindles');
            }
        });

        request.done(function (data) {
            $(".slimscroll").slimScroll({ destroy: true });
            MultiSpindles.update(data);
            Vue.nextTick(function () {
                MachineManager.initScrollBar();
            });

        });

        request.fail(function (jqXHR, textStatus, errorThrown) {
            console.debug(jqXHR);
            console.debug(textStatus);
            console.debug(errorThrown);
        });

    };

    var init = function (data, urlApi, posLabel) {
        urlParametersApi = urlApi;
        initVueModel(data.vm_multi_spindle);

        //inizializzo con la posizione minima selezionata es. 11
        $("g[data-mmspindle-index = '" + selectedPosition + "'] circle").css({ 'fill': 'black' });
        $("g[data-mmspindle-index = '" + selectedPosition + "'] text").css({ 'fill': 'white' });
        $('[data-mmspindle-label="POSIZIONE"]').html(posLabel);
        $("g[data-mmspindle-index = '" + selectedPosition + "'] circle").attr("r", "11");

        $("g[data-mmspindle-index]").click(function (e) {
            console.log(e.target.textContent);
            //chiamata ajax per caricare le variabili della posizione selezionata
            if (selectedPosition !== parseInt(e.target.textContent))
            {
                //qui va la chiamata ajax per caricare le variabili della posizione selezionata
                cp(selectedPosition, e.target.textContent);
            }
            
        });
    }

    var show = function () {
        vmMultiSpindles.showed = true;
    }

    var hide = function () {
        vmMultiSpindles.showed = false;
    }

    var initVueModel = function (data)
    {
        vmMultiSpindles = new Vue({
            el: '#CardMultiSpindles',
            data: {
                values: data,
                showed: true
            },
            computed: {
                colorKPI: function () {

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

        var vm_multi_spindle = data;
        if (vmMultiSpindles != null) {
            vmMultiSpindles.values = vm_multi_spindle;
        }

    }

    return {
        init: init,
        update: update,
        show: show,
        hide: hide
    }

}();