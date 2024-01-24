namespace Fanex.AccountingReports.Core.Models
{
    using System;

    [Serializable]
    public class MostVisitedPage
    {
        public MostVisitedPage()
        {
            Count = 1;
        }

        public string Id { get; set; }

        public int UserId { get; set; }

        public int MenuId { get; set; }

        public string UserName { get; set; }

        public DateTime LastVisitDay { get; set; }

        public string MenuName { get; set; }

        public string MenuURL { get; set; }

        public string Title { get; set; }

        public ulong Count { get; set; }

        public bool IsPinned { get; set; }

        public int Position { get; set; }
    }
}
