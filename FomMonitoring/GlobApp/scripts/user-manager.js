var UserManager = function () {
    var vmUsers;
    var resource;
    var contextUser;
    var roleUser;
    var table;
    var baseApiUrl;
    var baseUrl;
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
    };

    var enAction = {
        add: 0,
        modify: 1
    };

    var init = function (user, baseUrlA, resourceText) {
        baseApiUrl = baseUrlA + "/ajax/UserManagerApi";
        resource = resourceText;
        contextUser = user;
        roleUser = user.Role;
        baseUrl = baseUrlA;
        initVueModelUser();
        getData();
    };

    var setLanguageFlag;
    var initVueModelUser = function () {
        vmUsers = new Vue({
            el: '#user-modal-form',
            data: {
                baseUrl: baseUrl,
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
                    all: []
                },
                timeZones: {
                    active: '',
                    all: []
                },
                missing: {
                    Username: false,
                    Firstname: false,
                    LastName: false,
                    Role: false,
                    Customer: false,
                    Machines: false,
                    Languages: false,
                    Password: false,
                    ConfirmPassword: false,
                    TimeZone: false
                },
                enRoles: enRoles
            },
            methods: {
                formValidation: function() {
                    this.actual.Username == undefined ||
                        this.actual.Username == null ||
                        this.actual.Username.trim() === ""
                        ? this.missing.Username = true
                        : this.missing.Username = false;
                    this.actual.FirstName == undefined ||
                        this.actual.FirstName == null ||
                        this.actual.FirstName.trim() === ""
                        ? this.missing.FirstName = true
                        : this.missing.FirstName = false;
                    this.actual.LastName == undefined ||
                        this.actual.LastName == null ||
                        this.actual.LastName.trim() === ""
                        ? this.missing.LastName = true
                        : this.missing.LastName = false;
                    this.roles.active.length === "" ? this.missing.Role = true : this.missing.Role = false;
                    this.customers.active === "" && roleUser !== enRoles.Customer
                        ? this.missing.Customer = true
                        : this.missing.Customer = false;
                    this.machines.active.length === 0 ? this.missing.Machines = true : this.missing.Machines = false;
                    this.languages.active === "" ? this.missing.Languages = true : this.missing.Languages = false;
                    (this.timeZones.active == undefined || this.timeZones.active == null || this.timeZones.active === "") ? this.missing.TimeZone = true : this.missing.TimeZone = false;
                },
                selectOptionClass: function(val) {
                    if (!val.status || !val.enabled)
                        return true;
                },
                setLanguageFlag: function(val, url) {
                    return setLanguageFlag(val, url);
                }
            },
            mounted: function() {
                $('#role-input').selectpicker();
                $('#customer-input').selectpicker();
                $('#machines-input').selectpicker();
                $('#plants-input').selectpicker();
                $('#languages-input').selectpicker();
                $('#timezones-input').selectpicker();
            },
            updated: function() {
                vmUsers.$nextTick(function() {
                    $('#role-input').selectpicker('refresh');
                    $('#customer-input').selectpicker('refresh');
                    $('#machines-input').selectpicker('refresh');
                    $('#languages-input').selectpicker('refresh');
                    $('#timezones-input').selectpicker('refresh');
                });

            },
        });
    };

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
        });
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
    };

    var initDataTable = function (renderID, data) {
        data.forEach(function (elem, index) {
            elem.Name = elem.FirstName + ' ' + elem.LastName;
            elem.Modify = '<div class="button btn-modify" data-toggle="tooltip" title="' + resource.Modify + '" onclick="UserManager.modifyClickEvent(\'' + elem.ID + '\')" data-id="' + elem.ID + '"><i class="fa fa-pencil"></i></div>';
            if (elem.Enabled)
                elem.Enabled = '<span class="btn-active btn-enabled" data-toggle="tooltip" title="' + resource.EnabledUser + '"><i class="fa fa-check" aria-hidden="true"></i></span>';
            else
                elem.Enabled = '<span class="btn-disactive btn-enabled" data-toggle="tooltip"  title="' + resource.DisabledUser + '"><i class="fa fa-times" aria-hidden="true"></i></span>';
            elem.Language = '<img class="flag" src=' + setLanguageFlag(elem.LanguageName, vmUsers.$data.baseUrl) + ' data-toggle="tooltip"  title="' + resource.Language + ": " + elem.LanguageName + '">';
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
            { title: "", data: "Enabled", orderable: false, width: 2, className: "all" },
            { title: resource.Username, data: "Username", className: "all" },
            { title: resource.Name, data: "Name", width: 350, className: "all" },
            { title: "Email", data: "Email", className: "all" },
            { title: resource.Role, data: "RoleName", className: "all" }
        ];

        if (roleUser != enRoles.Customer)
            columns.push({ title: resource.Customer, data: "CustomerName", className: "all" });
        columns.push({ title: resource.Machines, data: "Machines", className: "all" });
        columns.push({ title: "", data: 'Language', orderable: false, width: 15, className: "all" });
        columns.push({ title: "", data: "Modify", orderable: false, className: "all" });
        columns.push({ title: "", data: "ChangePassword", orderable: false, className: "all" });
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
    };

    var addClickEvent = function () {
        action = enAction.add;
        clearActualUser();
        $('#user-modal .modal-title').html(resource.AddUser);
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
                //mostro i campi nel caso della create per customer
                $('#user-modal #form-group-password').removeClass('display-none');
                $('#user-modal #form-group-confirm-password').removeClass('display-none');

            });
        } else {
            getMachinesByCustomer();
            vmUsers.$nextTick(function () {
                $('#user-modal #customer-input').removeClass('input-read-only');
                $('#user-modal #machines-input').addClass('input-read-only');
                $("[data-id='machines-input']").addClass('background-disabled');
            });
        }
        getTimeZones();
        $('#user-modal').modal('show');
        $('#user-modal .js-modify').hide();
        $('#user-modal .js-add').show();
    };

    var getTimeZones = function() {
        $.get({
            url: baseApiUrl + '/GetTimeZones',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result != null) {
                    $.each(result,
                        function (index, entry) {
                            result[index] = entry += ' (GMT ' + moment().tz(index).format('Z') + ')';
                        });
                    vmUsers.timeZones.all = Object.entries(result);
                } else
                    errorSwal(resource.ErrorOccurred);
            },
            error: function (xhr, status, error) {
                errorSwal(resource.ErrorOccurred);
            }
        });
    }

    var addUser = function () {
        vmUsers.formValidation();
        if (controlValidation(enAction.add)) {
            var machines = [];
            vmUsers.machines.active.forEach(function (val, index) {
                machines.push({ Id: val });
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
                Password: vmUsers.actual.Password,
                TimeZone: vmUsers.timeZones.active
            };

            $.post({
                url: baseApiUrl + '/InsertUser',
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result == true) {
                        if (roleUser == enRoles.Customer) {
                            successSwal(resource.CreatedUser + "\n Username: " + data.Username + "\n Password: " + data.Password);
                        }
                        else
                            successSwal(resource.CreatedUser );
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
    };

    var modifyClickEvent = function (userID) {
        $.get({
            url: baseApiUrl + '/GetUser/' + userID,
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (result.user != null) {
                    vmUsers.actual = result.user;
                    vmUsers.roles.active = result.user.RoleCode;
                    vmUsers.machines.all = result.machines;
                    vmUsers.languages.active = result.user.LanguageId;
                    vmUsers.customers.active = result.user.CustomerName;
                    vmUsers.machines.active = _.pluck(result.user.Machines, 'Id');
                    getTimeZones();
                    vmUsers.$nextTick(function() {
                        vmUsers.timeZones.active = result.user.TimeZone;
                    });
                    $('#user-modal .modal-title').html(resource.ModifyUser);
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
                    $('#user-modal #form-group-password').addClass('display-none');
                    $('#user-modal #form-group-confirm-password').addClass('display-none');
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
    };

    var modifyUser = function () {
        action = enAction.modify;
        vmUsers.formValidation();
        if (controlValidation(action)) {
            var machines = [];
            vmUsers.machines.active.forEach(function (val, index) {
                machines.push({ Id: val });
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
                Password: vmUsers.actual.Password,
                TimeZone: vmUsers.timeZones.active
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
    };

    var resetPasswordClickEvent = function (userID) {
        vmUsers.actual.ID = userID;
        var text = resource.ResetUserPassword;
        var alert = alertSwal(text);

        alert.then(function (result) {
            if (result)
                resetPassword(vmUsers.actual.ID);
        });
    };

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
    };

    var deleteClickEvent = function (userID) {
        vmUsers.actual.ID = userID;
        var text = resource.DeleteUser;
        var alert = alertSwal(text);

        alert.then(function (result) {
            if (result)
                deleteUser(vmUsers.actual.ID);
        });
    };

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
    };

    var refreshTable = function () {
        table.destroy();
        getData();
    };

    var successSwal = function (text, func) {
        var msg = swal({
            title: "",
            text: text,
            icon: "success",
            allowOutsideClick: true,
            closeModal: true
        });
        return msg;
    };

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
        vmUsers.missing.Role = false;
        vmUsers.missing.Customer = false;
        vmUsers.missing.Machines = false;
        vmUsers.missing.Languages = false;
        vmUsers.missing.Password = false;
        vmUsers.missing.ConfirmPassword = false;
        vmUsers.missing.TimeZone = false;

        $('#machines-input').selectpicker('destroy');
        $('#customer-input').selectpicker('destroy');
        //commentata la riga perchè fa moltiplicare le opzioni vuote ogni volta cha si apre la select
        //$('#role-input').selectpicker('destroy');

        $('#user-modal #username-input').removeClass('input-read-only');

        $('#user-modal #customer-input').removeClass('input-read-only');
        $("[data-id='customer-input']").removeClass('background-disabled');

        $('#user-modal #role-input').removeClass('input-read-only');
        $("[data-id='role-input']").removeClass('background-disabled');

        $('#user-modal #machines-input').removeClass('input-read-only');
        $("[data-id='machines-input']").removeClass('background-disabled');

        $('#user-modal .form-password').css('display', 'block');
    };

    var controlValidation = function (action) {
        if (vmUsers.missing.Username === false &&
            vmUsers.missing.Firstname === false &&
            vmUsers.missing.LastName === false &&
            vmUsers.missing.Role === false &&
            vmUsers.missing.Customer === false &&
            vmUsers.missing.Machines === false &&
            vmUsers.missing.Languages === false &&
            vmUsers.missing.Password === false &&
            vmUsers.missing.ConfirmPassword === false &&
            vmUsers.missing.TimeZone === false) {

            if (vmUsers.actual.Email === "") {
                vmUsers.actual.Email = null;
            }
            else if (vmUsers.actual.Email != null && !controlValidationEmail(vmUsers.actual.Email)) {
                errorSwal(resource.EmailNotValid);
                return false;
            }

            if (action === enAction.add || (vmUsers.actual.Password || vmUsers.actual.ConfirmPassword)) {
                if (roleUser == enRoles.Customer) {
                    if (vmUsers.actual.Password == undefined || vmUsers.actual.Password == null || vmUsers.actual.Password.trim() == "" || vmUsers.actual.Password.length < 6) {
                        errorSwal(resource.PasswordPolicy);
                        return false;
                    }
                }

                if (vmUsers.actual.Password != vmUsers.actual.ConfirmPassword) {
                    errorSwal(resource.PasswordsNotSame);
                    return false;
                }
            }
            
            return true;
        } else
            return false;
    };

    var controlValidationEmail = function (email) {
        var regex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
        return regex.test(email);

    };

    setLanguageFlag = function (val, baseUrl) {
        var result;
        switch (val) {
        case 'English':
                result = baseUrl + '/Images/flags/en.png';
            break;
        case 'Italian':
                result = baseUrl + '/Images/flags/it.png';
            break;
        case 'Spanish':
                result = baseUrl + '/Images/flags/es.png';
            break;
        case 'French':
                result = baseUrl + '/Images/flags/fr.png';
            break;
        case 'German':
                result = baseUrl + '/Images/flags/de.png';
            break;
        }
        return result;
    };

    var errorSwal = function (text) {
        swal({
            title: resourceChangePassword.Error,
            text: text,
            icon: "error",
            allowOutsideClick: true,
            closeModal: true
        });
    };


    /*#region CHANGE PASSWORD */

    var initChangePassword = function (data, baseUrl, resourceText) {
        dataPassword = data;
        baseApiUrl = baseUrl + "/ajax/UserManagerApi";
        resourceChangePassword = resourceText;
    };

    var openChangePasswordModal = function (mustChange) {
        if (mustChange) {
            $('#change-password-modal #close').addClass("display-none");
            $('#change-password-modal .modal-footer .btn-annulla').addClass('display-none');
        }
        else {
            $('#change-password-modal #close').removeClass("display-none");
            $('#change-password-modal .modal-footer .btn-annulla').removeClass('display-none');
        }
        $('#last-password').val(null);
        $('#new-password').val(null);
        $('#repeat-new-password').val(null);        
        $('#change-password-modal').modal('show');
      
    };

    var setCurrentTimeZoneDropwdown = function () {
        var request = $.ajax({
            type: 'GET',
            contentType: 'application/json',
            url: baseApiUrl + "/GetCurrentTimeZone"
        });

        request.done(function (data) {
            if (data != null) {

                var dropdown = $('#timezone-modal-input');
                dropdown.val(data);
                
            }
        });

        request.fail(function (jqXHR, textStatus, errorThrown) {
            errorSwal(resourceChangePassword.ErrorOccurred);
        });
    }

    var loadTimeZone = function() {
        var dropdown = $('#timezone-modal-input');
        dropdown.empty();



        var request = $.ajax({
            type: 'GET',
            contentType: 'application/json',
            url: baseApiUrl + "/GetTimeZones",
        });

        request.done(function (data) {
            $.each(data,
                function (key, entry) {
                    dropdown.append($('<option></option>').attr('value', key).text(entry));
                });
            setCurrentTimeZoneDropwdown();
        });

        request.fail(function (jqXHR, textStatus, errorThrown) {
            errorSwal(resourceChangePassword.ErrorOccurred);
        });
    }


    var openChangeTimeZoneModal = function (mustChange) {

        if (mustChange) {
            $('#change-timezone-modal #modal-timezone-advertise').removeClass("display-none");
            $('#change-timezone-modal #close').addClass("display-none");
            $('#change-timezone-modal .modal-footer .btn-annulla').addClass('display-none');
        }
        else {
            $('#change-timezone-modal #modal-timezone-advertise').addClass("display-none");
            $('#change-timezone-modal #close').removeClass("display-none");
            $('#change-timezone-modal .modal-footer .btn-annulla').removeClass('display-none');
        }

        $('#change-timezone-modal').modal('show');


        loadTimeZone();


    };


    var changeTimeZoneClick = function () {
        var timezone = $('#timezone-modal-input').children("option:selected").val();
        if (timezone != null && timezone.length > 0) {
            var request = $.ajax({
                type: 'POST',
                contentType: 'application/json',
                url: baseApiUrl + "/ChangeTimeZone",
                data: JSON.stringify(timezone)
            });

            request.done(function (data) {
                $('#change-timezone-modal').modal('hide');
                var msg = successSwal("");
                msg.then(
                    (result) => {
                        window.location.reload();
                    });
            });

            request.fail(function (jqXHR, textStatus, errorThrown) {
                errorSwal(resourceChangePassword.ErrorOccurred);
            });
        }

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
            if (newPassword == oldPassword)
                errorSwal(resourceChangePassword.PasswordNotChanged);
            else if (newPassword != repeatPassword)
                errorSwal(resourceChangePassword.PasswordNotSame);
            else if (newPassword.length < 6)
                errorSwal(resourceChangePassword.PasswordPolicy);
            else
                changePassword(data);     
                
        }
        else
            errorSwal(resourceChangePassword.EnterPassword);
    };

    var checkSettingTimeZone = function() {
        var request = $.ajax({
            type: 'GET',
            contentType: 'application/json',
            url: baseApiUrl + "/GetCurrentTimeZone"
        });
        
        request.done(function (data) {
            if (data == null) 
                UserManager.openChangeTimeZoneModal(true);

        });

        request.fail(function (jqXHR, textStatus, errorThrown) {
            errorSwal(resourceChangePassword.ErrorOccurred);
        });
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
    };

    var checkFirstLogin = function (user) {
        if (user.Role == enRoles.HeadWorkshop || user.Role == enRoles.Operator) {
            $.ajax({
                url: baseApiUrl + "/CheckFirstLogin",
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result == true) {
                        UserManager.openChangePasswordModal(true);
                    }
                },
                error: function (xhr, status, error) {
                    errorSwal(resourceChangePassword.ErrorOccurred);
                }
            });
        }
        
            UserManager.checkSettingTimeZone();
        
    };


    var openDisclamerModal = function () {       
        $('#disclamer-modal').modal('show');
    };

    /*#endregion*/


    return {
        init: init,
        initChangePassword: initChangePassword,
        addClickEvent: addClickEvent,
        addUser: addUser,
        modifyUser: modifyUser,
        modifyClickEvent: modifyClickEvent,
        deleteClickEvent: deleteClickEvent,
        resetPasswordClickEvent: resetPasswordClickEvent,
        clearActualUser: clearActualUser,
        changePasswordClick: changePasswordClick,
        openChangePasswordModal: openChangePasswordModal,
        openDisclamerModal: openDisclamerModal,
        openChangeTimeZoneModal: openChangeTimeZoneModal,
        changeTimeZoneClick: changeTimeZoneClick,
        checkFirstLogin: checkFirstLogin,
        checkSettingTimeZone: checkSettingTimeZone,
        setCurrentTimeZoneDropwdown: setCurrentTimeZoneDropwdown,
        loadTimeZone: loadTimeZone
    };
}()