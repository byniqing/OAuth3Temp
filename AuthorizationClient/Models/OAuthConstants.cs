using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationClient.Models
{
    public static class OAuthConstants
    {
        /// <summary>
        /// AuthorizationServer project should run on this URL
        /// </summary>
        public readonly static string AuthorizationServerBaseAddress = "http://localhost:5001";

        /// <summary>
        /// ResourceServer project should run on this URL
        /// </summary>
        public readonly static string ResourceServerBaseAddress = "http://localhost:4823";

        /// <summary>
        /// AuthorizationCodeGrant project should be running on this URL.
        /// </summary>
        public readonly static string AuthorizeCodeCallBackPath = "http://localhost:6321/Home/AuthCode";


        public readonly static string AuthorizePath = "/connect/authorize";
        public readonly static string TokenPath = "/connect/token";
        public readonly static string LoginPath = "/Account/Login";
        public readonly static string LogoutPath = "/Account/Logout";
        public readonly static string ResourcesPath = "/api/Values";

        public readonly static string Clientid = "auth_clientid";
        public readonly static string Secret = "secret";
        public readonly static string State = "123456789";
        public readonly static string Scopes = "api1 offline_access";
    }
}
