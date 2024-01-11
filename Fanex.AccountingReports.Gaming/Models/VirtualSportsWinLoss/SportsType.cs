namespace Fanex.AccountingReports.Gaming.Models.VirtualSportsWinLoss
{
    using System;

    [Serializable]
    public class SportsType
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }
    }
}
