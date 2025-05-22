namespace Turnierplan.Core.Test.Unit.PublicId;

public sealed class PublicIdTest
{
    public static readonly TheoryData<ulong, string> TestData = new()
    {
        { 14694492078790806212, "MvtUrDfHgrE" },
        { 10227443303491505457, "I3vLX9JDw0x" },
        { 18026862037545437924, "PosR4NC3srk" },
        { 17128603320592063720, "O21BfgGd1zo" },
        { 15237898806672375662, "NN35DsXDj9u" },
        { 10099328368597058826, "IwoBaSIQMUK" },
        { 12544539674150040574, "K4XKF6sgfP_" },
        { 3208640904954793886, "CyHYecnnk_e" },
        { 11032232890197103020, "JkaXVzDlFGs" },
        { 13305492478795879879, "Limmun4QpXH" },
        { 7906740836077405291, "G26YuYReARr" },
        { 16111645196097995871, "N_YD-FUjMRf" },
        { 10896749135394172857, "Jc5B5rttu_5" },
        { 15755765703323558652, "NqnuW324iL8" },
        { 17593368711005807303, "PQoM6EF_SLH" },
        { 9816714365177795613, "Ig7_bQ15ZQd" },
        { 13504434704989135500, "LtpY9CLVaKM" },
        { 5669058873685316409, "E6siwJ4yhM5" },
        { 3107095892833835599, "Csen0Cfb5pP" },
        { 16858692694752741228, "On2G69INcds" },
        { 16214583574505971671, "OEFxdo7T9-X" },
        { 14385350968439707949, "MejCH0rK5kt" },
        { 14587856443549569985, "MpyeiVIKlPB" },
        { 1187325921212630135, "BB6OqJ2fgR3" },
        { 10264720171480258201, "I5znJxz3haZ" },
        { 9023087281429344555, "H04chN68PEr" },
        { 16898039717901881158, "OqB5ZgB_89G" },
        { 4444298463072247721, "D2tUee-PF_p" },
        { 540512069514020992, "AeASN0MMciA" },
        { 7903950238787527798, "G2weN2BS2R2" },
        { 12201166447635948152, "KlTQD1UwWJ4" },
        { 14607434961050607967, "Mq4CLQdCyFf" },
        { 13051705989080773089, "LUg_XI3-3Hh" },
        { 12430837525053662860, "KyDNN4da2aM" },
        { 2555055226288677300, "CN1YTDocSG0" },
        { 4855609018850276308, "ENilq0wwPPU" },
        { 2682859496371908869, "CU7bn9fh-EF" },
        { 4053244192903576824, "DhABB5R7rz4" },
        { 14146187856863365494, "MRRWuf2SQl2" },
        { 14223273772598391638, "MVjOCNW23NW" },
        { 2996625396552387587, "CmWJuvea0wD" },
        { 10416140679232141542, "JCNkMK8ILjm" },
        { 15120037998152776533, "NHVKnMuTftV" },
        { 15898008010075073740, "NyhEg3M1uzM" },
        { 16361614570615248046, "OMQIcOxYJSu" },
        { 8506371767516592645, "HYMtA3KPPoF" },
        { 17491578031857829232, "PK_kYut6YVw" },
        { 15419835193187904315, "NX_QmkRCCs7" },
        { 6057003537636590845, "FQOzKBIHQT9" },
        { 14560629344866341525, "MoRvz5e4xKV" }
    };

