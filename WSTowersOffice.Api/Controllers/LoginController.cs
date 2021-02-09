using Newtonsoft.Json;
using WSTowersOffice.Api.Models;
using WSTowersOffice.Api.Models.Enums;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WSTowersOffice.Api.Models.Exceptions;

namespace ShowProducts.API.Controllers
{
    /// <summary>
    /// Api Login Controller.
    /// </summary>
    [RoutePrefix("api/Login")]
    public class LoginController : ApiController
    {
        private WSTowersOfficeEntities db = new WSTowersOfficeEntities();

        /// <summary>
        /// GET Login Informations
        /// </summary>
        /// <param name="user">Nome de usuário</param>
        /// <param name="token">Token de login</param>
        /// <returns>
        /// IHttpActionResult
        /// </returns>
        #region GetLogin Informations
        [HttpGet]
        [Route("Informations")]
        public async Task<IHttpActionResult> GetAsync(string token)
        {

            //Obtém o contexto da solicitação 
            HttpRequest context = HttpContext.Current.Request;

            //Obtém a autenticação deste usuário
            Authentication logToken = await db.Authentication.Where(fs => fs.IP == context.UserHostAddress).FirstOrDefaultAsync(fs => fs.Token == token.ToString());

            //Obtém o usuário pelo nome de usuário
            Login logUser = await db.Login.FirstOrDefaultAsync(fs => fs.UserName == logToken.Login1.UserName);

            //Verifica se os objetos retornados são nulos e caso sejam retorna um 'NotFound'
            if (logUser == null)
            {
                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("Read this request headers")
                };
                response.Headers.Add("X-Error", "The user name did not return any results.");
                return ResponseMessage(response);
            }

            if (logToken == null)
            {
                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("Read the headers of this request")
                };
                response.Headers.Add("X-Error", "The login token did not return any results.");
                return ResponseMessage(response);
            }

            //Caso o ban esteja ativado irá verificar se já foi banido
            //if (GetConfigurations.ActiveBans)
            //{
            //    Ban ban = await db.Bans.Where(fs=>fs.BlackList.IP==context.UserHostAddress).
            //            FirstOrDefaultAsync(fs=>fs.AppliedDate.Add(fs.Time)<GetConfigurations.Now);
            //    if (ban != null)
            //    {
            //        HttpResponseMessage response = new HttpResponseMessage {
            //            StatusCode = HttpStatusCode.Unauthorized,
            //            Content = new StringContent(
            //            JsonConvert.SerializeObject(
            //                new BlackListProfile(ban)))
            //        };
            //        response.Headers.Add("X-Error", "You are banned!");
            //        return ResponseMessage(response);
            //    }
            //}

