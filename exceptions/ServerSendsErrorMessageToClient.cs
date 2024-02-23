using lib;

namespace chatty.exceptions;

public class ServerSendsErrorMessageToClient : BaseDto
{
    public string? errorMessage { get; set; }
    public string? receivedMessage { get; set; }
}