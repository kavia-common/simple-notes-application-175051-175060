using System;
using System.ComponentModel.DataAnnotations;
using NotesBackend.Models;

namespace NotesBackend.DTOs
{
    /// <summary>
    /// DTO used to create a new note.
    /// </summary>
    public sealed class CreateNoteDto
    {
        /// <summary>
        /// Title of the note. Required and must be non-empty/whitespace.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Title must not be empty.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Content/body of the note. Optional.
        /// </summary>
        public string? Content { get; set; }
    }

    /// <summary>
    /// DTO used to update an existing note.
    /// </summary>
    public sealed class UpdateNoteDto
    {
        /// <summary>
        /// Title of the note. Required and must be non-empty/whitespace.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Title must not be empty.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Content/body of the note. Optional.
        /// </summary>
        public string? Content { get; set; }
    }

    /// <summary>
    /// DTO representing a note returned to clients.
    /// </summary>
    public sealed class NoteDto
    {
        /// <summary>
        /// Unique identifier of the note.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the note.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Content/body of the note.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// When the note was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// When the note was last updated (UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    internal static class NoteMapping
    {
        public static NoteDto ToDto(this Note note) => new NoteDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        };
    }
}
