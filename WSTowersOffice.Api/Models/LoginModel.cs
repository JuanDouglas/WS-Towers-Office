using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSTowersOffice.Api.Models
{
    public class LoginModel
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool isValidEmail { get; set; }
        public DateTime CreateDate { get; set; }
        public string ValidKey { get; set; }
        public LoginModel(Login login)
        {
            ID = login.ID;
            UserName = login.UserName;
            Email = login.Email;
            isValidEmail = login.isValidEmail;
            CreateDate = login.CreateDate;
            ValidKey = login.ValidKey;
        }

        internal Login GetLogin()
        {
            throw new NotImplementedException();
        }
    }
}