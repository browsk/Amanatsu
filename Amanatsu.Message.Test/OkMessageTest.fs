namespace Amanatsu.Message.Test

module Tests =

    open Xunit
    open Amanatsu.Message
     
    [<Fact>]
    let TestPacketType() = 
        let message = new Ok()
        Assert.Equal(MessageType.Ok, (message :> IMessage).PacketType)