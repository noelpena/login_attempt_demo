using LoginAttemptDemo.Data;
using LoginAttemptDemo.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();
builder.Services.AddScoped<UserIpAddressModel>();
builder.Services.AddScoped<ResetAttemptsService>();
builder.Services.AddSingleton<ResetPasswordService>();
builder.Services.AddSingleton<LookupService>();
builder.Services.AddSingleton<EmailService>();


builder.Services.AddScoped<IDataProtector>(provider =>
{
    var dataProtectionProvider = provider.GetRequiredService<IDataProtectionProvider>();
    var protector = dataProtectionProvider.CreateProtector("login_attempt_demo");
    return protector;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
