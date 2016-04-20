var Login = function () {
  var createUserState = false;
  var exitRegister = false;
  var EvercamApi = "https://api.evercam.io/v1";

  var onBodyLoad = function() {
    if (getQueryStringByName("api_id") != null && getQueryStringByName("api_key") != null && getQueryStringByName("id") != null)
      get_user(getQueryStringByName("id"), getQueryStringByName("api_id"), getQueryStringByName("api_key"));
    else {
      if (localStorage.getItem("api_id") != null && localStorage.getItem("api_id") != undefined &&
        localStorage.getItem("api_key") != null && localStorage.getItem("api_key") != undefined)
        window.location = 'index.html';
      $("#country").select2({
        placeholder: '<i class="icon-map-marker"></i>&nbsp;Select a Country',
        allowClear: true,
        formatResult: format,
        formatSelection: format,
        escapeMarkup: function(m) {
          return m;
        }
      });
    }
  };

  var getQueryStringByName = function (name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
      results = regex.exec(location.search);
    return results == null ? null : decodeURIComponent(results[1].replace(/\+/g, " "));
  };

  var format = function(state) {
    if (!state.id) return state.text; // optgroup
    return "<img class='flag' src='assets/img/flags/" + state.id.toLowerCase() + ".png'/>&nbsp;&nbsp;" + state.text;
  };

  var clearForm = function() {
    $("#first_name").val("");
    $("#last_name").val("");
    $("#user_name").val("");
    $("#user_email").val("");
    $("#password").val("");
    $("#country").val("");
    $('.alert-error', $('.login-form')).slideUp();
    $('#LoaderRegister').hide();
    NProgress.done();
    $(".font-size16").removeClass("font-color-red");
    $('.alert-error').slideUp();
  };

  var handleRegisterUser = function() {
    $(".register_user").bind("click", function() {
      if (exitRegister) return;
      if (!createUserState) {
        createUserState = true;
        $("#divRegister").slideDown(900, function() {
          $(".cancel_register").fadeIn();
          $("#register-image").removeClass("padding-left-15");
        });
        $("#sign-in").slideUp(900)
      } else {
        if ($("#first_name").val() == "" && $("#last_name").val() == "" && $("#user_name").val() == "" && $("#user_email").val() == "" && $("#password").val() == "" && $("#country").val() == "") {
          $('.alert-error').slideDown();
          $('.alert-error span').html('Please enter required fields.');
          $(".font-size16").addClass("font-color-red");
          return
        } else {
          $(".font-size16").removeClass("font-color-red");
          var isReturn = false;

          if ($("#first_name").val() == "") {
            $('.alert-error').slideDown();
            $('.alert-error span').html('Please enter required field: First Name.');
            $("#spnReqFN").addClass("font-color-red");
            isReturn = true;
          }
          if ($("#last_name").val() == "") {
            $('.alert-error').slideDown();
            $('.alert-error span').html('Please enter required field: Last Name.');
            $("#spnReqLN").addClass("font-color-red");
            isReturn = true;
          }
          if ($("#user_name").val() == "") {
            $('.alert-error').slideDown();
            $('.alert-error span').html('Please enter required field: Username.');
            $("#spnReqUN").addClass("font-color-red");
            isReturn = true;
          }
          if ($("#user_email").val() == "") {
            $('.alert-error').slideDown();
            $('.alert-error span').html('Please enter required field: Email.');
            $("#spnReqEmail").addClass("font-color-red");
            isReturn = true;
          }
          if ($("#password").val() == "") {
            $('.alert-error').slideDown();
            $('.alert-error span').html('Please enter required field: Password.');
            $("#spnReqPass").addClass("font-color-red");
            isReturn = true;
          }
          if ($("#country").val() == "") {
            $('.alert-error').slideDown();
            $('.alert-error span').html('Please enter required field: Country.');
            $("#spnReqCountry").addClass("font-color-red");
            isReturn = true;
          }
          if (isReturn) {
            $('.alert-error').slideDown();
            $('.alert-error span').html('Please enter required fields.');
            return;
          }

          $(".font-size16").removeClass("font-color-red");
          $('.alert-error').slideUp();
          $('#LoaderRegister').css({
            position: 'absolute',
            top: ($('#divRegister').height() / 2) - 22,
            'z-index': '5',
            left: ($('#divRegister').width() / 2) - 22,
          });
          $('#LoaderRegister').show();
          NProgress.start();

          $.ajax({
            type: 'POST',
            crossDomain: true,
            url: EvercamApi + '/users.json',
            data: { firstname: $("#first_name").val(), lastname: $("#last_name").val(), username: $("#user_name").val(), country: $("#country").val().toLowerCase(), email: $("#user_email").val(), password: $("#password").val() },
            dataType: 'json',
            ContentType: 'application/json; charset=utf-8',
            success: function(res) {
              $('.alert-error').slideUp();
              createUserState = false;
              exitRegister = true;
              $("#spnCamcel").fadeOut();
              $('.register_user img').attr('src', 'assets/img/ecupg.png');
              $("#divRegister").slideUp(900, function() {
                $("#divSuccess").html('Thank you for registering, <b>' + res.users[0].username + '</b>. An email has been dispatched to <b>' + res.users[0].email + '</b> with details on how to activate your account.');
                $("#divSuccess").fadeIn();
                $("#register-image").addClass("padding-left-15");
                clearForm();
              });
            },
            error: function(xhr, textStatus) {
              $('.alert-error').slideDown();
              $('.alert-error span').html(xhr.responseJSON.message + ' ' + xhr.responseJSON.context);
              $('#LoaderRegister').hide();
              NProgress.done();
            }
          });
        }
      }
    });

    $(".cancel_register").bind("click", function() {
      createUserState = false;
      $("#sign-in").slideDown(900)
      $(".cancel_register").fadeOut();
      $("#divRegister").slideUp(900, function() {
        $("#register-image").addClass("padding-left-15");
        clearForm();
      });
    });
  };

  var loginButton = function () {
    $("#loginRequest1").on("click", function () {
      NProgress.start();
      login();
    });
  }

  var login = function () {
    NProgress.start();
    var username = $("#txtUsername").val();
    var password = $("#txtPassword").val();
    if (username == "" || password == "") {
      alert_notification(".bb-alert", "Invalid login/password combination");
      NProgress.done();
      return;
    }
    $.ajax({
      type: 'GET',
      crossDomain: true,
      url: EvercamApi + "/users/" + username + "/credentials?password=" + password,
      data: {},
      dataType: 'json',
      ContentType: 'application/json; charset=utf-8',
      success: function (response) {
        get_user(username, response.api_id, response.api_key)
        NProgress.done();
      },
      error: function (xhr, textStatus) {
        alert_notification(".bb-alert", xhr.responseJSON.message);
        NProgress.done();
      }
    });
  }

  var get_user = function (username, api_id, api_key) {
    $.ajax({
      type: 'GET',
      crossDomain: true,
      url: EvercamApi + "/users/" + username + "?api_id=" + api_id + "&api_key=" + api_key,
      data: {},
      dataType: 'json',
      ContentType: 'application/json; charset=utf-8',
      success: function (response) {
        localStorage.setItem("api_id", api_id);
        localStorage.setItem("api_key", api_key);
        localStorage.setItem("user", JSON.stringify(response.users[0]));
        window.location = 'index.html';
        NProgress.done();
      },
      error: function (xhr, textStatus) {
        alert_notification(".bb-alert", xhr.responseJSON.message);
        NProgress.done();
      }
    });
  }

  var alert_notification = function (selector, message) {
    var elem = $(selector);
    elem.find("span").html(message);
    elem.delay(200).fadeIn().delay(4000).fadeOut();
  }

  var onKeyEnter = function () {
    $("#txtPassword").on("keyup", function (e) {
      var code = e.keyCode || e.which;
      if (code == 13) {
        login();
      }
    });
  }

  return {
    init: function () {
      onBodyLoad();
      handleRegisterUser();
      loginButton();
      onKeyEnter();
      $(document).ready(function(){
        NProgress.done();
      });
    }
  };
}();