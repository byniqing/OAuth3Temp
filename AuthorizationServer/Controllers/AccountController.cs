using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthorizationServer.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using IdentityServer4.Test;
using IdentityServer4.Services;
using IdentityServer4.Models;
using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace AuthorizationServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly TestUserStore _userStore;
        private readonly IIdentityServerInteractionService _interaction;


        public AccountController(TestUserStore userStore,
            IIdentityServerInteractionService interaction)
        {
            _userStore = userStore;
            _interaction = interaction;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("index");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            /*
             因为页面用了模型绑定，只要接受returnUrl参数，就会被绑定
             */
            //ViewBag.returnUrl11 = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (login.Action.ToLower() == "cancel")
            {
                var request = await _interaction.GetAuthorizationContextAsync(login.ReturnUrl);
                if (request != null)
                {
                    ConsentResponse grantedConsent = ConsentResponse.Denied;
                    await _interaction.GrantConsentAsync(request, grantedConsent);
                    return Redirect(login.ReturnUrl);
                }
                else
                {
                    //异常处理
                }
            }
            if (ModelState.IsValid)
            {
                var user = _userStore.FindByUsername(login.UserName);
                if (user != null && _userStore.ValidateCredentials(user.Username, user.Password))
                {
                    #region 刚开始这样登录是不行的,这是cookie身份验证，cookie身份登录
                    var claims = new List<Claim> {
                        new Claim("name",login.UserName)
                     };
                    var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimIdentity);

                    //这种方式是cookie认证，这样登录无效

                    //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    //    claimsPrincipal,
                    //    new AuthenticationProperties
                    //    {
                    //        IsPersistent = true, //
                    //        ExpiresUtc = DateTime.Now.AddDays(5)
                    //    });



                    #endregion
                    #region 方法1 OAuth身份验证，identityserver身份验证
                    //idsrv验证sub是必须的
                    var claims1 = new List<Claim> {
                        new Claim(JwtClaimTypes.Subject,user.SubjectId),
                        new Claim(JwtClaimTypes.Name,user.Username)
                     };
                    var claimIdentity1 = new ClaimsIdentity(claims1, OAuthDefaults.DisplayName);
                    var claimsPrincipal1 = new ClaimsPrincipal(claimIdentity1);

                    //要用 idsrv
                    //await HttpContext.SignInAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme,
                    //    claimsPrincipal1,
                    //    new AuthenticationProperties
                    //    {
                    //        IsPersistent = true, //
                    //        ExpiresUtc = DateTime.Now.AddDays(5)
                    //    });
                    #endregion
                    #region 方法2 或者直接用identityserver4封装的扩展方法 这样登录才正确
                    var p = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.Now.AddDays(5)
                    };

                    //identityserver 是身份验证 ，identity 所以要用该方法
                    /*
                     * 
                     */
                    //Microsoft.AspNetCore.Http.AuthenticationManagerExtensions.SignInAsync(HttpContext, user.SubjectId, user.Username, p);

                    //或者
                    await HttpContext.SignInAsync(user.SubjectId, user.Username, p);
                    //登录成功，跳转到同意授权页面
                    return Redirect(login.ReturnUrl);
                    #endregion
                }
            }
            //else
            return View("login");
        }

        [HttpPost]
        public IActionResult Show()
        {
            return View();
        }
    }
}