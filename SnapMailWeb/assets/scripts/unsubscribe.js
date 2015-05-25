var Unsubscribe = function () {
    var EvercamApi = "https://api.evercam.io/v1";
    var appApiUrl = "http://snapmail.evercam.io/api/snapmails";
    var qs = new Querystring();
    var snapMailId = qs.get('id', 0);
    var unsubscribeEmail = qs.get('email', 0);

    var getSnapmail = function () {
        
        $("#spnEmail").html(unsubscribeEmail);

        $.ajax({
            type: 'GET',
            url: appApiUrl + "/" + snapMailId,
            dataType: 'json',
            ContentType: 'application/json; charset=utf-8',
            /*beforeSend: function (xhrObj) {
                xhrObj.setRequestHeader("Authorization", "Basic " + sessionStorage.getItem("oAuthToken"));
            },*/
            success: function (snapMail) {
                $("#spnTime").html(snapMail.notify_time);
            },
            error: function (xhr, textStatus) {
                
            }
        });

    };

    var handleSubscribe = function () {
        $(".Unsubscribe").bind("click", function () {
            $.ajax({
                type: 'POST',
                url: appApiUrl + "/" + snapMailId + "/unsubscribe",
                data: { email: unsubscribeEmail },
                dataType: 'json',
                ContentType: 'application/json; charset=utf-8',
                /*beforeSend: function (xhrObj) {
                    xhrObj.setRequestHeader("Authorization", "Basic " + sessionStorage.getItem("oAuthToken"));
                },*/
                success: function (snapMail) {
                    $(".message").html("You have successfully unsubscribed from this SnapMail.");
                    $(".Unsubscribelink").hide();
                },
                error: function (xhr, textStatus) {

                }
            });
        });
    };

    return {
        init: function () {
            getSnapmail();
            handleSubscribe();
        }
    };
}();