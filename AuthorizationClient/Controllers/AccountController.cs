using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Principal;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json.Linq;
using IdentityModel.Client;
using System.Net.Http;

namespace AuthorizationClient.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 从这里触发授权的，所以当授权成功也会回调该
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Authorize(string returnUrl)
        {
            var ck1 = HttpContext.Request.Query;
            string scheme = "OAuth";
            // see if windows auth has already been requested and succeeded
            //WindowsPrincipal 
            var userResult = await HttpContext.AuthenticateAsync(scheme);

            // DefaultAuthenticateScheme causes User to be set
            // var user = context.User;

            // This is what [Authorize] calls
            var user = userResult.Principal;
            var props1 = userResult.Properties;

            //已经授权，登录并跳转
            if (User.Identity.IsAuthenticated)
            {
                var ck = userResult.Properties.GetTokens();
                var token = userResult.Properties.GetTokenValue("access_token");
                Console.WriteLine(token);

                //根据token获取用户信息并且登录

                var httpClient = new HttpClient();

                 httpClient.SetBasicAuthenticationOAuth("cnblogs", "123");
                httpClient.SetBearerToken(token);
                //httpClient.PostAsync("");

                var response = httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
                {
                    Address = "",
                    ClientId = "",
                });


                return View("UserInfo");
            }


            //string scheme = "OAuth";
            ////如果已经认证通过，
            //if (User.Identity.IsAuthenticated)
            //{
            //    return Redirect("");
            //}
            ////触发授权，DefaultChallengeScheme配置的是OAuth
            //return  Challenge(scheme);
            return await ProcessWindowsLoginAsync("/Account/Authorize");

            //return View();
        }

        [HttpPost]
        public IActionResult Authorize()
        {
            string scheme = "OAuth";
            ////如果已经认证通过，
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("UserInfo");
            }
            //触发授权，DefaultChallengeScheme配置的是OAuth
            return Challenge(scheme);
            //return await ProcessWindowsLoginAsync("/Account/Authorize");

            //return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private async Task<IActionResult> ProcessWindowsLoginAsync(string returnUrl)
        {
            string scheme = "OAuth";
            // see if windows auth has already been requested and succeeded
            //WindowsPrincipal 
            var userResult = await HttpContext.AuthenticateAsync(scheme);

            // DefaultAuthenticateScheme causes User to be set
            // var user = context.User;

            // This is what [Authorize] calls
            var user = userResult.Principal;
            var props1 = userResult.Properties;

            //已经授权，登录并跳转
            if (User.Identity.IsAuthenticated)
            {
                var ck = userResult.Properties.GetTokens();
                var token = userResult.Properties.GetTokenValue("access_token");

                return View("UserInfo");
            }


            //if (userResult?.Principal is ClaimsPrincipal wp)
            //{
            //    // we will issue the external cookie and then redirect the
            //    // user back to the external callback, in essence, treating windows
            //    // auth the same as any other external authentication mechanism
            //    var props = new AuthenticationProperties()
            //    {
            //        RedirectUri = Url.Action("Callback"),
            //        Items =
            //        {
            //            { "returnUrl", returnUrl },
            //            { "scheme", scheme },
            //        }
            //    };

            //    var id = new ClaimsIdentity(scheme);

            //    var result11 = HttpContext.AuthenticateAsync("idsrv").Result;

            //    id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.Identity.Name));
            //    id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));

            //    await HttpContext.SignInAsync("idsrv",
            //        new ClaimsPrincipal(id),
            //        props);
            //    //HttpContext.SignInAsync(wp.Identity.Name, wp.Identity.Name, props);
            //    return Redirect(props.RedirectUri);
            //}
            else
            {
                //触发授权，DefaultChallengeScheme配置的是OAuth
                return Challenge(scheme);
                //await HttpContext.ChallengeAsync("OAuth");
            }
        }

        [HttpGet]
        public IActionResult UserInfo()
        {
            return View();
        }

        public void refresh_token()
        {
            var pairs = new Dictionary<string, string>()
                    {
                        { "client_id", "" },
                        { "client_secret", "" },
                        { "grant_type", "" },
                        { "refresh_token", "" }
                    };
            var content = new FormUrlEncodedContent(pairs);
            //var tokenResponse = PostAsync(metadata.TokenEndpoint, content, context.RequestAborted);
            //tokenResponse.EnsureSuccessStatusCode();
        }
    }
}