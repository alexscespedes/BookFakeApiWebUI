using System.Net;
using System.Text;
using System.Text.Json;
using BookFakeApiWebUI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookFakeApiWebUI.Pages.Books
{
    public class CreateModel : PageModel
    {
        public Book? Book { get; set; } = new();
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient() => _httpClientFactory.CreateClient("BookAspNetWebApi");

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var book = new StringContent(JsonSerializer.Serialize(Book), Encoding.UTF8, "application/json");

            var httpClient = CreateClient();
            var httpResponseMessage = await httpClient.PostAsync("Books", book);

            if (httpResponseMessage.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError(string.Empty, "Error creating book");
            return Page();
        }
    }
}
