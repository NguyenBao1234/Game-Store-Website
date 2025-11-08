using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POWStudio.Models;
using POWStudio.Models.ViewModel;

namespace POWStudio.Controllers;

public class AccountController : Controller
{

    private readonly UserManager<ApplicationUser> mUserManager;
    private readonly SignInManager<ApplicationUser> mSignInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) //inject
    {
        mUserManager = userManager;
        mSignInManager = signInManager;
    }
    //LOGIN ______________________-
    [HttpGet]
    [Route("/Login")]//if without it, url = "domain/Account/Login" instead of "domain/Login"
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    [Route("/Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel inLoginModel)
    {
        if (!ModelState.IsValid) return View(inLoginModel);
        var result = await mSignInManager.PasswordSignInAsync(inLoginModel.Email, inLoginModel.Password, inLoginModel.RememberMe, true);
        if (result.Succeeded) return RedirectToAction("Index", "Home");
        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa do đăng nhập sai quá nhiều lần.");
        }
        else if (result.IsNotAllowed)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản của bạn chưa được xác thực email hoặc không được phép đăng nhập.");
        }
        else if (result.RequiresTwoFactor)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản này yêu cầu xác thực hai yếu tố (2FA).");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
        }

        return View(inLoginModel);
    }
    
    [Route("/Logout")]
    public async Task<IActionResult> Logout()
    {
        return View();
    }
    
    //REGISTER_________________
    [HttpGet]
    [Route("/Register")]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    [Route("/Register")]
    public async Task<IActionResult> Register(RegisterModel inRegisterModel)
    {
        if (!ModelState.IsValid)
        {
            return View(inRegisterModel);
        }
        
        var user = new ApplicationUser { UserName = inRegisterModel.Email, Email = inRegisterModel.Email, DisplayName = inRegisterModel.Name };
        var result = await mUserManager.CreateAsync(user, inRegisterModel.Password);
        
        if (result.Succeeded)
        {
            //confirm email manually
            var token = await mUserManager.GenerateEmailConfirmationTokenAsync(user);
            await mUserManager.ConfirmEmailAsync(user, token);
            await mSignInManager.SignInAsync(user, true);
            return RedirectToAction("Index", "Home");
        }
        foreach (var error in result.Errors)
        {
            Console.WriteLine($"Error: {error.Code} - {error.Description}");
            ModelState.AddModelError("", error.Description);
        }
        
        Console.WriteLine("User created "+ (result.Succeeded ? "successful" : "failed"));
        return View(inRegisterModel);
    }
    //________________________
}