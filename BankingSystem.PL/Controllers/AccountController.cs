using AutoMapper;
using BankingSystem.BLL.Interfaces;
using BankingSystem.DAL.Models;
using BankingSystem.PL.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankingSystem.PL.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uniitOfWork;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper, IUnitOfWork uniitOfWork)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _uniitOfWork = uniitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var roles = await _roleManager.Roles
            .Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToListAsync();

            var Model = new RegisterViewModel
            {
                AvailableRoles = roles
            };
            // Check if a fixed role is provided via TempData
            if (TempData["FixedRole"] != null)
            {
                ViewData["FixedRole"] = TempData["FixedRole"];
            }
            return View(Model);
        }
        //[HttpPost]
        //public async Task<IActionResult> Register(RegisterViewModel UserToRegister)
        //{
        //    var roles = await _roleManager.Roles
        // .Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToListAsync();

        //    var Model = new RegisterViewModel
        //    {
        //        AvailableRoles = roles
        //    };
        //    if (UserToRegister is not null)
        //    {
        //        if(ModelState.IsValid)
        //        {
        //            if(UserToRegister.Role =="Customer")
        //            {
        //                Customer appUser = _mapper.Map<RegisterViewModel, ApplicationUser>(UserToRegister);
        //            }
        //            //if (appUser.Discriminator == "Customer")
        //            //{
        //            //    var Customer = new Customer()
        //            //    {
        //            //        Id = appUser.Id
        //            //    };
        //            //    _uniitOfWork.Repository<Customer>().Add(Customer);
        //            //}
        //          IdentityResult result = await _userManager.CreateAsync(appUser, UserToRegister.Password);
        //            if (result.Succeeded)
        //            {
        //                //Create Cookie
        //                await _userManager.AddToRoleAsync(appUser, UserToRegister.Role);
        //                //await _signInManager.SignInAsync(appUser, false);
        //                return RedirectToAction("Index", "Home");
        //            }
        //            else
        //            {
        //                foreach (var item in result.Errors)
        //                {
        //                    ModelState.AddModelError("", item.Description);
        //                }
        //           }
        //        }
        //        }
        //    return View(UserToRegister);
        //}

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel UserToRegister)
        {
            // Load roles again in case of return to the view
            var roles = await _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToListAsync();

            if (UserToRegister is not null)
            {
                ModelState.Remove("Salary");
                if (ModelState.IsValid)
                {
                    ApplicationUser appUser;

                    // Create the correct derived class based on role
                    if (UserToRegister.Role == "Customer") appUser = _mapper.Map<Customer>(UserToRegister);

                    else if (UserToRegister.Role == "Admin") appUser = _mapper.Map<Admin>(UserToRegister);

                    else if (UserToRegister.Role == "Manager") appUser = _mapper.Map<DAL.Models.Manager>(UserToRegister);

                    else if (UserToRegister.Role == "Teller") appUser = _mapper.Map<Teller>(UserToRegister);

                    else appUser = _mapper.Map<ApplicationUser>(UserToRegister);
                    appUser.Id = Guid.NewGuid().ToString();
                    IdentityResult result = await _userManager.CreateAsync(appUser, UserToRegister.Password);

                    // Ensure the role is "Teller" when added by a manager
                    if (User.IsInRole("Manager") && UserToRegister.Role != "Teller")
                    {
                        ModelState.AddModelError("Role", "Invalid role for manager-added employees.");
                        return View(UserToRegister);
                    }

                    // Check if the user was created successfully
                    if (result.Succeeded)
                    {
                        // Assign role
                        await _userManager.AddToRoleAsync(appUser, UserToRegister.Role);

                        // Optional: Sign in
                        // await _signInManager.SignInAsync(appUser, false);

                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            // Return view with roles and model in case of error
            UserToRegister.AvailableRoles = roles;
            return View(UserToRegister);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginUserViewModel UserToLogin)
        {
            if (UserToLogin is not null)
            {

                if (ModelState.IsValid)
                {
                    // Check User Is Found Or Not
                    ApplicationUser user = await _userManager.FindByEmailAsync(UserToLogin.Email);

                    if (user is not null)
                    {
                        bool found = await _userManager.CheckPasswordAsync(user, UserToLogin.Password);
                        if (found)
                        {
                            await _signInManager.SignInAsync(user, UserToLogin.RememberMe);

                            // if the role is Customer 
                            if (await _userManager.IsInRoleAsync(user, "Customer"))
                            {
                                return RedirectToAction("Details", "CustomerProfile", new { id = user.Id });
                            }
                            else if (await _userManager.IsInRoleAsync(user, "Manager"))
                            {
                                return RedirectToAction("GetAllCustomers", "ManagerCustomer", new { id = user.Id });
                            }
                            else if (await _userManager.IsInRoleAsync(user, "Teller"))
                            {
                                return RedirectToAction("GetAllCustomers", "HandleCustomer", new { id = user.Id });
                            }

                            // Default redirect for other roles
                            // You can customize this as needed
                            // return RedirectToAction("Index", "Home");
                            // or

                        }
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError(string.Empty, "Wrong Email Or Password");
                    return View(UserToLogin);
                }

            }
            return View(UserToLogin);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

                Console.WriteLine(resetLink);

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email);
                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        // Create ApplicationUser with "Customer" discriminator
                        var customerUser = new Customer
                        {
                            UserName = email.Split('@')[0],
                            Email = email,
                            Discriminator = "Customer",
                            Address = "",
                            FirstName = email.Split('@')[0],
                            LastName = email.Split('@')[0],
                            SSN = 00000000000000,
                            JoinDate = DateTime.Now,


                        };

                        var createResult = await _userManager.CreateAsync(customerUser);
                        if (createResult.Succeeded)
                        {
                            // Link external login
                            await _userManager.AddLoginAsync(customerUser, info);

                            // Assign "Customer" role
                            await _userManager.AddToRoleAsync(customerUser, "Customer");

                            // Insert into Customers table
                            _uniitOfWork.Repository<Customer>().Add(customerUser);
                            //_uniitOfWork.Complete();

                            // Sign in
                            await _signInManager.SignInAsync(customerUser, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            foreach (var error in createResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    else
                    {
                        // Add external login and sign in
                        await _userManager.AddLoginAsync(user, info);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                ViewBag.ErrorTitle = "Email claim not received from: " + info.LoginProvider;
                ViewBag.ErrorMessage = "Please contact support.";
                return View("Error");
            }
        }



    }
}
