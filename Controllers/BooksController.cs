using BookstoreApi.Models;
using BookstoreApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApi.Controllers;

// CRUD over the XML by ISBN  
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _repo;
    public BooksController(IBookRepository repo) => _repo = repo;

    // GET: /api/books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAll()
        => Ok(await _repo.GetAllAsync());

    // GET: /api/books/9051234567897
    [HttpGet("{isbn}")]
    public async Task<ActionResult<Book>> GetByIsbn(string isbn)
    {
        var book = await _repo.GetByIsbnAsync(isbn);
        return book is null ? NotFound() : Ok(book);
    }

    // POST: /api/books
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Book book)
    {
        try
        {
            await _repo.AddAsync(book);
            return CreatedAtAction(nameof(GetByIsbn), new { isbn = book.Isbn }, book);
        }
        catch (Exception ex) 
        { 
            return BadRequest(new { error = ex.Message }); 
        }
    }

    // PUT: /api/books/9051234567897
    [HttpPut("{isbn}")]
    public async Task<ActionResult> Update(string isbn, [FromBody] Book book)
    {
        try
        {
            await _repo.UpdateAsync(isbn, book);
            return NoContent();
        }
        catch (KeyNotFoundException) 
        { 
            return NotFound(); 
        }
        catch (Exception ex) 
        { 
            return BadRequest(new { error = ex.Message }); 
        }
    }

    // DELETE: /api/books/9051234567897
    [HttpDelete("{isbn}")]
    public async Task<ActionResult> Delete(string isbn)
    {
        try
        {
            await _repo.DeleteAsync(isbn);
            return NoContent();
        }
        catch (KeyNotFoundException) 
        { 
            return NotFound(); 
        }
    }
}
