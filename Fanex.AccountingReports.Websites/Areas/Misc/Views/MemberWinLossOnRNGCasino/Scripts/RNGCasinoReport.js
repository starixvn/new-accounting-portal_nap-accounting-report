Namespace.Register("Fanex.AccountingReports.Misc.RNGCasinoReport");
Fanex.AccountingReports.Misc.RNGCasinoReport = function () {
    var self = this;

    self.Tables = {        
        numbericTextbox: null,
    };

    self.cacheDomElements = function () {        
        self.Tables.numbericTextbox = $(".numberic-textbox");
    };

    self.InitPage = function () {  
        self.cacheDomElements();      
         self.setupCalendar(today, 'FromTransDate');
         self.setupCalendar(today, 'ToTransDate');
         self.bindingEvent();
    };

    self.setupCalendar = function (strdatetime, txtdate) {
        $('#'+ txtdate).datetimepicker({
            showSecond: true,
            dateFormat: 'M dd, yy',
            timeFormat: 'HH:mm:ss',
            hourGrid: 3,
            minuteGrid: 5,
            secondGrid: 5,
            separator: ' ',
            minDateTime: new Date(minDate),
            maxDateTime: new Date(today),
            onSelect: function(){               
            }         
        });

        if (strdatetime != null && strdatetime.toString() !== "") {
            $('#' + txtdate).val(strdatetime).datetimepicker('setDate', (new Date(strdatetime)));
        }
    };

    self.bindingEvent = function () {
        self.Tables.numbericTextbox.ForceDecimalOnly().ForceCurrencyOnly();
    };

};

var rngCasinoReport;

$(document).ready(function () {
    rngCasinoReport = new Fanex.AccountingReports.Misc.RNGCasinoReport();
    rngCasinoReport.InitPage();
});