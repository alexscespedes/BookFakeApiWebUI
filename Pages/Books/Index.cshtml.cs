using System.Text.Json;
using BookFakeApiWebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookFakeApiWebUI.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IEnumerable<Book>? Books { get; set; }

        private HttpClient CreateClient() => _httpClientFactory.CreateClient("BookAspNetWebApi");

        public async Task<IActionResult> OnGetAsync()
        {
            var httpClient = CreateClient();
            var httpResponseMessage = await httpClient.GetAsync("Books");

            if (!httpResponseMessage.IsSuccessStatusCode)
                return StatusCode((int)httpResponseMessage.StatusCode);

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            Books = await JsonSerializer.DeserializeAsync<IEnumerable<Book>>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Page();
        }
    }
}
