var Index = function () {
  var isLogin = false;
  var createUserState = false;
  var exitRegister = false;
  var EvercamApi = "https://api.evercam.io/v1";
  var appApiUrl = "http://snapmail.evercam.io/api/snapmails";
  var utilsApi = "http://snapmail.evercam.io/api";
  var appsCount = 1;
  var loggedInUser;
  var isFirst = true;
  var sliders = [];
  var user = null;

  function toTitleCase(str) {
    return str.charAt(0).toUpperCase() + str.substr(1);
  }

  var handleLoginSection = function() {
    if (localStorage.getItem("api_id") != null && localStorage.getItem("api_id") != undefined &&
      localStorage.getItem("api_key") != null && localStorage.getItem("api_key") != undefined) {
      user = JSON.parse(localStorage.getItem("user"));
      getCameras(true);
      getSnapmails();
    } else {
      window.location = 'login.html';
    }
  };

  var clearPage = function () {
    $("#liUsername").show();
    $("#lnkSignout").show();
    $("#btnNewApp").show();
    $("#divMainContainer").removeClass("container-bg");

    $("#newApp").html("");
    $("#newApp").fadeOut();
        
  }

  var handleLogout = function() {
    $("#lnkLogout").bind("click", function() {
      localStorage.removeItem("api_id");
      localStorage.removeItem("api_key");
      localStorage.removeItem("user");
      localStorage.removeItem("EvercamCameras");
      window.location = 'login.html';
      NProgress.start();
    });

    $("#btnLogout").bind("click", function() {
      localStorage.removeItem("api_id");
      localStorage.removeItem("api_key");
      localStorage.removeItem("user");
      localStorage.removeItem("EvercamCameras");
      window.location = 'login.html';
      NProgress.start();
    });

    var isRTL = false;
    if ($('body').css('direction') === 'rtl') {
      isRTL = true;
    }
    $('.timepicker-default').timepicker({
      minuteStep: 5,
      showSeconds: false,
      showMeridian: false
    });
    var d = new Date();
    $("#txtTime").val(d.getHours() + ':' + d.getMinutes());
  };

  var handleNewApp = function() {
    $("#lnNewApp").bind("click", function() {


    });

    $("#lnNewAppCol").bind("click", function() {
      $("#newApp").slideUp(800, function() {
        $("#newApp").html("");
        $("#lnNewApp").show();
        $("#lnNewAppCol").hide();
      });

      ApiAction = 'POST';
      $("#txtCameraCode0").val('');
    });

    $("#lnNewCamera").bind("click", function() {
      showNewCameraForm();
    });
    $("#lnNewCameraCol").bind("click", function() {
      $("#newApp").slideUp(800, function() {
        $("#newApp").html("");
        $("#lnNewCamera").show();
        $("#lnNewCameraCol").hide();
      });
    });
  };

  var clearForm = function() {
    $(".formButtonCancel").click();
    $(".modal-header h3").html("New SnapMail");
    $("#txtkey").val('');
    $("#txtRecipient").val('');
    var tz_info = jzTimezoneDetector.determine_timezone();
    $("#ddlTimezone").val(GetUtcZoneValue(tz_info.key));
    var d = new Date();
    $("#txtTime").val(d.getHours() + ':' + d.getMinutes());
    $("#divAlert").slideUp();
    $(".select2-search-choice").remove();
    $("span.tag").remove();
    $('#ddlCameras :selected').each(function(i, selected) {
      $(selected).removeAttr("selected");
    });
    $("#uniform-chkMon span").removeClass("checked");
    $("#chkMon").attr("checked", false);
    $("#uniform-chkTue span").removeClass("checked");
    $("#chkTue").attr("checked", false);
    $("#uniform-chkWed span").removeClass("checked");
    $("#chkWed").attr("checked", false);
    $("#uniform-chkThu span").removeClass("checked");
    $("#chkThu").attr("checked", false);
    $("#uniform-chkFri span").removeClass("checked");
    $("#chkFri").attr("checked", false);
    $("#uniform-chkSat span").removeClass("checked");
    $("#chkSat").attr("checked", false);
    $("#uniform-chkSun span").removeClass("checked");
    $("#chkSun").attr("checked", false);
    $("#uniform-chkAllDay span").removeClass("checked");
    $("#chkAllDay").attr("checked", false);
  };

  var CheckAppStatus = function (appCode) {
    $.ajax({
      type: 'GET',
      url: appApiUrl + '/apps/' + appCode,
      dataType: 'json',
      ContentType: 'application/json',
      success: function (res) {
        if (res.status == 0 || res.status == 1) {
          setTimeout(function () { CheckAppStatus(appCode); }, 10000);
        }
        else if (res.status == 2) {
          if (res.app_url) {
            $("#lnkDownload" + appCode).removeClass("link-disable").addClass("link-down");
            $("#lnkDownload" + appCode).attr("href", res.app_url);
            $("#lnkDownload" + appCode).attr("title", "Download .apk");
            $("#lnkShare" + appCode).removeClass("link-disable").addClass("link-down").addClass("open");
            $("#lnkShare" + appCode).attr("title", "Share .apk with friend");
            $('#tile' + appCode).effect("pulsate");
          }
        }
        else {

        }
      },
      error: function (xhr, textStatus) {

      }
    });
  };

  var GetWeekdaysSelected = function () {
    var wDays = "";
    if ($("#chkMon").attr("checked"))
      wDays += "Mon,";
    if ($("#chkTue").attr("checked"))
      wDays += "Tue,";
    if ($("#chkWed").attr("checked"))
      wDays += "Wed,";
    if ($("#chkThu").attr("checked"))
      wDays += "Thu,";
    if ($("#chkFri").attr("checked"))
      wDays += "Fri,";
    if ($("#chkSat").attr("checked"))
      wDays += "Sat,";
    if ($("#chkSun").attr("checked"))
      wDays += "Sun,";
    if (wDays.length > 0)
      return wDays.substring(0, wDays.lastIndexOf(','));
    return wDays;
  }
    
  $(".formButtonCancel, .modal-backdrop").live("click", function () {
    clearForm();
  });

  $(".formButtonOk").live("click", function () {
    $("#divAlert").slideUp();
    $("#divAlert").removeClass("alert-info").addClass("alert-error");
    var cameraIds = "", cameraNames="";
    $('#ddlCameras :selected').each(function (i, selected) {
      cameraIds += $(selected).val() + ",";
      cameraNames += $(selected).text() + ", ";
    });
        
    if (cameraIds == "") {
      $("#divAlert").slideDown();
      $("#divAlert span").html("Please select camera(s) to continue.");
      return;
    }
    cameraIds = cameraIds.substring(0, cameraIds.lastIndexOf(','));
    cameraNames = cameraNames.substring(0, cameraNames.lastIndexOf(','));

    if ($("#txtRecipient").val() != '') {
      var emails = $("#txtRecipient").val().split(",");
      for (var i = 0; i < emails.length; i++) {
        $("#divAlert span").html("");
        $("#divAlert").slideUp();
        if (!validateEmailByVal(emails[i])) {
          $("#divAlert").slideDown();
          $("#divAlert span").html("Invalid email '" + emails[i] + "'.");
          return;
        }
      }
    }
    else {
      if ($("#txtkey").val() == "") {
        $("#divAlert").slideDown();
        $("#divAlert span").html("Please enter recipients to continue.");
        return;
      }
    }
    if ($("#txtTime").val() == "") {
      $("#divAlert").slideDown();
      $("#divAlert span").html("Please select time to continue.");
      return;
    }
    if ($("#ddlTimezone").val() == "0") {
      $("#divAlert").slideDown();
      $("#divAlert span").html("Please select timezone to continue.");
      return;
    }
    if (GetWeekdaysSelected() == "") {
      $("#divAlert").slideDown();
      $("#divAlert span").html("Please select day(s) to continue.");
      return;
    }
    var o = {
      "user_id": user.id,
      "user_name": user.firstname + ' ' + user.lastname,
      "cameras": cameraIds,
      "camera_names": cameraNames,
      "recipients": $("#txtRecipient").val(),
      "notify_days": GetWeekdaysSelected(),
      "notify_time": $("#txtTime").val(),
      "timezone": $("#ddlTimezone").val(),
      "is_active": true,
      "access_token": localStorage.getItem("api_id") + ":" + localStorage.getItem("api_key")
    };
    var action = "POST", queryString = "";
    if ($("#txtkey").val() != "") {
      action = "PUT";
      queryString = "/" + $("#txtkey").val();
    }
    $.ajax({
      type: action,
      url: appApiUrl + queryString,
      data: o,
      dataType: 'json',
      ContentType: "application/x-www-form-urlencoded",
      beforeSend: function (xhrObj) {},
      success: function (data) {
        if ($("#txtkey").val() != "")
          getSnapmails(true, $("#txtkey").val());
        else
          getSnapmails(false, $("#txtkey").val());
        clearForm();
      },
      error: function (response) {
        $("#divAlert").removeClass("alert-info").addClass("alert-error");
        $("#divAlert span").html('Error: ' + response.responseJSON.ExceptionMessage);
        $("#divAlert").slideDown();
      }
    });
  });

  $('[name="AppIcon"]').live("click", function () {
    var id = $(this).attr("id");
    if (id == "chkChoose") {
      $("#divChooseFile").slideDown();
    }
    else
      $("#divChooseFile").slideUp();
  });
    
  var getDaysAbbrivation = function (notifyDays) {
    return notifyDays;
    /*var wDays = "";
        var days = notifyDays.split(",");
        for (var i = 0; i < days.length; i++) {
            switch (days[i]) {
                case "Mon":
                    wDays += "M";
                    break;
                case "Tue":
                    wDays += "T";
                    break;
                case "Wed":
                    wDays += "W";
                    break;
                case "Thu":
                    wDays += "T";
                    break;
                case "Fri":
                    wDays += "F";
                    break;
                case "Sat":
                    wDays += "S";
                    break;
                case "Sun":
                    wDays += "S";
                    break;
            }
        }
        return wDays;*/
  }

  var makeMailTo = function (emails) {
    if (emails == null) return "";
    var arEmails = emails.split(",");
    var strEmails = "";
    for (var i = 0; i < arEmails.length; i++) {
      strEmails += arEmails[i]+', ';//'<a href="mailto:' + arEmails[i] + ';">' + arEmails[i] + '</a>, ';
    }
    return strEmails.substring(0, strEmails.lastIndexOf(','));
  }

  var getSnapmails = function (refresh, key) {
    $("#liUsername").show();
    $("#lnkSignout").show();
    $("#btnNewApp").show();
    $("#divMainContainer").removeClass("container-bg");

    $("#newApp").html("");
    $("#newApp").fadeOut();

    $("#displayUsername").html(user.firstname + " " + user.lastname);
        
    if (refresh)
    {
      var index = $("#divSnapmails").children("#dataslot" + key).index();
      $.ajax({
        type: 'GET',
        url: appApiUrl + "/" + key,
        dataType: 'json',
        ContentType: 'application/json; charset=utf-8',
        success: function (snapMail) {
          if (snapMail) {
            $("#dataslot" + key).html(getSnapmailHtml(snapMail, index));
            loadCameraSnaps(snapMail.key, snapMail.cameras);
            $('#pop-' + snapMail.key).popbox({
              open: '#open-' + snapMail.key,
              box: '#box-' + snapMail.key,
              arrow: '#arrow-' + snapMail.key,
              arrow_border: '#arrow-border-' + snapMail.key,
              close: '#close-popup-' + snapMail.key
            });
            $(".rslides").responsiveSlides({
              auto: true,
              pager: true,
              nav: false,
              pause: true,
              speed: 500,
              namespace: "centered-btns"
            });
          }
        },
        error: function (xhr, textStatus) {
          console.log("Could not load Snapmail details.");
        }
      });
    }
    else
    {
      $("#divLoadingApps").fadeIn();
      $("#divLoadingApps").html('<img src="assets/img/loader3.gif" alt="Fetching Snapmails..."/>&nbsp;Fetching SnapMails...');
      $.ajax({
        type: 'GET',
        url: appApiUrl + "/users/" + user.id,
        dataType: 'json',
        ContentType: 'application/json; charset=utf-8',
        success: function (snapMail) {
          if (snapMail.length == 0) {
            $("#divSnapmails").html('');
            $("#divLoadingApps").html('You have not created any SnapMail. <a data-toggle="modal" href="#divForm" class="newApp">Click</a> to create one.');
          }
          else {
            $("#divSnapmails").html('');
            var html = '';
            for (var i = 0; i < snapMail.length; i++) {
              $("#divSnapmails").append(getSnapmailHtml(snapMail[i], i));
            }

            $(".AppsContainer").fadeIn();
            $("#divLoadingApps").fadeOut();

            for (var i = 0; i < snapMail.length; i++) {
              loadCameraSnaps(snapMail[i].key, snapMail[i].cameras);
              $('#pop-' + snapMail[i].key).popbox({
                open: '#open-' + snapMail[i].key,
                box: '#box-' + snapMail[i].key,
                arrow: '#arrow-' + snapMail[i].key,
                arrow_border: '#arrow-border-' + snapMail[i].key,
                close: '#close-popup-' + snapMail[i].key
              });
            }
            $(".rslides").responsiveSlides({
              auto: true,
              pager: true,
              nav: false,
              pause: true,
              speed: 500,
              namespace: "centered-btns"
            });
          }
        },
        error: function (xhr, textStatus) {
          $("#divSnapmails").html('');
          $("#divLoadingApps").html('You have not created any SnapMail. <a data-toggle="modal" href="#divForm" class="newApp">Click</a> to create one.');
        }
      });
    }
  }

  var getSnapmailHtml = function (snapMail, index) {
    var cameras = snapMail.cameras.split(',');
    var html = '<div id="dataslot' + snapMail.key + '" class="list-border margin-bottom10">';
    html += '    <div class="col-md-4" style="min-height:0px;">';
    html += '    <div class="card" style="min-height:0px;">';
    html += '        <div class="snapstack-loading" id="snaps-' + snapMail.key + '" >';
    html += '           <ul class="rslides" id="snapmail' + index + '">';
    for (var c = 0; c < cameras.length; c++) {
      html += '           <li><img class="stackimage" id="stackimage-' + snapMail.key + '-' + cameras[c] + '" alt="' + snapMail.camera_names.split(',')[c] + '" ><p>' + snapMail.camera_names.split(',')[c] + '</p></li>';
    }
    html += '           </ul>';
    html += '        </div>';
    html += '        <input type="hidden" id="txtCamerasId' + snapMail.key + '" value="' + snapMail.cameras + '" /><input type="hidden" id="txtRecipient' + snapMail.key + '" value="' + (snapMail.recipients == null ? "" : snapMail.recipients) + '" /><input type="hidden" id="txtTime' + snapMail.key + '" value="' + snapMail.notify_time + '" />';
    html += '        <input type="hidden" id="txtDays' + snapMail.key + '" value="' + snapMail.notify_days + '" /><input type="hidden" id="txtUserId' + snapMail.key + '" value="' + snapMail.user_id + '" /><input type="hidden" id="txtTimezone' + snapMail.key + '" value="' + snapMail.timezone + '" />';
    html += '        <div class="hash-label"><a data-toggle="modal" href="#divForm" class="tools-link linkActions" data-val="' + snapMail.key + '" data-action="e"><div class="camera-name">' + snapMail.camera_names + '</div></a> <span>@</span><div class="camera-time">' + snapMail.notify_time + ' (' + snapMail.timezone + ')</div><span>on</span><div class="camera-days">' + snapMail.notify_days.replace(/,/g, " ") + ' </div><span>sent to</span> <div class="camera-email">' + makeMailTo(snapMail.recipients) + '</div></div>';
    html += '    </div>';
    html += '    <div class="edit-snapmail" style="min-height:0px;">';
    html += '        <div class="text-right delete">';
    html += '             <span id=pop-' + snapMail.key + ' class="popbox2"><div id="open-' + snapMail.key + '" href="javascript:;" class="tools-link open2" data-val="' + snapMail.key + '"><div class="icon-button red"><core-icon icon="delete"></core-icon><paper-ripple class="circle recenteringTouch" fit></paper-ripple></div></div>';
    html += '             <div class="collapse-popup">';
    html += '               <div class="box2" id="box-' + snapMail.key + '" style="width:250px;">';
    html += '                   <div class="arrow2" id="arrow-' + snapMail.key + '"></div>';
    html += '                   <div class="arrow-border2" id="arrow-border-' + snapMail.key + '"></div>';
    html += '                   <div class="margin-bottom-10">Are you sure to delete this snapmail?</div>';
    html += '                   <div class="margin-bottom-10"><input class="button raised grey delete-btn linkActions" type="button" value=" DELETE " data-val="' + snapMail.key + '" data-action="r" /><div href="#" id="close-popup-' + snapMail.key + '" class="button delete-btn closepopup2 raised grey"><div class="center" fit>CANCEL</div><paper-ripple fit></paper-ripple></div></div>';
    html += '               </div>';
    html += '             </div></span>';
    html += '       </div>';
    html += '       </div>';
    html += '    </div>';
    html += '</div>';
    return html;
  };

  var loadCameraSnaps = function (key, cameras) {
    for (var d = 0; d < cameras.split(',').length ; d++) {
      var thumbnail_url = "https://media.evercam.io/v1/cameras/" + cameras.split(',')[d] + "/thumbnail?api_id=" + localStorage.getItem("api_id") + "&api_key=" + localStorage.getItem("api_key");
      $("#stackimage-" + key + "-" + cameras.split(',')[d]).attr('src', thumbnail_url);
      $("#stackimage-" + key + "-" + cameras.split(',')[d]).css('visibility', 'visible');
      $("#stackimage-" + key + "-" + cameras.split(',')[d]).next().css('visibility', 'visible');
    }
  }

  $(".app-img-radious-retry").live("click", function () {
    var oImg = $(this);
    var code = oImg.attr("data-val");
    oImg.attr("src", "assets/img/retry-app.gif");
    $.ajax({
      type: "PUT",
      url: appApiUrl + "/apps/" + code+"/0",
      //data: { "status": 0, "app_url": "" },
      contentType: "application/x-www-form-urlencoded",
      dataType: "json",
      success: function (data) {
        var thumbImage = data.thumb_file;
        if (!data.thumb_file)
          thumbImage = "../assets/img/noimagegray.jpg";
        oImg.attr("src", thumbImage);
        setTimeout(function () { CheckAppStatus(data.code); }, 30000);
      },
      error: function (xhrc, ajaxOptionsc, thrownErrorc) { oImg.attr("src", "assets/img/retry.png"); }
    });
  });

  $(".shareWithFriend").live("click", function () {
    var btn = $(this);
    btn.attr("value", " Sending... ");
    btn.attr("disabled", "disabled");
    var code = btn.attr("data-val");
    var msg = $("#txtEmailMsg" + code).val();
    var title = $("#divTitle" + code).attr("title");
    var o = {
      "from": localStorage.getItem("AppUsername"),
      "subject": localStorage.getItem("AppUsername") + " has shared 1ButtonApp '" + title + "' with you",
      "message": msg + '<br /><br /><img src="' + $("#divThumb" + code + " img").attr("src") + '"><br><br>Click <a href="' + $("#divAppUrl" + code + " a").attr("href") + '">here</a> to Download',
      "recipients": $("#txtShareEmail" + code).val(),
      "ccs": "",
      "bccs": ""
    };
    $.ajax({
      type: 'POST',
      url: utilsApi + "/email",
      data: o,
      dataType: 'json',
      ContentType: 'application/x-www-form-urlencoded',
      success: function (data) {
        $("#txtShareEmail" + code).val("");
        $("#txtEmailMsg" + code).val("");
        btn.attr("value", " Share ");
        btn.removeAttr("disabled");
        $("#divShareBox" + code).slideUp();
        $("#divMailSuccess" + code).css("color", "green");
        $("#divMailSuccess" + code).html("Mail sent successfully.");
        $("#divMailSuccess" + code).slideDown();
        setTimeout(function () {
          $("#divMailSuccess" + code).slideUp();
          $("#divShareBox" + code).slideDown();
        }, 8000);
      },
      error: function (xhr, textStatus) {
        btn.attr("value", " Share ");
        btn.removeAttr("disabled");
        $("#divMailSuccess" + code).css("color", "red");
        $("#divMailSuccess" + code).html("Failed to send Mail.");
        $("#divMailSuccess" + code).slideDown();
        setTimeout(function () {
          $("#divMailSuccess" + code).slideUp();
        }, 8000);
      }
    });
  });

  var getCameras = function(reload) {
    $.ajax({
      type: "GET",
      crossDomain: true,
      url: EvercamApi + "/cameras.json?api_id=" + localStorage.getItem("api_id") + "&api_key=" + localStorage.getItem("api_key"),
      data: { user_id: user.id },
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function(res) {
        localStorage.setItem("EvercamCameras", JSON.stringify(res));
        if (reload) {
          var cams = res;
          for (var i = 0; i < cams.cameras.length; i++) {
            var css = 'onlinec';
            if (!cams.cameras[i].is_online)
              css = 'offlinec';
            var thumbnail_url = cams.cameras[i].thumbnail_url;
            if (cams.cameras[i].thumbnail_url == null || cams.cameras[i].thumbnail_url == undefined || cams.cameras[i].thumbnail_url == "")
              thumbnail_url = "https://media.evercam.io/v1/cameras/" + cams.cameras[i].id + "/thumbnail?api_id=" + localStorage.getItem("api_id") + "&api_key=" + localStorage.getItem("api_key");
            if (cams.cameras[i].rights.indexOf("snapshot") > -1)
              $("#ddlCameras").append('<option class="' + css + '" data-val="' + thumbnail_url + '" value="' + cams.cameras[i].id + '" >' + cams.cameras[i].name + '</option>');
            else
              console.log("Insufficient rights: " + cams.cameras[i].id);
          }
        } else
          getSnapmails();
        $("#ddlCameras").select2({
          placeholder: 'Select Camera',
          allowClear: true,
          formatResult: format,
          formatSelection: format,
          escapeMarkup: function(m) {
            return m;
          }
        });
      },
      error: function(xhrc, ajaxOptionsc, thrownErrorc) {

      }
    });
  };

  var format = function (state) {
        
    if (!state.id) return state.text;
    if (state.id == "0") return state.text;
    if (state.element[0].attributes[1].nodeValue == "null")
      return "<table style='width:100%;'><tr><td style='width:90%;'><img style='width:35px;height:30px;' class='flag' src='assets/img/cam-img.jpg'/>&nbsp;&nbsp;" + state.text + "</td><td style='width:10%;' align='right'>" + "<img class='flag' src='assets/img/" + state.css + ".png'/>" + "</td></tr></table>";
    else
      return "<table style='width:100%;'><tr><td style='width:90%;'><img style='width:35px;height:30px;' class='flag' src='" + state.element[0].attributes[1].nodeValue + "'/>&nbsp;&nbsp;" + state.text + "</td><td style='width:10%;' align='right'>" + "<img class='flag' src='assets/img/" + state.css + ".png'/>" + "</td></tr></table>";
  }
        
  $('.linkActions').live('click', function (e) {
    var key = $(this).attr("data-val");
    var action = $(this).attr("data-action");
        
    if (action == 'r') {
      RemoveSnapmail(key);
    }
    else if (action == 'e') {
      $("#s2id_ddlCameras").hide();
      $("#txtkey").val(key);
      $(".modal-header h3").html("Edit SnapMail");
      $("#ddlTimezone").val($("#txtTimezone" + key).val());
      var emails = $("#txtRecipient" + key).val();
      if (emails != null || emails != "") {
        var arEmails = emails.split(",");
        for (var i = 0; i < arEmails.length; i++) {
          if (arEmails[i] != "") {
            $("#txtRecipient").addTag(arEmails[i]);
          }
        }
      }
      var days = $("#txtDays" + key).val().split(",");
      for (var i = 0; i < days.length; i++) {
        switch (days[i]) {
          case "Mon":
            $("#uniform-chkMon span").addClass("checked");
            $("#chkMon").attr("checked", true);
            break;
          case "Tue":
            $("#uniform-chkTue span").addClass("checked");
            $("#chkTue").attr("checked", true);
            break;
          case "Wed":
            $("#uniform-chkWed span").addClass("checked");
            $("#chkWed").attr("checked", true);
            break;
          case "Thu":
            $("#uniform-chkThu span").addClass("checked");
            $("#chkThu").attr("checked", true);
            break;
          case "Fri":
            $("#uniform-chkFri span").addClass("checked");
            $("#chkFri").attr("checked", true);
            break;
          case "Sat":
            $("#uniform-chkSat span").addClass("checked");
            $("#chkSat").attr("checked", true);
            break;
          case "Sun":
            $("#uniform-chkSun span").addClass("checked");
            $("#chkSun").attr("checked", true);
            break;
        }
      }
      var cameraIds = $("#txtCamerasId" + key).val().split(",");
      $("#ddlCameras").select2('val', cameraIds);
            
      $("#txtTime").val($("#txtTime" + key).val());
      $("#s2id_ddlCameras").show();
    }
    else if (action == 'd1') {
      SaveToDisk($(this).attr("data-url"), code);
    }
  });

  var SaveToDisk = function (fileURL, fileName) {
    // for non-IE
    if (!window.ActiveXObject) {
      var save = document.createElement('a');
      save.href = fileURL;
      save.target = '_blank';
      save.download = fileName || 'unknown';

      var event = document.createEvent('Event');
      event.initEvent('click', true, true);
      save.dispatchEvent(event);
      (window.URL || window.webkitURL).revokeObjectURL(save.href);
    }
    // for IE
    else if (!!window.ActiveXObject && document.execCommand) {
      var _window = window.open(fileURL, '_blank');
      _window.document.close();
      _window.document.execCommand('SaveAs', true, fileName || fileURL)
      _window.close();
    }
  }

  var RemoveSnapmail = function (key) {
    $("#dataslot" + key).fadeOut(1000, function () {
      $.ajax({
        type: "DELETE",
        url: appApiUrl + "/" + key,
        contentType: "application/x-www-form-urlencoded",
        dataType: "json",
        success: function (res) {
          $("#dataslot" + key).remove();
          if ($("#divSnapmails div").length == 0) {
            $("#divSnapmails").html('');
            $("#divLoadingApps").html('You have not created any SnapMail. <a data-toggle="modal" href="#divForm" class="newApp">Click</a> to create one.');
            $("#divLoadingApps").slideDown();
          }
        },
        error: function (xhrc, ajaxOptionsc, thrownErrorc) { }
      });
    });
  }
    
  var getUserLocalIp = function () {
    try{
      if (window.XMLHttpRequest) xmlhttp = new XMLHttpRequest();
      else xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");

      xmlhttp.open("GET", "http://api.hostip.info/get_html.php", false);
      xmlhttp.send();

      hostipInfo = xmlhttp.responseText.split("\n");

      for (i = 0; hostipInfo.length >= i; i++) {
        var ipAddress = hostipInfo[i].split(":");
        if (ipAddress[0] == "IP") return $("#user_local_Ip").val(ipAddress[1]);
      }
    }
    catch(e){}
  }

  var handleFileupload = function () {
    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
      // Uncomment the following to send cross-domain cookies:
      url: 'assets/plugins/jquery-file-upload/server/php/'
    });
        
    return;

    // Upload server status check for browsers with CORS support:
    if ($.support.cors) {
      $.ajax({
        url: 'assets/plugins/jquery-file-upload/server/php/',
        type: 'HEAD'
      }).fail(function () {
        $('<span class="alert alert-error"/>')
          .text('Upload server currently unavailable - ' +
          new Date())
          .appendTo('#fileupload');
      });
    }
  }

  var handleDayCheckBox = function () {
    $("#chkAllDay").bind("change", function () {
      CheckMutalWeek("All");
    });

    $(".days").bind("change", function () {
      CheckMutalWeek("One");
    });

    $('#txtRecipient').tagsInput({
      defaultText: 'Add Recipients',
      height: 'auto',
      width: 'auto',
      onRemoveTag: function (email)
      {
        $("#divAlert span").html("");
        $("#divAlert").slideUp();
      },
      onChangeTag: function (email)
      {
        $("#divAlert span").html("");
        $("#divAlert").slideUp();
        if (!validateEmailByVal(email)) {
          $("#divAlert").slideDown();
          $("#divAlert span").html("Invalid recipient email.");
        }
      },
      onAddTag: function (email) {
        $("#divAlert span").html("");
        $("#divAlert").slideUp();
        if (!validateEmailByVal(email)) {
          $("#divAlert").slideDown();
          $("#divAlert span").html("Invalid recipient email.");
        }
      }
    });

    var tz_info = jzTimezoneDetector.determine_timezone();
    $("#ddlTimezone").val(GetUtcZoneValue(tz_info.key));
  }

  var CheckMutalWeek = function (fullweek) {
    //saveMdTimes();
    if (fullweek == "All") {
      if ($("#chkAllDay").attr("checked")) {
        $("#uniform-chkMon span").addClass("checked");
        $("#chkMon").attr("checked", "checked");

        $("#uniform-chkTue span").addClass("checked");
        $("#chkTue").attr("checked", "checked");

        $("#uniform-chkWed span").addClass("checked");
        $("#chkWed").attr("checked", "checked");

        $("#uniform-chkThu span").addClass("checked");
        $("#chkThu").attr("checked", "checked");

        $("#uniform-chkFri span").addClass("checked");
        $("#chkFri").attr("checked", "checked");

        $("#uniform-chkSat span").addClass("checked");
        $("#chkSat").attr("checked", "checked");

        $("#uniform-chkSun span").addClass("checked");
        $("#chkSun").attr("checked", "checked");

      } else {
        $("#uniform-chkMon span").removeClass("checked");
        $("#chkMon").attr("checked", false);

        $("#uniform-chkTue span").removeClass("checked");
        $("#chkTue").attr("checked", false);

        $("#uniform-chkWed span").removeClass("checked");
        $("#chkWed").attr("checked", false);

        $("#uniform-chkThu span").removeClass("checked");
        $("#chkThu").attr("checked", false);

        $("#uniform-chkFri span").removeClass("checked");
        $("#chkFri").attr("checked", false);

        $("#uniform-chkSat span").removeClass("checked");
        $("#chkSat").attr("checked", false);

        $("#uniform-chkSun span").removeClass("checked");
        $("#chkSun").attr("checked", false);
      }
    } else {
      $("#uniform-chkAllDay span").removeClass("checked");
      $("#chkAllDay").attr("checked", false);
    }
  }

  return {
    init: function () {
      handleLoginSection();
      handleNewApp();
      handleLogout();
      handleDayCheckBox();
      $(document).bind('keyup', function(event){
        if (event.keyCode == 27) {
          clearForm();
        }
      });
      NProgress.done();
    }
  };
}();