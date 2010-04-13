namespace Amanatsu.Message

open System.Text
open System.Collections.Generic

type ValueSerializer() = 
    let data : List<byte> = new List<byte>();

    member this.append(x : byte) =
        data.Add(x)

    member this.append(x : int) =
        for i = 3 downto 0 do 
            this.append (byte (x >>> (i <<< 3)))

    member this.append(x : string) =
        this.append x.Length
        let stringData = Encoding.ASCII.GetBytes(x)
        Array.iter(fun elem -> this.append (byte elem)) stringData

    member this.append(x : MessageType) =
        this.append (byte x)

    member this.getBytes = 
        let length = Seq.init 4 (fun n -> (byte (data.Count >>> ((3 - n) <<< 3))))
        Seq.append length (data |> Seq.cast<byte>)
