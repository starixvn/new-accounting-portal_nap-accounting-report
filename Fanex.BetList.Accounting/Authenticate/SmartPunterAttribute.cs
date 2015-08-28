// -----------------------------------------------------------------------
// <copyright file="SmartPunterAttribute.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.BetList.Accounting.Authenticate
{
    using System.Collections;
    using Fanex.AuthenticationFilter;
    using Fanex.BetList.Accounting.Services;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SmartPunterAttribute : IAuthenticate
    {
        /// <summary>
        /// The _permission service.
        /// </summary>
        private readonly ICheckSmartPunterService _smartPunterService = new CheckSmartPunterService();

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartPunterAttribute"/> class.
        /// </summary>
        public SmartPunterAttribute()
        {
        }

        /// <summary>
        /// Gets or sets the policy.
        /// </summary>
        /// <value>The policy.</value>
        public AuthenticateSmartPunterAction.AuthenticationType Policy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the policy parameters.
        /// </summary>
        /// <value>The hash table of parameters.</value>
        public Hashtable ParamHastable
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Allows the read.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool AllowRead()
        {
            if (this.ParamHastable.ContainsKey("custId"))
            {
                int custId = Converter.To<int>(this.ParamHastable["custId"]);
                return !_smartPunterService.IsDeniedViewSmartPunter(custId, string.Empty);
            }

            var username = Converter.ToString2(this.ParamHastable["username"]);
            return !_smartPunterService.IsDeniedViewSmartPunter(null, username);
        }

        /// <summary>
        /// Allows the update.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool AllowUpdate()
        {
            return true;
        }
    }
}
