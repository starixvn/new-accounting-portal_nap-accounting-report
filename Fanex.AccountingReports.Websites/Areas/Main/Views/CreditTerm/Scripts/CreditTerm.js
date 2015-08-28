
Namespace.Register("Fanex.AccountingReports.Main.CreditTerm.CreditTerm");

Fanex.AccountingReports.Main.CreditTerm.CreditTerm = function () {
    var self = this;

    var _params = {
        checkerName: null,
        pre1: null,
        pre2: null
    };

    var _editRemarkParams = {
        accountRemark: null,
        custID: null,
        userID: null,
        errCode: null,
        errMessage: null,
        accountRemarkDate: null,
        language: 'E'
    };

    var _table = {
        preAgreedReport: null,
        preAgreedAmountHeader: null,
        installAmountHeader: null,
        creditTermContainer: null,
        creditTermContent: null,
        btnGenerate: null,
        txtCheckerName: null,
        txtFirstPreAgreedAmount: null,
        txtSecondPreAgreedAmount: null,
        btnExportExcel: null,
        preAgreedReportTitle: null
    };

    var gridOpts = {
        tblContentId: 'CreditTermContent',
        fullWidth: true,
        // because use girdScroller, gridHeight is gridExpandHeight.
        gridHeight: 235
    };

    self.initPage = function () {
        catchedDOMElement();
        bindingEvent();
        _table.creditTermContainer.initGrid(gridOpts);
        resetHtml();
    };

    var resetHtml = function () {
        _table.txtCheckerName.val('');
        _table.txtFirstPreAgreedAmount.val('');
        _table.txtSecondPreAgreedAmount.val('');
        _table.creditTermContent.empty();
        AccReport.popUp.hide();
    };

    var catchedDOMElement = function () {
        _table.preAgreedAmountHeader = $('#PreAgreedAmountHeader');
        _table.installAmountHeader = $('#InstallAmountHeader');
        _table.creditTermContainer = $('#CreditTermContainer');
        _table.creditTermContent = $('#CreditTermContent tbody');
        _table.btnGenerate = $('#btnGenerate');
        _table.txtCheckerName = $('#txtCheckname');
        _table.txtFirstPreAgreedAmount = $('#txtFromPreAmount');
        _table.txtSecondPreAgreedAmount = $('#txtToPreAmount');
        _table.btnExportExcel = $('#btnExport');
        _table.preAgreedReport = $('#PreAgreedReport');
        _table.preAgreedReportTitle = $('#PreAgreedReportTitle');
    };

    var bindingEvent = function () {
        _table.btnGenerate.click(function () {
            self.getReport();
        });

        _table.btnExportExcel.click(function () {
            self.exportExcel();
        });
    };

    self.getReport = function () {
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Main/CreditTerm/GetCreditTermReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (report) {
                    AccReport.popUp.hide();
                    self.renderReport(report);
                },
                error: function (jqXHR, event, ajaxSettings) {
                    if (jqXHR.status &&
                            jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                        var r = jQuery.parseJSON(jqXHR.responseText);
                        AccReport.popUp.show(r.message);
                    }
                    self.renderReport(null);
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

    var getParams = function () {
        _params.checkerName = _table.txtCheckerName.val();
        _params.pre1 = _table.txtFirstPreAgreedAmount.val();
        _params.pre2 = _table.txtSecondPreAgreedAmount.val();
    };

    var validateParams = function () {
        if (isNaN(_params.pre1) || parseFloat(_params.pre1) < 0) {
            AccReport.popUp.show("Invalid Pre-Agreed Amount!");
            _table.txtFirstPreAgreedAmount.focus();
            return false;
        }
        else {
            AccReport.popUp.hide();
        }

        if (isNaN(_params.pre2) || parseFloat(_params.pre2) < 0) {
            AccReport.popUp.show("Invalid Pre-Agreed Amount!");
            _table.txtSecondPreAgreedAmount.focus();
            return false;
        }
        else {
            AccReport.popUp.hide();
        }

        if (parseFloat(_params.pre1) > parseFloat(_params.pre2)) {
            AccReport.popUp.show("The first Pre-Agreed Amount must be less than or equals the second Pre-Agreed Amount!");
            _table.txtFirstPreAgreedAmount.focus();
            return false;
        }
        else {
            AccReport.popUp.hide();
        }
        return true;
    };

    self.renderReport = function (report) {
        var reportTitle = "Credit Term - Pre Agreed Report";

        if (report != null && report.length > 0) {
            var numberParentMembers = report.length;
            var html = "";

            for (var i = 0; i < numberParentMembers; i++) {
                var parentMember = report[i];
                html += renderParentMemberAndChild(parentMember);
            }

            $(".grid-toolbox").removeClass("display-none");
        }
        else {
            html = renderNoData();
            $(".grid-toolbox").addClass("display-none");
        }

        _table.preAgreedReportTitle.html(reportTitle);
        _table.creditTermContent.empty().append(html);
        _table.creditTermContainer.removeClass("display-none").gridScroller(gridOpts);
        _table.preAgreedReport.removeClass("display-none");
    };

    var renderParentMemberAndChild = function (parentMember) {
        var html = "";
        var childTable = "";
        var groupId = parentMember.GroupID;
        var overLimit = parentMember.OverLimit;

        if (groupId > 0) {
            var childRows = parentMember.Rows;

            html += renderParentRow(parentMember, groupId, overLimit);

            if (childRows != null && childRows.length > 0) {
                childTable = renderChildTable(childRows, parentMember.CurrencyName, overLimit);
            }

            html += childTable;
        }

        return html;
    };

    var renderChildTable = function (childRows, currencyName, overLimit) {
        var childTable = "";
        var numberChilds = childRows.length;
        childTable = renderHeaderChildTable(currencyName);

        for (var i = 0; i < numberChilds; i++) {
            var childRow = childRows[i];
            var childHtml = renderChildRow(childRow, overLimit, currencyName);

            childTable += childHtml;
        }

        childTable += '</tbody></table>';

        childTable = '<tr><td></td><td colspan="9">' + childTable + '</td>';
        return childTable;
    };

    var renderParentRow = function (parentMember, groupId, overLimit) {
        var cssClass = "";

        if (overLimit) {
            cssClass = groupId == 0 ? "odd-row" : "even-row";
        }
        else {
            cssClass = "limit-parent-row";
        }

        var html = '<tr class="' + cssClass + '">\
            <td>\
                <div class="' + 'align-center width40">' + parentMember.RoleName + '</div>\
            </td>\
            <td>\
                <div class="' + 'align-center width148"><b>' + parentMember.Username + '</b></div>\
            </td>\
            <td>\
                <div class="' + 'align-center width50">' + parentMember.CurrencyName + '</div>\
            </td>\
            <td>\
                <div class="' + 'align-right width110">' +
                    setColorToValue(parentMember.PreAgreeAmount) +
                '</div>\
            </td>\
            <td>\
                <div class="' + 'align-right width110">' +
                    setColorToValue(parentMember.Balance) +
                '</div>\
            </td>\
            <td>\
                <div class="' + 'align-right width110">'
                    + setColorToValue(parentMember.BalanceBase) +
                '</div>\
            </td>\
            <td>\
                <div class="' + 'align-left width300">' + parentMember.AccountRemark + '</div>\
            </td>\
            <td>\
                <div class="' + 'align-center width90">' + parentMember.PhoneNumber + '</div>\
            </td>\
            <td>\
                <div class="' + 'align-center width80">' + parentMember.CheckerName + '</div>\
            </td>\
            <td>\
                <div class="' + 'align-center width80">'
                        + formatDateTime(parentMember.AccountRemarkDate) + '</div>\
            </td>\
            </tr>'
                ;

        return html;
    };

    var renderHeaderChildTable = function (currencyName) {
        var childTable = "";

        childTable = '<table class="' + 'child-table-header' + '">\
                            <tbody>\
                                <tr>\
                                    <th>\
                                        <div div class="' + 'align-center width40">Role</div>\
                                    </th>\
                                    <th>\
                                        <div div class="' + 'align-center width100">Username</div>\
                                    </th>\
                                    <th>\
                                        <div div class="' + 'align-center width50">Curr</div>\
                                    </th>\
                                    <th>\
                                        <div div class="' + 'align-center width110">Balance</div>\
                                    </th>\
                                    <th>\
                                        <div div class="' + 'align-center width110">' + currencyName + '</div>\
                                    </th>\
                                    <th>\
                                        <div div class="' + 'align-center width110">RM</div>\
                                    </th>\
                                    <th>\
                                        <div div class="' + 'align-center width440">Remark</div>\
                                    </th>\
                                    <th>\<div div class="' + 'align-center width30"></div>\</td>\
                                    <th>\
                                        <div div class="' + 'align-center width80">Remark Timestamp</div>\
                                    </th>\
                               </tr>';

        return childTable;
    };

    var renderChildRow = function (childRows, overLimit, currencyParent) {
        var index = childRows.CustomerID;
        var cssClass = "";
        var balance = "";

        if (!overLimit) {
            cssClass = "limit-child-row";
        }   

        if (currencyParent == childRows.CurrencyName) {
            balance = "";
        }
        else {
            balance = setColorToValue(childRows.Balance);
        }

        var html = "<tr class='" + cssClass + "'>\
            <td>\
                <div class='align-center width40'>" + childRows.RoleName + "</div>\
            </td>\
            <td>\
                <div class='align-center width100' title='" + childRows.Username + "'><b>" + childRows.Username + "</b></div>\
            </td>\
            <td>\
                <div class='align-center width50'>" + childRows.CurrencyName + "</div>\
            </td>\
            <td>\
                <div class='align-right width110'>" + balance + "</div>\
            </td>\
            <td>\
                <div class='align-right width110'>"
                    + setColorToValue(childRows.BalanceGroup) +
                "</div>\
            </td>\
            <td>\
                <div class='align-right width110'>"
                    + setColorToValue(childRows.BalanceBase) +
                 "</div>\
            </td>\
            <td>\
                <div id='dv" + index + "' class='align-left width440'>" + childRows.AccountRemark + "</div>\
                <input id='txt" + index + "' type='text' \
                         onkeydown='EditRemarkKeyDown(event, " + index + ")' class='input-remark' />\
            </td>\
            <td>\
                <div id='dvEdit" + index + "'class='edit-remark width30'\
                     onclick='EditRemark(" + index + "," + JSON.stringify(childRows) + ");'>Edit</div>\
            </td>\
            <td>\
                <div id='dvDate" + index + "' class='align-center width80'>" +
                    formatDateTime(childRows.AccountRemarkDate) + "</div>\
            </td>\
            </tr>";

        return html;
    };

    var renderNoData = function () {
        var html = '<tr>\
                        <td colspan="10">\
                            <div class="no-data">There is no data</div>\
                        </td>\
                    </tr>';
        return html;
    };

    var setColorToValue = function (value) {
        if (value > 0) {
            return "<font color ='red'>"
                        + numeral(value).format(FormatConstant.TWO_DECIMAL_NUMBER) +
                   "</font>"
        }
        else {
            return numeral(value).format(FormatConstant.TWO_DECIMAL_NUMBER)
        }
    };

    EditRemark = function (index, member) {
        var divRemarkId = '#dv' + index;

        // "display: none" is used in IE8.
        if ($(divRemarkId).attr("style") == "display: none;"
          || $(divRemarkId).attr("style") == "display: none") {
            ChangeSaveToEdit(index);
        }
        else {
            var divEditID = '#dvEdit' + index;
            $(divEditID).text('Save');

            var textRemarkID = '#txt' + index;

            var remark = $(divRemarkId).text();
            $(divRemarkId).hide();

            $(textRemarkID).val(remark);
            $(textRemarkID).show();
            $(textRemarkID).focus();

            getEditRemarkParams(member);
        }
    };

    EditRemarkKeyDown = function (event, index) {
        if (event.keyCode == 13) {
            ChangeSaveToEdit(index);
        }
    };

    var ChangeSaveToEdit = function (index) {
        UpdateEditRemark(index);

        var divEditID = '#dvEdit' + index;
        $(divEditID).text('Edit');
    };

    UpdateEditRemark = function (id) {
        var divRemarkId = '#dv' + id;
        var textRemarkID = '#txt' + id;

        // "display: none" is used in IE8.
        if ($(divRemarkId).attr("style") == "display: none;"
            || $(divRemarkId).attr("style") == "display: none") {
            var editRemark = $(textRemarkID).val();
            var originalRemark = $(divRemarkId).text();

            if (editRemark != originalRemark) {
                _editRemarkParams.accountRemark = editRemark;

                $.ajax({
                    type: "POST",
                    url: window.rootUrl + "Main/CreditTerm/EditRemark",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify(_editRemarkParams),
                    beforeSend: function () {
                        AccReport.disableInput();
                    },
                    success: function (remark) {
                        AccReport.popUp.hide();
                        if (remark.ErrorCode > 0) {
                            AccReport.popUp.show(remark.ErrorMessage);
                        }
                        else {
                            $(divRemarkId).text(editRemark);
                            var newDateTime = formatDateTime(remark.AccountEditedRemarkDate);
                            $("#dvDate" + id).text(newDateTime);
                        }
                    },
                    error: function (jqXHR, event, ajaxSettings) {
                        if (jqXHR.status) {
                            if (jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                                var r = jQuery.parseJSON(jqXHR.responseText);
                                AccReport.popUp.show(r.message);
                            }
                            else if (jqXHR.status == AccReportData.Constants.HtmlErrorCode.AuthorityError) {
                                var responseText = permissionError;
                                AccReport.popUp.show(responseText);
                            }
                        }
                    },
                    complete: function () {
                        AccReport.enableInput();
                        $(divRemarkId).show();
                        $(textRemarkID).hide();
                    }
                });
            }
            else {
                $(divRemarkId).show();
                $(textRemarkID).hide();
            }
        }
    };

    var getEditRemarkParams = function (member) {
        _editRemarkParams.accountRemark = member.AccountRemark;
        _editRemarkParams.custID = member.CustomerID;
        _editRemarkParams.errCode = 0;
        _editRemarkParams.errMessage = "";
        _editRemarkParams.accountRemarkDate = member.AccountRemarkDate;
    };

    var formatDateTime = function (inputDateTime) {
        var newDate = new Date(inputDateTime);
        var outputDateTime = (newDate.getMonth() + 1) + "/" + newDate.getDate() + " "
                            + newDate.getHours() + ":" + newDate.getMinutes();

        return outputDateTime;
    };

    self.exportExcel = function () {
        AccReport.downloadFile(window.rootUrl + "Main/CreditTerm/ExportCreditTermReport", _params);
    };
};