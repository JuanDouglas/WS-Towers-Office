using ShowProducts.API.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WSTowersOffice.Api.Models.Exceptions;
using static ShowProducts.API.Controllers.LoginController;

namespace WSTowersOffice.Api.Models.Attributes
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    sealed class LoginRequiredAttribute : ValidationAttribute, IClientValidatable
    {
        private string ErrorMessageLogin { get; set; }
        public LoginRequiredAttribute()
        {

        }
        public override bool IsValid(object value)
        {
            try
            {
                LoginInformations loginInformations = LoginController.ValidLogin();
                return loginInformations.IsValid;
            }
            catch (AuthenticationException e)
            {
                ErrorMessageLogin = e.Message;
                return false;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule()
            {
                ValidationType = "AuthenticationError",
                ErrorMessage = ErrorMessageLogin
            };
        }
    }
}