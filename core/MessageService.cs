using helloworld.Infrastructure;

namespace helloworld.core;

public class MessageService
{
    private readonly MessageRepository _messageRepository;
    
    public MessageService(MessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }
    
    public async Task<int> InsertMessage(string content, int room, string nickname)
    {
       return await _messageRepository.InsertMessage(content, room, nickname);
    }
    
    public async Task<List<Message>> GetRecentMessages(int room)
    {
        return await _messageRepository.GetRecentMessages(room);
    }
}