using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POWStudio.Models;
using POWStudio.Models.ViewModels;

namespace POWStudio.Controllers;

public class AccountController : Controller
{

    private readonly UserManager<ApplicationUser> mUserManager;
    private readonly SignInManager<ApplicationUser> mSignInManager;
    private readonly GameStoreDbContext mDbContext;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, GameStoreDbContext dbContext) //inject
    {
        mUserManager = userManager;
        mSignInManager = signInManager;
        mDbContext = dbContext;
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
    // GET: /Logout - Dùng để hiển thị trang sau khi đăng xuất HOẶC kiểm tra trạng thái
    [Route("/Logout")]
    [HttpGet]
    public IActionResult Logout()
    {
        if (!mSignInManager.IsSignedIn(User))
        {
            
            if (TempData["LoggedOutSuccess"] != null)
            {
                TempData.Remove("LoggedOutSuccess"); 
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        return View(); 
    }
    [Route("/Logout")]
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> LogoutPost()
    {
        await mSignInManager.SignOutAsync();
        Console.WriteLine("User logged out.");
        //return View();
        TempData["LoggedOutSuccess"] = true;
        
        // Chuyển hướng đến Action có tên "Logout" (chính là [HttpGet] Logout())
        return RedirectToAction(nameof(Logout));
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
    //FORGOT PASSWORD________________________
    [HttpGet]
    [Route("/ForgotPassword")]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPassModel());
    }
    [HttpPost]
    [Route("/ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(ForgotPassModel inForgotPassModel)
    {
        if (!ModelState.IsValid) return View(inForgotPassModel);
        var user = await mUserManager.FindByEmailAsync(inForgotPassModel.Email);
        if (user == null)
        {
            ModelState.AddModelError("","Account not exist");
            return View(inForgotPassModel);
        }
        var token = await mUserManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Action(
            "ResetPasswordFromForgot",
            "Account",
            new { userId = user.Id, token = token },
            protocol: Request.Scheme);
        //minh hoa email nhan
        Console.WriteLine($" Reset Password Link for {inForgotPassModel.Email}: {callbackUrl}");
        inForgotPassModel.bSuccess =  true;
        return View(inForgotPassModel);
    }
    
    //RESET PASSWORD
    // [GET] /Account/ResetPassword?userId=xxx&token=yyy
    [HttpGet]
    public async Task<IActionResult> ResetPasswordFromForgot(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            return RedirectToAction("Error", "Home" , new {message = "Link expired or invalid"});
        var user = await mUserManager.FindByIdAsync(userId);
        if (user == null) return RedirectToAction("Error", "Home" , new {message = "Link expired or invalid"});
        var model = new ResetPassModel
        {
            UserId = userId,
            Token = token,
            Email = user.Email,
        };

        return View(model);
    }
    // [POST] /Account/ResetPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPasswordFromForgot(ResetPassModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await mUserManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            model.bSuccess = false;
            ModelState.AddModelError("", "Không tìm thấy người dùng.");
            return View(model);
        }

        var result = await mUserManager.ResetPasswordAsync(user, model.Token, model.Password);

        if (result.Succeeded)
        {
            model.bSuccess = true;
            return View(model);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }
    public IActionResult Profile()
    {
        var userId = mUserManager.GetUserId(User);
        var displayName = mDbContext.Users.FirstOrDefault(u => u.Id == userId)?.DisplayName;
        var emai = mDbContext.Users.FirstOrDefault(u => u.Id == userId)?.Email;
        ViewBag.DisplayName = displayName;
        ViewBag.UserId = userId;
        ViewBag.Email = emai;
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateDisplayName( string newDisplayName)
    {
        if (string.IsNullOrWhiteSpace(newDisplayName) || newDisplayName.Length < 3)
            return BadRequest("Invalid display name");

        var userId = mUserManager.GetUserId(User);
        var user = await mUserManager.FindByIdAsync(userId);
        
        Console.WriteLine(newDisplayName);

        if (user == null) return NotFound();

        user.DisplayName = newDisplayName;
        await mUserManager.UpdateAsync(user);

        return Ok(new { success = true, displayName = user.DisplayName });
    }

    public IActionResult LinkedAccounts()
    {
        return View();
    }

    public IActionResult Security()
    {
        return View();
    }

    public IActionResult PaymentSettings()
    {
        return View();
    }

    public IActionResult Order(int page = 1, int pageSize = 10)
    {
        var orders = mDbContext.Order.OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var total = mDbContext.Order.Count();

        var orderList = new List<OrderUListItemVM>();
        foreach (var order in orders)
        {
            var itemsInOrder = mDbContext.OrderItem
                .Where(x => x.OrderId == order.Id)
                .Select(x => new GameInOrderVM
                {
                    Name = x.Game.Title,
                    Price = x.Game.Price??0,
                })
                .ToList();
            var userOrder = mDbContext.UserOrder.FirstOrDefault(uo=>uo.OrderId == order.Id);
            var user = mDbContext.Users.FirstOrDefault(u=>u.Id == userOrder.UserId);
            var userDisplayName = user.DisplayName;
            var userEmail = user.Email;
            var mainName = itemsInOrder[0].Name;
            var subName = itemsInOrder.Count == 1 ? "" : $"and {itemsInOrder.Count - 1} more";

            orderList.Add(new OrderUListItemVM
            {
                OrderId = order.Id,
                OriginalPrice = order.Price,
                DiscountAmount = order.DiscountAmount,
                MainName = mainName,
                SubName = subName,
                OrderDate = order.OrderDate,
                Games = itemsInOrder,
            });
        }

        var orderVM = new OrderUVM
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            OrderListItemVms = orderList
        };

        return View(orderVM);
    }

    public IActionResult RedeemCode()
    {
        return View();
    }
}