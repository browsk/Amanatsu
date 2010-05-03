open System
open System.Net
open System.Net.Sockets
open System.Threading
open System.Text
open System.Collections.Generic
open System.Linq
open Amanatsu.Message
open Amanatsu.Command

let mutable anyErrors = false

let intValue(data: byte[]) = 
    (int)(data.[0] <<< 24) + (int)(data.[1] <<< 16) + (int)(data.[2] <<< 8) + (int)(data.[3])
    

let handleClient (client: TcpClient) = async {
    use stream = client.GetStream()
    //stream.WriteByte(0uy);
//    do! stream.AsyncWrite(quote, 0, 1)  // write header
//    while true do
//        do! asyncWriteStockQuote(stream) 

    stream.ReadTimeout <- Timeout.Infinite
    
    let (buffer : byte[]) = Array.create 512 0uy
    let socketClosed = ref false
    while not !socketClosed do
        let lengthBytesRead = stream.Read(buffer, 0, 4)
        
        printfn "Read %A bytes : %A" lengthBytesRead buffer
        
        let packetLen = intValue buffer
        
        let bytesRead = stream.Read(buffer, 0, packetLen)
        
        printfn "Read %A bytes for message type %A: %A" bytesRead buffer.[0] buffer
        
        processCommand (Array.toList buffer)
        
        socketClosed := (bytesRead = 0)
        
    stream.Close();
    
    client.Close();
    
    printfn "Closed socket"
    }

let serverMain() = 
    let socket = new TcpListener(IPAddress.Any, 1400)
    socket.Start()
    
    let thread = new Thread(ThreadStart(fun _ -> 
        while true do
            let client = socket.AcceptTcpClient()
            let stream = client.GetStream()
            
            let response = Encoding.ASCII.GetBytes("XBMSP-1.0 1.0 Amanatsu XBMC Server\n")
            
            
            stream.Write(response, 0, response.Length)

            //let (buffer : byte[]) = Array.create 512 0uy
            let buffer : List<byte> = new List<byte>()
            
            let responseRead = ref false
            
            while not !responseRead do
                let char = stream.ReadByte();
                if (char = (int)'\n' || char < 0) then
                    responseRead := true
                else
                    buffer.Add((byte)char)
            
            printfn "We are talking to %A" (Encoding.ASCII.GetString(buffer.ToArray()))
            
            Async.Start ( async {
                try
                    use _holder = client
                    do! handleClient client
                with e ->
                    if not(anyErrors) then
                        anyErrors <- true
                        printfn "Error : %A" e
                    raise e
                } )
    ), IsBackground = true)
    
    thread.Start()
    
            

let processDiscoveryQuery(messageId : int, sender : IPEndPoint) =
    let response = {ListenAddress = ""; ListenPort = "1400"; Version = "XBMSP-1.0 1.0 Amanatsu Server 0.1\n"; 
                    Comment = Dns.GetHostName()}
    
    let serializer = new ValueSerializer()
    serializer.append MessageType.DiscoveryReply
    serializer.append messageId
    serializer.append response.ListenAddress
    serializer.append response.ListenPort
    serializer.append response.Version
    serializer.append response.Comment
    
    let data = serializer.getBytes
    
    let client = new UdpClient()
    let bytesSent = client.Send(data.ToArray(), data.Count(), sender)
    
    printfn "Sent %A bytes" bytesSent
    
    

let processMessage(packet : byte[], sender : IPEndPoint) =
    let packetLength = packet.[0] <<< 24 ||| packet.[1] <<< 16 ||| packet.[2] <<< 8 ||| packet.[3]
    let packetType = packet.[4]
    let messageId = (int packet.[5] <<< 24) ||| (int packet.[6] <<< 16) ||| (int packet.[7] <<< 8) ||| (int packet.[8])
    
    printfn "Packet length is %A and type is %A" packetLength packetType
    
    processDiscoveryQuery(messageId, sender)
        
let discoveryListener(client : UdpClient) = 
    while true do
        let mutable sender = new IPEndPoint(IPAddress.Any, 0)
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

serverMain();

discoveryServiceMain()

while true do
    do Thread.Sleep 1000
