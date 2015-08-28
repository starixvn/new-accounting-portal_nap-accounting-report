$(document).ready(function () {
	var reportManager = new Fanex.AccountingReports.Main.WinLossSummary3rd.ReportManager();
	reportManager.initPage();
});

Namespace.Register('Fanex.AccountingReports.Main.WinLossSummary3rd.ReportManager');
Fanex.AccountingReports.Main.WinLossSummary3rd.ReportManager = function () {
	var self = this;
	// Summary & Detail
	var _winLossSummary = new Fanex.AccountingReports.Main.WinLossSummary3rd.WinLossSummary();
	var _gameMatchDetails = new Fanex.AccountingReports.Main.WinLossSummary3rd.GameMatchDetails();

	var _tables = {
		filterContainer: null,
		winlossSumReportContainer: null,
		gameMatchDetailContainer: null,
		scoreInformationContainer: null,
		btnGenerate: null,
		btnHistoryReport: null,
		datepickerplugin: null,
		// history
		filterContainerHistory: null,
		btnGenerateHistory: null,
		btnBack: null,
		monthPicker: null,
		// recalculate
		recalcualteToday: null,
		recalculateYesterday: null,
		// note
		noteTitle: null,
		noteContent: null,
		showing: true
	};
	var _params = {
		reportParams: {
			FromDate: null,
			ToDate: null
		},
		isHistory: false
	};

	self.initPage = function () {
		cachedDOMElements();
		renderDatePicker();
		bindingEvent();

		_tables.recalcualteToday.html(viewResources.Today + ' (' + calToday + ')');
		_tables.recalculateYesterday.html(viewResources.Yesterday + ' (' + calYesterday + ')');
		// init Summary & Detail
		_winLossSummary.initPage();
		_gameMatchDetails.initPage();
		_tables.filterContainerHistory.addClass('display-none');
		scoreInfoManager.InitPage();
	};
	var renderDatePicker = function () {
		$('#datepickerplugin').nexDatePicker({
			nexDatePickerType: 'daterange',
			datePickerFormat: {
				changeMonth: true,
				changeYear: true,
				dateFormat: FormatConstant.M_DD_YY_STRING,
				minDate: new Date(minDate),
				maxDate: new Date(today)
			},
			firstDatePicker: {
				Id: 'firstDatePickerId1',
				Label: viewResources.From,
				InitDate: today,
				LabelCss: 'lbl-field col_2',
				TextboxCss: 'edit-field col_6'
			},
			secondDatePicker: {
				Id: 'secondDatePickerId1',
				Label: viewResources.To,
				InitDate: today,
				LabelCss: 'lbl-field col_1',
				TextboxCss: 'edit-field col_6'
			}
		});
		// history: render month-picker
		$('#ddlMonth').monPickr({
			today: new Date(today),
			maxDate: new Date(today),
			minDate: new Date(minDateHistory),
			showDateRange: true,
			fromDayId: 'ddlFromDay',
			toDayId: 'ddlToDay',
			sortDesc: true
		});
	};
	var cachedDOMElements = function () {
		_tables.filterContainer = $('#winlossSummaryReportFilter');
		_tables.winlossSumReportContainer = $('#winlossSumReportContainer');
		_tables.gameMatchDetailContainer = $('#gameMatchDetailContainer');
		_tables.scoreInformationContainer = $('#scoreInformationContainer');
		_tables.btnGenerate = $('#btnGenerate');
		_tables.btnHistoryReport = $('#btnHistoryReport');
		_tables.filterNote = $('#notemessage');
		_tables.datepickerplugin = $('#datepickerplugin');
		// history
		_tables.filterContainerHistory = $('#winlossSummaryReportFilter-history');
		_tables.btnGenerateHistory = $('#btnGenerate-history');
		_tables.btnBack = $('#btnBack');
		_tables.monthPicker = $('#ddlMonth');
		// recalculate
		_tables.recalcualteToday = $('#recalculateToday');
		_tables.recalculateYesterday = $('#recalculateYesterday');
		// note
		_tables.noteTitle = $('#note-title');
		_tables.noteContent = $('#note-content');
	};
	var bindingEvent = function () {
		_tables.btnGenerate.unbind('click');
		_tables.btnGenerate.click(function () {
			_params.isHistory = false;
			getReport();
		});

		_tables.btnHistoryReport.unbind('click');
		_tables.btnHistoryReport.click(function () {
			showHistoryReport();
		});
		// history
		_tables.btnGenerateHistory.unbind('click');
		_tables.btnGenerateHistory.click(function () {
			_params.isHistory = true;
			getReport();
		});

		_tables.btnBack.unbind('click');
		_tables.btnBack.click(function () {
			showCurrentReport();
		});
		// recalculate
		_tables.recalcualteToday.unbind('click');
		_tables.recalcualteToday.click(function () {
			RecalculateWinLoss(calToday);
		});

		_tables.recalculateYesterday.unbind('click');
		_tables.recalculateYesterday.click(function () {
			RecalculateWinLoss(calYesterday);
		});
		// note
		_tables.noteTitle.unbind('click');
		_tables.noteTitle.click(function () {
			_tables.noteContent.toggle(function () {
				if (_tables.showing) {
					_tables.noteTitle.addClass('note-title-expand');
				}
				else {
					_tables.noteTitle.removeClass('note-title-expand');
				}
				_tables.showing = !_tables.showing;
			});
		});
	};
	var getReport = function () {
		getParams();
		_winLossSummary.getReport(_params);
	};
	var getParams = function () {
		if (!_params.isHistory) {
			var dateRange = _tables.datepickerplugin.nexDatePicker('getDateRange');
			_params.reportParams.FromDate = dateRange.FromDate;
			_params.reportParams.ToDate = dateRange.ToDate;
		}
		else {
			_params.reportParams.FromDate = _tables.monthPicker.monPickr("getFromDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER);
			_params.reportParams.ToDate = _tables.monthPicker.monPickr("getToDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER);
		}
	};
	var showHistoryReport = function () {
		// Filter: hide Current
		_tables.filterContainer.removeClass('display-none');
		_tables.filterContainer.addClass('display-none');
		// Filter: show History
		_tables.filterContainerHistory.removeClass('display-none');

		// Report: hide
		invisibleReports();
	};
	var showCurrentReport = function () {
		// Filter: hide History
		_tables.filterContainerHistory.removeClass('display-none');
		_tables.filterContainerHistory.addClass('display-none');
		// Filter: show Current
		_tables.filterContainer.removeClass('display-none');

		// Report: hide
		invisibleReports();
	};
	var invisibleReports = function () {
		_tables.winlossSumReportContainer.removeClass('display-none');
		_tables.winlossSumReportContainer.addClass('display-none');

		_tables.gameMatchDetailContainer.removeClass('display-none');
		_tables.gameMatchDetailContainer.addClass('display-none');

		_tables.scoreInformationContainer.removeClass('display-none');
		_tables.scoreInformationContainer.addClass('display-none');
	};
	ViewMatchGameDetails = function (fromDate, toDate, sportId, betId, live, parentName, drillDownByType) {
		if (sportId < 0) {
			sportId = betId;
			betId = 0;
		}
		_gameMatchDetails.getReport({
			reportParams: {
				FromDate: fromDate,
				ToDate: toDate,
				SportType: sportId,
				BetType: betId,
				LiveIndicator: live,
				DrillDownByType: drillDownByType
			}
		}, parentName);
	};
	RecalculateWinLoss = function (day) {
		AccReport.disableInput();
		$.ajax({
			type: 'POST',
			url: window.rootUrl + 'Main/WinLossSummary3rd/RecalculateWinLoss',
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			data: JSON.stringify({ WinlostDate: day }),
			beforeSend: function () { },
			success: function (rs) {
				if (rs && rs.Code == 0) {
					AccReport.popUp.show('Recalculate on ' + day + ' successfully.', null, 1);
				}
			},
			error: function (jqXhr, event, ajaxSettings) {
				if (jqXhr.status) {
					if (jqXhr.status == 500) { // server errors
						var r = jQuery.parseJSON(jqXhr.responseText);
						AccReport.popUp.show(r.message);
					}
					else if (jqXhr.status == 403) { // don't have permission
						AccReport.popUp.show(viewResources.NoFunctionPermission);
					}
				}
			},
			complete: function () {
				AccReport.enableInput();
			}
		});
	};
}