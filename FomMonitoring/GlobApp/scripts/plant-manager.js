var PlantManager = function () {
    var vmPlants;
    var resource;
    var contextUser;
    var roleUser;
    var table;
    var baseApiUrl;
    

    var enAction = {
        add: 0,
        modify: 1
    };


    var enRoles = {
        Administrator: 0,
        Operator: 1,
        HeadWorkshop: 2,
        Assistance: 3,
        Customer: 4,
        UserApi: 5
    }

    var init = function (user, baseUrl, resourceText) {
        baseApiUrl = baseUrl + "/ajax/PlantManagerApi";
        baseUserApiUrl = baseUrl + "/ajax/UserManagerApi";
        resource = resourceText;
        contextUser = user;
        roleUser = user.Role;

        initVueModelPlant();
        getData();
        
    }

    var initVueModelPlant = function () {
        vmPlants = new Vue({
            el: '#plant-modal-form',
            data: {
                plants: {},
                actual: {},
                customers: {
                    active: '',
                    all: []
                },
                machines: {
                    active: [],
                    all: []
                },
                missing: {
                    Name: false,
                    Address: false
                }
            },
            computed: {
                activeMachines: function () {
                    return this.machines.active;
                }
            },
            watch: {
                activeMachines: function (machine, oldmachine) {
                    if (oldmachine != null && machine != null) {
                        var intersection = machine.filter(x => !oldmachine.includes(x));
                        console.log(intersection);
                        if (intersection != null && intersection[0] != null) {
                            requestAssociatedPlant(intersection[0]).then(r => {
                                if (r.Plant != null && vmPlants.actual.Id != r.Plant.Id) {
                                    var text = resource.MachineWithPlantConfirm;
                                    //"Macchina già associata al plant {0}, continuare?";
                                    text = text.replace("{0}", r.Plant.Name);
                                    var alert = alertSwal(text);

                                    alert.then(
                                        (result) => {
                                            if (result != true) {
                                                vmPlants.machines.active = oldmachine;
                                                vmPlants.$nextTick(function () {
                                                    $('#customer-input').selectpicker('refresh');
                                                    $('#machines-input').selectpicker('refresh');
                                                });
                                            }
                                        });
                                }
                            });
                        }

                    }
                }
                
            },
            methods: {
                formValidation: function () {
                    this.actual.Name == undefined || this.actual.Name == null || this.actual.Name.trim() == "" ? this.missing.Name = true : this.missing.Name = false
                    this.customers.active == "" && roleUser != enRoles.Customer ? this.missing.Customer = true : this.missing.Customer = false
                    this.machines.active.length == 0 ? this.missing.Machines = true : this.missing.Machines = false
                },
                selectOptionClass: function (val) {
                    if (!val.status || !val.enabled)
                        return true;
                },
                changeCustomer: function () {
                    getMachinesByCustomer();
                }
            },
            mounted: function () {
                $('#customer-input').selectpicker();
                $('#machines-input').selectpicker();
            },
            updated: function () {
                vmPlants.$nextTick(function () {
                    $('#customer-input').selectpicker('refresh');
                    $('#machines-input').selectpicker('refresh');
                });

            },
        })
    }

    var getData = function () {
        $.get({
            url: baseApiUrl + '/GetPlants',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result != null) {
                    vmPlants.plants = result.Plants;
                    vmPlants.machines.all = result.Machines;
                    vmPlants.customers.all = result.Customers;

                    initDataTable("#plants-table", vmPlants.plants);
                }
            },
            error: function (xhr, status, error) {
                errorSwal(resource.ErrorOccurred);
            }
        })
    };

    
    var getMachinesByCustomer = function () {
        //$('#customer-input').on('hidden.bs.select', function (e, clickedIndex, newValue, oldValue) {
        //    $('#machines-input').prop("disabled", false);

        //    var customer = $("#customer-input option:selected").val();

        var customer = vmPlants.customers.active;
        console.log(customer);
            if (customer != null && customer != '')
                $.ajax({
                    url: baseUserApiUrl + "/GetMachinesByCustomer/" + customer,
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    success: function (result) {

                        vmPlants.machines.all = result;
                        Vue.nextTick(function () {
                            $('#machines-input').selectpicker('destroy');
                            $('#plant-modal #machines-input').removeClass('input-read-only');
                            $('#machines-input').selectpicker();
                        });

                    }
                });

        //});
    }

    var initDataTable = function (renderID, data) {
        data.forEach(function (elem, index) {
            elem.Name = elem.Name;
            elem.Modify = '<div class="button btn-modify" data-toggle="tooltip" title="' + resource.Modify + '" onclick="PlantManager.modifyClickEvent(\'' + elem.Id + '\')" data-id="' + elem.Id + '"><i class="fa fa-pencil"></i></div>';
            elem.Delete = '<div class="button btn-modify" data-toggle="tooltip"  title="' + resource.Delete + '" onclick="PlantManager.deleteClickEvent(\'' + elem.Id + '\')" data-id="' + elem.Id + '"><i class="fa fa-trash"></i></div>';

            // contollo sulla lunghezza dei nomi delle macchine
            if (elem.MachineSerials != null && elem.MachineSerials.length > 1)
                elem.Machines = '<div data-toggle="popover" data-content="' + elem.MachineSerials.join(", ") + '" data-placement="bottom" data-trigger="hover">' + elem.MachineSerials.slice(0, 25) + "..." + '</div>';
            else
                elem.Machines = elem.MachineSerials;
        });

        var columns = [
            { title: resource.PlantName, data: "Name", className: "all"},
            { title: resource.Address, data: "Address", className: "all" }
        ];

        if (roleUser != enRoles.Customer)
            columns.push({ title: resource.PlantCustomer, data: "CustomerName", className: "all"});

        columns.push({ title: resource.PlantMachines, data: "Machines", className: "all" });

        columns.push({ title: "", data: "Modify", orderable: false, className: "all"});
        columns.push({ title: "", data: "Delete", orderable: false, className: "all"});

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

    var addClickEvent = function () {
        action = enAction.add;
        clearActualPlant();
        $('#plant-modal .modal-title').html(resource.AddPlant);
        $('#plant-modal .form-check').hide();
        
        if (roleUser == enRoles.Customer) {
            vmPlants.customers.active = contextUser.Username;
            getMachinesByCustomer();
        } else {
            vmPlants.$nextTick(function () {
                $('#plant-modal #customer-input').removeClass('input-read-only');
                $('#plant-modal #machines-input').addClass('input-read-only');
                $("[data-id='machines-input']").addClass('background-disabled');
            });
        }
        
        

        $('#plant-modal').modal('show');
        $('#plant-modal .js-modify').hide();
        $('#plant-modal .js-add').show();
    }

    var addPlant = function () {
        vmPlants.formValidation();
        if (controlValidation()) {
            var machines = [];
            vmPlants.machines.active.forEach(function (val, index) {
                machines.push({ Id: val })
            });

            var data = {
                Id: vmPlants.actual.Id,
                Name: vmPlants.actual.Name.replace(/\s/g, ''),
                Address: vmPlants.actual.Address,
                CustomerName: vmPlants.customers.active,
                Machines: machines
            };

            $.post({
                url: baseApiUrl + '/InsertPlant',
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result == true) {
                        successSwal(resource.PlantCreated);
                        clearActualPlant();
                        $('#plant-modal').modal('hide');
                        refreshTable();
                    } else {
                        errorSwal(resource.Error);
                    }
                },
                error: function (xhr, status, error) {
                    errorSwal(resource.ErrorOccurred);
                }
            });

        }
    }

    var modifyClickEvent = function (userID) {

        clearActualPlant();
        $('#plant-modal #machines-input').removeClass('input-read-only');
        $.get({
            url: baseApiUrl + '/GetPlant/' + userID,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result.Plant != null) {
                    vmPlants.actual = result.Plant;

                    vmPlants.customers.active = result.Plant.CustomerName;

                    vmPlants.$nextTick(function () {
                        vmPlants.machines.active = _.pluck(result.Plant.Machines, 'Id');
                        vmPlants.machines.all = result.Machines;

                        $('#plant-modal .modal-title').html(resource.PlantModify);
                        $('#plant-modal').modal('show');
                        $('#plant-modal .js-modify').show();
                        $('#plant-modal .js-add').hide();

                    });
                } else
                    errorSwal(resource.ErrorOccurred);
            },
            error: function (xhr, status, error) {
                errorSwal(resource.ErrorOccurred);
            }
        });
    }

    var requestAssociatedPlant = function (idMachine) {
        return $.ajax({
            url: baseApiUrl + "/GetPlantByMachine/" + idMachine,
            type: "GET",
            contentType: "application/json; charset=utf-8",
        });
    }


    var modifyPlant = function () {
        action = enAction.modify;
        vmPlants.formValidation();
        if (controlValidation()) {
            var machines = [];
            vmPlants.machines.active.forEach(function (val, index) {
                machines.push({ Id: val })
            });

            var data = {
                Id: vmPlants.actual.Id,
                Name: vmPlants.actual.Name.replace(/\s/g, ''),
                Address: vmPlants.actual.Address,
                CustomerName: vmPlants.customers.active,
                Machines: machines
            };

            $.ajax({
                url: baseApiUrl + "/EditPlant",
                data: JSON.stringify(data),
                type: "PUT",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result == true) {
                        successSwal(resource.PlantModified);
                        clearActualPlant();
                        $('#plant-modal').modal('hide');
                        refreshTable();
                    } else {
                        clearActualPlant();
                        errorSwal(resource.ErrorOccurred);
                    }
                },
                error: function (xhr, status, error) {
                    clearActualPlant();
                    errorSwal(resource.ErrorOccurred);
                }
            });
        }
    }

    var deleteClickEvent = function (id) {

        $.ajax({
            url: baseApiUrl + "/GetMachinesByPlant/" + id,
            type: "GET",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result.length > 0) {
                    errorSwal(resource.PlantImpossibileDeleted);
                }
                else {
                    vmPlants.actual.ID = id;
                    var text = resource.PlantDelete;
                    var alert = alertSwal(text);

                    alert.then(function (result) {
                        if (result)
                            deletePlant(id);
                    });
                }
            }
        });





    }

    var deletePlant = function (id) {
        var plantId = id;

        $.ajax({
            url: baseApiUrl + "/DeletePlant/" + plantId,
            data: JSON.stringify(plantId),
            type: "DELETE",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result == true) {
                    successSwal(resource.PlantDeleted);
                    refreshTable();
                } else {
                    errorSwal(resource.ErrorOccurred);
                }
            },
            error: function (xhr, status, error) {
                errorSwal(resource.ErrorOccurred);
            }
        });
    }

    var refreshTable = function () {
        table.destroy();
        getData();
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
    }

    var clearActualPlant = function () {
        vmPlants.actual = {};
        vmPlants.customers.active = '';
        vmPlants.machines.active = [];
        if (roleUser != enRoles.Customer)
            vmPlants.machines.all = [];
        
        vmPlants.missing.Name = false;
        vmPlants.missing.Address = false;

        $('#machines-input').selectpicker('destroy');
       
        $('#plant-modal #machines-input').removeClass('input-read-only');
        $("[data-id='machines-input']").removeClass('background-disabled');
        
    }

    var controlValidation = function () {
        if (vmPlants.missing.Name == false &&
            vmPlants.missing.Address == false) {

            return true;
        } else
            return false;
    }


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
        addClickEvent: addClickEvent,
        addPlant: addPlant,
        modifyPlant: modifyPlant,
        modifyClickEvent: modifyClickEvent,
        deleteClickEvent: deleteClickEvent,
        clearActualPlant: clearActualPlant
    }
}()