﻿
using Blazored.LocalStorage;
using CnGalWebSite.Core.Services;
using CnGalWebSite.Core.Services.Query;
using CnGalWebSite.IdentityServer.Admin.Shared;
using CnGalWebSite.IdentityServer.Admin.Shared.Extensions;
using CnGalWebSite.IdentityServer.Admin.Shared.Options;
using CnGalWebSite.IdentityServer.Admin.Shared.Services;
using CnGalWebSite.IdentityServer.Admin.WASM;
using CnGalWebSite.IdentityServer.Admin.WASM.Services;
using Masa.Blazor;
using Masa.Blazor.Presets;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
//判断是否 SSR
StaticOptions.IsSSR = StaticOptions.PreSetIsSSR == null ? false : StaticOptions.PreSetIsSSR.Value;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//本地存储
builder.Services.AddBlazoredLocalStorage();


//注册身份验证的HttpClient
builder.Services.AddScoped<CustomAuthorizationMessageHandler>();
builder.Services.AddHttpClient("AuthAPI")
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("AuthAPI"));

//注册匿名的HttpClient
builder.Services.AddHttpClient("AnonymousAPI");

//注入自定义服务
builder.Services.AddScoped<INavigationService, NavigationService>();
builder.Services.AddScoped<IHttpService, HttpService>();

//添加Ids管理后台
builder.Services.AddIdentityServerAdmin();

//添加OpenId服务
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Local", options.ProviderOptions);
    options.UserOptions.RoleClaim = "role";
    options.UserOptions.NameClaim = "name";
    options.ProviderOptions.ResponseMode = "query";
    options.ProviderOptions.ResponseType = "code";
}).AddAccountClaimsPrincipalFactory<CustomUserFactory>();
await builder.Build().RunAsync();
