using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NotesBackend.Models;

namespace NotesBackend.Services
{
    /// <summary>
    /// Thread-safe in-memory implementation of INoteRepository using ConcurrentDictionary.
    /// Registered as a singleton to persist across requests for the app's lifetime.
    /// </summary>
    public sealed class InMemoryNoteRepository : INoteRepository
    {
        private readonly ConcurrentDictionary<Guid, Note> _store = new();

        public Task<IReadOnlyCollection<Note>> ListAsync()
        {
            var values = _store.Values
                .OrderByDescending(n => n.UpdatedAt)
                .ToArray();
            return Task.FromResult<IReadOnlyCollection<Note>>(values);
        }

        public Task<Note?> GetAsync(Guid id)
        {
            _store.TryGetValue(id, out var note);
            return Task.FromResult(note);
        }

        public Task<Note> CreateAsync(string title, string content)
        {
            var now = DateTime.UtcNow;
            var note = new Note
            {
                Id = Guid.NewGuid(),
                Title = title,
                Content = content,
                CreatedAt = now,
                UpdatedAt = now
            };

            _store[note.Id] = note;
            return Task.FromResult(note);
        }

        public Task<Note?> UpdateAsync(Guid id, string title, string content)
        {
            // Ensure atomic update: fetch existing, create updated copy and swap
            if (!_store.TryGetValue(id, out var existing))
            {
                return Task.FromResult<Note?>(null);
            }

            var updated = new Note
            {
                Id = existing.Id,
                Title = title,
                Content = content,
                CreatedAt = existing.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            _store[id] = updated;
            return Task.FromResult<Note?>(updated);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var removed = _store.TryRemove(id, out _);
            return Task.FromResult(removed);
        }

        /// <summary>
        /// Helper to seed sample notes (intended for development-time only).
        /// </summary>
        /// <param name="count">Number of notes to seed.</param>
        public void Seed(int count = 2)
        {
            if (_store.IsEmpty)
            {
                for (int i = 1; i <= count; i++)
                {
                    var now = DateTime.UtcNow.AddMinutes(-i);
                    var note = new Note
                    {
                        Id = Guid.NewGuid(),
                        Title = $"Sample Note {i}",
                        Content = $"This is the content of sample note {i}.",
                        CreatedAt = now,
                        UpdatedAt = now
                    };
                    _store[note.Id] = note;
                }
            }
        }
    }
}
