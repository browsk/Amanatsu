namespace Amanatsu.Message

type public Ok() = 
    interface IMessage with 
        member this.PacketType = MessageType.Ok