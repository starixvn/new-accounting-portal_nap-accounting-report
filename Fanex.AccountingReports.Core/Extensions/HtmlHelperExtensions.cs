// --------------------------------------------------------------------------
// <copyright file="HtmlHelperExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.10.10">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        #region Date Picker

        /// <summary>
        /// The dates picker.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="name">The name value.</param>
        /// <param name="date">The date value.</param>
        /// <param name="minDate">The minimum date.</param>
        /// <param name="maxDate">The maximum date.</param>
        /// <param name="formatDate">The format date.</param>
        /// <param name="unavailDates">The unavailable dates.</param>
        /// <returns>Dates picker.</returns>
        /// <exception cref="System.ArgumentNullException">Argument Null Exception.</exception>
        public static MvcHtmlString DatePicker(
            this HtmlHelper htmlHelper,
            string name,
            DateTime? date,
            DateTime? minDate = null,
            DateTime? maxDate = null,
            string formatDate = DateTimeFormatConstants.ShortDate,
            IEnumerable<DateTime> unavailDates = null)
        {
            if (string.IsNullOrWhiteSpace(formatDate))
            {
                throw new ArgumentNullException(Errors.FormatDateIsNull);
            }

            string disabledDate = (unavailDates != null && unavailDates.Count() > 0) ?
                string.Join(",", unavailDates.Select(d => d.ToString(DateTimeFormatConstants.ShortDate))) :
                string.Empty;

            var textbox = htmlHelper.TextBox(
                name,
                date.HasValue ? date.Value.ToString(formatDate) : string.Empty,
                new
                {
                    @class = "datepicker",
                    mindate = minDate != null && minDate != DateTime.MinValue ?
                                ((DateTime)minDate).ToString(DateTimeFormatConstants.YearMonthDateWithSlash) :
                                Labels.Symbol_Null,
                    maxdate = maxDate != null && maxDate != DateTime.MinValue ?
                                ((DateTime)maxDate).ToString(DateTimeFormatConstants.YearMonthDateWithSlash) :
                                Labels.Symbol_Null,
                    disabledDate = disabledDate
                });

            return MvcHtmlString.Create(textbox.ToString());
        }

        /// <summary>
        /// The date picker for.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML string.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="minDate">The minimum date.</param>
        /// <param name="maxDate">The maximum date.</param>
        /// <param name="formatDate">The format date.</param>
        /// <param name="unavailDates">The unavailable dates.</param>
        /// <returns>The date picker.</returns>
        public static MvcHtmlString DatePickerFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            DateTime? minDate = null,
            DateTime? maxDate = null,
            string formatDate = DateTimeFormatConstants.ShortDate,
             IEnumerable<DateTime> unavailDates = null)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            DateTime? value = DateTime.Now;
            if (metaData.Model is DateTime? || metaData.Model is DateTime)
            {
                value = (DateTime?)metaData.Model;
            }
            else
            {
                value = null;
            }

            return html.DatePicker(metaData.PropertyName, value, minDate, maxDate, formatDate, unavailDates);
        }

        #endregion Date Picker

        #region Year Picker

        /// <summary>
        /// The years picker.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="name">The name value.</param>
        /// <param name="fromYear">From year.</param>
        /// <param name="currentYear">The current year.</param>
        /// <returns>Years picker.</returns>
        public static MvcHtmlString YearPicker(
            this HtmlHelper htmlHelper,
            string name,
            int fromYear = 2000,
            int currentYear = 2000)
        {
            var list = new List<SelectListItem>();
            var endYear = DateTime.Now.Year;

            for (var i = endYear; i >= fromYear; i--)
            {
                list.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i == currentYear
                });
            }

            var dropDownList = htmlHelper.DropDownList(name, list, new { @class = "yearpicker" });
            return MvcHtmlString.Create(dropDownList.ToString());
        }

        public static MvcHtmlString YearPickerFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            int fromYear = 2000)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var currentYear = (int)metaData.Model;
            return html.YearPicker(metaData.PropertyName, fromYear, currentYear);
        }

        #endregion Year Picker

        #region CheckBoxList

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name)
        {
            return CheckBoxList(htmlHelper, name, null);
        }

        public static MvcHtmlString CheckBoxList(
            this HtmlHelper htmlHelper,
            string name,
            IEnumerable<SelectListItem> selectList)
        {
            return CheckBoxList(htmlHelper, name, selectList, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString CheckBoxList(
            this HtmlHelper htmlHelper,
            string name,
            IEnumerable<SelectListItem> selectList,
            object htmlAttributes)
        {
            return CheckBoxList(htmlHelper, name, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return CheckBoxListFor(htmlHelper, expression, null);
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList)
        {
            return CheckBoxListFor(htmlHelper, expression, selectList, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            object htmlAttributes)
        {
            return CheckBoxListFor(
                htmlHelper,
                expression,
                selectList,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// The checkbox list for.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="selectList">The select list.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns>The checkbox list.</returns>
        /// <exception cref="System.ArgumentNullException">Argument Null Exception.</exception>
        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return CheckBoxList(
                htmlHelper,
                ExpressionHelper.GetExpressionText(expression),
                selectList,
                htmlAttributes);
        }

        #endregion CheckBoxList

        #region Dropdownlist

        /// <summary>
        /// Drops down list for.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="selectList">The select list.</param>
        /// <param name="firstItem">The first item.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns>Drops down list.</returns>
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
           Expression<Func<TModel, TProperty>> expression,
           IEnumerable<SelectListItem> selectList,
           SelectListItem firstItem,
           object htmlAttributes)
        {
            if (firstItem != null && firstItem.Value != null)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(firstItem);

                if (selectList != null)
                {
                    foreach (SelectListItem item in selectList)
                    {
                        list.Add(item);
                    }
                }

                selectList = list;
            }

            return SelectExtensions.DropDownListFor(htmlHelper, expression, selectList, htmlAttributes);
        }

        #endregion Dropdownlist
    }
}