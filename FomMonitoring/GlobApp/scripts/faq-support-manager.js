var FaqSupportManager = function ()
{
    var resource;
    var baseApiUrl;
   
    var initLogin = function (baseUrl, resourceText) {
        resource = resourceText;
        baseApiUrl = baseUrl + "/Account";
        initMenu();

    }

    var inviaForm = function() {
        var data = {
            Nome: $('#nome_cognome').val(),
            Azienda: $('#azienda').val(),
            Email: $('#email').val(),
            Prefisso: $('#prefisso').val(),
            Telefono: $('#telefono').val(),
            NomeMacchina: $('#nome_macchina').val(),
            Seriale: $('#seriale').val(),
            Testo: $('#testo').val(),
            File: $('#file')[0].files.length > 0 ? $('#file')[0].files[0] : null
        };

        var formData = new FormData($('#supportForm')[0]);

        $.ajax({

         
            contentType: false,
            processData: false,
            cache: false,
            url: baseApiUrl + "/InviaSupportReq",
            data: formData,
            type: "POST",
            success: function (out) {
                if (out.result == true) {
                    successSwal(resource.RichiestaInviata);
                    var dim = '-' + $('#support-menu').width() + 'px';
                    $('#support-menu').css('right', dim);
                    $('#supportForm')[0].reset();
                } else {
                    errorSwal(out.msg);
                }
            },
            error: function (xhr, status, error) {
                errorSwal(resource.ErrorOccurred);
            }
        });
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

    var successSwal = function (text) {
        swal({
            title: "",
            text: text,
            icon: "success",
            allowOutsideClick: true,
            closeModal: true
        });
    }

    var initMenu = function ()
    {
        $('.js-open-menu').click(function (e)
        {
            e.preventDefault();
            $('#mobile-login-menu').css('right', '0');
        });

        $('.js-close-menu').click(function (e)
        {
            e.preventDefault();
            var dim = '-' + $('#mobile-login-menu').width() + 'px';
            $('#mobile-login-menu').css('right', dim);
        });
    
        $('.js-open-support').click(function (e) {
            e.preventDefault();
            $('#support-menu').css('right', '0');
        });

        $('.js-close-support').click(function (e) {
            e.preventDefault();
            var dim = '-' + $('#support-menu').width() + 'px';
            $('#support-menu').css('right', dim);
        });

        $('.js-open-faq').click(function (e) {
            e.preventDefault();
            $('#faq-menu').css('right', '0');
        });

        $('.js-close-faq').click(function (e) {
            e.preventDefault();
            var dim = '-' + $('#faq-menu').width() + 'px';
            $('#faq-menu').css('right', dim);
        });

        $('.js-open-faq-item').click(function (e) {
            e.preventDefault();
            var id = e.target.id.substring(5);

            if ($('#up_' + id).css('display') === "block") {
                $('#up_' + id).css('display', 'none');
                $('#down_' + id).css('display', 'block');
                var classe1 = '.faq-menu-item-' + id;
                $(classe1).css('display', 'none');
            } else {
                $('.faq-item i.icon-arrow-up').css('display', 'none');
                $('.faq-item i.icon-arrow-down').css('display', 'block');
                $('.faq-item div.sub-menu').css('display', 'none');

                $('#down_' + id).css('display', 'none');
                $('#up_' + id).css('display', 'block');
                var classe = '.faq-menu-item-' + id;
                $(classe).css('display', 'block');
            }
            
        });

        $("#inviaBtn").click(function (e) {
            e.preventDefault();
            inviaForm();
        });

    }

    var controlValidationEmail = function (email) {
        var regex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
        return regex.test(email);

    };

    var validateSupportForm = function(e) {
        if (e.required && (e.value === "" || !e.value)) {
            $('#' + e.id).addClass("missingField");
            $('#inviaBtn').prop('disabled', true);
        } else {
            $('#' + e.id).removeClass("missingField");
        }
        checkInvia();
    }

    var checkFileSize = function(e) {
        var maxAllowedSize = 3 * 1024 * 1024;
        if (e.files && e.files[0] && e.files[0].size > maxAllowedSize) {
            // Here you can ask your users to load correct file
            $('#' + e.id).addClass("missingField");
            $('#inviaBtn').prop('disabled', true);
            errorSwal(resource.MaxFileSizeError);
        } else {
            $('#' + e.id).removeClass("missingField");
        }

        checkInvia();
    }

    var checkInvia = function () {
        var maxAllowedSize = 3 * 1024 * 1024;

        if (!$('#nome_cognome').val() || !$('#azienda').val() ||
            !$('#email').val() || !$('#prefisso').val() || !$('#telefono').val() || !$('#testo').val()) {
            $('#inviaBtn').prop('disabled', true);
        } else if ($("#file").files && $("#file").files[0] && $("#file").files[0].size > maxAllowedSize) {
            $('#inviaBtn').prop('disabled', true);
        }
        else if (!controlValidationEmail($('#email').val())) {
            $('#inviaBtn').prop('disabled', true);
            $('#email').addClass("missingField");
        }
        else {
            $('#email').removeClass("missingField");
            $('#inviaBtn').prop('disabled', false);
        }
    }


    var initBootstrapSelect = function ()
    {
        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent))
        {
            $('.bs-select').selectpicker({
                mobile: true,
                iconBase: 'fa',
                tickIcon: 'fa-check'
            });
        }
        else
        {
            $('.bs-select').selectpicker({
                iconBase: 'fa',
                tickIcon: 'fa-check'
            });
        }
    }

    return {
        initLogin: initLogin,
        initBootstrapSelect: initBootstrapSelect,
        validateSupportForm: validateSupportForm,
        checkFileSize: checkFileSize
}

}();