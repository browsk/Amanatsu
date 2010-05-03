namespace Amanatsu.Message.Test


module public ValueDeserializerTests =


    open Amanatsu.Message
    open Xunit
    open Xunit.Extensions

    [<Fact>]
    let Test_Can_Get_Byte() =
        let data = [| 12uy; 13uy; 14uy |] |> Seq.ofArray
        
        let deserializer = new ValueDeserializer(data) 
        
        let value = deserializer.getByte
        
        Assert.Equal(12uy, value)

    [<Fact>]
    let Test_Can_Get_Consecutive_Byte() =
        let data = [| 12uy; 13uy; 14uy |] |> Seq.ofArray
        
        let deserializer = new ValueDeserializer(data) 
        
        Assert.Equal(12uy, deserializer.getByte)
        Assert.Equal(13uy, deserializer.getByte)
        Assert.Equal(14uy, deserializer.getByte)

    [<Fact>]
    let Test_Can_Get_Int() =
        let data = [| 1uy; 2uy; 3uy ;4uy |] |> Seq.ofArray
        
        let deserializer = new ValueDeserializer(data) 
        
        Assert.Equal(0x01020304, deserializer.getInt)


    [<Fact>]
    let Test_Can_Get_Consecutive_Ints() =
        let data = [| 1uy; 2uy; 3uy; 4uy; 0uy; 2uy; 21uy; 3uy |] |> Seq.ofArray
        
        let deserializer = new ValueDeserializer(data) 
        
        Assert.Equal(0x01020304, deserializer.getInt)
        Assert.Equal(0x00021503, deserializer.getInt)
