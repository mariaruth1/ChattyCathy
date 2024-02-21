using chatty.Infrastructure;

namespace chatty.core;

public class MessageService
{
    private readonly MessageRepository _messageRepository;
    
    public MessageService(MessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }
    
    public async Task<int> InsertMessage(string content, int room, string nickname, DateTime date)
    {
        return await _messageRepository.InsertMessage(content, room, nickname, date);
    }
    
    public async Task<List<ChatMessage>> GetRecentMessages(int room)
    {
        return await _messageRepository.GetRecentMessages(room);
    }
}