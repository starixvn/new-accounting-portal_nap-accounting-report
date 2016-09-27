   
Namespace.Register("Fanex.AccountingReports.Special.CheckBet.CheckBet");
Fanex.AccountingReports.Special.CheckBet.CheckBet = function () {
    var self = this;
    var _tables = {
        reportFilter: null,
        datepickerplugin: null,
        btnGenerate: null,

        report: null,
        reportContent: null,
        reportTitle: null,
    };

     var _options = {
        report: {
            tblContentId: 'checkBetReportContent'
        }
    };

    var _params = {
        checkBetReport: {
            WinLossDate: null,
            TicketType: null
        }
    };
    
    self.TempHtml = {
        contentHtml: '',
    };

    self.initPage = function () {
        initGridScrollerTables();
        cachedDOMElements();
        renderDatePicker();
        bindingEvent();
    };

    var initGridScrollerTables = function () {
        $("#checkBetReport").initGrid({
        });
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.reportFilter = $("#checkBetReportFilter");

        _tables.report = $("#checkBetReport");
        _tables.reportHeader = $("#checkBetReportHeader");
        _tables.reportContent = _tables.report.find("#checkBetReportContent tbody");
        _tables.reportTitle = _tables.reportHeader.find("#checkBetReportTitle");

    };

      var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "single",          
            firstDatePicker: {
                Id: "CheckDate",
                Label: "Date",
                InitDate: "",
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
                
            },

            datePickerFormat:{       
                dateFormat: "M dd, yy",          
                minDate : new Date(minDate),
                maxDate: new Date(currentDate)
            }
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };

     var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });
    };

     self.btnGenerateClick = function () {
        self.getCheckBetReport();
    };

    self.getCheckBetReport = function(){
        hideReport();       
        $('.ui-dialog').hide();

        var date = _tables.datepickerplugin.nexDatePicker("getSingleDate");
        _params.checkBetReport.WinLossDate = date;
        _params.checkBetReport.TicketType = 0;
        $.ajax({
                type: "POST",
                url: window.rootUrl + "Special/CheckBet/SelCheckedBet",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({
                    WinLossDate: _params.checkBetReport.WinLossDate,
                    TicketType: _params.checkBetReport.TicketType
                }),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    self.renderReport(rs);
                },
                error: function (event, jqXHR, ajaxSettings) {
                    self.renderReport(null);
                },
                complete: function () {
                    AccReport.enableInput();
                }
            });
    };

    self.renderReport = function(report) {
        var reportTitle = "Check Bet Date " + _params.checkBetReport.WinLossDate ;
        _tables.reportTitle.html(reportTitle);
        if(report != null)
        {
            self.resetTempHtml();
            self.renderReportRow(report);
            self.removeSpaceIE9TempHtml();
            _tables.reportContent.empty().append(self.TempHtml.contentHtml);                
        }

        _tables.report.removeClass("display-none")
                      .gridScroller(_options.report);

        _tables.reportHeader.removeClass("display-none");
        _tables.report.removeClass("display-none");       
        
    };
    self.resetTempHtml = function () {
        self.TempHtml.contentHtml = '';
    };
    self.removeSpaceIE9TempHtml = function () {
        self.TempHtml.contentHtml = fixBugsBrowser.removeSpaceIE9(self.TempHtml.contentHtml);
       
    };
    self.renderReportRow = function (row) {
       
        var wlDate = '<td><div class="col100  align-center">' + _params.checkBetReport.WinLossDate + '</div></td>';
        var normal = parseInt(row.NormalTicket) > 0 ? '<div class="col150 align-center link" onclick="getNormalTickets(\'' +  _params.checkBetReport.WinLossDate + '\')" >' + row.NormalTicket + '</div>' : '<div class="col150 align-center">' + row.NormalTicket + '</div>';
        var reject = parseInt(row.RejectTicket) > 0 ? '<div class="col150 align-center link" onclick="getRejectTickets(\'' +  _params.checkBetReport.WinLossDate + '\')" >' + row.RejectTicket + '</div>' : '<div class="col150 align-center">' + row.RejectTicket + '</div>';
        self.TempHtml.contentHtml += '<tr>' + wlDate + 
                                            '<td>' + normal + '</td>\
                                            <td>' + reject + '</td>\
                                    </tr>';
    };  
};


