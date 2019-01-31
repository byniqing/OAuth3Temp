using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace AuthorizationServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResource()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
        public static IEnumerable<ApiResource> GetResources()
        {
            var api1 = new ApiResource("api1", "my api");
            var api2 = new ApiResource
            {
                Name = "OAuth.ApiName", //这是资源名称
                Description = "2",
                DisplayName = "33",
                Scopes = {
                    new Scope{
                        Name="OAuth1", //这里是指定客户端能使用的范围名称 , 是唯一的
                        Description="描述",
                        DisplayName="获得你的个人信息，好友关系",
                        Emphasize=true,
                        Required=true,
                        //ShowInDiscoveryDocument=true,
                    },
                    new Scope{
                        Name="OAuth2",
                        Description="描述",
                        DisplayName="分享内容到你的博客",
                        Emphasize=true,
                        Required=true,
                    },
                    new Scope{
                        Name="OAuth3",
                        Description="描述",
                        DisplayName="获得你的评论",
                    }
                }
            };
            return new List<ApiResource> {
                api1,api2
            };
        }

        /// <summary>
        /// 这里是配置客户端的信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> {
                new Client{
                    ClientId="oidc",
                    ClientName="name",
                    ClientSecrets={ new Secret("secret".Sha256())},
                    //AllowedGrantTypes={GrantType.AuthorizationCode }
                   AllowedGrantTypes = GrantTypes.Code,
                   AllowedScopes={
                        "api2"
                    },
                   RedirectUris={"http://localhost:5001/signin-oidc" },
                   PostLogoutRedirectUris ={ "http://localhost:5001/signout-callback-oidc"}
                },
                 new Client{
                    ClientId="OAuth.Client",
                    ClientName="博客园",
                    ClientSecrets={ new Secret("secret".Sha256())},
                    //AllowedGrantTypes={GrantType.AuthorizationCode }
                   AllowedGrantTypes = GrantTypes.Code,
                   //RequireConsent=true,
                   ClientUri="http://www.cnblogs.com", //客户端
                   LogoUri="https://www.cnblogs.com/images/logo_small.gif",
                   //FrontChannelLogoutUri = "http://localhost:44077/signout-oidc",
                   AllowedScopes={
                        "OAuth1","OAuth2","OAuth3"
                    },
                  
                   //AccessToken过期时间， in seconds (defaults to 3600 seconds / 1 hour)
                    //AccessTokenLifetime = 3600, 
                     //设置AuthorizationCode的有效时间，in seconds (defaults to 300 seconds / 5 minutes)
                        //AuthorizationCodeLifetime = 300, 
                        //RefreshToken的最大过期时间，in seconds. Defaults to 2592000 seconds / 30 day
                        //AbsoluteRefreshTokenLifetime = 2592000,  


                   //RedirectUris={"http://localhost:5001/signin-oauth" },
                    RedirectUris={"http://localhost:5001/Account/PostAuthorize" },
                   PostLogoutRedirectUris ={ "http://localhost:5001/signout-callback-oauth"},
                   //BackChannelLogoutUri="http://localhost:5001/signout-oauth",
                   //PostLogoutRedirectUris = { "http://localhost:5001/" }
                }
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser> {
                new TestUser{
                    SubjectId="10000",
                    Username="cnblogs",
                    Password="123"
                }
            };
        }
    }
}
