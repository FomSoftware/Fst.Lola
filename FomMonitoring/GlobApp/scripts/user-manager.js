var UserManager = function ()
{
    var userManagerAPI;
    var vmUsers;

    var init = function (urlAPI)
    {
        userManagerAPI = urlAPI;

        initVueModelUser();
        getUsers();
    }

    var initVueModelUser = function ()
    {
        vmUsers = new Vue({
            el: '#user-modal-form',
            data : {
                users: {},
                actual: {},
                roles: {
                    active: '',
                    all: []
                },
                missing : {
                    Username: false,
                    Firstname: false,
                    LastName: false,
                    Email: false,
                    Role: false,
                    Password: false,
                    RepeatPassword : false
                }
            },
            methods : {
                formValidation: function ()
                {
                    this.actual.Username == undefined || this.actual.Username == null || this.actual.Username.trim() == "" ? this.missing.Username = true : this.missing.Username = false
                    this.actual.FirstName == undefined || this.actual.FirstName == null || this.actual.FirstName.trim() == "" ? this.missing.FirstName = true  : this.missing.FirstName = false
                    this.actual.LastName == undefined || this.actual.LastName == null || this.actual.LastName.trim() == "" ? this.missing.LastName = true  : this.missing.LastName = false
                    this.actual.Email == undefined || this.actual.Email == null || this.actual.Email.trim() == "" ? this.missing.Email = true  : this.missing.Email = false
                    this.roles.active == "" ? this.missing.Role = true  : this.missing.Role = false
                    this.actual.Password == undefined || this.actual.Password == null || this.actual.Password.trim() == "" ? this.missing.Password = true : this.missing.Password = false
                    this.actual.RepeatPassword == undefined || this.actual.RepeatPassword == null || this.actual.RepeatPassword.trim() == "" ? this.missing.RepeatPassword = true : this.missing.RepeatPassword = false
                }
            }
        })
    }

    var getUsers = function ()
    {
        $.get({
            url: userManagerAPI + '/Users',
            contentType: "application/json; charset=utf-8",
            success: function (result)
            {
                vmUsers.users = result.Data;
                initDataTable("#users-table", result.Data);
            },
            error: function (xhr, status, error)
            {
                alert(error);
            }
        }).then(function(result)
        {
            getRoles();
        });

    };

    var getRoles = function ()
    {
        $.get({
            url: userManagerAPI + '/Roles',
            contentType: "application/json; charset=utf-8",
            success: function (result)
            {
                vmUsers.roles.all = result.Data;
            },
            error: function (xhr, status, error)
            {
                alert(error);
            }
        });
    }

    var initDataTable = function (renderID, data)
    {

        data.forEach(function (elem, index)
        {
            elem.Modify = '<div class="button btn-modify" onclick="UserManager.modifyClickEvent(\'' + elem.ID + '\')" data-id="' + elem.ID + '">Modifica</div>';
            if (elem.Enabled)
            {
                elem.Switch = '<div class="button btn-disactive" onclick="UserManager.disabledClickEvent(\'' + elem.ID + '\')" data-id="' + elem.ID + '">Disabilita</div>';
            }
            else
            {
                elem.Switch = '<div class="button btn-active" onclick="UserManager.enabledClickEvent(\'' + elem.ID + '\')" data-id="' + elem.ID + '">Abilita</div>';
            }
        });

        var config = {
            data: data,
            columns: [{ title: "Username", data: "Username" },
                       { title: "Nome", data: "FirstName" },
                       { title: "Cognome", data: "LastName" },
                       { title: "Email", data: "Email" },
                       { title: "Ruolo", data: "Roles[0].Name" },
                       { title: "", data: "Modify", orderable: false },
                       { title: "", data: "Switch", orderable: false }
            ],
            info: false,
            order: [],
            paging: false,
            //scrollX: true,
            responsive: true,
            autoWidth: true,
            //order: [[orderDefault, orderType]],
            language: {
                search: "",
                searchPlaceholder: "Cerca",
                emptyTable: "Non ci sono dati da visualizzare"
            },
        };
        var table = $(renderID).DataTable(config);
    }

    var addClickEvent = function ()
    {
            $('#user-modal .modal-title').text("AGGIUNGI UTENTE");
            $('#user-modal').modal('show');
            $('#user-modal .js-modify').hide();
            $('#user-modal .js-add').show();           
    }
    
    var addUser = function ()
    {
        vmUsers.formValidation();
        if (controlValidation())
            {
                var data = {
                    User: {
                        "ID": vmUsers.actual.ID,
                        "DefaultHomePage": "",
                        "Email": vmUsers.actual.Email,
                        "Enabled": true,
                        "Username": vmUsers.actual.Username,
                        "FirstName": vmUsers.actual.FirstName,
                        "LastName": vmUsers.actual.LastName,
                        "Password": vmUsers.actual.Password
                    },
                    RoleIDs: [vmUsers.roles.active],
                    GroupIDs: []
                };

                $.post({
                    url: userManagerAPI + '/Users',
                    data: JSON.stringify(data),
                    contentType: "application/json; charset=utf-8",
                    success: function (result)
                    {
                        if (!result.HasError)
                        {
                            successSwal("Utente creato correttamente.");
                            refreshTable();
                        }
                    },
                    error: function (xhr, status, error)
                    {
                        errorSwal("Si è verificato un errore con la creazione del nuovo utente.");
                    }
                });
        } else
        {
            null
        }
    }

    var modifyClickEvent = function (userID)
    {
        $('#user-modal .modal-title').text("MODIFICA UTENTE");
            $('#user-modal').modal('show');
            $('#user-modal .js-add').hide();
            $('#user-modal .js-modify').show();

           var user = $.grep(vmUsers.users, function (element, index)
            {
                return element.ID == userID;
           });
           vmUsers.actual = Object.assign({}, user[0]);
           vmUsers.actual.Password = "";
           vmUsers.actual.RepeatPassword = "";
           vmUsers.roles.active = vmUsers.actual.Roles[0].ID;
    }

    var modifyUser = function ()
    {
        vmUsers.formValidation();
        if ( controlValidation() )
             {
               var data = {
                    User: {
                        "ID": vmUsers.actual.ID,
                        "Domain": "",
                        "DefaultHomePage": "",
                        "Email": vmUsers.actual.Email,
                        "Enabled": true,
                        "Username": vmUsers.actual.Username,
                        "FirstName": vmUsers.actual.FirstName,
                        "LastName": vmUsers.actual.LastName,
                        "Password": vmUsers.actual.Password
                    },
                    RoleIDs: [vmUsers.roles.active],
                    GroupIDs: []
                };

                $.ajax({
                    url: userManagerAPI + '/Users',
                    data: JSON.stringify(data),
                    type: "PUT",
                    contentType: "application/json; charset=utf-8",
                    success: function (result)
                    {
                        if (!result.HasError)
                        {
                            successSwal("Utente aggiornato correttamente.");
                            refreshTable();
                        } else
                        {
                            errorSwal("Si è verificato un errore con l'aggiornamento del utente.");
                        }
                    },
                    error: function (xhr, status, error)
                    {
                        errorSwal("Si è verificato un errore con l'aggiornamento del utente.");
                    }
                });
        } else
        {
            null
        }
    }

    var disabledClickEvent = function (userID)
    {
            vmUsers.actual = $.grep(vmUsers.users, function (element, index)
            {
                return element.ID == userID;
            });

            vmUsers.actual = vmUsers.actual[0];

            swal({
                title: "",
                text: "Si è sicuri di voler disabilitare questo utente?",
                className: "text-modal-disactive",
                icon: "warning",
                buttons: {
                    cancel: {
                        text: "No, torna indietro",
                        visible: true,
                        className: "cancel",
                        closeModal: true,
                    },
                    confirm: {
                        text: "Si, continua",
                        visible: true,
                        className: "confirm",
                        closeModal: true
                    }
                }
            }).then(function(result)
            {
                if (result)
                    disabledUser(userID);
            });
    }

    var enabledClickEvent = function (userID) {
        vmUsers.actual = $.grep(vmUsers.users, function (element, index) {
            return element.ID == userID;
        });

        vmUsers.actual = vmUsers.actual[0];

        swal({
            title: "",
            text: "Si è sicuri di voler abilitare questo utente?",
            className: "text-modal-disactive",
            icon: "warning",
            buttons: {
                cancel: {
                    text: "No, torna indietro",
                    visible: true,
                    className: "cancel",
                    closeModal: true,
                },
                confirm: {
                    text: "Si, continua",
                    visible: true,
                    className: "confirm",
                    closeModal: true
                }
            }
        }).then(function(result) {
            if (result)
                enabledUser(userID);
        });
    }

    var enabledUser = function (userID) {
        $.ajax({
            url: userManagerAPI + '/Users' + "/" + userID + "/Enable",
            type: "PUT",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                if (!result.HasError) {
                    successSwal("Utente abilitato correttamente.");
                    refreshTable();
                } else {
                    errorSwal("Si è verificato un errore con l'abilitazione del utente.");
                }
            },
            error: function (xhr, status, error) {
                errorSwal("Si è verificato un errore con l'abilitazione del utente.");
            }
        });
    }

    var disabledUser = function (userID)
    {
        $.ajax({
            url: userManagerAPI + '/Users' + "/" + userID,
            type: "DELETE",
            contentType: "application/json; charset=utf-8",
            success: function (result)
            {
                if (!result.HasError)
                {
                    successSwal("Utente eliminato correttamente.");
                    refreshTable();
                } else
                {
                    errorSwal("Si è verificato un errore con l'eliminazione del utente.");
                }
            },
            error: function (xhr, status, error)
            {
                errorSwal("Si è verificato un errore con l'eliminazione del utente.");
            }
        });
    }

    var refreshTable = function ()
    {
        $.get({
            url: userManagerAPI + '/Users',
            contentType: "application/json; charset=utf-8",
            success: function (result)
            {
                result.Data.forEach(function (elem, index)
                {
                    elem.Modify = '<div class="button btn-modify" onclick="UserManager.modifyClickEvent(\'' + elem.ID + '\')" data-id="' + elem.ID + '">Modifica</div>';
                    if (elem.Enabled) {
                        elem.Switch = '<div class="button btn-disactive" onclick="UserManager.disabledClickEvent(\'' + elem.ID + '\')" data-id="' + elem.ID + '">Disabilita</div>';
                    }
                    else {
                        elem.Switch = '<div class="button btn-active" onclick="UserManager.enabledClickEvent(\'' + elem.ID + '\')" data-id="' + elem.ID + '">Abilita</div>';
                    }
                });

                var table = $("#users-table").DataTable();
                table.clear();
                table
                    .rows
                    .add(result.Data)
                    .draw();

                vmUsers.users = result.Data;
                vmUsers.actual = {};
                vmUsers.roles.active = '';
                $('#user-modal').modal('hide');

            }
        });
    }

    var successSwal = function (text)
    {
        swal({
            title: "",
            text: text,
            icon: "success",
            allowOutsideClick: true,
            closeModal: true
        });
    }

    var errorSwal = function (text)
    {
        swal({
            title: "Errore",
            text: text,
            icon: "error",
            allowOutsideClick: true,
            closeModal: true
        });
    }

    var clearActualUser = function ()
    {
        vmUsers.actual = {};
        vmUsers.roles.active = '';
        vmUsers.missing.Username = false;
        vmUsers.missing.Firstname = false,
        vmUsers.missing.LastName = false,
        vmUsers.missing.Email = false,
        vmUsers.missing.Role = false,
        vmUsers.missing.Password = false,
        vmUsers.missing.RepeatPassword = false
    }
    
    var controlValidation = function ()
    {
        if (vmUsers.missing.Username == false &&
           vmUsers.missing.Firstname == false &&
           vmUsers.missing.LastName == false &&
           vmUsers.missing.Email == false &&
           vmUsers.missing.Role == false &&
           vmUsers.missing.Password == false)
        {
            if (!controlValidationEmail(vmUsers.actual.Email))
            {
                errorSwal("L'email inserita non è valida");
                return false;
            }
            if (vmUsers.actual.Password.trim().length < 6)
            {
                errorSwal("La password inserita è troppo corta.");
                return false;
            }
            if (vmUsers.actual.Password != vmUsers.actual.RepeatPassword)
            {
                errorSwal("Le password inserite non sono uguali.");
                return false;
            }
            return true;
        } else
        {
            return false;
        }
    }
    
    var controlValidationEmail = function (email)
    {
        var regex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
        return regex.test(email);
     
    }

    return {
        init: init,
        addClickEvent: addClickEvent,
        addUser: addUser,
        modifyUser: modifyUser,
        modifyClickEvent: modifyClickEvent,
        enabledClickEvent: enabledClickEvent,
        disabledClickEvent: disabledClickEvent,
        clearActualUser: clearActualUser
    }
}()