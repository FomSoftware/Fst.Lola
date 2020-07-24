var UserSetting = function () {
    var resource;
    var contextUser;
    var table;
    var baseApiUrl;
    var vmSettings;


    var editException = function () {
        var timezone = vmSettings.timeZones.active;
        var idMachine = vmSettings.machines.active;
        console.log(idMachine);
        console.log(timezone);
            var request = $.ajax({
                type: 'POST',
                contentType: 'application/json',
                url: baseApiUrl + "/EditException",
                data: JSON.stringify({
                    idMachine: idMachine,
                    timezone: timezone
                })
            });

            request.done(function (data) {
                $('#change-timezone-modal').modal('hide');
                refreshTable();
                var msg = successSwal("");
                msg.then(
                    () => {
                        window.location.reload();
                    });
            });

            request.fail(function (jqXHR, textStatus, errorThrown) {
                errorSwal();
            });
        

    }


    var init = function (user, baseUrl, resourceText) {
        baseApiUrl = baseUrl + "/ajax/UserSettingApi";
        resource = resourceText;
        contextUser = user;
        getTimeZones();
        initVueModelSetting();
        getData();
        getDataModal();
    }

    var initVueModelSetting = function () {
        vmSettings = new Vue({
            el: '#machine-timezone-modal',
            data: {
                baseUrl: baseApiUrl,
                actual: {},
                machines: {
                    active: [],
                    all: []
                },
                machinesTable: {
                    active: [],
                    all: []
                },
                timeZones: {
                    active: [],
                    all: []
                }
            }
        });
    };


    var getData = function () {
        $.get({
            url: baseApiUrl + '/GetMachinesWithException',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result != null) {
                    console.log(result);
                    vmSettings.machinesTable.all = result;
                    initDataTable("#users-setting-table", result);
                }
            },
            error: function (xhr, status, error) {
                errorSwal(resource.ErrorOccurred);
            }
        });
    };

    var getDataModal = function () {
        $.get({
            url: baseApiUrl + '/GetMachinesWithoutException',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result != null) {
                    console.log(result);
                    vmSettings.machines.all = result;
                }
            },
            error: function (xhr, status, error) {
                errorSwal(resource.ErrorOccurred);
            }
        });
    };

    var setCurrentTimeZoneDropdown = function () {
        var request = $.ajax({
            type: 'GET',
            contentType: 'application/json',
            url: baseApiUrl + "/GetCurrentTimeZone"
        });

        request.done(function (data) {
            if (data != null) {

                var dropdownUs = $('#timezone-input');
                dropdownUs.val(data);
            }
        });

        request.fail(function (jqXHR, textStatus, errorThrown) {
            errorSwal(resourceChangePassword.ErrorOccurred);
        });
    }

    var getTimeZones = function () {
        var dropdownUs = $('#timezone-input');
        dropdownUs.empty();
        $.get({
            url: baseApiUrl + '/GetTimeZones',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result != null) {
                    vmSettings.timeZones.all = result;
                    $.each(result,
                        function (index, entry) {
                            dropdownUs.append($('<option></option>').attr('value', entry.Code).text(entry.Description));
                        });
                    setCurrentTimeZoneDropdown();
                } else
                    errorSwal(resource.ErrorOccurred);
            },
            error: function (xhr, status, error) {
                errorSwal(resource.ErrorOccurred);
            }
        });
    }

    var changeTimeZoneClick = function () {
        var timezone = $('#timezone-input').children("option:selected").val();
        if (timezone != null && timezone.length > 0) {
            var request = $.ajax({
                type: 'POST',
                contentType: 'application/json',
                url: baseApiUrl + "/ChangeTimeZone",
                data: JSON.stringify(timezone)
            });

            request.done(function (data) {

                vmSettings.machines.active = {};
                vmSettings.timeZones.active = {};
                
                var msg = successSwal("");
                msg.then(
                    () => {
                        window.location.reload();
                    });
            });

            request.fail(function (jqXHR, textStatus, errorThrown) {

                vmSettings.machines.active = {};
                vmSettings.timeZones.active = {};
                errorSwal(resource.ErrorOccurred);
            });
        }

    }

    var deleteException = function (id)
    {
        var request = $.ajax({
            type: 'POST',
            contentType: 'application/json',
            url: baseApiUrl + "/DeleteException",
            data: JSON.stringify({
                idMachine: id
            })
        });

        request.done(function (data) {
            var msg = successSwal("");
            refreshTable();
            msg.then(
                () => {
                    window.location.reload();
                });
        });

        request.fail(function (jqXHR, textStatus, errorThrown) {
            errorSwal();
        });
    }

    var deleteClickEvent = function (id) {
        var text = resource.DeleteException;
        var alert = alertSwal(text);

        alert.then(function (result) {
            if (result)
                deleteException(id);
        });
    };
    var addClickEvent = function () {
        $("#machine-form").show();
        $('#machine-timezone-modal .modal-title').html(resource.ManageException);
        vmSettings.machines.active = {};
        vmSettings.timeZones.active = {};
        $('#machine-timezone-modal').modal('show');
    };

    var modifyClickEvent = function (id, timezone) {
        $("#machine-form").hide();
        $('#machine-timezone-modal .modal-title').html(resource.ManageException);
        vmSettings.machines.active = id;
        vmSettings.timeZones.active = timezone;
        $('#machine-timezone-modal').modal('show');
    };

    var initDataTable = function (renderID, data) {
        data.forEach(function (elem, index) {
            elem.MachineName = elem.MachineName;
            elem.Modify = '<div class="button btn-modify" data-toggle="tooltip" title="' + resource.Modify + '" onclick="UserSetting.modifyClickEvent(\'' + elem.Id + '\', \'' + elem.TimeZone +'\')" data-id="' + elem.Id + '"><i class="fa fa-pencil"></i></div>';
            elem.Serial = elem.Serial;
            elem.Delete = '<div class="button btn-modify" data-toggle="tooltip"  title="' + resource.Delete + '" onclick="UserSetting.deleteClickEvent(\'' + elem.Id + '\')" data-id="' + elem.ID + '"><i class="fa fa-trash"></i></div>';

        });

        var columns = [
            { title: resource.Machine, data: "MachineName", className: "all" }
        ];
        

        columns.push({ title: resource.Serial, data: "Serial", className: "all" });
        columns.push({ title: resource.Timezone, data: "TimeZone", className: "all" });

        columns.push({ title: "", data: "Modify", orderable: false, className: "all" });
        columns.push({ title: "", data: "Delete", orderable: false, className: "all" });

        var config = {
            data: data,
            columns: columns,
            info: false,
            order: [],
            paging: false,
            responsive: true,
            autoWidth: false,
            language: {
                search: "",
                searchPlaceholder: resource.Search,
                emptyTable: resource.NoRecordsAvailables,
                zeroRecords: resource.NothingFound
            },
        };

        table = $(renderID).DataTable(config);

        //init tooltip 
        $('[data-toggle="tooltip"]').tooltip();

        //init popover
        $('[data-toggle="popover"]').popover();
    }
   


    var refreshTable = function () {
        table.destroy();
        getData();
        getDataModal();
    }

    var successSwal = function (text) {
        swal({
            title: "",
            text: text,
            icon: "success",
            allowOutsideClick: true,
            closeModal: true
        });
    }
    var alertSwal = function (text) {
        var alert = swal({
            title: "",
            text: text,
            className: "text-modal-disactive",
            icon: "warning",
            buttons: {
                cancel: {
                    text: resource.ComeBack,
                    visible: true,
                    className: "cancel",
                    closeModal: true,
                },
                confirm: {
                    text: resource.Continue,
                    visible: true,
                    className: "confirm",
                    closeModal: true
                }
            }
        });
        return alert;
    };

    var errorSwal = function (text) {
        swal({
            title: "",
            text: text,
            icon: "error",
            allowOutsideClick: true,
            closeModal: true
        });
    }






    /*#endregion*/


    return {
        init: init,
        editException: editException,
        changeTimeZoneClick: changeTimeZoneClick,
        modifyClickEvent: modifyClickEvent,
        deleteClickEvent: deleteClickEvent,
        addClickEvent: addClickEvent
}
}()