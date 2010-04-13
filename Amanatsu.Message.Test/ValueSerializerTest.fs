namespace Amanatsu.Message.Test


module public ValueSerializerTest =


    open Amanatsu.Message
    open Xunit
    open Xunit.Extensions
    open System.Collections.Generic

    type Comparer () = 
        interface IComparer<byte> with
            member this.Compare(x, y) =
                int (x - y)
        
    [<Fact>]
    let testMe() = Assert.True(true)
    
    [<Fact>]
    let Test_ValueSerializer_Can_Serialize_A_Single_Byte() = 
            let serializer = new ValueSerializer()
            serializer.append(4uy)
            let bytes = serializer.getBytes
            Assert.Equal(5, Seq.length bytes)
           
            let expected = [| 0uy; 0uy; 0uy; 1uy; 4uy|] :> seq<byte>
            
            
            Assert.Equal(expected, bytes)
