using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using AmendoasDreams.Api.Models;

namespace AmendoasDreams.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DreamsController : ControllerBase
{
    private readonly IDbConnection _connection;

    public DreamsController(IDbConnection connection)
    {
        _connection = connection;
    }

    // GET api/dreams
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        const string sql = @"
            SELECT d.*, t.id, t.name
            FROM dreams d
            LEFT JOIN dream_tags dt ON dt.dream_id = d.id
            LEFT JOIN tags t ON t.id = dt.tag_id
            ORDER BY d.dream_date DESC";

        var dreamDictionary = new Dictionary<int, Dream>();

        await _connection.QueryAsync<Dream, Tag, Dream>(
            sql,
            (dream, tag) =>
            {
                if (!dreamDictionary.TryGetValue(dream.Id, out var existing))
                {
                    existing = dream;
                    dreamDictionary[dream.Id] = existing;
                }
                if (tag is not null)
                    existing.Tags.Add(tag);
                return existing;
            },
            splitOn: "id"
        );

        return Ok(dreamDictionary.Values);
    }

    // GET api/dreams/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        const string sql = @"
            SELECT d.*, t.id, t.name
            FROM dreams d
            LEFT JOIN dream_tags dt ON dt.dream_id = d.id
            LEFT JOIN tags t ON t.id = dt.tag_id
            WHERE d.id = @Id";

        var dreamDictionary = new Dictionary<int, Dream>();

        await _connection.QueryAsync<Dream, Tag, Dream>(
            sql,
            (dream, tag) =>
            {
                if (!dreamDictionary.TryGetValue(dream.Id, out var existing))
                {
                    existing = dream;
                    dreamDictionary[dream.Id] = existing;
                }
                if (tag is not null)
                    existing.Tags.Add(tag);
                return existing;
            },
            new { Id = id },
            splitOn: "id"
        );

        var result = dreamDictionary.Values.FirstOrDefault();
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // POST api/dreams
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Dream dream)
    {
        const string sql = @"
            INSERT INTO dreams (title, description, dream_date, sleep_time, wake_time, mood)
            VALUES (@Title, @Description, @DreamDate, @SleepTime, @WakeTime, @Mood)
            RETURNING id";

        var newId = await _connection.ExecuteScalarAsync<int>(sql, dream);
        dream.Id = newId;
        return CreatedAtAction(nameof(GetById), new { id = newId }, dream);
    }

    // PUT api/dreams/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Dream dream)
    {
        const string sql = @"
            UPDATE dreams
            SET title       = @Title,
                description = @Description,
                dream_date  = @DreamDate,
                sleep_time  = @SleepTime,
                wake_time   = @WakeTime,
                mood        = @Mood,
                updated_at  = NOW()
            WHERE id = @Id";

        var rows = await _connection.ExecuteAsync(sql, new { dream.Title, dream.Description, dream.DreamDate, dream.SleepTime, dream.WakeTime, dream.Mood, Id = id });
        if (rows == 0)
            return NotFound();

        return NoContent();
    }

    // DELETE api/dreams/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        const string sql = "DELETE FROM dreams WHERE id = @Id";
        var rows = await _connection.ExecuteAsync(sql, new { Id = id });
        if (rows == 0)
            return NotFound();

        return NoContent();
    }

    // POST api/dreams/5/tags/2
    [HttpPost("{id}/tags/{tagId}")]
    public async Task<IActionResult> AddTag(int id, int tagId)
    {
        const string checkDream = "SELECT COUNT(1) FROM dreams WHERE id = @Id";
        var dreamExists = await _connection.ExecuteScalarAsync<int>(checkDream, new { Id = id });
        if (dreamExists == 0)
            return NotFound(new { message = $"Sonho {id} não encontrado." });

        const string checkTag = "SELECT COUNT(1) FROM tags WHERE id = @Id";
        var tagExists = await _connection.ExecuteScalarAsync<int>(checkTag, new { Id = tagId });
        if (tagExists == 0)
            return NotFound(new { message = $"Tag {tagId} não encontrada." });

        const string sql = @"
            INSERT INTO dream_tags (dream_id, tag_id)
            VALUES (@DreamId, @TagId)
            ON CONFLICT DO NOTHING";

        await _connection.ExecuteAsync(sql, new { DreamId = id, TagId = tagId });
        return NoContent();
    }

    // DELETE api/dreams/5/tags/2
    [HttpDelete("{id}/tags/{tagId}")]
    public async Task<IActionResult> RemoveTag(int id, int tagId)
    {
        const string sql = @"
            DELETE FROM dream_tags
            WHERE dream_id = @DreamId AND tag_id = @TagId";

        var rows = await _connection.ExecuteAsync(sql, new { DreamId = id, TagId = tagId });
        if (rows == 0)
            return NotFound(new { message = $"Associação entre sonho {id} e tag {tagId} não encontrada." });

        return NoContent();
    }
}
