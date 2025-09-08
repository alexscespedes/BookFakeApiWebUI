using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using BookFakeApiWebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookFakeApiWebUI.Pages.Books
{
    public class DetailsModel : PageModel
    {
        public Book? Book { get; set; } = new();
        private readonly IHttpClientFactory _httpClientFactory;

        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient() => _httpClientFactory.CreateClient("BookAspNetWebApi");

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var httpClient = CreateClient();
            var httpResponseMessage = await httpClient.GetAsync($"Books/{id}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return StatusCode((int)httpResponseMessage.StatusCode);
            }

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            Book = await JsonSerializer.DeserializeAsync<Book>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Page();
        }
    }
}
