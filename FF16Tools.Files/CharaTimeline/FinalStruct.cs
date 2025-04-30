namespace FF16Tools.Files.CharaTimeline
{

    public class InternalFinalStruct : BaseStruct
    {
        public override int _totalSize => 0x28;

        int UnkType;
        int[] Pad = new int[9];
    }

    public class FinalStruct : BaseStruct
    {
        public override int _totalSize => 0x2C;

        [OffsetAttribute("Sub", typeof(InternalFinalStruct))]
        int DataOffset;
    }
}
