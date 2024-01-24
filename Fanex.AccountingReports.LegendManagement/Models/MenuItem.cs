namespace Fanex.AccountingReports.LegendManagement.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class MenuItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Href { get; set; }

        public bool IsOpenNewWindow { get; set; }

        public int ParentId { get; set; }

        public string FunctionName { get; set; }

        public string Target { get; set; }

        public bool IsHide { get; set; }

        public int Order { get; set; }

        public int Level { get; set; }

        public string System { get; set; }

        public List<MenuItem> SubMenu { get; set; }
    }
}
