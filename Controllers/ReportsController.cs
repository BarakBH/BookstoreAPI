using System.Net.Mime;
using System.Text;
using BookstoreApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApi.Controllers;

// Simple HTML report (authors shown as comma-separated when there are multiple).
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IBookRepository _repo;
    public ReportsController(IBookRepository repo) => _repo = repo;



    // GET: /api/reports/books (Content-Type: text/html)
    [HttpGet("books")]
    public async Task<IActionResult> BooksHtml()
    {
        var books = await _repo.GetAllAsync();

        var sb = new StringBuilder();

        sb.Append("""
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1" />
          <title>Bookstore Report</title>
          <style>
            body { font-family: system-ui, Arial, sans-serif; margin: 24px; }
            table { border-collapse: collapse; width: 100%; }
            th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
            th { background: #f3f3f3; }
            tr:nth-child(even) { background: #fafafa; }
            .right { text-align: right; }
          </style>
        </head>
        <body>
          <h1>Bookstore Report</h1>
          <table>
            <thead>
              <tr>
                <th>title</th>
                <th>author</th>
                <th>category</th>
                <th>Year</th>
                <th class="right">price</th>
              </tr>
            </thead>
            <tbody>
        """);

        foreach (var b in books.OrderBy(x => x.Title.Value, StringComparer.OrdinalIgnoreCase))
        {
            var authors = string.Join(", ", b.Authors);
            sb.Append($"""
              <tr>
                <td>{Html(b.Title.Value)}</td>
                <td>{Html(authors)}</td>
                <td>{Html(b.Category)}</td>
                <td>{b.Year}</td>
                <td class="right">{b.Price:0.##}</td>
              </tr>
            """);
        }

        sb.Append("""
            </tbody>
          </table>
        </body>
        </html>
        """);

        return Content(sb.ToString(), MediaTypeNames.Text.Html, Encoding.UTF8);

        static string Html(string? s)
            => System.Net.WebUtility.HtmlEncode(s ?? string.Empty);
    }
}
