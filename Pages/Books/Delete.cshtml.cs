using System.Net;
using System.Text;
using System.Text.Json;
using BookFakeApiWebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookFakeApiWebUI.Pages.Books
{
    public class DeleteModel : PageModel
    {
        public Book? Book { get; set; } = new();
        public string? ErrorMessage { get; set; }
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IHttpClientFactory httpClientFactory, ILogger<DeleteModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        private HttpClient CreateClient() => _httpClientFactory.CreateClient("BookAspNetWebApi");

        public async Task<IActionResult> OnGetAsync(int id, bool? saveChangesError = false)
        {
            var httpClient = CreateClient();
            var httpResponseMessage = await httpClient.GetAsync($"Books/{id}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = string.Format("Delete {ID} failed. Try again", id);
            }

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

            var httpResponseMessage = await httpClient.DeleteAsync($"Books/{id}");

            try
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                    return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessage);
                return RedirectToAction("./Delete", new { id, saveChangesError = true });
            }

            return Page();
        }
    }
}
