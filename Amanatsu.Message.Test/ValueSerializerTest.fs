namespace Amanatsu.Message.Test


module public ValueSerializerTests =


    open Amanatsu.Message
    open Xunit
    open Xunit.Extensions
    open System.Collections.Generic

    type Comparer () = 
        interface IComparer<byte> with
            member this.Compare(x, y) =
                int (x - y)
        
    [<Fact>]
    let Test_ValueSerializer_Can_Serialize_an_int() = 
        let serializer = new ValueSerializer()
        serializer.append(0x12345678);
        let bytes = serializer.getBytes
        Assert.Equal(8, Seq.length bytes)
        
        let expected = [| 0uy; 0uy; 0uy; 4uy; 0x12uy; 0x34uy; 0x56uy; 0x78uy |] |> Seq.ofArray
        
        Assert.Equal(expected, bytes)

    [<Fact>]
    let Test_ValueSerializer_Can_Serialize_A_Single_Byte() = 
        let serializer = new ValueSerializer()
        let b = 5uy
        serializer.append(b)
        let bytes = serializer.getBytes
        Assert.Equal(5, Seq.length bytes)
       
        let expected = [| 0uy; 0uy; 0uy; 1uy; b|] |> Seq.ofArray
        
        Assert.Equal(expected, bytes)

    [<Fact>]
    let Test_ValueSerializer_Can_Serialize_A_String() = 
        let serializer = new ValueSerializer()
        
        serializer.append("hello")
        let bytes = serializer.getBytes
        Assert.Equal(13, Seq.length bytes)
       
        let expected = [| 0uy; 0uy; 0uy; 9uy; 0uy; 0uy; 0uy; 5uy; (byte 'h'); (byte 'e'); (byte 'l'); (byte 'l'); (byte 'o') |] |> Seq.ofArray
        
        Assert.Equal(expected, bytes)


    [<Fact>]
    let Test_ValueSerializer_Can_Serialize_Multiple_Values() = 
        let serializer = new ValueSerializer()
        
        serializer.append("hello")
        serializer.append(0x00aa5500)
        serializer.append(0x01uy)
        let bytes = serializer.getBytes
        Assert.Equal(18, Seq.length bytes)
       
        let expected = [| 0uy; 0uy; 0uy; 14uy; 0uy; 0uy; 0uy; 5uy; (byte 'h'); (byte 'e'); (byte 'l'); (byte 'l'); (byte 'o'); 0x00uy; 0xaauy; 0x55uy; 0x00uy; 0x01uy |] |> Seq.ofArray
        
        Assert.Equal(expected, bytes)
        
    [<Fact>]
    let Test_ValueSerializer_getBytes_works_when_no_data_is_appended() = 
        let serializer = new ValueSerializer()
        
        let bytes = serializer.getBytes
        Assert.Equal(4, Seq.length bytes)
       
        let expected = [| 0uy; 0uy; 0uy; 0uy; |] |> Seq.ofArray
        
        Assert.Equal(expected, bytes)

    