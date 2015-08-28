
var crossDomainManagement = {
    jsonpCallback: null,
    crossDomainCall: function (url, data, fnSuccess, fnError) {
        $.ajax({
            type: 'GET',
            url: url,
            contentType: "application/json",
            dataType: 'jsonp',
            jsonp: false,
            crossDomain: true,
            data: data,
            success: fnSuccess,
            error: fnError
        });
    }
};

function jsonpCallback(data) {
    crossDomainManagement.jsonpCallback(data);
}