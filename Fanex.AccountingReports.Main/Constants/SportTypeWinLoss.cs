namespace Fanex.AccountingReports.Main.Common
{
    public enum SportTypeWinLoss : int
    {
        /// <summary>
        /// The unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The sport book.
        /// </summary>
        SportBook,

        /// <summary>
        /// The racing.
        /// </summary>
        Racing,

        /// <summary>
        /// The virtual sports.
        /// </summary>
        VirtualSports,

        /// <summary>
        /// The virtual racing.
        /// </summary>
        VirtualRacing,

        /// <summary>
        /// The number game.
        /// </summary>
        NumberGame, // special sport of SportBook

        Bullfighting
    }
}
