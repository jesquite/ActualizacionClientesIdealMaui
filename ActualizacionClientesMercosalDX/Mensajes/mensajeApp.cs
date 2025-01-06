using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ActualizacionClientesIdealMaui.Mensajes;

public class mensajeApp : ValueChangedMessage<string>
{
    public mensajeApp(string value) : base(value)
    {

    }
}


