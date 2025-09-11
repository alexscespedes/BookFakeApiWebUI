using System.Net;
using System.Text;
using System.Text.Json;
using BookFakeApiWebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookFakeApiWebUI.Pages.Books
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public Book? Book { get; set; } = new();
        private readonly IHttpClientFactory _httpClientFactory;

        public EditModel(IHttpClientFactory httpClientFactory)
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

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            Book = await JsonSerializer.DeserializeAsync<Book>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var httpClient = CreateClient();

            if (id == null)
            {
                return NotFound();
            }

            var book = new StringContent(JsonSerializer.Serialize(Book), Encoding.UTF8, "application/json");

            var httpResponseMessage = await httpClient.PutAsync($"Books/{id}", book);

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (httpResponseMessage.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError(string.Empty, "Error updating a book");
            return Page();
        }
    }
}
