using Fleck;
using helloworld.Services;
using lib;

namespace helloworld.ClientEvents;

public class ClientWantsToSignInDto : BaseDto
{
    public string Username { get; set; } = null!;
}

public class ClientWantsToSignIn : BaseEventHandler<ClientWantsToSignInDto>
{
    public override Task Handle(ClientWantsToSignInDto dto, IWebSocketConnection socket)
    {
        StateService.Connections[socket.ConnectionInfo.Id].Username = dto.Username;
        socket.SendDto(new ServerConfirmsSignIn
        {
            Message = dto.Username + " has signed in."
        });
        return Task.CompletedTask;
    }
}

public class ServerConfirmsSignIn : BaseDto
{
    public string Message { get; set; } = null!;
}