
Namespace.Register("Fanex.AccountingReports.Misc.DailyCreatedCustomers.DailyCreatedCustomers");
Fanex.AccountingReports.Misc.DailyCreatedCustomers.DailyCreatedCustomers = function () {
    var self = this;

    var _tables = {
        datepickerplugin: null,
        btnGenerate: null,

        report: null,       
        reportContent: null    
    };  

    var _params = {
        reportParams: {
            Fromdate: null,
            Todate: null
        }
    };


    self.initPage = function () {
        cachedDOMElements();
        renderDatePicker();
        bindingEvent();
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.report = $("#dailyCreatedCustomersReport");
        _tables.reportContent = _tables.report.find("#dailyCreatedCustomersReportGrid");
    };
    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });
    };
    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "daterange",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: "M dd, yy"
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: labelTo,
                InitDate: today,
                LabelCss: "lbl-field col_1",
                TextboxCss: "edit-field col_6"
            },
            onSelectDatePicker: function () {
            },
            maxDate: new Date(today)
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };
    self.btnGenerateClick = function () {
        HideDetailsReport();
        self.getReport();
    };
    self.getReport = function () {
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Misc/DailyCreatedCustomers/GetReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params.reportParams),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    AccReport.popUp.hide();
                    self.renderReport(rs);
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
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        _params.reportParams.Fromdate = dateRange.FromDate + " 00:00:00";
        _params.reportParams.Todate = dateRange.ToDate + " 23:59:59";
    };
    var validateParams = function () {
        return true;
    };
    self.renderReport = function (report) {
        var reportTitle = "Daily Created Customers Report from " + _params.reportParams.Fromdate.replace("00:00:00","") +
                                                        " to " + _params.reportParams.Todate.replace("23:59:59","");

        if (report &&
            report.CreatingDetails &&
            report.CreatingDetails.length > 0) {
            var bodyData = [];
            var infoLength = report.CreatingDetails.length;
            for (var i = 0; i < infoLength; i++) {
                var info = report.CreatingDetails[i];
                bodyData.push(
                    {
                        Columns: [
                            { Name: "Date", Value: info.CreatedDate },
                            { Name: "Super", Value: info },                       
                            { Name: "Direct_Master", Value: info },
                            { Name: "Direct_Agent", Value: info },                       
                            {Name: "Master", Value: info },
                            { Name: "Agent", Value: info },
                            { Name: "Member", Value: info }                       
                        ]
                    }
                );
            }

            if (report.CreatingDetails.length > 0) {
                var footerData = [
                    {
                        Columns: [
                            { Name: "Date", Value: "Total" },
                            { Name: "Super", Value: numeral(report.SuperAmount).format(FormatConstant.INT_NUMBER) },                       
                            { Name: "Direct_Master", Value: numeral(report.DirectMasterAmount).format(FormatConstant.INT_NUMBER) },
                            { Name: "Direct_Agent", Value: numeral(report.DirectAgentAmount).format(FormatConstant.INT_NUMBER) },                       
                            {Name: "Master", Value: numeral(report.MasterAmount).format(FormatConstant.INT_NUMBER) },
                            { Name: "Agent", Value: numeral(report.AgentAmount).format(FormatConstant.INT_NUMBER) },
                            { Name: "Member", Value: numeral(report.MemberAmount).format(FormatConstant.INT_NUMBER) }                       
                        ]
                    }
                ]
            }

            self.RenderGrid(bodyData, footerData, reportTitle);
        }
        else {
            self.RenderGrid([], [], reportTitle);
        }
    };

    self.RenderGrid = function (bodyData, footerData, title) {
        var columns = [
            { HeaderText: "Date", Name: "Date", HeaderAlign: "center", CellAlign: "center", Width: 100 },
            { HeaderText: "Super", Name: "Super", HeaderAlign: "center", CellAlign: "right", Width: 60,
                CustomCellRender: function (currentCell) {
                    return self.getDrildownLink(Enums.RoleId.Super, currentCell.Value.CreatedDate, currentCell.Value.SuperAmount, false);
                }
            },        
            {HeaderText: "Direct Master", Name: "Direct_Master", HeaderAlign: "center", CellAlign: "right", Width: 60,
                CustomCellRender: function (currentCell) {
                    return self.getDrildownLink(Enums.RoleId.DirectMaster, currentCell.Value.CreatedDate, currentCell.Value.DirectMasterAmount, true);
                }
            },
            { HeaderText: "Direct Agent", Name: "Direct_Agent", HeaderAlign: "center", CellAlign: "right", Width: 60,
                CustomCellRender: function (currentCell) {
                    return self.getDrildownLink(Enums.RoleId.DirectAgent, currentCell.Value.CreatedDate, currentCell.Value.DirectAgentAmount, true);
                }
            },           
            {HeaderText: "Master", Name: "Master", HeaderAlign: "center", CellAlign: "right", Width: 60,
                CustomCellRender: function (currentCell) {
                    return self.getDrildownLink(Enums.RoleId.DirectMaster, currentCell.Value.CreatedDate, currentCell.Value.MasterAmount, false);
                }
             },
            { HeaderText: "Agent", Name: "Agent", HeaderAlign: "center", CellAlign: "right", Width: 60,
                CustomCellRender: function (currentCell) {
                    return self.getDrildownLink(Enums.RoleId.DirectAgent, currentCell.Value.CreatedDate, currentCell.Value.AgentAmount, false);
                }
            },
            { HeaderText: "Member", Name: "Member", HeaderAlign: "center", CellAlign: "right", Width: 60,
                CustomCellRender: function (currentCell) {
                    return self.getDrildownLink(Enums.RoleId.Member, currentCell.Value.CreatedDate, currentCell.Value.MemberAmount, false);
                }
            }        
        ];

        var gridOptions = {
            columns: columns,
            showTitle: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                }
            },
            bodyRows: bodyData,
            showFooter: true,
            footerRows: footerData,
            gridExpandHeight: 160,            
            numberOfFrozenColumn: 0,
            marginRight: 20,
            showPager: true,
            pagerOption: {
                itemsPerPage: 50,
                currentPage: 0,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: bodyData.length
            }
        };

        _tables.reportContent.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.getDrildownLink = function (roleId, date, amount, isDirect) {
        if (amount == 0) {
            return "<div>" + amount + "</div>";
        }

        return "<div class='link bold' onclick=\"ViewReportDetails('" + roleId + "','" + date + "'," + isDirect + ")\">"
                                        + numeral(amount).format(FormatConstant.INT_NUMBER) + "</div>";
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Misc/DailyCreatedCustomers/ExportReport", _params.reportParams);
    };
}