Namespace.Register("Fanex.AccountingReports.Special.CheckBet.NormalChangedTickets");
Fanex.AccountingReports.Special.CheckBet.NormalChangedTickets = function () {
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
            isShowPager: false,
            tblContentId: 'normalChangedTicketsContent',
            manualCopyHeader: true,
            fullHeight: true,
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
        $("#normalChangedTickets").initGrid({
            isShowPager: false
        });
    };
    var cachedDOMElements = function () {
        _tables.report = $("#normalChangedTickets");
        _tables.reportHeader = $("#normalChangedTicketsHeader");
        _tables.reportContent = _tables.report.find("#normalChangedTicketsContent tbody");
        _tables.reportTitle = _tables.reportHeader.find("#normalChangedTicketsTitle");
        _tables.reportRowHeader = _tables.report.find("#normalChangedTicketsRowHeader tbody");
        _tables.reportRowFooter = _tables.report.find(".row_footer_content thead");
        _tables.reportColFooter = _tables.report.find(".col_footer_content thead");
        _tables.reportPagination = _tables.report.find("#pagination");
    };

      self.getNormalTickets = function (checkDate) {
       
        AccReport.disableInput();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Special/CheckBet/SelNormalTickets",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                WinLossDate: checkDate,
                TicketType: 1
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
         var reportTitle = "Normal Tickets";
        _tables.reportTitle.html(reportTitle);
        if(report && report.NormalTicketList && report.NormalTicketList.length > 0)
        {
            self.resetTempHtml();
            self.renderReportRow(report.NormalTicketList);
            self.renderReportTotalRow(report.TotalChangedStake, report.TotalChangedOdds, report.TotalChangedBetteam, report.TotalChangedPT);
            self.removeSpaceIE9TempHtml();
             _tables.reportRowHeader.empty().append(self.TempHtml.frozenHtml);
            _tables.reportContent.empty().append(self.TempHtml.contentHtml);
            _tables.reportRowFooter.empty().append(self.TempHtml.footerFrozenHtml);
            _tables.reportColFooter.empty().append(self.TempHtml.footerContentHtml);
        }
          _tables.report.removeClass("display-none")
                        .gridScroller(_options.report);

        _tables.reportHeader.removeClass("display-none");
        _tables.report.removeClass("display-none");
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
     self.removeSpaceIE9TempHtml = function () {
        self.TempHtml.frozenHtml = fixBugsBrowser.removeSpaceIE9(self.TempHtml.frozenHtml);
        self.TempHtml.contentHtml = fixBugsBrowser.removeSpaceIE9(self.TempHtml.contentHtml);
        self.TempHtml.footerFrozenHtml = fixBugsBrowser.removeSpaceIE9(self.TempHtml.footerFrozenHtml);
        self.TempHtml.footerContentHtml = fixBugsBrowser.removeSpaceIE9(self.TempHtml.footerContentHtml);
    };
    self.renderReportRow = function(report){
        for(var i = 0; i < report.length; i++)
        {
            var row = report[i]
            var frozenHtml = '';
            frozenHtml = '<td><div class="table-cell col100 two-row-text align-center link" onclick="getBetList(\'' + row.RefNo + '\')">' + row.RefNo + '</div></td>';
            self.TempHtml.frozenHtml += '<tr>'+ frozenHtml + '</tr>';
            var colStake = parseFloat(row.OldStake) == parseFloat(row.NewStake) ? row.NewStake : '<span>' + row.OldStake + '<br/></span><span style="color:#d47800">' + row.NewStake + '</span>';
            var colOdds =   parseFloat(row.OldOdds) == parseFloat(row.NewOdds) ? row.NewOdds : '<span>' + row.OldOdds + '<br/></span><span style="color:#d47800">' + row.NewOdds + '</span>';
            var colBetteam =   row.OldBetteam == row.NewBetteam ? row.NewBetteam : '<span>' + row.OldBetteam + '<br/></span><span style="color:#d47800">' + row.NewBetteam + '</span>';
            var colTotalPT =   parseFloat(row.OldTotalPT) == parseFloat(row.NewTotalPT) ? row.NewTotalPT : '<span>' + row.OldTotalPT + '<br/></span><span style="color:#d47800">' + row.NewTotalPT + '</span>';
            self.TempHtml.contentHtml += '<tr><td><div class="table-cell col100 two-row-text align-center link" onclick="getBetList(\'' + row.RefNo + '\')">' + row.RefNo + '</div></td>\
                                                <td><div class="table-cell col80 align-center">' + row.TransDate + '</div></td>\
                                                <td><div class="table-cell col60 align-center">' + row.CurrencyName + '</div></td>\
                                                <td><div class="table-cell col100 align-center">' + row.Bettype + '</div></td>\
                                                <td><div class="table-cell col140 align-center">' + row.Username + '</div></td>\
                                                <td><div class="table-cell col80 align-center">' + colStake + '</div></td>\
                                                <td><div class="table-cell col80 align-center">' + colOdds + '</div></td>\
                                                <td><div class="table-cell col140 align-center">' + colBetteam + '</div></td>\
                                                <td><div class="table-cell col140 align-center">' + colTotalPT + '</div></td>\
                                                <td><div class="table-cell col60 align-center">' + row.Status + '</div></td>\
                                                <td><div class="table-cell col140 align-center">' + row.Creator + '</div></td>\
                                        </tr>';
        }
    }
    self.renderReportTotalRow = function (TotalChangedStake, TotalChangedOdds, TotalChangedBetteam, TotalChangedPT) {
        var frozenHtml = '';
        frozenHtml = '<th><div class="col100 align-center">Count</div></th>';
        self.TempHtml.footerFrozenHtml += '<tr>' + frozenHtml + ' </tr>';
        self.TempHtml.footerContentHtml += '<tr>' +
                                            frozenHtml +
                                            '<th><div class="col80 align-center">&nbsp</div></th>\
                                            <th><div class="col60 align-center">&nbsp</div></th>\
                                            <th><div class="col100 align-center">&nbsp</div></th>\
                                            <th><div class="col140 align-center">&nbsp</div></th>\
                                            <th><div class="col80 align-center">' + TotalChangedStake + '</div></th>\
                                            <th><div class="col80 align-center">' + TotalChangedOdds + '</div></th>\
                                            <th><div class="col140 align-center">' + TotalChangedBetteam + '</div></th>\
                                            <th><div class="col140 align-center">' + TotalChangedPT + '</div></th>\
                                            <th><div class="col60 align-center">&nbsp</div></th>\
                                            <th><div class="col140 align-center">&nbsp</div></th>\
                                        </tr>';
    };
     
};

