﻿var AssistanceManager = function() {

    var resource;

    var initVue = function(data, resourcePage) {
        initMachineFilter(data);
        resource = resourcePage;
    };

    var changeMachine = function () {
        if ($("#serial-filter").val() != "") {
            $("#company-filter").prop("disabled", true);
            $("#serial-filter").prop("disabled", false);
        }
        $("#company-filter").val("");
    }
    var changeCompany = function () {
        if ($("#company-filter").val() != "") {
            $("#company-filter").prop("disabled", false);
            $("#serial-filter").prop("disabled", true);
        }
        $("#serial-filter").val("");
    }

    var initMachineFilter = function(data) {
        var vmSerialFilter = new Vue({
            el: '#serial-filter',
            data: {
                machines: data.machines,
            },
            mounted: function() {
                $('#serial_select').selectpicker();
            }
        });

        $('#serial_select').on('changed.bs.select',
            function(e) {
                var serialID = $(this).val();
                // AssistanceManager.callAjaxMesViewModelData(serialID);
            });

        var companyFilter = new Vue({
            el: '#company-filter',
            data: {
                customers: data.customers
            },
            mounted: function () {
                $('#company_select').selectpicker();
            }
        });

        $('#company_select').on('changed.bs.select',
            function (e) {
                var customerID = $(this).val();
                // AssistanceManager.callAjaxMesViewModelData(serialID);
            });
    }

    function validate(e) {
        if ($("#serial-filter").val() == "" && $("#company-filter").val() == "") {
            swal({
                title: "",
                text: resource.ErrorSelect,
                className: "text-modal-disactive",
                icon: "error",
                buttons: {
                    confirm: {
                        text: "OK",
                        visible: true,
                        className: "confirm",
                        closeModal: true
                    }
                }
            });
            e.preventDefaults();
        } else {
            $("#assistanceForm").submit();
        }
    }

    function cancel() {
        $("#serial-filter").val("");
        $("#company-filter").val("");
        $("#company-filter").prop("disabled", false);
        $("#serial-filter").prop("disabled", false);
    }
    

    return {
        initVue: initVue,
        changeCompany: changeCompany,
        changeMachine: changeMachine,
        validate: validate,
        cancel: cancel
}
}();