            //Verifica se o token corresponde ao 'UserName'
            if (logToken.Login1.UserName != logUser.UserName)
            {
                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Content = new StringContent("Read the headers of this request")
                };
                response.Headers.Add("X-Error", "Some requisition fields return different values.");
                response.Headers.Add("Conflict-Field", "user = UserName");
                response.Headers.Add("Conflicting-Field", "token = Authentication Token");
                return ResponseMessage(response);
            }
            var model = new LoginModel(logUser);
            model.Password = "********";
            //Retorna o login 
            return Ok(model);
        }
        #endregion

        #region Authentication
        [HttpGet]
        [Route("Authetication/Logout")]
        public async Task<IHttpActionResult> GetAsync(bool isWeb)
        {

            UriBuilder builder = new UriBuilder(Request.RequestUri)
            {
                Path = "api/Login/Authetication/Logout",
                Query = $"isWeb={isWeb}&thisAdress=false&ipAdress={HttpContext.Current.Request.UserHostAddress}"
            };

            return Redirect(builder.ToString());
        }
        /// <summary>
        /// Authentication Module 
        /// </summary>
        /// <param name="user">User Name</param>
        /// <param name="pas">Password</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("Authetication/Logout")]
        public async Task<IHttpActionResult> GetAsync(bool? isWeb, bool thisAdress, string ipAdress)
        {
            LoginInformations validResult = await ValidLoginAsync();
            if (!validResult.IsValid)
            {
                return Unauthorized();
            }

            Authentication authentication = null;
            if (thisAdress)
            {
                authentication = await db.Authentication.FirstOrDefaultAsync(fs => fs.IP == HttpContext.Current.Request.UserHostAddress);
            }
            else
            {
                if (ipAdress == null)
                {
                    return BadRequest();
                }
                authentication = await db.Authentication.FirstOrDefaultAsync(fs => fs.IP == ipAdress);
            }
            if (authentication == null)
            {
                return NotFound();
            }
            db.Entry(authentication).State = EntityState.Deleted;
            await db.SaveChangesAsync();
            if (isWeb.Value)
            {
                string url_post = string.Empty;

                UriBuilder builder = new UriBuilder(Request.RequestUri)
                {
                    Path = "Home/Index",
                    Query = string.Empty
                };
                url_post = builder.ToString();


                return Redirect(url_post);
            }

            return Ok(new Authenticated(authentication));
        }
        /// <summary>
        /// Authentication Module 
        /// </summary>
        /// <param name="user">User Name</param>
        /// <param name="pas">Password</param>
        /// <returns>IHttpActionResult</returns>
        //[HttpGet]
        //[Route("Authetication")]
        //public async Task<IHttpActionResult> GetAsync()
        //{
        //    bool isWeb = RequestContext.IsLocal;
        //    if (!isWeb)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        UriBuilder builder = new UriBuilder(Request.RequestUri)
        //        {
        //            Path = "Logins/AuthenticationMessage",
        //            Query = $"isError={true}&title={"Ei!"}&message={"Vamos lá tente digitar um pouco nos campos 'Email' e 'Senha' sem isso não podemos identificar-lo."}"
        //        };
        //        return Redirect(builder.ToString());
        //    }
        //}
        ///// Authentication Module 
        ///// </summary>
        ///// <param name="user">User Name</param>
        ///// <param name="pas">Password</param>
        ///// <returns>IHttpActionResult</returns>
        //[HttpGet]
        //[Route("Authetication")]
        //public async Task<IHttpActionResult> GetAsync(string user, string pas)
        //{

        //    UriBuilder builder = new UriBuilder(Request.RequestUri)
        //    {
        //        Path = "api/Login/Authetication",
        //        Query = $"user={user}&pas={pas}&viewpage={false}&post={"home"}"
        //    };
        //    return Redirect(builder.ToString());


        //}
        /// Authentication Module 
        /// </summary>
        /// <param name="user">User Name</param>
        /// <param name="pas">Password</param>
        /// <returns>IHttpActionResult</returns>

        //public async Task<IHttpActionResult> GetAsync(string user, string pas, bool viewpage)
        //{

        //    UriBuilder builder = new UriBuilder(Request.RequestUri)
        //    {
        //        Path = "api/Login/Authetication",
        //        Query = $"user={user}&pas={pas}&viewpage={viewpage}&post={"home"}"
        //    };
        //    return Redirect(builder.ToString());

        //}
        //[HttpGet]
        //[ResponseType(typeof(LoginCodeModel))]
        //[Route("Authetication/LoginCode")]
        //public async Task<IHttpActionResult> GetLoginCodeAsync()
        //{
        //    HttpRequest context = HttpContext.Current.Request;
        //    var existIP = await db.IP.FirstOrDefaultAsync(fs => fs.IP1 == context.UserHostAddress);
        //    if (existIP == null)
        //    {
        //        existIP = new IP
        //        {
        //            IP1 = context.UserHostAddress,
        //            AlreadyBeenBanned = false,
        //            Confiance = 2
        //        };
        //        db.IP.Add(existIP);
        //        await db.SaveChangesAsync();
        //    }

        //    #region ExistCodeToken
        //    Guid token;
        //    int code;
        //    Random rd = new Random();
        //    bool exist = true;
        //    do
        //    {
        //        token = Guid.NewGuid();
        //        Authorization existAutorization = await db.Authorization.FirstOrDefaultAsync(fs => fs.ValidToken == token.ToString());
        //        if (existAutorization == null)
        //        {
        //            exist = false;
        //        }
        //    } while (exist);

        //    exist = true;
        //    do
        //    {
        //        code = rd.Next(10000, 99999);
        //        Authorization existAutorization = await db.Authorization.FirstOrDefaultAsync(fs => fs.Code == code);
        //        if (existAutorization == null)
        //        {
        //            exist = false;
        //        }
        //    } while (exist);
        //    #endregion

        //    db.Authorization.Add(new Authorization()
        //    {
        //        CreateDate = DateTime.Now,
        //        ValidToken = token.ToString(),
        //        Code = code,
        //        IP = context.UserHostAddress,
        //        LastUpdateDate = DateTime.Now
        //    });
        //    await db.SaveChangesAsync();

        //    Authorization autorization = await db.Authorization.FirstOrDefaultAsync(fs => fs.ValidToken == token.ToString());
        //    return Ok(new LoginCodeModel(autorization));

        //}
        //[HttpGet]
        //[ResponseType(typeof(AuthorizationStatus))]
        //[Route("Authetication/AutorizationStatus")]
        //public async Task<IHttpActionResult> GetAutorizationStatusAsync(string token, int code)
        //{
        //    Authorization autorization = await db.Authorization.FirstOrDefaultAsync(fs => fs.ValidToken == token && fs.Code == code);
        //    if (autorization == null)
        //    {
        //        return NotFound();
        //    }
        //    if (autorization.Authentication != null)
        //    {
        //        Provider provider = (await db.Login.FirstOrDefaultAsync(fs => fs.ID == autorization.Authentication.Login)).Provider.FirstOrDefault(fs => true);
        //        return Ok(new AuthorizationStatus()
        //        {
        //            Logued = true,
        //            LoginToken = autorization.Authentication.Token,
        //            UserKey = provider.ValidKey,
        //            Status = AuthorizationStatusCode.Autorized
        //        });
        //    }
        //    if (DateTime.Now > autorization.LastUpdateDate.AddMinutes(5))
        //    {
        //        Random rd = new Random();
        //        bool exist = true;
        //        do
        //        {
        //            code = rd.Next(10000, 99999);
        //            Authorization existAutorization = await db.Authorization.FirstOrDefaultAsync(fs => fs.Code == code);
        //            if (existAutorization == null)
        //            {
        //                exist = false;
        //            }
        //        } while (exist);
        //        autorization.Code = code;
        //        autorization.LastUpdateDate = DateTime.Now;
        //        db.Entry(autorization).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        AuthorizationStatus autorizationStatus = new AuthorizationStatus()
        //        {
        //            NewCode = code,
        //            Logued = false,
        //            Status = AuthorizationStatusCode.NewCode
        //        };
        //        return Ok(autorizationStatus);
        //    }

        //    return Ok(new AuthorizationStatus()
        //    {
        //        Logued = false,
        //        Status = AuthorizationStatusCode.AwaitingAuthetication,
        //    });
        //}
        //[HttpPost]
        //[ResponseType(typeof(AuthorizationStatus))]
        //[Route("Authetication/AuthorizeAuthentication")]
        //public async Task<IHttpActionResult> GetAuthorizeAuthenticationAsync(int code, bool? isweb, string post)
        //{
        //    LoginInformations loginInformations = await ValidLoginAsync();
        //    if (!loginInformations.IsValid)
        //    {
        //        if (isweb.Value)
        //        {
        //            var parseQuery = HttpUtility.ParseQueryString(string.Empty);
        //            parseQuery["post"] = Request.RequestUri.AbsoluteUri;
        //            UriBuilder builder = new UriBuilder(Request.RequestUri.AbsoluteUri)
        //            {
        //                Path = "Logins/Authentication",
        //                Query = parseQuery.ToString()
        //            };

        //            return Redirect(builder.ToString());
        //        }
        //        return Unauthorized();
        //    }
        //    Authentication thisAuthentication = await db.Authentication.FirstOrDefaultAsync(fs => fs.Token == loginInformations.LoginToken);
        //    Authorization authorization = await db.Authorization.FirstOrDefaultAsync(fs => fs.Code == code);
        //    Guid token;
        //    bool exist = true;
        //    do
        //    {
        //        token = Guid.NewGuid();
        //        Authentication existAutorization = await db.Authentication.FirstOrDefaultAsync(fs => fs.Token == token.ToString());
        //        if (existAutorization == null)
        //        {
        //            exist = false;
        //        }
        //    } while (exist);
        //    db.Authentication.Add(new Authentication()
        //    {
        //        Date = DateTime.Now,
        //        Login = thisAuthentication.Login,
        //        User_Agent = HttpContext.Current.Request.UserHostAddress,
        //        IP = authorization.IP,
        //        Token = token.ToString()
        //    });

        //    await db.SaveChangesAsync();

        //    thisAuthentication = await db.Authentication.FirstOrDefaultAsync(fs => fs.Token == token.ToString());
        //    authorization.AuthenticationID = thisAuthentication.ID;
        //    db.Entry(authorization).State = EntityState.Modified;


        //    await db.SaveChangesAsync();
        //    if (isweb.Value)
        //    {
        //        return Redirect(post);
        //    }
        //    return Ok();
        //}

        [HttpGet]
        [Route("Authentication")]
        public async Task<IHttpActionResult> Get(string user, string pas, bool viewpage)
        {
            //obtém o contexto desta requisição 
            HttpRequest context = HttpContext.Current.Request;

            string post = "home";
            //Obtém o login no banco de dados
            Login result = await db.Login.FirstOrDefaultAsync(fs => fs.UserName == user);
            if (result == null)
            {//Obtém o login no banco de dados
                result = await db.Login.FirstOrDefaultAsync(fs => fs.Email == user);
            }
            //Determina se a requisição veio deste servidor
            bool isWeb = viewpage;
            //Verificar se o usuário existe.
            if (result != null)
            {
                //Caso o ban esteja ativado irá verificar se já foi banido
                //if (GetConfigurations.ActiveBans)
                //{
                //    Ban ban = await db.Bans.Where(fs=>fs.BlackList.IP==context.UserHostAddress).
                //        FirstOrDefaultAsync(fs=>fs.AppliedDate.Add(fs.Time)<GetConfigurations.Now);
                //    if (ban != null)
                //    {
                //        HttpResponseMessage response = new HttpResponseMessage();
                //        response.StatusCode = HttpStatusCode.Unauthorized;
                //        response.Content = new StringContent(
                //            JsonConvert.SerializeObject(
                //                new BlackListProfile(ban)));
                //        response.Headers.Add("X-Error", "You are banned!");
                //        return ResponseMessage(response);
                //    }
                //}

                //Verifica se a senha está correta
                if (!PasswordCompare(result.Password, pas))
                {
                    if (!isWeb)
                    {
                        //Retorna um 'Unauthorized' caso a senha esteja errada 'UserName'
                        return Unauthorized();
                    }
                    else
                    {
                        var query = HttpUtility.ParseQueryString(string.Empty);
                        query["isError"] = true.ToString();
                        query["title"] = "Hummmm!";
                        query["post"] = post;
                        query["message"] = "Parece que você está digitando com pressa, digite um pouco mais devagar...";
                        UriBuilder builder = new UriBuilder(Request.RequestUri)
                        {
                            Path = "Logins/AuthenticationMessage",
                            Query = query.ToString()
                        };
                        return Redirect(builder.ToString());
                    }
                }

                //Cria o Authenticate object
                Authentication authentication;
                try
                {
                    string token = Guid.NewGuid().ToString();
                    //Verifica se ele já existe e alterar
                    authentication = await db.Authentication.Where(fs => fs.Login1.ID == result.ID).
                    FirstOrDefaultAsync(fs => fs.IP == context.UserHostAddress);

                    db.Authentication.Remove(authentication);
                    await db.SaveChangesAsync();

                    authentication.Token = CryptographyString(token);
                    authentication.User_Agent = context.UserAgent;
                    authentication.Login = result.ID;

                    db.Authentication.Add(authentication);
                    await db.SaveChangesAsync();

                    authentication.Token = token;
                    if (authentication == null)
                    {
                        throw new ArgumentNullException();
                    }
                }
                catch (ArgumentNullException)
                {
                    string token = Guid.NewGuid().ToString();
                    //Cria um novo e adiona no banco caso não exista
                    authentication = new Authentication()
                    {
                        Date = DateTime.Now,
                        IP = context.UserHostAddress,
                        Token = CryptographyString(token),
                        User_Agent = context.UserAgent,
                        Login = result.ID
                    };
                    var existIP = await db.IP.FirstOrDefaultAsync(fs => fs.IP1 == context.UserHostAddress);
                    if (existIP == null)
                    {
                        existIP = new IP
                        {
                            IP1 = context.UserHostAddress,
                            AlreadyBeenBanned = false,
                            Confiance = 2
                        };
                        db.IP.Add(existIP);
                        await db.SaveChangesAsync();
                    }
                    db.Authentication.Add(authentication);
                    await db.SaveChangesAsync();
                    authentication.Token = token;
                }

                if (!isWeb)
                {
                    //Retorna o objeto da authenticação 
                    HttpResponseMessage response = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                        JsonConvert.SerializeObject(
                            new Authenticated(authentication)))
                    };
                    return ResponseMessage(response);
                }
                else
                {

                    HttpResponseMessage response = new HttpResponseMessage();
                    var login_token_cookie = new CookieHeaderValue("login_token", authentication.Token) { Expires = DateTime.Now.AddDays(30), Secure = false, HttpOnly = true, Domain = Request.RequestUri.Host, Path = "/" };
                    var login = db.Login.FirstOrDefault(fs => fs.ID == authentication.Login);
                    var user_key_cookie = new CookieHeaderValue("user_key", login.ValidKey) { Expires = DateTime.Now.AddDays(30), Secure = false, HttpOnly = true, Domain = Request.RequestUri.Host, Path = "/" };
                    response.Headers.AddCookies(new CookieHeaderValue[] {login_token_cookie ,
                            user_key_cookie});
                    response.StatusCode = HttpStatusCode.Redirect;
                    if (post == "home")
                    {
                        UriBuilder builder = new UriBuilder(Request.RequestUri)
                        {
                            Path = "Employees/List",
                            Query = string.Empty
                        };
                        post = builder.ToString();
                    }
                    Uri uri = new Uri(post);
                    response.Headers.Location = uri;
                    return ResponseMessage(response);
                }

            }
            else
            {
                if (!isWeb)
                {
                    //Retorna um 'NotFound' caso não encontre nenhum login com este 'UserName'
                    return NotFound();
                }
                else
                {
                    var query = HttpUtility.ParseQueryString(string.Empty);
                    query["isError"] = true.ToString();
                    query["title"] = "Hey!";
                    query["post"] = post;
                    query["message"] = "Parece que este usuário não existe na nossa base de dados";
                    UriBuilder builder = new UriBuilder(Request.RequestUri)
                    {
                        Path = "Logins/AuthenticationMessage",
                        Query = query.ToString()
                    };
                    return Redirect(builder.ToString());
                }
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Excluir um login
        /// </summary>
        /// <param name="user_key">Nome de Usuário</param>
        /// <param name="pas">Senha de login</param>
        /// <param name="login_token">Token de login</param>
        /// <returns></returns>
        [ResponseType(typeof(LoginModel))]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteAsync(string user_key, string pas, string login_token)
        {
            //obtém o contexto desta requisição 
            HttpContext context = HttpContext.Current;

            //Obtém o login no banco de dados
            Login result = db.Authentication.FirstOrDefault(fs => fs.Token == login_token.ToString()).Login1;

            //Verificar se o usuário existe.
            if (result != null)
            {
                //Retorna um 'NotFound' caso não encontre nenhum login com este 'UserName'
                return NotFound();
            }

            //Valida o usuário
            try
            {
                await ValidLoginAsync(login_token, user_key);
            }
            catch (AuthenticationException e)
            {
                return ResponseMessage(e.Response);
            }

            //Verifica se a senha está correta
            if (!PasswordCompare(
                result.Password,
                pas))
            {
                return Unauthorized();
            }

            //Remove as autenticações deste usuário
            foreach (Authentication authentication in db.Authentication.Where(fs => fs.Login1.ID == result.ID))
            {
                db.Authentication.Remove(authentication);
            }


            db.Login.Remove(result);

            await db.SaveChangesAsync();
            return Ok(new LoginModel(result));
        }
        #endregion

        //#region Put
        ///// <summary>
        ///// Adiciona novo Login.
        ///// </summary>
        ///// <param name="login">Modelo Contendo informações de login.</param>
        ///// <returns>retorna o modelo com as informações adionadas.</returns>
        //[ResponseType(typeof(LoginModel))]
        //[HttpPut]
        //public async Task<IHttpActionResult> Put(LoginModel login)
        //{
        //    #region ValidModel

        //    //Valida se já existe um Login com o mesmo E-mail cadastrado.
        //    Login exists = await db.Login.FirstOrDefaultAsync(fs => fs.Email == login.Email);
        //    if (exists != null)
        //    {
        //        ModelState.AddModelError("Email", "There is already a registration with this 'E-mail'.");
        //    }

        //    //Valida se já existe um Login com o mesmo UserName cadastrado.
        //    exists = await db.Login.FirstOrDefaultAsync(fs => fs.UserName == login.UserName);
        //    if (exists != null)
        //    {
        //        ModelState.AddModelError("UserName", "There is already a registration with this 'UserName'.");
        //    }
        //    //Valida se já existe um Login com o mesmo CPF cadastrado.
        //    exists = await db.Login.FirstOrDefaultAsync(fs => fs.Provider.First().CNPJ == login.Company.CNPJ);
        //    if (exists != null)
        //    {
        //        ModelState.AddModelError("CPF", "There is already a registration with this 'CPF'.");
        //    }
        //    //Valida se o 'UserName'contém .
        //    if (login.UserName.Contains(' '))
        //    {
        //        ModelState.AddModelError("UserName", "A username cannot contain spaces.");
        //    }
        //    //Verifica se o modelo é valido
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    #endregion
        //    #region LoginInsert
        //    //Adiciona a data de criação do login
        //    login.CreateDate = DateTime.Now;
        //    //Criptografa a senha
        //    login.Password = CryptographyString(login.Password);
        //    //Trasforma a Classe LoginModel em um mOdelo do banco de dados.
        //    var loginModel = login.GetLogin();
        //    try
        //    {
        //        //Adiciona ao DBContext
        //        db.Login.Add(loginModel);
        //        //Salva no banco
        //        await db.SaveChangesAsync();
        //    }
        //    catch (APIException)
        //    {
        //        return InternalServerError();
        //    }
        //    #endregion
        //    #region UserInsert
        //    //Obtém o Modelo do banco de dados.
        //    var userModel = login.GetProviderModel();
        //    loginModel = await db.Login.FirstOrDefaultAsync(predicate: fs => fs.UserName == login.UserName);
        //    try
        //    {
        //        userModel.LoginID = loginModel.ID;
        //        //Obtém uma chave de usuário valida.
        //        bool validKey = false;
        //        do
        //        {
        //            userModel.ValidKey = Regex.Replace(Guid.NewGuid().ToString(), @"^[A-Za-z0-9]$", "");
        //            var result = await db.Provider.FirstOrDefaultAsync(fs => fs.ValidKey == userModel.ValidKey);
        //            if (result == null)
        //            {
        //                validKey = true;
        //            }
        //        } while (!validKey);

        //        db.Provider.Add(userModel);
        //        await db.SaveChangesAsync();
        //    }
        //    catch (Exception)
        //    {
        //        db.Login.Remove(loginModel);
        //        await db.SaveChangesAsync();
        //        return InternalServerError();
        //    }
        //    #endregion


        //    return Ok();
        //}
        //#endregion

        //#region Post
        ///// <summary>
        ///// Atualizar Login
        ///// </summary>
        ///// <param name="login_id">ID de usuário</param>
        ///// <param name="model">Modelo com as informações para atualizar</param>
        ///// <param name="login_token">Token de login</param>
        ///// <param name="user_key">Chave de verificação do usuário</param>
        ///// <returns>Novo Modelo com as informações atualizadas.</returns>
        //[ResponseType(typeof(LoginModel))]
        //[HttpPost]
        //public async Task<IHttpActionResult> Post(int login_id, string user_key, [FromBody] LoginModel model, string login_token)
        //{
        //    //Obtém o usuário pelo nome de usuário
        //    Login logUser = await db.Login.FirstOrDefaultAsync(fs => fs.ID == login_id);

        //    //Obtém o contexto da solicitação 
        //    HttpRequest context = HttpContext.Current.Request;
        //    //Valida o usuário
        //    try
        //    {
        //        await ValidLoginAsync(login_token, user_key);
        //    }
        //    catch (AuthenticationException e)
        //    {
        //        return ResponseMessage(e.Response);
        //    }
        //    //Verifica se o email atualizado já foi usado
        //    if (model.Email == null)
        //    {
        //        Login usedEmail = await db.Login.FirstOrDefaultAsync(fs => fs.Email == logUser.Email);
        //        if (usedEmail == null)
        //        {
        //            HttpResponseMessage response = new HttpResponseMessage
        //            {
        //                StatusCode = HttpStatusCode.Conflict,
        //                Content = new StringContent("Read the headers of this request")
        //            };
        //            response.Headers.Add("X-Error", "There is already a record with the same value as the 'UserName' field");
        //            return ResponseMessage(response);
        //        }
        //    }
        //    //Verifica se o nome de usuário já foi usado
        //    if (model.UserName == null)
        //    {
        //        Login usedEmail = await db.Login.FirstOrDefaultAsync(fs => fs.Email == logUser.Email);
        //        if (usedEmail == null)
        //        {
        //            HttpResponseMessage response = new HttpResponseMessage
        //            {
        //                StatusCode = HttpStatusCode.Conflict,
        //                Content = new StringContent("Read the headers of this request")
        //            };
        //            response.Headers.Add("X-Error", "There is already a record with the same value as the 'UserName' field");
        //            return ResponseMessage(response);
        //        }
        //    }

        //    model.ID = logUser.ID;
        //    db.Entry(model).State = EntityState.Modified;
        //    await db.SaveChangesAsync();
        //    return Ok(
        //        new LoginModel(
        //            await db.Login.FirstOrDefaultAsync(fs => fs.ID == logUser.ID)));
        //}
        //#endregion

        #region Cryptography services

        public static string CryptographyString(string value)
        {
            return HashGeneration(value);
        }
        public static string HashGeneration(string password)
        {
            // Configurations
            int workfactor = 10; // 2 ^ (10) = 1024 iterations.

            string salt = BCrypt.Net.BCrypt.GenerateSalt(workfactor);
            string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hash;
        }
        public static bool PasswordCompare(string hash, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        #endregion

        #region ValidUser

        internal async static Task<LoginInformations> ValidLoginAsync(string token, string user_valid_key)
        {
            if (token == null)
            {
                token = string.Empty;
            }
            if (user_valid_key == null)
            {
                user_valid_key = string.Empty;
            }
            WSTowersOfficeEntities db = new WSTowersOfficeEntities();
            var context = System.Web.HttpContext.Current;

            //Obtém a autenticação deste usuário
            Authentication logToken = await db.Authentication.FirstOrDefaultAsync(fs => fs.IP == context.Request.UserHostAddress);
            if (logToken == null)
            {
                return new LoginInformations(user_valid_key, token) { IsValid = false, Message = "This 'login token' does not exist or does not exist on the system or is wrong." };
            }

            if (!PasswordCompare(logToken.Token, token.ToString()))
            {
                return new LoginInformations(user_valid_key, token) { IsValid = false, Message = "This 'login token' does not exist or does not exist on the system or is wrong." };
            }

            //Valida o IP fornecido
            if (context.Request.UserHostAddress != logToken.IP)
            {
                return new LoginInformations(user_valid_key, token) { IsValid = false, Message = "This 'login token' does not validing for your IP." };
            }

            //Valida a chave de verificação da tabela User.
            if (logToken.Login1.ValidKey != user_valid_key)
            {
                return new LoginInformations(user_valid_key, token) { IsValid = false, Message = "This 'login token' does not validing for your IP." };
            }

            return new LoginInformations(user_valid_key, token) { IsValid = true, Message = "This login validation result is success" };
        }
        internal static async Task<LoginInformations> ValidLoginAsync()
        {
            try
            {
                var loginInformations = GetLoginInformations();

                return await ValidLoginAsync(loginInformations.LoginToken, loginInformations.UserKey);
            }
            catch (AuthenticationException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static LoginInformations ValidLogin()
        {
            try
            {
                var loginInformations = GetLoginInformations();

                return ValidLogin(loginInformations.LoginToken, loginInformations.UserKey);

            }
            catch (AuthenticationException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        internal static LoginInformations ValidLogin(string token, string user_valid_key)
        {
            WSTowersOfficeEntities db = new WSTowersOfficeEntities();
            var context = System.Web.HttpContext.Current;
            //Obtém a autenticação deste usuário
            Authentication logToken = db.Authentication.FirstOrDefault(fs => fs.IP == context.Request.UserHostAddress);

            if (logToken == null)
            {
                return new LoginInformations(user_valid_key, token) { IsValid = false, Message = "This 'login token' does not exist or does not exist on the system or is wrong." };
            }

            if (!PasswordCompare(logToken.Token, token.ToString()))
            {
                return new LoginInformations(user_valid_key, token) { IsValid = false, Message = "This 'login token' does not exist or does not exist on the system or is wrong." };
            }

            //Valida o IP fornecido
            if (context.Request.UserHostAddress != logToken.IP)
            {
                return new LoginInformations(user_valid_key, token) { IsValid = false, Message = "This 'login token' does not validing for your IP." };
            }

            //Valida a chave de verificação da tabela User.
            if (logToken.Login1.ValidKey != user_valid_key)
            {
                return new LoginInformations(user_valid_key, token) { IsValid = false, Message = "This 'login token' does not validing for your IP." };
            }

            return new LoginInformations(user_valid_key, token) { IsValid = true, Message = "This login validation result is success" };
        }
        internal static LoginInformations GetLoginInformations()
        {
            string userKey = string.Empty;
            string loginToken = string.Empty;
            try
            {
                userKey = System.Web.HttpContext.Current.Request.Cookies.Get("user_key").Value;
                loginToken = System.Web.HttpContext.Current.Request.Cookies.Get("login_token").Value;

            }
            catch (NullReferenceException)
            {
                try
                {
                    loginToken = System.Web.HttpContext.Current.Request.Headers.Get("auth-login-token");
                    userKey = System.Web.HttpContext.Current.Request.Headers.Get("auth-user-key");
                }
                catch (NullReferenceException)
                {
                    try
                    {
                        userKey = System.Web.HttpContext.Current.Request.QueryString.Get("user_key");
                        loginToken = System.Web.HttpContext.Current.Request.QueryString.Get("login_token");
                    }
                    catch (NullReferenceException)
                    {
                        userKey = string.Empty;
                        loginToken = string.Empty;
                    }
                }
            }
            return new LoginInformations(userKey, loginToken);
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public class LoginInformations
        {
            public string UserKey { get; set; }
            public string LoginToken { get; set; }
            public bool IsValid { get; set; }
            public string Message { get; set; }
            public LoginInformations(string userKey, string loginToken)
            {
                UserKey = userKey;
                LoginToken = loginToken;
                IsValid = false;
                Message = "No Message here!";
            }
        }

    }
}