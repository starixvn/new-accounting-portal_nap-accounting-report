WeekPickerManager = function () {
    this.DATE_FORMAT_STRING = "dd M yy";
    this.MonthNames = ["Jan", "Feb", "Mar", "Apr",
                       "May", "Jun", "Jul", "Aug",
                       "Sep", "Oct", "Nov", "Dec"];

    this.registerWeekPicker = function () {
        // week picker plugin
        var configs = {};

        $.fn.weekpicker = function (options, func) {

            var $this = $(this);
            //            $this.css("margin", "2px");
            var $calendar = $this.nextAll("div:first");

            var id = $this.attr("id");
            if (!configs[id]) {
                configs[id] = { date: new Date(maxDate) };
            }

            var defaults = configs[id];

            var displayYear = defaults.date.getFullYear();
            var selectCurrentWeek = function () {
                window.setTimeout(function () {
                    $calendar.find('.ui-datepicker-current-day a').addClass('ui-state-active')
                }, 1);
            };

            var selectWeek = function (date, hideOnSelect) {

                if (!date) {
                    date = $calendar.datepicker('getDate');
                    if (!date)
                        date = new Date(maxDate);
                }

                var dat = date.getDate();
                var day = date.getDay();
                defaults.startDate = new Date(date.getFullYear(), date.getMonth(), 1);
                if (day == 0) {
                    day = 7;
                }

                defaults.startDate.setDate(dat - day + 1);
                defaults.endDate = new Date(date.getFullYear(), date.getMonth(), 1);
                defaults.endDate.setDate(dat - day + 7);

                if (date) {
                    //var week = date.getWeekOfYear();
                    var week = $.datepicker.iso8601Week(defaults.startDate);
                    $this.val(week + " (" + weekPickerManager.GetFormattedDate(defaults.startDate) + ")");
                }

                if (defaults.selectHandlers) {
                    for (var i = 0; i < defaults.selectHandlers.length; i++) {
                        defaults.selectHandlers[i](defaults.startDate, defaults.endDate, $this[0]);
                    }
                }

                if (hideOnSelect) {
                    $this.weekpicker("hide");
                }

                //if (defaults.startDate == date) {
                $calendar.datepicker('setDate', defaults.startDate);
                //}
                displayYear = defaults.startDate.getFullYear();
                selectCurrentWeek();
            };

            if (typeof options == "string") {
                if (options == "hide") {
                    $calendar.fadeOut("fast");
                }
                else if (options == "show") {
                    if ($calendar.is(":visible")) {
                        return;
                    }

                    $calendar.show();
                }
                else if (options == "onSelect") {
                    if (!defaults.selectHandlers) {
                        defaults.selectHandlers = [];
                    }

                    defaults.selectHandlers.push(func);
                }
                else if (options == "getDates") {
                    return {
                        start: defaults.startDate,
                        end: defaults.endDate.compareTo(new Date(maxDate)) == 1 ? maxDate : defaults.endDate
                    };
                }
                else if (options == "setDate") {
                    selectWeek(func);
                }
            }
            else {
                $.extend(defaults, options);
                var offset = $this.offset();

                $this.addClass("datepicker hasDatepicker");
                $this.after("<div class='weekcalendar'></div>"); //<img style='position: relative;margin-top: 1px' class='ui-datepicker-trigger' src='" + $("#currentDomain").val() + "/App_Themes/" + currentTheme + "/Images/calendar1.gif' alt='' title=''>
                $calendar = $this.next();
                $calendar.css({ "position": "absolute", "z-index": 1000, "margin-top": offset.top, "float": "left" });
                var ishover = false;
                $.extend(defaults, {
                    dateFormat: weekPickerManager.DATE_FORMAT_STRING,
                    showOn: null,
                    buttonImageOnly: true,
                    buttonText: "",
                    //buttonImage: $("#currentDomain").val() + "/App_Themes/" + currentTheme + "/Images/calendar.gif",
                    firstDay: 1,
                    showOtherMonths: true,
                    selectOtherMonths: true,
                    showWeek: true,
                    changeYear: true,
                    changeMonth: true,
                    //                calculateWeek: function (date, a, b, c) {
                    //                    return date.getWeekOfYear();
                    //                },
                    onSelect: function (startDate, endDate, instance) {
                        selectWeek(null, true);
                    },
                    beforeShowDay: function (date) {
                        var cssClass = '';
                        if (defaults.startDate <= date && date <= defaults.endDate) {
                            cssClass = 'ui-datepicker-current-day';
                        }

                        return [true, cssClass];
                    },
                    onChangeMonthYear: function (year, month, inst) {
                        selectCurrentWeek();
                        displayYear = year;
                    }
                });

                $calendar.datepicker(defaults);
                var show = function () {
                    ishover = true;
                    $this.weekpicker("show");
                }

                $calendar.hide();
                $this.width(144).mousedown(show);

                function enter() {
                    ishover = true;
                }

                function leave() {
                    ishover = false;
                }

                $calendar.mouseenter(enter).mouseleave(leave);
                $this.mouseenter(enter).mouseleave(leave);

                $("html").bind("click", function (e) {
                    if (!ishover) {
                        $this.weekpicker("hide");
                    }
                });
                $this.nextAll("img.ui-datepicker-trigger:first").click(function () {
                    if ($calendar.is(":visible")) {
                        $this.weekpicker("hide");
                    }
                    else {
                        show();
                    }
                });

                // set initial value    
                selectWeek(defaults.date);
            }
        }

        if ($('.weekpicker').length > 0) {
            $('.weekpicker').each(function () {
                $(this).weekpicker({
                    date: new Date($(this).attr('data-value')),
                    minDate: new Date($(this).attr('data-mindate')),
                    maxDate: new Date($(this).attr('data-maxdate'))
                });
            });
        }

        $('.weekcalendar table.ui-datepicker-calendar tr').die().live({
            mousemove: function () {
                $(this).find('td a').addClass('ui-state-hover');
            },
            mouseleave: function () {
                $(this).find('td a').removeClass('ui-state-hover');
            }
        });
    };

    this.GetFormattedDate = function (date) {
        return moment(date).format(FormatConstant.MMM_DD_YYYY_STRING);
    };
};

var weekPickerManager = new WeekPickerManager();