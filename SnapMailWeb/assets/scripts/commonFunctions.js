function validateEmail(email) {
    var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
    var address = document.getElementById(email).value;
    //remove all white space from value before validating 
    var addresstrimed = address.replace(/ /gi, '');
    if (reg.test(addresstrimed) == false) {
        return false;
    }
    else
        return true;
}

function validateEmailByVal(email) {
    var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
    //var address = document.getElementById(email).value;
    //remove all white space from value before validating 
    var addresstrimed = email.replace(/ /gi, '');
    if (reg.test(addresstrimed) == false) {
        return false;
    }
    else
        return true;
}

///////////////////////////////////////////////////////////////////
function Querystring(qs) { // optionally pass a querystring to parse
    this.params = {};

    if (qs == null) qs = location.search.substring(1, location.search.length);
    if (qs.length == 0) return;

    // Turn <plus> back to <space>
    // See: http://www.w3.org/TR/REC-html40/interact/forms.html#h-17.13.4.1
    qs = qs.replace(/\+/g, ' ');
    var args = qs.split('&'); // parse out name/value pairs separated via &

    // split out each name=value pair
    for (var i = 0; i < args.length; i++) {
        var pair = args[i].split('=');
        var name = decodeURIComponent(pair[0]);

        var value = (pair.length == 2)
			? decodeURIComponent(pair[1])
			: name;

        this.params[name] = value;
    }
}

Querystring.prototype.get = function (key, default_) {
    var value = this.params[key];
    return (value != null) ? value : default_;
}

Querystring.prototype.contains = function (key) {
    var value = this.params[key];
    return (value != null);
}

//QUERY STRING

function DeleteConform() {
    if (!confirm("Are you sure to Delete? "))
        return false;
}



