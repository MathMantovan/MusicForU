using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using MusicForU.Application.DTOs;

namespace MusicForU.Web.Controllers;

public class AuthController : Controller
{
    private readonly IHttpClientFactory _factory;
    public AuthController(IHttpClientFactory factory) => _factory = factory;

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var client = _factory.CreateClient("Api");
        var response = await client.PostAsJsonAsync("api/auth/login", dto);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "E-mail ou senha inválidos.";
            return View();
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResultDto>();
        SaveSession(result!);

        if (!await HasActiveSubscription(result!))
            return RedirectToAction("ChoosePlan", "Subscription");

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var client = _factory.CreateClient("Api");
        var response = await client.PostAsJsonAsync("api/auth/register", dto);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "E-mail já cadastrado.";
            return View();
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResultDto>();
        SaveSession(result!);

        return RedirectToAction("ChoosePlan", "Subscription");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    private void SaveSession(AuthResultDto result)
    {
        HttpContext.Session.SetString("Token", result.Token);
        HttpContext.Session.SetString("UserName", result.Name);
        HttpContext.Session.SetString("UserId", result.UserId.ToString());
    }

    private async Task<bool> HasActiveSubscription(AuthResultDto result)
    {
        var client = _factory.CreateClient("Api");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
        var response = await client.GetAsync("api/subscription/mine");
        return response.IsSuccessStatusCode;
    }
}
