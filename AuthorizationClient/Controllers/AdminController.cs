using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using AuthorizationClient.Models;
using Newtonsoft.Json;

namespace AuthorizationClient.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
           // var ck = HttpContext.AuthenticateAsync().Result;
           //ck.Properties.GetTokenValue("")
            return View();
        }
    }
}