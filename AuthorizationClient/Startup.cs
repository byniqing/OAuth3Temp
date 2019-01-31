using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;

namespace AuthorizationClient
{
    public class Startup
    {
        /*
         https://github.com/aspnet/AspNetCore/blob/master/src/Security/Authentication/samples/SocialSample/Startup.cs
         https://ankitsharmablogs.com/authentication-using-linkedin-asp-net-core-2-0/
             */

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region 解释记载
            // services.AddAuthentication(option =>
            // {
            //     /*
            //      要想使用认证系统，必要先注册Scheme
            //      而每一个Scheme必须指定一个Handler
            //      AuthenticationHandler 负责对用户凭证的验证
            //      这里指定的默认认证是cookie认证
            //      Scheme可以翻译为方案，即默认的认证方案
            //      */
            //     option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //     //option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //     //option.DefaultChallengeScheme = OAuthDefaults.DisplayName;

            //     /*
            //      因为这里用到了多个中间件，（AddAuthentication，AddCookie，AddOAuth）
            //      所以要设置一个默认中间件OAuthDefaults.DisplayName 的默认值是OAuth
            //      指定AddOAuth是默认中间件，在AddOAuth配置了很多选项

            //     如果只用了一个中间件，则可以不写，是否还记得cookie认证
            //     services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .AddCookie(option =>
            //     {
            //         ///Account/Login?ReturnUrl=%2Fadmin
            //         option.LoginPath = "/login/index";
            //         //option.ReturnUrlParameter = "params"; //指定参数名称
            //         //option.Cookie.Domain
            //         option.AccessDeniedPath = "/login/noAccess";
            //         option.Cookie.Expiration = TimeSpan.FromSeconds(4);
            //         option.Events = new CookieAuthenticationEvents
            //         {
            //             OnValidatePrincipal = LastChangedValidator.ValidateAsync
            //         };
            //     });
            //      */
            //     option.DefaultChallengeScheme = OAuthDefaults.DisplayName;
            // })
            //.AddCookie()
            //.AddOAuth(OAuthDefaults.DisplayName,
            //option => {
            //    option.ClientId = "oauth";
            //    option.ClientSecret = "secret";
            //    option.AuthorizationEndpoint = "http://localhost:5000/connect/authorize";
            //    option.TokenEndpoint = "http://localhost:5000/connect/token";
            //    option.CallbackPath = "/signin-oauth";
            //     //option.Scope.Clear();
            //     option.Scope.Add("api");
            //     // 事件执行顺序 ：
            //     // 1.创建Ticket之前触发
            //     //option.Events.OnCreatingTicket = context => Task.CompletedTask;
            //     // 2.创建Ticket失败时触发
            //     //option.Events.OnRemoteFailure = context => Task.CompletedTask;
            //     // 3.Ticket接收完成之后触发
            //     //option.Events.OnTicketReceived = context => Task.CompletedTask;
            //     // 4.Challenge时触发，默认跳转到OAuth服务器
            //     // options.Events.OnRedirectToAuthorizationEndpoint = context => context.Response.Redirect(context.RedirectUri);
            // }); 
            #endregion

            services.AddMvc();

            services.AddAuthentication(options =>
            {
                //options.DefaultAuthenticateScheme=OAuthDefaults.DisplayName
                //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = "Cookies";
                options.DefaultSignInScheme = "Cookies";
                options.DefaultChallengeScheme = "OAuth";
            })
            .AddCookie()
            .AddOAuth("OAuth", optins =>
            {
                optins.ClientId = "OAuth.Client";
                optins.ClientSecret = "secret";
                optins.AuthorizationEndpoint = "http://localhost:5000/connect/authorize";
                optins.TokenEndpoint = "http://localhost:5000/connect/token";
                optins.SaveTokens = true;
                optins.CallbackPath = "/Account/PostAuthorize";
                optins.Scope.Add("OAuth1");
                optins.Scope.Add("OAuth2");
                optins.Scope.Add("OAuth3");

                optins.Events = new OAuthEvents()
                {
                    //OnRemoteFailure = HandleOnRemoteFailure,

                    //远程异常触发
                    OnRemoteFailure = OAuthFailureHandler =>
                    {
                        var msg = OAuthFailureHandler.Failure.Message;
                        var authProperties = optins.StateDataFormat.Unprotect(OAuthFailureHandler.Request.Query["state"]);

                        var redirectUrl = authProperties.RedirectUri;
                        if (redirectUrl.Contains("/"))
                        {
                            redirectUrl = string.Format($"{redirectUrl.Substring(0, redirectUrl.LastIndexOf("/") + 1)}#");// redirectUrl.Substring(0, redirectUrl.IndexOf("/") + 1);
                        }
                        //"http://localhost:5001/#"
                        OAuthFailureHandler.Response.Redirect(redirectUrl);
                        OAuthFailureHandler.HandleResponse();
                        return Task.FromResult(0);
                    }
                };
            });
        }
        private async Task HandleOnRemoteFailure(RemoteFailureContext context)
        {
            var url = context.Request.Host.ToString();
          
            context.HandleResponse();
            await Task.FromResult(0);
            //await Task.Run(() =>
            //{
            //    context.Response.Redirect("/Home", true);
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();

            app.Map("/profile11", signinApp =>
            {
                signinApp.Run(async context =>
                {
                    await context.Response.WriteAsync($"<h1>你好，当前登录用户： {UrlEncoder.Default.Encode(context.User.Identity.Name)}</h1>");
                    await context.Response.WriteAsync("<a class=\"btn btn-default\" href=\"/Account/Logout\">退出</a>");

                    await context.Response.WriteAsync($"<h2>AuthenticationType：{context.User.Identity.AuthenticationType}</h2>");

                    await context.Response.WriteAsync("<h2>Claims:</h2>");
                    //await context.Response.WriteAsync("",context.User.Claims.Select(c => new string[] { c.Type, c.Value }));

                    // 在第一章中介绍过HandleAuthenticateOnceAsync方法，在此调用并不会有多余的性能损耗。
                    var result = await context.AuthenticateAsync();

                    result.Properties.GetTokens().Select(token => new string[] { token.Name, token.Value });
                });
            });
            // 首页
            //app.Run(async(context) => {
            //    await context.Response.WriteAsync($"<h2>Hello OAuth Authentication</h2>");
            //    await context.Response.WriteAsync("<a class=\"btn btn-default\" href=\"/profile\">我的信息</a>");
            //});
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
