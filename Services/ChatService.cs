using System;
using Microsoft.EntityFrameworkCore;
using ProgramaYA.Areas.Identity.Data;
// using SKWebChatBot.Data;

namespace SKWebChatBot.Services;

public class ChatService(ApplicationDbContext _dbContext)
{
    public async Task AddMessageAsync(string sessionId, string message, string sender)
    {
        var chatMessage = new ChatMessage
        {
            Sender = sender,
            SessionId = sessionId,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        _dbContext.ChatMessages.Add(chatMessage);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(string sessionId)
    {
        return await _dbContext.ChatMessages
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }
}