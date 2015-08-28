// --------------------------------------------------------------------------
// <copyright file="BetTeam.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Helper.SportHelper
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class BetTeam
    {
        /// <summary>
        /// Gets the bet team.
        /// </summary>
        /// <param name="betTeam">The bet team.</param>
        /// <param name="betType">Type of the bet.</param>
        /// <param name="language">The language.</param>
        /// <returns>The bet team string.</returns>
        public static string GetBetTeam(string betTeam, int betType, string language)
        {
            string rs = betTeam;
            if (string.IsNullOrEmpty(language))
            {
                language = "e";
            }

            string lang = language.ToLower();
            if (lang.Equals("c"))
            {
                lang = "cs";
            }

            if (string.IsNullOrEmpty(betTeam))
            {
                return rs;
            }

            if (betTeam.Length < 2)
            {
                return rs;
            }

            if (betType == 14)
            {
                ////FGLG
                string ha, fglg;
                ha = betTeam.Substring(0, 1);
                fglg = betTeam.Substring(betTeam.Length - 1, 1);
                string strHA, strFGLG;
                strHA = GetHA(ha, lang);
                strFGLG = GetFGLG(fglg, lang);
                rs = strHA + strFGLG;
            }
            else if (betType == 16)
            {
                ////HTFT
                string home, away;
                home = betTeam.Substring(0, 1);
                away = betTeam.Substring(betTeam.Length - 1, 1);
                string strHome, strAway;
                strHome = GetHome(home, lang);
                strAway = GetAway(away, lang);
                rs = strHome + strAway;
            }

            return rs;
        }

        /// <summary>
        /// Gets the ha.
        /// </summary>
        /// <param name="ha">The home or away bet on.</param>
        /// <param name="lang">The language.</param>
        /// <returns>The ha string.</returns>
        private static string GetHA(string ha, string lang)
        {
            string rs = string.Empty;
            switch (ha)
            {
                case "1":
                    switch (lang)
                    {
                        case "cs":
                            rs = "主";
                            break;
                        default:
                            rs = "Home";
                            break;
                    }

                    break;
                case "2":
                    switch (lang)
                    {
                        case "cs":
                            rs = "客";
                            break;
                        default:
                            rs = "Away";
                            break;
                    }

                    break;
                default:
                    switch (lang)
                    {
                        case "cs":
                            rs = "無";
                            break;
                        default:
                            rs = "No";
                            break;
                    }

                    break;
            }

            return rs;
        }

        /// <summary>
        /// Gets the FGLG.
        /// </summary>
        /// <param name="ha">The home or away bet on.</param>
        /// <param name="lang">The language.</param>
        /// <returns>The FGLG string.</returns>
        private static string GetFGLG(string ha, string lang)
        {
            string rs = string.Empty;
            switch (ha)
            {
                case "1":
                    switch (lang)
                    {
                        case "cs":
                            rs = "先";
                            break;
                        default:
                            rs = " FG";
                            break;
                    }

                    break;
                case "2":
                    switch (lang)
                    {
                        case "cs":
                            rs = "後";
                            break;
                        default:
                            rs = " LG";
                            break;
                    }

                    break;
                default:
                    switch (lang)
                    {
                        case "cs":
                            rs = "進球";
                            break;
                        default:
                            rs = " Goal";
                            break;
                    }

                    break;
            }

            return rs;
        }

        /// <summary>
        /// Gets the home.
        /// </summary>
        /// <param name="ha">The home or away bet on.</param>
        /// <param name="lang">The language.</param>
        /// <returns>The home string.</returns>
        private static string GetHome(string ha, string lang)
        {
            string rs = string.Empty;
            switch (ha)
            {
                case "1":
                    switch (lang)
                    {
                        case "cs":
                            rs = "主";
                            break;
                        default:
                            rs = "H";
                            break;
                    }

                    break;
                case "2":
                    switch (lang)
                    {
                        case "cs":
                            rs = "客";
                            break;
                        default:
                            rs = "A";
                            break;
                    }

                    break;
                default:
                    switch (lang)
                    {
                        case "cs":
                            rs = "和";
                            break;
                        default:
                            rs = "D";
                            break;
                    }

                    break;
            }

            return rs;
        }

        /// <summary>
        /// Gets the away.
        /// </summary>
        /// <param name="ha">The home or away bet on.</param>
        /// <param name="lang">The language.</param>
        /// <returns>The away string.</returns>
        private static string GetAway(string ha, string lang)
        {
            string rs = string.Empty;
            switch (ha)
            {
                case "1":
                    switch (lang)
                    {
                        case "cs":
                            rs = "主";
                            break;
                        default:
                            rs = "H";
                            break;
                    }

                    break;
                case "2":
                    switch (lang)
                    {
                        case "cs":
                            rs = "客";
                            break;
                        default:
                            rs = "A";
                            break;
                    }

                    break;
                default:
                    switch (lang)
                    {
                        case "cs":
                            rs = "和";
                            break;
                        default:
                            rs = "D";
                            break;
                    }

                    break;
            }

            return rs;
        }
    }
}
