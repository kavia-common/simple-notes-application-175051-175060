using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NotesBackend.Models;

namespace NotesBackend.Services
{
    /// <summary>
    /// Repository abstraction for storing and retrieving notes.
    /// </summary>
    public interface INoteRepository
    {
        // PUBLIC_INTERFACE
        /// <summary>
        /// Gets all notes.
        /// </summary>
        /// <returns>List of notes.</returns>
        Task<IReadOnlyCollection<Note>> ListAsync();

        // PUBLIC_INTERFACE
        /// <summary>
        /// Gets a note by its identifier.
        /// </summary>
        /// <param name="id">Note id.</param>
        /// <returns>Note if found; otherwise null.</returns>
        Task<Note?> GetAsync(Guid id);

        // PUBLIC_INTERFACE
        /// <summary>
        /// Creates a new note with the specified title and content.
        /// </summary>
        /// <param name="title">Title of the note.</param>
        /// <param name="content">Content of the note.</param>
        /// <returns>The created note.</returns>
        Task<Note> CreateAsync(string title, string content);

        // PUBLIC_INTERFACE
        /// <summary>
        /// Updates an existing note if found.
        /// </summary>
        /// <param name="id">Note id.</param>
        /// <param name="title">New title.</param>
        /// <param name="content">New content.</param>
        /// <returns>Updated note if found; otherwise null.</returns>
        Task<Note?> UpdateAsync(Guid id, string title, string content);

        // PUBLIC_INTERFACE
        /// <summary>
        /// Deletes a note by id.
        /// </summary>
        /// <param name="id">Note id.</param>
        /// <returns>True if deleted; false if not found.</returns>
        Task<bool> DeleteAsync(Guid id);
    }
}