function GetUtcZone(key) {

    switch (key) {
        case '-720,0':
            return '(UTC-12:00) International Date Line West';
            break;
        case '-660,0':
            return '(UTC-11:00) Coordinated Universal Time-11';
            break;
        case '-600,1':
            return '(UTC-11:00) Coordinated Universal Time-11';
            break;
        case '-660,1,s':
            return '(UTC-11:00) Samoa';
            break;
        case '-600,0':
            return '(UTC-10:00) Hawaii';
            break;
        case '-570,0':
            return '';
            break;
        case '-540,0':
            return '(UTC-09:00) Alaska';
            break;
        case '-540,1':
            return '(UTC-09:00) Alaska';
            break;
        case '-480,1':
            return '(UTC-08:00) Pacific Time (US & Canada)';
            break;
        case '-480,0':
            return '(UTC-08:00) Pacific Time (US & Canada)';
            break;
        case '-420,0':
            return '(UTC-07:00) Mountain Time (US & Canada)';
            break;
        case '-420,1':
            return '(UTC-07:00) Mountain Time (US & Canada)';
            break;
        case '-360,0':
            return '(UTC-06:00) Central Time (US & Canada)';
            break;
        case '-360,1':
            return '(UTC-06:00) Central Time (US & Canada)';
            break;
        case '-360,1,s':
            return '(UTC-06:00) Central Time (US & Canada)';
            break;
        case '-300,0':
            return '(UTC-05:00) Bogota, Lima, Quito';
            break;
        case '-300,1':
            return '(UTC-05:00) Eastern Time (US & Canada)';
            break;
        case '-270,0':
            return '(UTC-12:00) International Date Line West';
            break;
        case '-240,1':
            return '(UTC-04:00) Atlantic Time (Canada)';
            break;
        case '-240,0':
            return '(UTC-04:00) Atlantic Time (Canada)';
            break;
        case '-240,1,s':
            return '(UTC-04:00) Atlantic Time (Canada)';
            break;
        case '-210,1':
            return '(UTC-03:30) Newfoundland';
            break;
        case '-180,1':
            return '(UTC-03:00) Greenland';
            break;
        case '-180,0':
            return '(UTC-03:00) Buenos Aires';
            break;
        case '-180,1,s':
            return '(UTC-03:00) Montevideo';
            break;
        case '-120,0':
            return '(UTC-02:00) Mid-Atlantic';
            break;
        case '-120,1':
            return '(UTC-02:00) Coordinated Universal Time-02';
            break;
        case '-60,1':
            return '(UTC-01:00) Azores';
            break;
        case '-60,0':
            return '(UTC-01:00) Cape Verde Is.';
            break;
        case '0,0':
            return '(UTC) Coordinated Universal Time';
            break;
        case '0,1':
            return '(UTC) Dublin, Edinburgh, Lisbon, London';
            break;
        case '60,1':
            return '(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna';
            break;
        case '60,0':
            return '(UTC+01:00) West Central Africa';
            break;
        case '60,1,s':
            return '(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague';
            break;
        case '120,1':
            return '(UTC+02:00) Beirut';
            break;
        case '120,0':
            return '(UTC+02:00) Harare, Pretoria';
            break;
        case '180,1':
            return '(UTC+04:00) Moscow, St. Petersburg, Volgograd';
            break;
        case '180,0':
            return '(UTC+03:00) Baghdad';
            break;
        case '210,1':
            return '(UTC+03:30) Tehran';
            break;
        case '240,0':
            return '(UTC+04:00) Abu Dhabi, Muscat';
            break;
        case '240,1':
            return '(UTC+04:00) Yerevan';
            break;
        case '270,0':
            return '(UTC+04:30) Kabul';
            break;
        case '300,1':
            return '(UTC+06:00) Ekaterinburg';
            break;
        case '300,0':
            return '(UTC+05:00) Islamabad, Karachi';
            break;
        case '330,0':
            return '(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi';
            break;
        case '345,0':
            return '(UTC+05:45) Kathmandu';
            break;
        case '360,0':
            return '(UTC+06:00) Dhaka';
            break;
        case '360,1':
            return '(UTC+06:00) Astana';
            break;
        case '390,0':
            return '(UTC+06:30) Yangon (Rangoon)';
            break;
        case '420,1':
            return '(UTC+08:00) Krasnoyarsk';
            break;
        case '420,0':
            return '(UTC+07:00) Bangkok, Hanoi, Jakarta';
            break;
        case '480,0':
            return '(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi';
            break;
        case '480,1':
            return '(UTC+09:00) Irkutsk';
            break;
        case '525,0':
            return '';
            break;
        case '525,1,s':
            return '';
            break;
        case '540,1':
            return '(UTC+10:00) Yakutsk';
            break;
        case '540,0':
            return '(UTC+09:00) Osaka, Sapporo, Tokyo';
            break;
        case '570,0':
            return '(UTC+09:30) Darwin';
            break;
        case '570,1,s':
            return '(UTC+09:30) Adelaide';
            break;
        case '600,0':
            return '(UTC+10:00) Brisbane';
            break;
        case '600,1':
            return '(UTC+11:00) Vladivostok';
            break;
        case '600,1,s':
            return '(UTC+10:00) Canberra, Melbourne, Sydney';
            break;
        case '630,1,s':
            return '';
            break;
        case '660,1':
            return '(UTC+12:00) Magadan';
            break;
        case '660,0':
            return '(UTC+11:00) Solomon Is., New Caledonia';
            break;
        case '690,0':
            return '';
            break;
        case '720,1,s':
            return '(UTC+12:00) Auckland, Wellington';
            break;
        case '720,0':
            return 'UTC+12';
            break;
        case '765,1,s':
            return '';
            break;
        case '780,0':
            return "(UTC+13:00) Nuku'alofa";
            break;
        case '840,0':
            return '';
            break;
        default:
            return '';
    }
}

