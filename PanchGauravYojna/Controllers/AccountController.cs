using BL.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MO.Common;
using MO.Login;
using SDGApplication_V2;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace PanchGauravYojna.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ILogin _iLogin;
        private readonly SSOdeatilCredentials _appSettings;
        public AccountController(ILogger<AccountController> logger, ILogin login, IOptions<SSOdeatilCredentials> appSettings)
        {
            _logger = logger;
            _iLogin = login;
            _appSettings = appSettings.Value;
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/SignIn")]
        public async Task<IActionResult> SignIn(loginModels.loginCredentials lc)
        {

            var referer = Request.Headers["Referer"].ToString();
            try
            {
                //string pass = "asq"; //Security.Decrypt(lc.password);
                //Hash password using SHA256 (no decryption now)
                //var pass1 = "Agrtonk@123";
                //pass1 = ComputeSha256Hash(pass1);
                //var pass2 = "Superbikaneralldepart@123";
                //pass2 = ComputeSha256Hash(pass2);
                //var pass3 = "Supertonkalldepart@123";
                //pass3 = ComputeSha256Hash(pass3);
                //var pass4 = "Indtonk@123";
                //pass4 = ComputeSha256Hash(pass4);
                //var pass5 = "Sporttonk@123";
                //pass5 = ComputeSha256Hash(pass5);
                //var pass6 = "Tortonk@123";
                //pass6 = ComputeSha256Hash(pass6);
                //var pass7 = "Supertonk@123";
                //pass7 = ComputeSha256Hash(pass7);
                //var pass8 = "Admindes@123";
                //pass8 =await ComputeSha256Hash(pass8);

                string username = lc.Username; 
                string password = lc.Password; // HashPasword

                string sessionNonce = HttpContext.Items["CSPNonce"].ToString();
                if (string.IsNullOrEmpty(sessionNonce))
                {
                    loginModels.loginResult rs = new loginModels.loginResult()
                    {
                        isAuthenticate = false,
                        message = "Session expired or Invalid",
                    };
                    return Json(rs);
                }
                //lc.password = ComputeSha256Hash(lc.password);
                //lc.password = pass;
                var result = await _iLogin.userAuthenticate(lc);
                if (result.isAuthenticate)
                {
                    var identity = _iLogin.claims(result.user);
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.Session.SetString("SessionSalt", GenerateSalt());
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);                 
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                loginModels.loginResult rs = new loginModels.loginResult()
                {
                    isAuthenticate = false,
                    message = "Invalid Login !",
                };
                return Json(rs);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private async Task<string> ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string GenerateSalt(int size = 16)
        {
            byte[] saltBytes = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPasswordWithSalt(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + salt);
            var hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Login()
        {
            string nonce = Guid.NewGuid().ToString("N"); // or use random string
            HttpContext.Session.SetString("loginNonce", nonce);

            ViewData["loginNonce"] = nonce; // Pass to Razor view
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(loginModels.loginCredentials lc)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(lc); // or View(model) if not using AJAX partial
            //}
            var referer = Request.Headers["Referer"].ToString();
            try
            {

                string sessionNonce = lc.nonce;//HttpContext.Session.GetString("loginNonce");//lc.nonce;
                string clientPasswordHash = lc.HashedPassword;
                //get real Password
                string REALPASSWORD = await _iLogin.userAuthenticate_getPassword(lc.Username);
                string combined = REALPASSWORD + sessionNonce;
                string serverHash = HashPasswordWithSalt(REALPASSWORD,sessionNonce); // must match client hash

                //loginModels.login_result rs = new loginModels.login_result()
                //{
                //    isAuthenticate = false,
                //    isRedirect = false,
                //    status = false,
                //    message = "serverHash-"+serverHash+ "$" + clientPasswordHash +"$"+ sessionNonce +"Passwordread-"+ REALPASSWORD,
                //};
                //return Json(rs);
                if (clientPasswordHash != serverHash)
                {
                    loginModels.login_result rs = new loginModels.login_result()
                    {
                        isAuthenticate = false,
                        isRedirect = false,
                        status = false,
                        message = "Invalid Login !",
                    };
                    return Json(rs);
                }
                else
                {
                    lc.Password = REALPASSWORD;
                    var result = await _iLogin.userAuthenticate(lc);
                    if (result.isAuthenticate)
                    {
                        var identity = _iLogin.claims(result.user);
                        var principal = new ClaimsPrincipal(identity);
                        //HttpContext.Session.SetString("SessionSalt", GenerateSalt());
                        var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        result.isRedirect = true;
                        result.status = true;
                        result.redirectUrl = Url.Action("Dashboard", "Indicator"); // 👈 Add this                
                        return Json(result);
                    }
                    else
                    {
                        result.isRedirect = false;
                        result.status = false;
                    }
                    return Json(result);
                    }


                }
            catch (Exception ex)
            {
                loginModels.loginResult rs = new loginModels.loginResult()
                {
                    isAuthenticate = false,
                    message = "Invalid Login !",
                };
                return Json(rs);
            }
        }
    }
}
