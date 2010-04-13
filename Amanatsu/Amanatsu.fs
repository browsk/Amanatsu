open System
open System.Net
open System.Net.Sockets
open System.Threading
open System.Text
open System.Collections.Generic
open System.Linq
open Amanatsu.Message

let processDiscoveryQuery(messageId : int, sender : IPEndPoint) =
    let response = {ListenAddress = ""; ListenPort = ""; Version = "XBMSP-1.0 1.0 Amanatsu Server 0.1\n"; Comment = "Hello"}
    
    let serializer = new ValueSerializer()
    serializer.append MessageType.DiscoveryReply
    serializer.append messageId
    serializer.append response.ListenAddress
    serializer.append response.ListenPort
    serializer.append response.Version
    serializer.append response.Comment
    
    //let data = serializer.getBytes
    
    

let processMessage(packet : byte[], sender : IPEndPoint) =
    let packetLength = packet.[0] <<< 24 ||| packet.[1] <<< 16 ||| packet.[2] <<< 8 ||| packet.[3]
    let packetType = packet.[4]
    let messageId = (int packet.[5] <<< 24) ||| (int packet.[6] <<< 16) ||| (int packet.[7] <<< 8) ||| (int packet.[8])
    
    printfn "Packet length is %A and type is %A" packetLength packetType
    
    processDiscoveryQuery(messageId, sender)
        
let discoveryListener(client : UdpClient) = 
    let mutable sender = new IPEndPoint(IPAddress.Broadcast, 0)
    let data = client.Receive(&sender)
    printfn "Received %A bytes from %A" (data.Length) sender
    processMessage(data, sender)
    
let discoveryServiceMain() = 
    let socket = new UdpClient(1400)
    discoveryListener(socket)

let serializer = new ValueSerializer()

serializer.append "hello"

let bytes = serializer.getBytes

printfn "%A" bytes

discoveryServiceMain()