function GetUtcZoneValue(key) {

    switch (key) {
        case '-720,0':
            return 'Dateline Standard Time';
            break;
        case '-660,0':
            return 'UTC-11';
            break;
        case '-600,1':
            return 'UTC-11';
            break;
        case '-660,1,s':
            return '(UTC-11:00) Samoa';
            break;
        case '-600,0':
            return 'Hawaiian Standard Time';
            break;
        case '-570,0':
            return '';
            break;
        case '-540,0':
            return 'Alaskan Standard Time';
            break;
        case '-540,1':
            return 'Alaskan Standard Time';
            break;
        case '-480,1':
            return 'Pacific Standard Time';
            break;
        case '-480,0':
            return 'Pacific Standard Time';
            break;
        case '-420,0':
            return 'Mountain Standard Time';
            break;
        case '-420,1':
            return 'Mountain Standard Time';
            break;
        case '-360,0':
            return 'Central Standard Time';
            break;
        case '-360,1':
            return 'Central Standard Time';
            break;
        case '-360,1,s':
            return 'Central Standard Time';
            break;
        case '-300,0':
            return 'SA Pacific Standard Time';
            break;
        case '-300,1':
            return 'Eastern Standard Time';
            break;
        case '-270,0':
            return 'Dateline Standard Time';
            break;
        case '-240,1':
            return 'Atlantic Standard Time';
            break;
        case '-240,0':
            return 'Atlantic Standard Time';
            break;
        case '-240,1,s':
            return 'Atlantic Standard Time';
            break;
        case '-210,1':
            return 'Newfoundland Standard Time';
            break;
        case '-180,1':
            return 'Greenland Standard Time';
            break;
        case '-180,0':
            return 'Argentina Standard Time';
            break;
        case '-180,1,s':
            return 'Montevideo Standard Time';
            break;
        case '-120,0':
            return 'Mid-Atlantic Standard Time';
            break;
        case '-120,1':
            return 'UTC-02';
            break;
        case '-60,1':
            return 'Azores Standard Time';
            break;
        case '-60,0':
            return 'Cape Verde Standard Time';
            break;
        case '0,0':
            return 'UTC';
            break;
        case '0,1':
            return 'GMT Standard Time';
            break;
        case '60,1':
            return 'W. Europe Standard Time';
            break;
        case '60,0':
            return 'W. Central Africa Standard Time';
            break;
        case '60,1,s':
            return 'Central Europe Standard Time';
            break;
        case '120,1':
            return 'Middle East Standard Time';
            break;
        case '120,0':
            return 'outh Africa Standard Time';
            break;
        case '180,1':
            return 'Russian Standard Time';
            break;
        case '180,0':
            return 'Arabic Standard Time';
            break;
        case '210,1':
            return 'Iran Standard Time';
            break;
        case '240,0':
            return 'Arabian Standard Time';
            break;
        case '240,1':
            return 'Caucasus Standard Time';
            break;
        case '270,0':
            return 'Afghanistan Standard Time';
            break;
        case '300,1':
            return 'Ekaterinburg Standard Time';
            break;
        case '300,0':
            return 'Pakistan Standard Time';
            break;
        case '330,0':
            return 'India Standard Time';
            break;
        case '345,0':
            return 'Nepal Standard Time';
            break;
        case '360,0':
            return 'Bangladesh Standard Time';
            break;
        case '360,1':
            return 'Central Asia Standard Time';
            break;
        case '390,0':
            return 'Myanmar Standard Time';
            break;
        case '420,1':
            return 'North Asia Standard Time';
            break;
        case '420,0':
            return 'SE Asia Standard Time';
            break;
        case '480,0':
            return 'China Standard Time';
            break;
        case '480,1':
            return 'North Asia East Standard Time';
            break;
        case '525,0':
            return '';
            break;
        case '525,1,s':
            return '';
            break;
        case '540,1':
            return 'Yakutsk Standard Time';
            break;
        case '540,0':
            return 'Tokyo Standard Time';
            break;
        case '570,0':
            return 'AUS Central Standard Time';
            break;
        case '570,1,s':
            return 'Cen. Australia Standard Time';
            break;
        case '600,0':
            return 'E. Australia Standard Time';
            break;
        case '600,1':
            return 'Vladivostok Standard Time';
            break;
        case '600,1,s':
            return 'AUS Eastern Standard Time';
            break;
        case '630,1,s':
            return '';
            break;
        case '660,1':
            return 'Magadan Standard Time';
            break;
        case '660,0':
            return 'Central Pacific Standard Time';
            break;
        case '690,0':
            return '';
            break;
        case '720,1,s':
            return 'New Zealand Standard Time';
            break;
        case '720,0':
            return 'UTC+12';
            break;
        case '765,1,s':
            return '';
            break;
        case '780,0':
            return "Tonga Standard Time";
            break;
        case '840,0':
            return '';
            break;
        default:
            return '';
    }
}