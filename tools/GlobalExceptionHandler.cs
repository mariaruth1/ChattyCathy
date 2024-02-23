using System.Data;
using chatty.exceptions;
using chatty.exceptions.customExceptions;
using chatty.exceptions.events;
using Fleck;
using Serilog;

namespace chatty.tools;

public static class GlobalExceptionHandler
{
    public static void Handle(this Exception exception, IWebSocketConnection socket, string? message)
    {
        Log.Error(exception, "Global exception handler");
        socket.SendDto(new ServerSendsErrorMessageToClient
        {
            receivedMessage = message,
            errorMessage = exception.Message
        });

        switch (exception)
        {
            case DuplicateNameException:
                socket.SendDto(new ServerRejectsSignIn());
                break;
            case InvalidUsernameException:
                socket.SendDto(new ServerRejectsSignIn());
                break;
            case RoomLimitException:
                socket.SendDto(new ServerRejectsEnterRoom());
                break;
        }
    }
}