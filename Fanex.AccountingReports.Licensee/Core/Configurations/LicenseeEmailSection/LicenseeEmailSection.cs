namespace Fanex.AccountingReports.Licensee.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LicenseeEmailSection : ConfigurationSection
    {
        public static LicenseeEmailSection Instance
        {
            get
            {
                var storeProcedureSection =
                 ConfigurationManager.GetSection("LicenseeEmailSection") as LicenseeEmailSection;
                return storeProcedureSection;
            }
        }

        [ConfigurationProperty("mails", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MailCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public MailCollection Mails
        {
            get
            {
                var storeProcedureCollection =
                    (MailCollection)base["mails"];
                return storeProcedureCollection;
            }
        }
    }
}
