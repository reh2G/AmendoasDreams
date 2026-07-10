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
}
