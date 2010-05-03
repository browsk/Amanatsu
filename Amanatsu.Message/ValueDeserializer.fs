namespace Amanatsu.Message

open System.Text

type ValueDeserializer(data : seq<byte>) = 
    let mutable d = data
    
    member this.getByte =
        let value = Seq.head d
        d <- Seq.skip 1 d
        value
        
    member this.getInt = 
        let value = (Seq.take 4 d) |> Seq.fold (fun acc elem -> ((acc <<< 8) + (int)elem)) 0 
        d <- Seq.skip 4 d
        value
        
