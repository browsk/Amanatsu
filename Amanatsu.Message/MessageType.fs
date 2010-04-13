namespace Amanatsu.Message

type MessageType = 
    | Ok = 1
    | Error = 2
    | Handle = 3
    | Null = 10
    | DiscoveryQuery = 90
    | DiscoveryReply = 91