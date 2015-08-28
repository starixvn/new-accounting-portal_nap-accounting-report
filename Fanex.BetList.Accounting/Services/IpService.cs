// --------------------------------------------------------------------------
// <copyright file="IpService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.12.26">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text.RegularExpressions;
    using System.Web;
    using Fanex.BetList.Accounting.Constants;
    using Fanex.BetList.Accounting.Models;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class IpService
    {
        private List<IPVNBean> _listVNIP;

        public IpService()
        {
        }

        public IpService(List<IPVNBean> listVnIp)
        {
            this._listVNIP = listVnIp;
        }

        /// <summary>
        /// Replaces the VNIP.
        /// </summary>
        /// <param name="ip">The IP address.</param>
        /// <returns>Replaced the VNIP.</returns>
        public static string ReplaceVNIP(string ip)
        {
            var ipvnReplace = Converter.ToString2(ConfigurationManager.AppSettings["ipvnreplace"]);
            if (IsVNIP(ip))
            {
                if (Converter.ToBool(HttpContext.Current.Session[CommonConstants.ViewIPVN]) ||
                    ipvnReplace == "1")
                {
                    return ip;
                }

                return ipvnReplace;
            }

            return ip;
        }

        /// <summary>
        /// Determines whether the specified IP/IS/VNIP.
        /// </summary>
        /// <param name="ip">The IP address.</param>
        /// <returns>Is VN IP or not.</returns>
        public static bool IsVNIP(string ip)
        {
            try
            {
                double ipNumber = 0;
                ipNumber = IP2Number(ip);
                List<IPVNBean> listIPVN = ReportService.IPVN;
                int total = listIPVN.Count;
                for (int i = 0; i < total; i++)
                {
                    IPVNBean item = listIPVN[i];
                    if (item == null)
                    {
                        return false;
                    }

                    if (item.IPFrom <= ipNumber && ipNumber <= item.IPTo)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
            }

            return false;
        }

        /// <summary>
        /// Is the p2 number.
        /// </summary>
        /// <param name="ip">The IP address.</param>
        /// <returns>The p2 number.</returns>
        public static double IP2Number(string ip)
        {
            try
            {
                double longIP = 0;
                if (IsValidIP(ip))
                {
                    string[] sIP = ip.Split('.');
                    longIP =
                        (double.Parse(sIP[0]) * Math.Pow(256, 3))
                        + (double.Parse(sIP[1]) * Math.Pow(256, 2))
                        + (double.Parse(sIP[2]) * 256)
                        + double.Parse(sIP[3]);
                }

                return longIP;
            }
            catch (Exception)
            {
            }

            return 0.0;
        }

        /// <summary>
        /// Determines whether [is valid IP] [the specified IP address].
        /// </summary>
        /// <param name="ipAddress">The IP address.</param>
        /// <returns>Is valid IP.</returns>
        public static bool IsValidIP(string ipAddress)
        {
            string pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])" +
                @"(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
            var check = new Regex(pattern);
            if (string.IsNullOrEmpty(ipAddress))
            {
                return false;
            }

            return check.IsMatch(ipAddress, 0);
        }
    }
}