using System;

namespace NotesBackend.Models
{
    /// <summary>
    /// Domain model representing a Note entity stored in the repository.
    /// </summary>
    public sealed class Note
    {
        /// <summary>
        /// Unique identifier for the note.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the note. Must be non-empty when creating/updating.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Content/body of the note. Can be empty.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the note was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when the note was last updated (UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
