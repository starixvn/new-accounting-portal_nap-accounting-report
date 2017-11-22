namespace Fanex.BetList.Accounting.Extensions
{
    using Fanex.BetList.Accounting.Models;
    using Fanex.BetList.Accounting.Resources;
    using Fanex.BetList.Core.Builder;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class BetListExtension
    {

        /// <summary>
        /// Builds the stake.
        /// </summary>
        /// <param name="betDetail">The bet detail.</param>
        /// <param name="builder">The builder.</param>
        /// <returns>The stake.</returns>
        public static string BuildStake(this TicketDetail betDetail, ref BetListHTMLBuilder builder)
        {
            string stakeBuilder = string.Empty;
            stakeBuilder = builder.BuildStake(betDetail.Ticket);
            if (betDetail.IsShowOtExRate)
            {
                stakeBuilder +=
                    "<br />" +
                    "<span class=\"r exrate2\">" + betDetail.VirtualRate + "</span>" +
                    "<br />" +
                    "<span class=\"r exrate2\">(" + Labels.Ex_Rate_2 + ")</span>";
            }

            return stakeBuilder;
        }
    }
}
