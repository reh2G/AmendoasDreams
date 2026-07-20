using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using AmendoasDreams.Api.Models;

namespace AmendoasDreams.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly IDbConnection _connection;

    public TagsController(IDbConnection connection)
    {
        _connection = connection;
    }

    // GET api/tags
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        const string sql = "SELECT id, name FROM tags ORDER BY name";
        var tags = await _connection.QueryAsync<Tag>(sql);
        return Ok(tags);
    }

    // GET api/tags/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        const string sql = "SELECT id, name FROM tags WHERE id = @Id";
        var tag = await _connection.QueryFirstOrDefaultAsync<Tag>(sql, new { Id = id });

        if (tag is null)
            return NotFound();

        return Ok(tag);
    }

    // POST api/tags
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Tag tag)
    {
        const string sql = @"
            INSERT INTO tags (name)
            VALUES (@Name)
            RETURNING id";

        var newId = await _connection.ExecuteScalarAsync<int>(sql, tag);
        tag.Id = newId;
        return CreatedAtAction(nameof(GetById), new { id = newId }, tag);
    }

    // DELETE api/tags/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        const string sql = "DELETE FROM tags WHERE id = @Id";
        var rows = await _connection.ExecuteAsync(sql, new { Id = id });

        if (rows == 0)
            return NotFound();

        return NoContent();
    }
}
