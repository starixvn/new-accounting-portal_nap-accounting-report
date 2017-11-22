namespace Fanex.AccountingReports.Special.Models.SpecialMatchAnalysis
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>    
    [Serializable]
    public class BaseModel
    {
        public BaseModel()
        {
            Name = string.Empty;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
