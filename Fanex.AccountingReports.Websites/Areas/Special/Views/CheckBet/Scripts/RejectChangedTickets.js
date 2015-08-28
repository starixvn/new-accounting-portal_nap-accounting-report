Namespace.Register("Fanex.AccountingReports.Special.CheckBet.RejectChangedTickets");
Fanex.AccountingReports.Special.CheckBet.RejectChangedTickets = function () {
     var self = this;

     var _tables = {
        reportFilter: null,

        report: null,
        reportContent: null,
        reportTitle: null,
        reportRowHeader: null,
        reportRowFooter: null,
        reportColFooter: null,
        reportPagination: null,
    };

    var _options = {
        
        report: {
            tblContentId: 'rejectChangedTicketsContent',
            fullHeight: true,
            freezeHeight: 600,
            gridHeight: 280,
            column:[100,80,60,100,140,80,80,140,140,60,140],
            numberFreezeCol: 1
        }
    };
     var _params = {
        normalChangedTicket: {
            CheckDate : null,
            TicketType: null
        }
    };
    
    self.initPage = function () {
        initGridScrollerTables();
        cachedDOMElements();
    };

    var initGridScrollerTables = function () {
        $("#rejectChangedTickets").initGrid({
            isShowPager: false
        });
    };
    var cachedDOMElements = function () {
        _tables.report = $("#rejectChangedTickets");
        _tables.reportHeader = $("#rejectChangedTicketsHeader");
        _tables.reportContent = _tables.report.find("#rejectChangedTicketsContent tbody");
        _tables.reportTitle = _tables.reportHeader.find("#rejectChangedTicketsTitle");
        _tables.reportRowHeader = _tables.report.find("#rejectChangedTicketsRowHeader tbody");
        _tables.reportRowFooter = _tables.report.find(".row_footer_content thead");
        _tables.reportColFooter = _tables.report.find(".col_footer_content thead");
        _tables.reportPagination = _tables.report.find("#pagination");
    };

    self.getRejectTickets = function (checkDate) {
       
        AccReport.disableInput();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Special/CheckBet/SelRejectTickets",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                WinLossDate: checkDate,
                TicketType: 2
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
    self.renderReport = function(report){
        var reportTitle = "Void/Reject Tickets";
        _tables.reportTitle.html(reportTitle);
        if(report && report.RejectTicketList && report.RejectTicketList.length > 0)
        {
            self.resetTempHtml();
            self.renderReportRow(report.RejectTicketList);
            self.renderReportTotalRow(report.TotalChangedStatus);
//            self.removeSpaceIE9TempHtml();
             _tables.reportRowHeader.empty().append(self.TempHtml.frozenHtml);
            _tables.reportContent.empty().append(self.TempHtml.contentHtml);
            _tables.reportRowFooter.empty().append(self.TempHtml.footerFrozenHtml);
            _tables.reportColFooter.empty().append(self.TempHtml.footerContentHtml);
        }

        _tables.report.removeClass("display-none")
                      .gridScroller(_options.report);

         
        _tables.reportHeader.removeClass("display-none");
    };
      self.TempHtml = {
        frozenHtml: '',
        contentHtml: '',
        footerFrozenHtml: '',
        footerContentHtml: ''
    };

     self.resetTempHtml = function () {
        self.TempHtml.frozenHtml = '';
        self.TempHtml.contentHtml = '';
        self.TempHtml.footerFrozenHtml = '';
        self.TempHtml.footerContentHtml = '';
        _tables.report.addClass("display-none");
        _tables.reportHeader.addClass("display-none");
        _tables.report.addClass("display-none");
    };
   self.showReport = function () {
        _tables.reportHeader.removeClass("display-none");
        _tables.report.removeClass("display-none");
    };
    self.hideReport = function () {
        _tables.reportHeader.addClass("display-none");
        _tables.report.addClass("display-none");
    };
     self.renderReportRow = function(report){
        for(var i = 0; i < report.length; i++)
        {
            var row = report[i]
            var frozenHtml = '';
            frozenHtml = '<td><div class="table-cell col100 two-row-text align-center link" onclick="getBetList(\'' + row.RefNo + '\')">' + row.RefNo + '</div></td>';
            self.TempHtml.frozenHtml += '<tr>'+ frozenHtml + '</tr>';
            var colStatus = row.OldStake == row.NewStatus ? row.NewStatus : '<span>' + row.OldStatus + '<br/></span><span style="color:#d47800">' + row.NewStatus + '</span>';
            self.TempHtml.contentHtml += '<tr>' + frozenHtml + 
                                                '<td><div class="table-cell col80 two-row-text align-center">' + row.TransDate + '</div></td>\
                                                <td><div class="col60 table-cell align-center">' + row.Currency + '</div></td>\
                                                <td><div class="col100 table-cell align-center">' + row.Bettype + '</div></td>\
                                                <td><div class="col140 table-cell align-center">' + row.Username + '</div></td>\
                                                <td><div class="col80 table-cell align-center">' + row.Stake + '</div></td>\
                                                <td><div class="col80 table-cell align-center">' + row.Odds + '</div></td>\
                                                <td><div class="col140 table-cell align-center">' + row.Betteam + '</div></td>\
                                                <td><div class="col140 table-cell align-center">' + row.TotalPT + '</div></td>\
                                                <td><div class="col60 table-cell align-center">' + colStatus + '</div></td>\
                                                <td><div class="col140 table-cell align-center">' + row.Creator + '</div></td>\
                                        </tr>';
        }
    }

     self.renderReportTotalRow = function (TotalChangedStatus) {
        var frozenHtml = '';
        frozenHtml = '<th><div class="col100 align-center">Count</div></th>';
        self.TempHtml.footerFrozenHtml += '<tr>' + frozenHtml + ' </tr>';
        self.TempHtml.footerContentHtml += '<tr>' +
                                            frozenHtml +
                                            '<th><div class="col80 align-center">&nbsp</div></th>\
                                            <th><div class="col60 align-center">&nbsp</div></th>\
                                            <th><div class="col100 align-center">&nbsp</div></th>\
                                            <th><div class="col140 align-center">&nbsp</div></th>\
                                            <th><div class="col80 align-center">&nbsp</div></th>\
                                            <th><div class="col80 align-center">&nbsp</div></th>\
                                            <th><div class="col140 align-center">&nbsp</div></th>\
                                            <th><div class="col140 align-center">&nbsp</div></th>\
                                            <th><div class="col60 align-center">' + TotalChangedStatus + '</div></th>\
                                            <th><div class="col140 align-center">&nbsp</div></th>\
                                        </tr>';
    };
     
   
};