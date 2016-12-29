// --------------------------------------------------------------------------
// <copyright file="UserController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Websites.Areas.UserControl.Controllers
{
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.Common;
    using Fanex.Logging;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web;
    using Models;
    using NAP.Core.Configuration;
    using NAP.Core.Models;
    using NAP.Core.Models.ParameterObject;
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UserController : BaseController
    {
        // GET: /UserControl/User/
        private readonly IUserService _userService;

        private readonly ISecurityService securityService;

        public UserController()
            : this(null)
        {
        }

        public UserController(IUserService userService)
        {
            _userService = userService ?? new UserService();
            securityService = new SecurityService();
        }

        #region Change Password

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View(new UpdatePasswordModel());
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Changes the password view.</returns>
        [HttpPost]
        public ActionResult ChangePassword(UpdatePasswordModel criteria)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _userService.Get(AppSession.CurrentUser.Id);
                    string oldPwd = Cryptography.CfsEnCode(criteria.OldUserPwd);

                    if (oldPwd.Equals(user.Password))
                    {
                        var password = Cryptography.CfsEnCode(criteria.UserPwd);

                        var result = securityService.UpdatePassword(NapSettings.Settings.SiteId, user.Id, password);
                        ViewBag.Success = result;
                        ViewBag.Message = result ? string.Empty : "Sorry, an error occurred while processing your request, please notify the administrators.";
                    }
                    else
                    {
                        ViewBag.Message = ViewResources.Wrong_password;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
                throw;
            }

            return View(criteria);
        }

        #endregion

        #region Profile

        /// <summary>
        /// The User profile.
        /// </summary>
        /// <returns>The User profile page.</returns>
        [HttpGet]
        public ActionResult UserProfile()
        {
            try
            {
                var user = _userService.Get(AppSession.CurrentUser.Id);

                return View(user);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
                throw;
            }


        }

        /// <summary>
        /// The User profile.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The User profile page.</returns>
        [HttpPost]
        public ActionResult UserProfile(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var oldUser = _userService.Get(CurrentSession.UserId);
                    var parameter = new UpdateUserInfoParameter
                    {
                        CustId = oldUser.Id,
                        Username = oldUser.Username,
                        FirstName = user.FirstName ?? oldUser.FirstName,
                        LastName = user.LastName ?? oldUser.LastName,
                        Email = user.Email ?? oldUser.Email,
                        GroupId = oldUser.Group == null ? 0 : oldUser.Group.Id
                    };

                    var result = securityService.UpdateProfile(this.SiteId, parameter);

                    ViewBag.Success = result;
                    ViewBag.Message = result ? string.Empty : "Sorry, an error occurred while processing your request, please notify the administrators.";
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
                throw;
            }

            return View(user);
        }

        #endregion
    }
}
