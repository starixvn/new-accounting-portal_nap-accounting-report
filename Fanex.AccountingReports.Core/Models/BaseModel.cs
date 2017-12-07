namespace Fanex.AccountingReports.Core.Models
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class BaseModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string AliasName { get; set; }
    }
}
