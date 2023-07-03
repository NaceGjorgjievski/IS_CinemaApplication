
using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using Cinema.Domain.Identity;
using Cinema.Services.Interface;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cinema.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<CinemaApplicationUser> userManager;
        private readonly SignInManager<CinemaApplicationUser> signInManager;
        
        private readonly IUserService _userService;
        public AccountController(UserManager<CinemaApplicationUser> userManager, SignInManager<CinemaApplicationUser> signInManager, IUserService userService)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
            this._userService = userService;
        }

        public IActionResult Register()
        {
            UserRegistrationDto model = new UserRegistrationDto();
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationDto request)
        {
            if (ModelState.IsValid)
            {
                var userCheck = await userManager.FindByEmailAsync(request.Email);
                if (userCheck == null)
                {
                    var user = new CinemaApplicationUser
                    {
                        FirstName = request.Name,
                        LastName = request.LastName,
                        UserName = request.Email,
                        NormalizedUserName = request.Email,
                        Email = request.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        PhoneNumber = request.PhoneNumber,
                        UserCart = new ShoppingCart()
                    };
                    var result = await userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddClaimAsync(user, new Claim("UserRole", "StandardUser"));
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            return View(request);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            UserLoginDto model = new UserLoginDto();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed yet");
                    return View(model);

                }
                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);

                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Users()
        {
            
            var users = this._userService.getAllUsers();
            List<string> userRoles = new List<string>();
            foreach(var user in users)
            {
               userRoles.Add(this._userService.GetUserRole(user));
            }
            ViewData["UserRoles"] = userRoles;
            return View(users);
        }

        public async Task<IActionResult> SetAdmin(string id)
        {
            this._userService.SetAdmin(id);
            if (signInManager.IsSignedIn(User))
            {
                var user = await userManager.GetUserAsync(User);
                await signInManager.RefreshSignInAsync(user);
            }
            return RedirectToAction("Users", "Account");
        }
        
        public async Task<IActionResult> SetStandardUser(string id)
        {
            this._userService.SetStandardUser(id);
            if (signInManager.IsSignedIn(User))
            {
                var user = await userManager.GetUserAsync(User);
                await signInManager.RefreshSignInAsync(user);
            }
            return RedirectToAction("Users", "Account");
        }

        public IActionResult RemoveUser(string id)
        {
            this._userService.RemoveUser(id);
            return RedirectToAction("Users", "Account");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ImportUsers(IFormFile file)
        {

            //make a copy
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";


            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            //read data from uploaded file
            string pathToFile = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            List<CinemaApplicationUser> userList = new List<CinemaApplicationUser>();

            using (var stream = System.IO.File.Open(pathToFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        CinemaApplicationUser user = new CinemaApplicationUser
                        {
                            UserName = reader.GetValue(0).ToString(),
                            NormalizedUserName = reader.GetValue(0).ToString(),
                            Email = reader.GetValue(0).ToString(),
                            EmailConfirmed = true,
                            PhoneNumberConfirmed = true,
                            UserCart = new ShoppingCart()
                        };
                        var result = await userManager.CreateAsync(user, reader.GetValue(1).ToString());
                        if (result.Succeeded)
                        {
                            await userManager.AddClaimAsync(user, new Claim("UserRole", reader.GetValue(2).ToString()));
                        }
                    }
                }
            }
            return RedirectToAction("Users", "Account");
        }  
    }
}
