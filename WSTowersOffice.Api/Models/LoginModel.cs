using System;
using System.ComponentModel.DataAnnotations;

namespace WSTowersOffice.Api.Models
{
    /// <summary>
    /// Classe Modelo de Logins.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// ID do login.
        /// </summary>
        internal int ID { get; set; }

        /// <summary>
        /// Nome de usuário usado para logar.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Nome de Usuário")]
        [StringLength(maximumLength: 100, MinimumLength = 3, ErrorMessage = "O texto do campo {0} está fora dos limites de caracteres ({1},{2}). ")]
        public string UserName { get; set; }
        /// <summary>
        /// Senha de usuário usada no Login.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Senha")]
        [StringLength(maximumLength: 200, MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Confirme sua Senha")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Email do usuário.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [EmailAddress]
        [StringLength(maximumLength: 300, MinimumLength = 3)]
        public string Email { get; set; }

        /// <summary>
        /// Data que o login foi criado.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Indica se a conta já teve o Email validado.
        /// </summary>
        public bool VerifiedAccount { get; internal set; }

        /// <summary>
        /// Construtor Básico.
        /// </summary>
        /// <param name="userName">Nome de usuário.</param>
        /// <param name="password">Senha de usuário.</param>
        /// <param name="email">Endereço de Email.</param>
        public LoginModel(string userName, string password, string email)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(password));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }

        /// <summary>
        /// Construtor a partir do Entity FreameWork 
        /// </summary>
        /// <param name="login">Classe de Login.</param>
        public LoginModel(Login login)
        {
            ID = login.ID;
            UserName = login.UserName;
            Password = login.Password;
            VerifiedAccount = login.isValidEmail;
            Email = login.Email;
        }

        public LoginModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Login GetLogin()
        {
            Login lg = new Login();
            lg.UserName = UserName;
            lg.isValidEmail = VerifiedAccount;
            lg.Password = Password;
            lg.Email = Email;

            return lg;
        }
    }
}