    [Fact]
    public void PublicId___Default_Constructor___Generates_Random_Id()
    {
        const int count = 1000;

        Enumerable.Range(0, count).Select(_ => new Core.PublicId.PublicId()).Should().OnlyHaveUniqueItems();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(89145)]
    [InlineData(1561234)]
    [InlineData(12365265344235)]
    [InlineData(23564743928945)]
    [InlineData(0b01000000_00000000_00000000_00000000_00000000_00000000_00000000_00000000)]
    [InlineData(0b10000000_00000000_00000000_00000000_00000000_00000000_00000000_00000000)]
    public void PublicId__Constructor_With_Unsigned_Long___Works_As_Expected(ulong parameter)
    {
        new Core.PublicId.PublicId(parameter).Value.Should().Be(parameter);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void PublicId__Constructor_With_String___Works_As_Expected(ulong value, string representation)
    {
        var publicId = new Core.PublicId.PublicId(representation);

        publicId.Value.Should().Be(value);
        publicId.ToString().Should().Be(representation);
        $"{publicId}".Should().Be(representation);
    }

    [Theory]
    [InlineData("Zg==")]
    [InlineData("wESkZP_b32kk7")]
    [InlineData("wESkZP_b32k===")]
    [InlineData("wESkZP_b32kwESkZP_b32k==")]
    public void PublicId__Constructor_With_String_Called_With_Invalid_Inputs___Throws_Exception(string input)
    {
        var func = () => new Core.PublicId.PublicId(input);
        func.Should().ThrowExactly<ArgumentException>().WithMessage("String representation must be 11 characters long. (Parameter 'representation')");
    }

    [Theory]
    [InlineData("/0oVOc_f_2M", 0)]
    [InlineData(":0oVOc_f_2M", 0)]
    [InlineData("@0oVOc_f_2M", 0)]
    [InlineData("[0oVOc_f_2M", 0)]
    [InlineData("`0oVOc_f_2M", 0)]
    [InlineData("{0oVOc_f_2M", 0)]
    [InlineData("Q0oVOc_f_2M", 0)]
    [InlineData("30oVOc_f_2M", 0)]
    [InlineData("A0oV.c_f_2M", 4)]
    [InlineData("A0oVOc_)_2M", 7)]
    [InlineData("A0oVOc_f_2`", 10)]
    public void PublicId__Constructor_With_String_Called_With_Invalid_Input_Chars___Throws_Exception(string input, int invalidIndex)
    {
        var func = () => new Core.PublicId.PublicId(input);
        func.Should().ThrowExactly<ArgumentException>().WithMessage($"String representation contains invalid character at index {invalidIndex}. (Parameter 'representation')");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(14832452345)]
    [InlineData(153645342)]
    [InlineData(3438942134)]
    public void PublicId___Equality_Check___Works_As_Expected(ulong value)
    {
        var publicId1 = new Core.PublicId.PublicId(value);
        var publicId2 = new Core.PublicId.PublicId(value);
        var publicId3 = new Core.PublicId.PublicId(value - 1);
        var publicId4 = new Core.PublicId.PublicId(value + 2);

        (publicId1 == publicId2).Should().BeTrue();
        (publicId2 == publicId1).Should().BeTrue();

        (publicId1 == publicId3).Should().BeFalse();
        (publicId1 == publicId4).Should().BeFalse();

        (publicId2 == publicId3).Should().BeFalse();
        (publicId2 == publicId4).Should().BeFalse();

        (publicId3 == publicId4).Should().BeFalse();
    }

    [Theory]
    [InlineData(long.MinValue)]
    [InlineData(-9223372036854775807)]
    [InlineData(-223372036854775807)]
    [InlineData(-23372036854775807)]
    [InlineData(-3372036854775807)]
    [InlineData(-372036854775807)]
    [InlineData(long.MaxValue)]
    [InlineData(223372036854775807)]
    [InlineData(23372036854775807)]
    [InlineData(3372036854775807)]
    [InlineData(372036854775807)]
    [InlineData(-3)]
    [InlineData(-2)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void PublicId___ToSignedInt64___Works_As_Expected(long value)
    {
        var publicId = new Core.PublicId.PublicId(value);

        publicId.Value.Should().Be(unchecked((ulong)value));

        var publicId2 = new Core.PublicId.PublicId(publicId.Value);

        publicId2.ToSignedInt64().Should().Be(value);
    }

    [Fact]
    public void PublicId___Round_Trip_With_Random_Values___Works_As_Expected()
    {
        var rng = new Random(32464576);
        var seenStrings = new HashSet<string>();

        var buffer = new byte[8];

        for (var i = 0; i < 5_000_000; i++)
        {
            rng.NextBytes(buffer);

            var publicId = new Core.PublicId.PublicId();
            var stringRepresentation = publicId.ToString();
            var convertedBack = new Core.PublicId.PublicId(stringRepresentation);

            publicId.Value.Should().Be(convertedBack.Value);
            seenStrings.Add(stringRepresentation).Should().BeTrue();
        }
    }

    [Fact]
    public void PublicId___Consecutive_Public_Ids___Do_Not_Serialize_To_Same_String()
    {
        for (var i = 0; i < 1500; i++)
        {
            var current = new Core.PublicId.PublicId(i);
            var next = new Core.PublicId.PublicId(i + 1);

            current.ToString().Should().NotBe(next.ToString());
        }
    }

    [Theory]
    [InlineData("C1dnikX8vL4", "C1dnikX8vL5", "C1dnikX8vL6", "C1dnikX8vL7", "C1dnikX8vL8")]
    [InlineData("F137TsVtQPQ", "F137TsVtQPR", "F137TsVtQPS", "F137TsVtQPT", "F137TsVtQPU")]
    [InlineData("ChpBHYkKW84", "DhpBHYkKW84", "EhpBHYkKW84", "FhpBHYkKW84", "GhpBHYkKW84")]
    public void PublicId___Consecutive_String_Representations___Do_Not_Deserialize_To_Same_Public_Id(params string[] inputs)
    {
        for (var i = 1; i < inputs.Length; i++)
        {
            var previous = new Core.PublicId.PublicId(inputs[i - 1]);
            var current = new Core.PublicId.PublicId(inputs[i]);

            previous.Value.Should().NotBe(current.Value);
        }
    }
}
