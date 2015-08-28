
$(document).ready(function () {
    var reportManager = new Fanex.AccountingReports.Racing.RacingDailyReport.ReportManager();
    reportManager.initPage();
});


Namespace.Register("Fanex.AccountingReports.Racing.RacingDailyReport.ReportManager");

Fanex.AccountingReports.Racing.RacingDailyReport.ReportManager = function () {

    var self = this;

    var racingDailyReportHasPositoinTaking = new Fanex.AccountingReports.Racing.RacingDailyReport.RacingDailyReport();

    var racingDailyReport = new Fanex.AccountingReports.Racing.RacingDailyReport.RacingDailyReportApply50Percent();

    var _table = {
        DatePickerPlugin: null,
        ListLicensee: null,
        BtnGenerate: null,
        overlay: null,
        DetailBetListPopup: null,
        DetailBetListTitle: null,
        DetailBetListContainer: null,
        DetailBetListContent: null,
        DetailBetListFooter: null
    };

    var _params = {
        Licensee: {
            ProductPrefix: "RB"
        },
        RacingDaily: {
            WinLostDate: null,
            Site: null,
            HasPositoinTaking: null
        },
        DetailBetList: {
            LeagueID: null,
            RaceNo: null,
            Currency: null,
            WinLostDate: null,
            Site: null
        }
    };

    var _detailBetListOption = {
        tblContentId: 'tblContent',
        fullWidth: true,
        gridHeight: 235
    };

    self.initPage = function () {
        catchedDOMElement();
        renderDatePicker();
        getListLicensee();
        bindingEvent();
        racingDailyReport.initPage();
        racingDailyReportHasPositoinTaking.initPage();
        _table.DetailBetListContainer.initGrid(_detailBetListOption);
    };

    var catchedDOMElement = function () {
        _table.DatePickerPlugin = $('#datepickerplugin');
        _table.ListLicensee = $('#ListLicensee');
        _table.BtnGenerate = $('#btnGenerate');
        _table.overlay = $('#overlay');
        _table.DetailBetListPopup = $('#DetailBetListPopup');
        _table.DetailBetListTitle = $('#DetailBetListTitle');
        _table.DetailBetListContainer = $('#DetailBetListContainer');
        _table.DetailBetListContent = $('#tblContent tbody');
        _table.DetailBetListFooter = _table.DetailBetListContainer.find(".col_footer_content thead");
    };

    var renderDatePicker = function () {
        var toDay = new Date();
        var minDate = new Date(balanceUpDate);

        _table.DatePickerPlugin.nexDatePicker({
            nexDatePickerType: "single",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                maxDate: toDay,
                minDate: minDate,
                dateFormat: "M dd, yy"
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: toDay,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },
            onSelectDatePicker: function () {
            }
        });
    };

    var getListLicensee = function () {
        AccReport.disableInput();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Racing/RacingWinLossByLicensee/GetLicensee",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(_params.Licensee),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (licensee) {
                appendLicensee(licensee);
            },
            error: function (jqXHR, event, ajaxSettings) {
                appendLicensee([]);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };

    var appendLicensee = function (listLicensee) {
        if (listLicensee != null && listLicensee.length > 0) {
            var numberLicensee = listLicensee.length;

            for (var i = 0; i < numberLicensee; i++) {
                var optionLicensee = '';
                var licensee = listLicensee[i];

                if (i == 0) {
                    optionLicensee = '<option selected value="' + licensee.Name
                    + '" pt="' + licensee.HasPositoinTaking + '">' + licensee.Name + '</option>';
                }
                else {
                    optionLicensee = '<option value="' + licensee.Name
                    + '" pt="' + licensee.HasPositoinTaking + '">' + licensee.Name + '</option>';
                }

                _table.ListLicensee.append(optionLicensee);
            }
        }
    };

    var bindingEvent = function () {
        _table.BtnGenerate.click(function () {
            getDailyReport();
        });
    };

    var getDailyReport = function () {
        getRacingParams();

        if (IsApply50PercentRacingReport == 'True') {
            racingDailyReportHasPositoinTaking.reset();
            racingDailyReportHasPositoinTaking.getRacingParams(_params.RacingDaily);
            racingDailyReportHasPositoinTaking.getDailyReport();
        }
        else {
            if (_params.RacingDaily.HasPositoinTaking == 'true') {
                racingDailyReportHasPositoinTaking.reset();
                racingDailyReportHasPositoinTaking.getRacingParams(_params.RacingDaily);
                racingDailyReportHasPositoinTaking.getDailyReport();
            }
            else {
                racingDailyReport.reset();
                racingDailyReport.getRacingParams(_params.RacingDaily);
                racingDailyReport.getDailyReport();
            }
        }
    };

    var getRacingParams = function () {
        _params.RacingDaily.WinLostDate = _table.DatePickerPlugin.nexDatePicker("getSingleDate");
        _params.RacingDaily.Site = _table.ListLicensee.val();
        _params.RacingDaily.HasPositoinTaking = _table.ListLicensee.find('option:selected').attr("pt");
    };

    OpenDetailBetList = function (leagueId, raceid, currencyId, name) {
        AccReport.disableInput();
        getDetailBetListDetailParams(leagueId, raceid, currencyId);

        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Racing/RacingWinLossByLicensee/GetDetailBetList",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params.DetailBetList),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (report) {
                    AccReport.popUp.hide();
                    renderDetailBetList(report, name);
                },
                error: function (jqXHR, event, ajaxSettings) {
                    if (jqXHR.status &&
                            jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                        var r = jQuery.parseJSON(jqXHR.responseText);
                        AccReport.popUp.show(r.message);
                    }
                    renderDetailBetList(null);
                },
                complete: function () {
                    AccReport.enableInput();
                }
            });
        }
        else {
            AccReport.enableInput();
        }

    };

    var validateParams = function () {
        return true;
    };

    var getDetailBetListDetailParams = function (leagueId, raceid, currencyId) {
        _params.DetailBetList.LeagueID = leagueId;
        _params.DetailBetList.Currency = currencyId;
        _params.DetailBetList.RaceNo = raceid;
        _params.DetailBetList.Site = _params.RacingDaily.Site;
        _params.DetailBetList.WinLostDate = _params.RacingDaily.WinLostDate;
    };

    var renderDetailBetList = function (report, name) {
        if (report != null && report.length >= 1) {
            var dialogTitle = 'Tote Odds Racing (' + _params.DetailBetList.Site + ') Detail Bet List';
            var reportTitle = 'Racing Daily Report (' + _params.DetailBetList.Site + ') - '
                                    + name + ' - Date: ' + _params.DetailBetList.WinLostDate;
            _table.DetailBetListPopup.dialog({
                title: dialogTitle,
                autoOpen: false,
                height: 635,
                width: 1152,
                close: function () {
                    _table.overlay.addClass('display-none');
                }
            }).css("font-size", "12px");

            var bodyData = '';
            var footerData = '';
            var classHide = IsApply50PercentRacingReport == 'True' ? '' : 'hide';

            bodyData = renderContentDetailBetList(report, bodyData, classHide);
            footerData = renderTotalDetailBetList(report[report.length - 1], footerData, classHide);

            _table.overlay.removeClass('display-none');
            _table.DetailBetListPopup.dialog("open");
            _table.DetailBetListTitle.text(reportTitle);
            _table.DetailBetListContent.empty().append(bodyData);
            _table.DetailBetListFooter.empty().append(footerData);
            _table.DetailBetListContainer.removeClass("display-none").gridScroller(_detailBetListOption);
        }
    };

    var renderContentDetailBetList = function (report, bodyData, classHide) {
        var numberBetList = report.length;

        for (var i = 0; i < numberBetList - 1; i++) {
            var bet = report[i];
            bodyData = renderRowDetailBetList(bet, bodyData, i + 1, classHide);
        }

        return bodyData;
    };

    var renderRowDetailBetList = function (row, bodyData, index, classHide) {
        var odds = row.Odds_Str;
        var memberWinLoss = numeral(row.MemberWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER);
        var memberWinLossHtml = row.MemberWinLoss < 0 ? '<b class="red">' + memberWinLoss + '</b>' : memberWinLoss;
        var licenseeWinLoss = numeral(row.LicenseeWinLossPositionTaking).format(FormatConstant.TWO_DECIMAL_NUMBER);
        var licenseeWinLossHtml = row.LicenseeWinLossPositionTaking < 0 ? '<b class="red">' + licenseeWinLoss + '</b>' : licenseeWinLoss;
        var ibcWinLoss = numeral(row.IBCWinLossPositionTaking).format(FormatConstant.TWO_DECIMAL_NUMBER);
        var ibcWinLossHtml = row.IBCWinLossPositionTaking < 0 ? '<b class="red">' + ibcWinLoss + '</b>' : ibcWinLoss;

        if (row.IsCombineBetType) {
            bodyData += '<tr>\
                    <td><div class="width20 center">' + index + '</div></td>\
                    <td><div class="width100 center" >' + row.Member + '</div></td>\
                    <td><div class="width100 center" >' + row.Username + '</div></td>\
                    <td><div class="width150 center">Ref No: <b>' + row.TransactionID + '</b><br>'
                                                            + row.TransactionDate + '</div></td>\
                    <td><div class="width180 right">\
                        <span class="bl-choice">' + row.HorseBetType + '</span><br>\
                            <span class="bl-bettype">' + row.BetType + '</span>\
                        </div></td>\
                    <td><div class="width80 right"><b>Win: </b>' + odds + '<br><b>Place: </b>'
                                                + row.PlaceOdds + '</div></td>\
                    <td><div class="width80 right">' + numeral(row.Stake).format(FormatConstant.INT_NUMBER) + (row.IsShowExchangeRate ? '<br><span class="exrate2">'
                            + row.ActualRate
                            + '</span><br><span class="exrate2">' + row.ExchangeRate + '</span>' : '') + '</div></td>\
                    <td><div class="width100 right">' + memberWinLossHtml + '<br><span class="exrate2 b">'
                            + numeral(row.MemberCommissionIncome).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</span></div></td>\
                    <td><div class="width100 right">' + licenseeWinLossHtml + '<br><span class="exrate2 b">'
                            + numeral(row.LicenseeCommissionIncome).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</span></div></td>\
                    <td class="' + classHide + '"><div class="width100 right">' + ibcWinLossHtml
                        + '<br><span class="exrate2 b">' + numeral(row.IBCCommissionIncome).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</span></div></td>\
                    <td><div class="width80 center"><b>Win: </b>' + row.WinStatus + '<br><b>Place: </b>'
                            + row.PlaceStatus + '</div></td>\
                    <td><div class="width80 right">' + numeral(row.Dividend).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
               </tr>';
        }
        else {
            bodyData += '<tr>\
                    <td><div class="width20 center">' + index + '</div></td>\
                    <td><div class="width100 center" >' + row.Member + '</div></td>\
                    <td><div class="width100 center" >' + row.Username + '</div></td>\
                    <td><div class="width150 center">Ref No: <b>' + row.TransactionID + '</b><br>'
                            + row.TransactionDate + '</div></td>\
                    <td><div class="width180 right">\
                        <span class="bl-choice">' + row.HorseBetType + '</span><br>\
                            <span class="bl-bettype">' + row.BetType + '</span>\
                        </div></td>\
                    <td><div class="width80 right">' + odds + '</div></td>\
                    <td><div class="width80 right">' + numeral(row.Stake).format(FormatConstant.INT_NUMBER)
                        + (row.IsShowExchangeRate ? '<br><span class="exrate2">' + row.ActualRate
                        + '</span><br><span class="exrate2">' + row.ExchangeRate + '</span>' : '') + '</div></td>\
                    <td><div class="width100 right">' + memberWinLossHtml + '<br><span class="exrate2 b">'
                        + numeral(row.MemberCommissionIncome).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</span></div></td>\
                    <td><div class="width100 right">' + licenseeWinLossHtml + '<br><span class="exrate2 b">'
                        + numeral(row.LicenseeCommissionIncome).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</span></div></td>\
                    <td class="' + classHide + '"><div class="width100 right">' + ibcWinLossHtml
                        + '<br><span class="exrate2 b">' + numeral(row.IBCCommissionIncome).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</span></div></td>\
                    <td><div class="width80 center">' + row.Status + '</div></td>\
                    <td><div class="width80 right">' + numeral(row.Dividend).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
               </tr>';
        }

        return bodyData;
    };

    var renderTotalDetailBetList = function (row, footerData, classHide) {
        var memberWinLoss = numeral(row.MemberWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER);
        var memberWinLossHtml = row.MemberWinLoss < 0 ? '<b class="red">' + memberWinLoss + '</b>' : memberWinLoss;
        var licenseeWinLoss = numeral(row.LicenseeWinLossPositionTaking).format(FormatConstant.TWO_DECIMAL_NUMBER);
        var licenseeWinLossHtml = row.LicenseeWinLossPositionTaking < 0 ? '<b class="red">' + licenseeWinLoss + '</b>' : licenseeWinLoss;
        var ibcWinLoss = numeral(row.IBCWinLossPositionTaking).format(FormatConstant.TWO_DECIMAL_NUMBER);
        var ibcWinLossHtml = row.IBCWinLossPositionTaking < 0 ? '<b class="red">' + ibcWinLoss + '</b>' : ibcWinLoss;

        footerData = '<tr><th colspan="6" class="right"><div style="width:657px;">Total:</div></th>\
                    <th><div class="width80 right">' + numeral(row.Stake).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                    <th><div class="width100 right">' + memberWinLossHtml
                        + '<br><span class="exrate2">' + numeral(row.MemberCommissionIncome).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</span></div></th>\
                    <th><div class="width100 right">' + licenseeWinLossHtml + '<br><span class="exrate2">'
                        + numeral(row.LicenseeCommissionIncome).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</span></div></th>\
                    <th class="' + classHide + '"><div class="width100 right">' + ibcWinLossHtml
                        + '<br><span class="exrate2 b">' + numeral(row.IBCCommissionIncome).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</span></div></th>\
                    <th><div class="width80 right">&nbsp;</div></th>\
                    <th><div class="width80 right">' + numeral(row.Dividend).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                </tr>';

        return footerData;
    };
};
