var Login = function () {
  var createUserState = false;
  var exitRegister = false;
  var EvercamApi = "https://api.evercam.io/v1";

var onBodyLoad = function() {
  if (getQueryStringByName("api_id") != null && getQueryStringByName("api_key") != null && getQueryStringByName("id") != null) {
    NProgress.start();
    get_user(getQueryStringByName("id"), getQueryStringByName("api_id"), getQueryStringByName("api_key"));
  } else {
    if (localStorage.getItem("api_id") != null && localStorage.getItem("api_id") != undefined &&
      localStorage.getItem("api_key") != null && localStorage.getItem("api_key") != undefined)
      window.location = 'index.html';
  }
};

var getQueryStringByName = function (name) {
  name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
  var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
    results = regex.exec(location.search);
  return results == null ? null : decodeURIComponent(results[1].replace(/\+/g, " "));
};

var loginButton = function () {
  $("#loginRequest1").on("click", function () {
    login();
  });
}

var login = function () {
  var username = $("#txtUsername").val();
  var password = $("#txtPassword").val();
  if (username == "" || password == "") {
    alert_notification(".bb-alert", "Invalid login/password combination");
    return;
  }
  NProgress.start();
  $.ajax({
    type: 'GET',
    crossDomain: true,
    url: EvercamApi + "/users/" + username + "/credentials?password=" + password,
    data: {},
    dataType: 'json',
    ContentType: 'application/json; charset=utf-8',
    success: function (response) {
      get_user(username, response.api_id, response.api_key)
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
      loginButton();
      onKeyEnter();
      NProgress.done();
    }
  };
}();