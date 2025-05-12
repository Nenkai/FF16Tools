namespace FF16Tools.Files.Timelines.Chara
{

    public class InternalFinalStruct : BaseStruct
    {
        public override int TotalSize => 0x28;

        public int UnkType;
        public int[] Pad = new int[9];
    }

    public class FinalStruct : BaseStruct
    {
        public override int TotalSize => 0x4;        
        public int DataOffset;

        [RelativeField("DataOffset")]
        public InternalFinalStruct Sub;
    }
}
