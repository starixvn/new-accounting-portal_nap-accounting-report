// -----------------------------------------------------------------------
// <copyright file="SportsType.cs" company="Nexcel Solutiond Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Models.VirtualSportsWinLoss
{
    using System;

    /// <summary>
    /// The model of the sports type.
    /// </summary>
    [Serializable]
    public class SportsType
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }
    }
}
