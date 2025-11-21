using Microsoft.AspNetCore.Http.HttpResults;
using NSwag.Annotations;
using NotesBackend.DTOs;
using NotesBackend.Models;
using NotesBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "Notes API";
    settings.Description = "A simple Minimal APIs backend for managing notes. CRUD over an in-memory repository.";
    settings.Version = "1.0.0";
    settings.DocumentName = "v1";
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register in-memory repository as singleton
builder.Services.AddSingleton<INoteRepository, InMemoryNoteRepository>();

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Configure OpenAPI/Swagger
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.Path = "/docs";
});

// Health check endpoint
// PUBLIC_INTERFACE
app.MapGet("/", () => new { message = "Healthy" })
   .WithSummary("Health check")
   .WithDescription("Returns a simple healthy message to indicate the service is up.");

// Development-only seeding
if (app.Environment.IsDevelopment())
{
    var repo = app.Services.GetRequiredService<INoteRepository>() as InMemoryNoteRepository;
    repo?.Seed(2);
}

// Notes endpoints under /api/notes
var notes = app.MapGroup("/api/notes")
               .WithTags("Notes");

/// <summary>
/// Gets all notes.
/// </summary>
notes.MapGet("/", async (INoteRepository repo) =>
{
    var list = await repo.ListAsync();
    return Results.Ok(list.Select(n => n.ToDto()));
})
.WithName("ListNotes")
.WithSummary("List notes")
.WithDescription("Returns all notes available in the repository.")
.Produces<IEnumerable<NoteDto>>(StatusCodes.Status200OK);

/// <summary>
/// Gets a note by id.
/// </summary>
notes.MapGet("/{id:guid}", async Task<Results<Ok<NoteDto>, NotFound>> (Guid id, INoteRepository repo) =>
{
    var note = await repo.GetAsync(id);
    if (note is null) return TypedResults.NotFound();
    return TypedResults.Ok(note.ToDto());
})
.WithName("GetNoteById")
.WithSummary("Get note")
.WithDescription("Fetches a note by its unique identifier.")
.Produces<NoteDto>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status404NotFound);

/// <summary>
/// Creates a new note.
/// </summary>
notes.MapPost("/", async Task<Results<Created<NoteDto>, ValidationProblem>> (CreateNoteDto dto, INoteRepository repo, HttpContext ctx) =>
{
    // Manual validation enforcing non-empty title beyond DataAnnotations in Minimal APIs
    if (string.IsNullOrWhiteSpace(dto.Title))
    {
        return TypedResults.ValidationProblem(new Dictionary<string, string[]>
        {
            ["Title"] = new[] { "Title is required and must not be empty." }
        });
    }

    var created = await repo.CreateAsync(dto.Title.Trim(), dto.Content?.Trim() ?? string.Empty);
    var resultDto = created.ToDto();

    var location = $"{ctx.Request.Scheme}://{ctx.Request.Host}/api/notes/{resultDto.Id}";
    return TypedResults.Created(location, resultDto);
})
.WithName("CreateNote")
.WithSummary("Create note")
.WithDescription("Creates a new note. Title must be non-empty.")
.Produces<NoteDto>(StatusCodes.Status201Created)
.ProducesValidationProblem(StatusCodes.Status400BadRequest);

/// <summary>
/// Updates an existing note.
/// </summary>
notes.MapPut("/{id:guid}", async Task<Results<Ok<NoteDto>, NotFound, ValidationProblem>> (Guid id, UpdateNoteDto dto, INoteRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title))
    {
        return TypedResults.ValidationProblem(new Dictionary<string, string[]>
        {
            ["Title"] = new[] { "Title is required and must not be empty." }
        });
    }

    var updated = await repo.UpdateAsync(id, dto.Title.Trim(), dto.Content?.Trim() ?? string.Empty);
    if (updated is null) return TypedResults.NotFound();

    return TypedResults.Ok(updated.ToDto());
})
.WithName("UpdateNote")
.WithSummary("Update note")
.WithDescription("Updates the title and content of an existing note.")
.Produces<NoteDto>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status404NotFound)
.ProducesValidationProblem(StatusCodes.Status400BadRequest);

/// <summary>
/// Deletes a note by id.
/// </summary>
notes.MapDelete("/{id:guid}", async Task<Results<NoContent, NotFound>> (Guid id, INoteRepository repo) =>
{
    var deleted = await repo.DeleteAsync(id);
    return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
})
.WithName("DeleteNote")
.WithSummary("Delete note")
.WithDescription("Deletes the note identified by the provided id.")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status404NotFound);

app.Run();