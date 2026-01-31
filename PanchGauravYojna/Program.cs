using BL.PageAccessRequirement;
using BL.Progres;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using PanchGauravYojna;
using PanchGauravYojna.Service;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews(); // Or AddRazorPages(), AddMvc()
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
// Add services to the container.
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = ".AspNetCore.Antiforgery.GFtadfaI-b4";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // or Always for HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict; // Adjust as necessary
});
builder.Services.ConfigureApplicationCookie(options =>
{    
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    // Strict SameSite policy to prevent CSRF via cross-site cookies
    // Custom cookie name (optional)
    options.Cookie.Name = ".AspNetCore.Cookie";
    // Redirect paths (optional)
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/LogOut";

    // Expiration
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});
builder.Services.AddSession(options =>
{
    
    options.Cookie.HttpOnly = true; // Prevent JavaScript from accessing the cookie
    options.Cookie.IsEssential = true; // Mark the cookie as essential   
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure the cookie is only sent over HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict; // Strict SameSite policy
    options.IdleTimeout = TimeSpan.FromMinutes(30);// Session timeout duration

});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    //options.Cookie.Name = ".AspNetCore.Cookies";
    //options.Cookie.HttpOnly = true;
    //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // ? Cookie sent over HTTPS only
    //options.Cookie.SameSite = SameSiteMode.Strict;         // ? STRONGEST PROTECTION (use Lax only if needed)
    //                                                       // ? Prevent access via JavaScript
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/LogOut";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);     // Set session timeout
    options.SlidingExpiration = true;                      // Extend session on activity
    options.AccessDeniedPath = "/Account/AccessDenied";  // 403 redirect

});
//builder.Services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

#region[4. START : SERVICES INJECTOR]

new ServiceInject(builder.Services);
#endregion
// ✅ 🔹 ADD YOUR AUTHORIZATION + HTTP CONTEXT REGISTRATIONS HERE 🔹 ✅

//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddScoped<IAuthorizationHandler, PageAccessHandler>();

builder.Services.AddAuthorization(options =>
{
    // Policy for database-based permission check
    options.AddPolicy("PageAccess", policy =>
        policy.Requirements.Add(new PageAccessRequirement("")));
});

// ✅ END OF AUTHORIZATION SECTION
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/StatusError"); // Custom error page in production
    app.UseHttpsRedirection();
    app.UseHsts();    
}
app.UseStatusCodePagesWithRedirects("/StatusError/{0}");
//app.UseMiddleware<SanitizePathMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var headers = ctx.Context.Response.Headers;
        headers.Remove("Server");
        headers.Remove("X-Powered-By");
    }
});
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();
// 🔹 YOUR CUSTOM ACCESS DENIED MIDDLEWARE
app.UseMiddleware<AccessDeniedMiddleware>();   // 🟢 RIGHT PLACE
app.UseSession();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Ensure custom error pages are routed
    endpoints.MapControllerRoute(
        name: "error",
        pattern: "/StatusError/{statusCode}");
    //defaults: new { controller = "Error", action = "Errortest" });
});
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
