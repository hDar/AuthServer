using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Auth.Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Auth.Domain.Authentication;
using System.Security.Claims;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.ComponentModel.DataAnnotations;
using IdentityModel.Client;

namespace Auth.Client.Controllers
{
    public class HomeController : Controller
    {

        [Authorize]
        public IActionResult Index()
        {
            ViewData["Message"] = "Users Information";
            return View();
        }

        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var userId = GetClaimValue("sub");
            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            //var test = client.RequestClientCredentialsAsync("scope1 scope2");


            HttpResponseMessage response = await client.GetAsync(new Uri("http://localhost:5001/api/User/Get?userId=" + userId));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return View("Home");
            }

            var json = await response.Content.ReadAsStringAsync();
            ViewBag.Json = JsonConvert.DeserializeObject(json);
            return View("Json");
        }


        [Authorize]
        public async Task<IActionResult> AddRootUser()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            var userId = GetClaimValue("sub");
           
            var userData = new UserBindingModel
            {
                Email = "austin.root@straxcorp.com",
                Password = "Straximages#2017",
                ConfirmPassword = "Straximages#2017",
                Civility = "Mr",
                FirstName = "Admin",
                LastName = "Austin Hospital",
                IsActive = true,                
                ApprovedBy = "7c976a5f-16b9-4bc8-8688-040aa13cb46d",
                IsPasswordTemporary = true,                
                UserRole = "root_user"
            };
            var accountData = new AccountBindingModel
            {
                Name = "Austin Hospital",
                Region = "AU",
                Data = "",
                AddedBy = userId,
                User = userData
            };

            var json = JsonConvert.SerializeObject(accountData, Formatting.Indented);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = client.PostAsync("http://localhost:5000/api/Account/AddRootUser", byteContent).Result;
            var result = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                
                ViewBag.Json = JsonConvert.SerializeObject(result, Formatting.Indented); ;
                return View("json");
            }
            return RedirectToAction("Error", "Home");
        }

        [Authorize]
        public async Task<IActionResult> AddUser()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            var userId = GetClaimValue("sub");
            var organisationId = GetClaimValue("organisationId");
            var mockUsers = MockUserData(organisationId, userId);
            var resultArray = new JArray();

            foreach (var obj in mockUsers)
            {
                var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync("http://localhost:5000/api/Account/AddUser", byteContent).Result;
                var result = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    resultArray.Add(JsonConvert.DeserializeObject(result));
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }

               
            }
            ViewBag.Json = resultArray;
            return View("json");
        }


        [Authorize]
        public async Task<IActionResult> UpdateUserProfile()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            var userEmail = GetClaimValue("email");

            var userData = new UserBindingModel
            {   Email = userEmail,
                IsActive = true,                
                IsPasswordTemporary = false                
            };         

            var json = JsonConvert.SerializeObject(userData, Formatting.Indented);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = client.PostAsync("http://localhost:5000/api/Account/Update", byteContent).Result;
            var result = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ViewBag.Json = result;
                return View("json");
            }
            return RedirectToAction("Error", "Home");
        }


        [Authorize]
        public async Task<IActionResult> ResetPassword()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            var email = GetClaimValue("email");

            var userData = new ResetPasswordViewModel
            {
                Email = email,
                Password = "Straximages#2018",
                ConfirmPassword = "Straximages#2018",
                Code = accessToken
            };

            var json = JsonConvert.SerializeObject(userData, Formatting.Indented);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = client.PostAsync("http://localhost:5000/api/Account/ResetPassword", byteContent).Result;
            var result = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ViewBag.Json = result;
                return View("json");
            }
            return RedirectToAction("Error", "Home");
        }

        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            var userId = GetClaimValue("sub");
            var content = await client.GetStringAsync("http://localhost:5000/api/Account/GetAll?userId=" + userId);
            
            ViewBag.Json = JArray.Parse(content).ToString();
            return View("Json");
        }


        [Authorize]
        public async Task<IActionResult> GetOrganisationUsers()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            var organisationId = GetClaimValue("organisationId");
            var content = await client.GetStringAsync("http://localhost:5000/api/Account/GetOrgUsers?organisationId=" + organisationId);

            ViewBag.Json = JArray.Parse(content).ToString();
            return View("Json");
        }



        //[Authorize]
        //public async Task<IActionResult> ResetPassword()
        //{
        //    var accessToken = await HttpContext.GetTokenAsync("access_token");
        //    var userEmail = GetClaimValue("email");
        //    var client = new HttpClient();
        //    client.SetBearerToken(accessToken);

        //    //dynamic passwordModel = new JObject();
        //    //passwordModel.Email = userEmail;
        //    //passwordModel.Password = "Straximages#2018";
        //    //passwordModel.ConfirmPassword = "Straximages#2018";
        //    //passwordModel.Code = accessToken;

        //    var model = new ResetPasswordBindingModel();
        //    model.Email = userEmail;
        //    model.Password = "Straximages#2018";
        //    model.ConfirmPassword = "Straximages#2018";
        //    model.Code = "dsadad";


        //    var json = JsonConvert.SerializeObject(model, Formatting.Indented);
        //    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");


        //    client.BaseAddress = new Uri("http://localhost:5000");

        //    var response = await client.PostAsync("account/ResetPassword", httpContent);


        //    var result = await response.Content.ReadAsStringAsync();
        //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        ViewBag.Json = result;
        //        return View("json");
        //    }


        //    return View("Exception occured : " + response.Content.ToString());


        //    //HttpResponseMessage response = await client.GetAsync(new Uri("http://localhost:5001/api/User/Get?userId=" + userId));
        //    //if (response.StatusCode != HttpStatusCode.OK)
        //    //{
        //    //    return View("Home");
        //    //}

        //    //var json = await response.Content.ReadAsStringAsync();
        //    //ViewBag.Json = json;
        //    //return View("Json");
        //}





        [Authorize]
        //[Authorize(Roles = DomainRoles.SuperAdmin)]
        public IActionResult UserManagement()
        {
            ViewData["Message"] = "Users page.";
            return View();
        }


        //[Authorize]
        //public async Task<IActionResult> GetAllUsers()
        //{
        //    var accessToken = await HttpContext.GetTokenAsync("access_token");

        //    var client = new HttpClient();
        //    client.SetBearerToken(accessToken);
        //    var content = await client.GetStringAsync("http://localhost:5001/api/User/test");

        //    ViewBag.Json = JArray.Parse(content).ToString();
        //    return View("Json");
        //}



        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




        private string GetClaimValue(string claim) {            
            // Get the claims values            
            var claimValue = User.Claims.Where(c => c.Type == claim)
                               .Select(c => c.Value).SingleOrDefault();

            return claimValue;
        }


        //public class ResetPasswordBindingModel
        //{            
        //    public string Email { get; set; }

        //    public string Password { get; set; }

        //    public string ConfirmPassword { get; set; }

        //    public string Code { get; set; }
        //}


        private IEnumerable<UserBindingModel> MockUserData(string organisationId, string userId) {
            var userList = new List<UserBindingModel>
            {
                new UserBindingModel
                {
                    Email = "austin.centre@straxcorp.com",
                    Password = "Straximages#2017",
                    ConfirmPassword = "Straximages#2017",
                    Civility = "Mr",
                    FirstName = "Centre",
                    LastName = "Austin Hospital",
                    IsActive = true,
                    ApprovedBy = userId,
                    IsPasswordTemporary = true,
                    UserRole = DomainRoles.Centre,
                    OrganisationId = organisationId
                    
                },
                new UserBindingModel
                {
                    Email = "austin.doctor@straxcorp.com",
                    Password = "Straximages#2017",
                    ConfirmPassword = "Straximages#2017",
                    Civility = "Mr",
                    FirstName = "Doctor",
                    LastName = "Austin Hospital",
                    IsActive = true,
                    ApprovedBy = userId,
                    IsPasswordTemporary = true,
                    UserRole = DomainRoles.Doctor,
                    OrganisationId = organisationId
                }
            };

            return userList;
        }

        public class AccountBindingModel
        {            
            public string Name { get; set; }
            public string Region { get; set; }
            public string Data { get; set; }
            public string AddedBy { get; set; }
            public UserBindingModel User { get; set; }
        }

        public class UserBindingModel
        {            
            public string Email { get; set; }            
            public string Password { get; set; }            
            public string ConfirmPassword { get; set; }

            public string Civility { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public bool IsActive { get; set; }
            public string OrganisationId { get; set; }
            public string ApprovedBy { get; set; }
            public bool IsPasswordTemporary { get; set; }
            public string UserRole { get; set; }
        }


        public class ResetPasswordViewModel
        {           
            public string Email { get; set; }
                        
            public string Password { get; set; }
                        
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }
    }
}
