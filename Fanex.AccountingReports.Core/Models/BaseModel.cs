namespace Fanex.AccountingReports.Core.Models
{
    using System;

    [Serializable]
    public class BaseModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string AliasName { get; set; }
    }
}
