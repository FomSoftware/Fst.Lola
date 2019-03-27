var UserManager = function () {
    var vmUsers;
    var resource;
    var contextUser;
    var roleUser;
    var table;
    var baseApiUrl;

    // change password
    var dataPassword;
    var resourceChangePassword;

    var enRoles = {
        Administrator: 0,
        Operator: 1,
        HeadWorkshop: 2,
        Assistance: 3,
        Customer: 4,
        UserApi: 5
    }

    var enAction = {
        add: 0,
        modify: 1
    };

    var init = function (user, baseUrl, resourceText) {
        baseApiUrl = baseUrl + "/ajax/UserManagerApi";
        resource = resourceText;
        contextUser = user;
        roleUser = user.Role;

        initVueModelUser();
        getData();
    }

    var initVueModelUser = function () {
        vmUsers = new Vue({
            el: '#user-modal-form',
            data: {
                users: {},
                actual: {},
                roles: {
                    active: '',
                    all: []
                },
                customers: {
                    active: '',
                    all: []
                },
                machines: {
                    active: [],
                    all: []
                },
                languages: {
                    active: '',
                    all: [],
                },
                missing: {
                    Username: false,
                    Firstname: false,
                    LastName: false,
                    Email: false,
                    Role: false,
                    Customer: false,
                    Machines: false,
                    Languages: false
                },
                enRoles: enRoles
            },
            methods: {
                formValidation: function () {
                    this.actual.Username == undefined || this.actual.Username == null || this.actual.Username.trim() == "" ? this.missing.Username = true : this.missing.Username = false
                    this.actual.FirstName == undefined || this.actual.FirstName == null || this.actual.FirstName.trim() == "" ? this.missing.FirstName = true : this.missing.FirstName = false
                    this.actual.LastName == undefined || this.actual.LastName == null || this.actual.LastName.trim() == "" ? this.missing.LastName = true : this.missing.LastName = false
                    this.actual.Email == undefined || this.actual.Email == null || this.actual.Email.trim() == "" ? this.missing.Email = true : this.missing.Email = false
                    this.roles.active.length == 0 ? this.missing.Role = true : this.missing.Role = false
                    this.customers.active == "" && roleUser != enRoles.Customer ? this.missing.Customer = true : this.missing.Customer = false
                    this.machines.active.length == 0 ? this.missing.Machines = true : this.missing.Machines = false
                    this.languages.active == "" ? this.missing.Languages = true : this.missing.Languages = false
                },
                selectOptionClass: function (val) {
                    if (!val.status || !val.enabled)
                        return true;
                },
                setLanguageFlag: function (val) {
                    return setLanguageFlag(val)
                },
            },
            mounted: function () {
                $('#role-input').selectpicker();
                $('#customer-input').selectpicker();
                $('#machines-input').selectpicker();
                $('#plants-input').selectpicker();
                $('#languages-input').selectpicker();
            },
            updated: function () {
                vmUsers.$nextTick(function () {
                    $('#role-input').selectpicker('refresh');
                    $('#customer-input').selectpicker('refresh');
                    $('#machines-input').selectpicker('refresh');
                    $('#languages-input').selectpicker('refresh');
                });

            },
        })
    }

    var getData = function () {
        $.get({
            url: baseApiUrl + '/GetUsers',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result != null) {
                    vmUsers.users = result.users;
                    vmUsers.roles.all = result.roles;
                    vmUsers.customers.all = result.customers;
                    vmUsers.machines.all = result.machines;
                    vmUsers.languages.all = result.languages;

                    initDataTable("#users-table", result.users);
                }
            },
            error: function (xhr, status, error) {
                errorSwal(resource.ErrorOccurred);
            }
        })
    };

    var getMachinesByCustomer = function () {
        $('#customer-input').on('hidden.bs.select', function (e, clickedIndex, newValue, oldValue) {
            $('#machines-input').prop("disabled", false);

            var customer = $("#customer-input option:selected").val();
            if (customer != null && customer != '')
                $.ajax({
                    url: baseApiUrl + "/GetMachinesByCustomer/" + customer,
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    success: function (result) {

                        vmUsers.machines.all = result;
                        Vue.nextTick(function () {
                            $('#machines-input').selectpicker('destroy');
                            $('#user-modal #machines-input').removeClass('input-read-only');
                            $('#machines-input').selectpicker();
                        });

                    }
                });

        });
    }

    var initDataTable = function (renderID, data) {
        data.forEach(function (elem, index) {
            elem.Name = elem.FirstName + ' ' + elem.LastName;
            elem.Modify = '<div class="button btn-modify" data-toggle="tooltip" title="' + resource.Modify + '" onclick="UserManager.modifyClickEvent(\'' + elem.ID + '\')" data-id="' + elem.ID + '"><i class="fa fa-pencil"></i></div>';
            if (elem.Enabled)
                elem.Enabled = '<span class="btn-active btn-enabled" data-toggle="tooltip" title="' + resource.EnabledUser + '"><i class="fa fa-check" aria-hidden="true"></i></span>';
            else
                elem.Enabled = '<span class="btn-disactive btn-enabled" data-toggle="tooltip"  title="' + resource.DisabledUser + '"><i class="fa fa-times" aria-hidden="true"></i></span>';
            elem.Language = '<img class="flag" src=' + setLanguageFlag(elem.LanguageName) + ' data-toggle="tooltip"  title="' + resource.Language + ": " + elem.LanguageName + '">'
            elem.ChangePassword = '<div class="button btn-modify" data-toggle="tooltip"  title="' + resource.ResetPassword + '" onclick="UserManager.resetPasswordClickEvent(\'' + elem.ID + '\')" data-id="' + elem.ID + '"><i class="fa fa-lock"></i></div>';
            if (elem.RoleCode != enRoles.Administrator && elem.RoleCode != enRoles.Customer)
                elem.Delete = '<div class="button btn-modify" data-toggle="tooltip"  title="' + resource.Delete + '" onclick="UserManager.deleteClickEvent(\'' + elem.ID + '\')" data-id="' + elem.ID + '"><i class="fa fa-trash"></i></div>';
            else
                elem.Delete = "";
            elem.Customer = "";

            // contollo sulla lunghezza dei nomi delle macchine
            if (elem.MachineSerials.length > 1)
                elem.Machines = '<div data-toggle="popover" data-content="' + elem.MachineSerials + '" data-placement="bottom" data-trigger="hover">' + elem.MachineSerials.slice(0, 25) + "..." + '</div>';
            else
                elem.Machines = elem.MachineSerials;
        });

        var columns = [
            { title: "", data: "Enabled", orderable: false, width: 2 },
            { title: resource.Username, data: "Username" },
            { title: resource.Name, data: "Name" },
            { title: "Email", data: "Email" },
            { title: resource.Role, data: "RoleName" }
        ];

        if (roleUser != enRoles.Customer)
            columns.push({ title: resource.Customer, data: "CustomerName" });
        columns.push({ title: resource.Machines, data: "Machines", width: 40 });
        columns.push({ title: "", data: 'Language', orderable: false, width: 15 });
        columns.push({ title: "", data: "Modify", orderable: false });
        columns.push({ title: "", data: "ChangePassword", orderable: false })
        columns.push({ title: "", data: "Delete", orderable: false });

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

        $('#user-modal .modal-title').text(resource.AddUser);
        $('#user-modal .form-check').hide();
        $('#form-customer-input').show();
        $('#form-role-input-disabled').hide();
        $('#form-customer-input-disabled').hide();

        if (roleUser == enRoles.Customer) {
            vmUsers.customers.active = contextUser.Username;
            vmUsers.$nextTick(function () {
                $('#form-customer-input').hide();
                $('#user-modal #machines-input').removeClass('input-read-only');
                $("[data-id='machines-input']").removeClass('background-disabled');
            });
        } else {
            getMachinesByCustomer();
            vmUsers.$nextTick(function () {
                $('#user-modal #customer-input').removeClass('input-read-only');
                $('#user-modal #machines-input').addClass('input-read-only');
                $("[data-id='machines-input']").addClass('background-disabled');
            });
        }

        $('#user-modal').modal('show');
        $('#user-modal .js-modify').hide();
        $('#user-modal .js-add').show();
    }

    var addUser = function () {
        vmUsers.formValidation();
        if (controlValidation()) {
            var machines = [];
            vmUsers.machines.active.forEach(function (val, index) {
                machines.push({ Id: val })
            });

            var data = {
                ID: null,
                Username: vmUsers.actual.Username.replace(/\s/g, ''),
                FirstName: vmUsers.actual.FirstName,
                LastName: vmUsers.actual.LastName,
                Email: vmUsers.actual.Email,
                CustomerName: vmUsers.customers.active,
                RoleCode: vmUsers.roles.active,
                LanguageId: vmUsers.languages.active,
                Machines: machines,
                Enabled: true,
            };

            $.post({
                url: baseApiUrl + '/InsertUser',
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result == true) {
                        successSwal(resource.CreatedUser);
                        clearActualUser();
                        $('#user-modal').modal('hide');
                        refreshTable();
                    } else {
                        errorSwal(resource.UsernameExists);
                    }
                },
                error: function (xhr, status, error) {
                    errorSwal(resource.ErrorOccurred);
                }
            });

        }
    }

    var modifyClickEvent = function (userID) {
        $.get({
            url: baseApiUrl + '/GetUser/' + userID,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result.user != null) {
                    vmUsers.actual = result.user;
                    vmUsers.roles.active = result.user.RoleCode;
                    vmUsers.customers.active = result.user.CustomerName;
                    vmUsers.machines.all = result.machines;
                    vmUsers.languages.active = result.user.LanguageId;
                    vmUsers.machines.active = _.pluck(result.user.Machines, 'Id');

                    $('#user-modal .modal-title').text(resource.ModifyUser);
                    $('#user-modal').modal('show');
                    $('#user-modal .js-add').hide();
                    $('#user-modal .js-modify').show();

                    $('#user-modal .form-check').show();
                    $('#form-customer-input').hide();
                    $('#form-customer-input-disabled').show();
                    $('#user-modal #username-input').addClass('input-read-only');

                    // se è un customer
                    if (vmUsers.roles.active == enRoles.Customer) {
                        $('#user-modal #role-input').addClass('input-read-only');
                        $("[data-id='role-input']").addClass('background-disabled');
                    }

                    // se è admin o customer
                    if (vmUsers.roles.active == enRoles.Operator || vmUsers.roles.active == enRoles.HeadWorkshop) {
                        $('#form-role-input-disabled').hide();
                        $('#form-role-input').show();
                    }
                    else {
                        vmUsers.roles.activeName = result.user.RoleName;
                        $('#form-role-input-disabled').show();
                        $('#form-role-input').hide();
                    }
                } else
                    errorSwal(resource.ErrorOccurred);
            },
            error: function (xhr, status, error) {
                errorSwal(resource.ErrorOccurred);
            }
        });
    }

    var modifyUser = function () {
        action = enAction.modify;
        vmUsers.formValidation();
        if (controlValidation()) {
            var machines = [];
            vmUsers.machines.active.forEach(function (val, index) {
                machines.push({ Id: val })
            });

            var data = {
                ID: vmUsers.actual.ID,
                Username: vmUsers.actual.Username.replace(/\s/g, ''),
                FirstName: vmUsers.actual.FirstName,
                LastName: vmUsers.actual.LastName,
                Email: vmUsers.actual.Email,
                CustomerName: vmUsers.customers.active,
                RoleCode: vmUsers.roles.active,
                LanguageId: vmUsers.languages.active,
                Machines: machines,
                Enabled: vmUsers.actual.Enabled,
            };

            $.ajax({
                url: baseApiUrl + "/EditUser",
                data: JSON.stringify(data),
                type: "PUT",
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result == true) {

                        successSwal(resource.UserSuccessfullyModify);
                        clearActualUser();
                        $('#user-modal').modal('hide');
                        refreshTable();
                    } else {
                        clearActualUser();
                        errorSwal(resource.ErrorOccurred);
                    }
                },
                error: function (xhr, status, error) {
                    clearActualUser();
                    errorSwal(resource.ErrorOccurred);
                }
            });
        }
    }

    var resetPasswordClickEvent = function (userID) {
        vmUsers.actual.ID = userID;
        var text = resource.ResetUserPassword;
        var alert = alertSwal(text);

        alert.then(function (result) {
            if (result)
                resetPassword(vmUsers.actual.ID)
        });
    }

    var resetPassword = function (id) {
        var userId = id;

        $.ajax({
            url: baseApiUrl + "/ResetUserPassword/" + userId,
            data: JSON.stringify(userId),
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result == true) {
                    successSwal(resource.PasswordSuccessfullyReseted);
                    clearActualUser();
                    $('#user-modal').modal('hide');
                } else {
                    errorSwal(resource.ErrorOccurred);
                }
            },
            error: function (xhr, status, error) {
                errorSwal(resource.ErrorOccurred);
            }
        });
    }

    var deleteClickEvent = function (userID) {
        vmUsers.actual.ID = userID;
        var text = resource.DeleteUser;
        var alert = alertSwal(text);

        alert.then(function (result) {
            if (result)
                deleteUser(vmUsers.actual.ID)
        });
    }

    var deleteUser = function (id) {
        var userId = id;

        $.ajax({
            url: baseApiUrl + "/DeleteUser/" + userId,
            data: JSON.stringify(userId),
            type: "DELETE",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result == true) {
                    successSwal(resource.UserSuccessfullyDeleted);
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

    var clearActualUser = function () {
        vmUsers.actual = {};
        vmUsers.roles.active = '';
        vmUsers.customers.active = '';
        vmUsers.machines.active = [];
        if (roleUser != enRoles.Customer)
            vmUsers.machines.all = [];

        vmUsers.languages.active = '';
        vmUsers.missing.Username = false;
        vmUsers.missing.FirstName = false;
        vmUsers.missing.LastName = false;
        vmUsers.missing.Email = false;
        vmUsers.missing.Role = false;
        vmUsers.missing.Customer = false;
        vmUsers.missing.Machines = false;
        vmUsers.missing.Languages = false;

        $('#machines-input').selectpicker('destroy');
        $('#customer-input').selectpicker('destroy');
        $('#role-input').selectpicker('destroy');

        $('#user-modal #username-input').removeClass('input-read-only');

        $('#user-modal #customer-input').removeClass('input-read-only');
        $("[data-id='customer-input']").removeClass('background-disabled');

        $('#user-modal #role-input').removeClass('input-read-only');
        $("[data-id='role-input']").removeClass('background-disabled');

        $('#user-modal #machines-input').removeClass('input-read-only');
        $("[data-id='machines-input']").removeClass('background-disabled');

        $('#user-modal .form-password').css('display', 'block');
    }

    var controlValidation = function () {
        if (vmUsers.missing.Username == false &&
            vmUsers.missing.Firstname == false &&
            vmUsers.missing.LastName == false &&
            vmUsers.missing.Email == false &&
            vmUsers.missing.Role == false &&
            vmUsers.missing.Customer == false &&
            vmUsers.missing.Machines == false &&
            vmUsers.missing.Languages == false) {
            if (!controlValidationEmail(vmUsers.actual.Email)) {
                errorSwal(resource.EmailNotValid);
                return false;
            }
            return true;
        } else
            return false;
    }

    var controlValidationEmail = function (email) {
        var regex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
        return regex.test(email);

    }

    var setLanguageFlag = function (val) {
        var result;
        switch (val) {
            case 'English':
                result = '/Images/flags/en.png';
                break;
            case 'Italiano':
                result = '/Images/flags/it.png';
                break;
            case 'Spanish':
                result = '/Images/flags/es.png';
                break;
            case 'French':
                result = '/Images/flags/fr.png';
                break;
        }
        return result;
    }

    var errorSwal = function (text) {
        swal({
            title: resourceChangePassword.Error,
            text: text,
            icon: "error",
            allowOutsideClick: true,
            closeModal: true
        });
    }


    /*#region CHANGE PASSWORD */

    var initChangePassword = function (data, baseUrl, resourceText) {
        dataPassword = data;
        baseApiUrl = baseUrl + "/ajax/UserManagerApi";
        resourceChangePassword = resourceText;
    }

    var openChangePasswordModal = function () {
        $('#last-password').val(null);
        $('#new-password').val(null);
        $('#repeat-new-password').val(null);
        $('#change-password-modal').modal('show');
    }

    var changePasswordClick = function () {
        var oldPassword = $('#last-password').val();
        var newPassword = $('#new-password').val();
        var repeatPassword = $('#repeat-new-password').val();

        var data = {
            IdUser: null,
            OldPassword: oldPassword,
            NewPassword: newPassword
        };

        if (newPassword != "" && repeatPassword != "") {
            if (newPassword == repeatPassword)
                changePassword(data);
            else
                errorSwal(resourceChangePassword.PasswordNotSame);
        }
        else
            errorSwal(resourceChangePassword.EnterPassword);
    }

    var changePassword = function (data) {
        var request = $.ajax({
            type: 'POST',
            contentType: 'application/json',
            url: baseApiUrl + "/ChangePassword",
            data: JSON.stringify(data)
        });

        request.done(function (data) {
            if (data == true) {
                successSwal(resourceChangePassword.ChangePasswordSuccessfully);
                $('#change-password-modal').modal('hide');
            }
            else
                errorSwal(resourceChangePassword.PasswordSamePrevious);

        });

        request.fail(function (jqXHR, textStatus, errorThrown) {
            errorSwal(resourceChangePassword.ErrorOccurred);
        });
    }

    /*#endregion*/


    return {
        init: init,
        initChangePassword: initChangePassword,
        addClickEvent: addClickEvent,
        addUser: addUser,
        modifyUser: modifyUser,
        modifyClickEvent: modifyClickEvent,
        deleteClickEvent: deleteClickEvent,
        clearActualUser: clearActualUser,
        resetPasswordClickEvent: resetPasswordClickEvent,
        clearActualUser: clearActualUser,
        changePasswordClick: changePasswordClick,
        openChangePasswordModal: openChangePasswordModal
    }
}()