﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using ECommerceApp.Data;
using ECommerceApp.Services;
using System.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Redirect here if not authenticated
        options.AccessDeniedPath = "/AccessDenied"; // Redirect for unauthorized access
    });

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CartService>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddDbContext<ECommerceAppContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ECommerceAppContext") ?? throw new InvalidOperationException("Connection string 'ECommerceAppContext' not found.")));

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

// Enable session middleware
app.UseSession(); 

// Use Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
