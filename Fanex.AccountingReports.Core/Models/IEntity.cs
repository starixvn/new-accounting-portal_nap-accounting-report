namespace Fanex.AccountingReports.Core.Models
{
    public interface IEntity<TId, TName>
    {
        TId Id { get; set; }
        TName Name { get; set; }
    }
}
