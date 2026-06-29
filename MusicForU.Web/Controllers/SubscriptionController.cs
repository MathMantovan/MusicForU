using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using MusicForU.Application.DTOs;

namespace MusicForU.Web.Controllers;

public class SubscriptionController : Controller
{
    private readonly IHttpClientFactory _factory;
    public SubscriptionController(IHttpClientFactory factory) => _factory = factory;

    private HttpClient CreateAuthClient()
    {
        var client = _factory.CreateClient("Api");
        var token = HttpContext.Session.GetString("Token");
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [HttpGet]
    public async Task<IActionResult> ChoosePlan()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var client = CreateAuthClient();
        var plans = await client.GetFromJsonAsync<List<PlanDto>>("api/plan") ?? new();
        return View(plans);
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe(int planId)
    {
        var client = CreateAuthClient();
        var userId = int.Parse(HttpContext.Session.GetString("UserId")!);

        var plans = await client.GetFromJsonAsync<List<PlanDto>>("api/plan") ?? new();
        var plan = plans.FirstOrDefault(p => p.Id == planId);
        if (plan is null) return RedirectToAction("ChoosePlan");

        if (plan.Price > 0)
        {
            var txResult = await client.PostAsJsonAsync("api/transaction/authorize",
                new AuthorizeTransactionDto(userId, "MusicForU-Store", plan.Price));

            var txResponse = await txResult.Content.ReadFromJsonAsync<TransactionResultDto>();
            if (txResponse is null || !txResponse.Authorized)
            {
                ViewBag.Error = txResponse?.Message ?? "Erro ao processar pagamento.";
                return View("ChoosePlan", plans);
            }
        }

        await client.PostAsJsonAsync("api/subscription", new CreateSubscriptionDto(userId, planId));

        return RedirectToAction("Index", "Home");
    }
}
