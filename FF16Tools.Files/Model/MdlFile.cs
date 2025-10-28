using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using System.Text;
using Syroot.BinaryData;
using FF16Tools.Shared;
using FF16Tools.Files.Model.ExternalContent;

namespace FF16Tools.Files.Model;

public class MdlFile
{
    public uint MainHeaderSize => Version >= 20 ? 0xA8u : 0x98u;

    /// <summary>
    /// Version.
    /// </summary>
    public byte Version;

    public byte MainFlags;

    public byte ModelType;

    public byte UnkFlags_0x16;

    /// <summary>
    /// Attribute sets which determine what attributes to use from the attribute list.
    /// </summary>
    public List<MdlFlexVertexInfo> AttributeSets { get; set; } = [];

    /// <summary>
    /// Attribute that determine the layout of vertices in a vertex buffer.
    /// </summary>
    public List<MdlFlexVertexAttribute> Attributes { get; set; } = [];

    /// <summary>
    /// The min and max bounding box of the entire model.
    /// </summary>
    public float[] BoundingBox { get; private set; } = new float[8];

    /// <summary>
    /// Unknown values
    /// </summary>
    public byte[] UnknownEntries { get; private set; } = new byte[0x10];

    /// <summary>
    /// A list of material files that are externally referenced. 
    /// </summary>
    public List<string> MaterialFileNames { get; set; } = [];

    /// <summary>
    /// A list of vertex buffers used to store compressed vertex buffer data.
    /// </summary>
    public ModelBuffer[] vBuffers { get; private set; } = new ModelBuffer[8];

    /// <summary>
    /// A list of index buffers used to store compressed index buffer data.
    /// </summary>
    public ModelBuffer[] idxBuffers { get; private set; } = new ModelBuffer[8];

    /// <summary>
    /// Buffer with an unknown purpose.
    /// </summary>
    public ModelBuffer UnknownBuffer1 = new ModelBuffer();

    /// <summary>
    /// Buffer with an unknown purpose.
    /// </summary>
    public ModelBuffer UnknownBuffer2 = new ModelBuffer();

    /// <summary>
    /// The model spec header
    /// </summary>
    public MeshSpecsHeader SpecsHeader { get; set; } = new();

    /// <summary>
    /// A list of level of detail meshes
    /// </summary>
    public List<MdlLODModelInfo> LODModels = [];

    /// <summary>
    /// A list of meshes used by the LODs.
    /// </summary>
    public List<MdlMeshInfo> MeshInfos = [];

    /// <summary>
    /// A list of sub draw calls for drawing less faces.
    /// </summary>
    public List<SubDrawPart> SubDrawCalls { get; set; } = [];

    /// <summary>
    /// A list of muscle joints for muscle calculations.
    /// </summary>
    public List<MdlJointMuscleEntry> JointMuscles { get; set; } = [];

    /// <summary>
    /// A list of joint faces with an unknown purpose.
    /// </summary>
    public List<MdlUnkJointParam> JointFacesEntries { get; set; } = [];

    /// <summary>
    /// A list of joints that have a joint name and position.
    /// </summary>
    public List<JointEntry> Joints { get; set; } = [];

    /// <summary>
    /// A list of bounding boxes used to attach to joints for culling.
    /// </summary>
    private List<MdlJointBounding> JointBoundings { get; set; } = [];

    public float[] JointMaxBounds { get; set; } = [float.MinValue, float.MinValue, float.MinValue,
                                     float.MaxValue, float.MaxValue, float.MaxValue];

    /// <summary>
    /// A list of joint names.
    /// These joints are all rigged and referenced to vertex data.
    /// </summary>
    public List<string> JointNames { get; set; } = [];

    /// <summary>
    /// A list of joint face names.
    /// </summary>
    public List<string> JointFaceNames { get; set; } = [];

    /// <summary>
    /// A list of joint muscle names.
    /// </summary>
    public List<string> JointMuscleNames { get; set; } = [];

    /// <summary>
    /// A list of material names. These always match the amount of material files.
    /// </summary>
    public List<string> MaterialNames { get; set; } = [];

    /// <summary>
    /// A list of parts with an unknown purpose.
    /// </summary>
    public List<string> Options { get; set; } = [];

    /// <summary>
    /// A list of parts with an unknown purpose.
    /// </summary>
    public List<string> AdditionalParts { get; set; } = [];

    /// <summary>
    /// A list of parts with an unknown purpose.
    /// </summary>
    public List<string> VFXEntries { get; set; } = [];

    //Extra section at the end with an unknown purpose
    private byte[] ExtraSection { get; set; } = [];
    public MdlExternalContentHeader ExternalContentHeader { get; set; } = new();

    //MCEX section used to store embedded data like collision.
    private byte[] McexSection { get; set; } = [];

    //Extra section at the end with an unknown purpose
    private byte[] ExtraSection2 { get; set; } = [];

    // Add field to store data for bones not in base MDL file
    private List<GeneratedJointData>? _generatedJoints;

    // Add generated data to existing joint collections
    public void SetGeneratedJoints(IEnumerable<GeneratedJointData> joints)
    {
        _generatedJoints = new List<GeneratedJointData>(joints);

        foreach (var genJoint in _generatedJoints.OrderBy(j => j.Index))
        {
            Joints.Add(genJoint.Joint);
            JointBoundings.Add(genJoint.Bounding);
            JointNames.Add(genJoint.Name);
        }
    }


    public static MdlFile Open(string file)
    {
        using var fs = File.OpenRead(file);
        var mdlFile = new MdlFile();
        mdlFile.Read(fs);
        return mdlFile;
    }

    public void Save(string path)
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        Save(fs);
    }

    public void Save(Stream stream)
    {
        Write(new SmartBinaryStream(stream));
    }

    #region Reading
    public void Read(Stream stream)
    {
        SmartBinaryStream bs = new SmartBinaryStream(stream);

        if (bs.ReadUInt32() != 0x204C444D) // 'MDL '
            throw new Exception("Invalid model file, magic did not match.");

        Version = bs.Read1Byte();

        if (Version != 28)
            Console.WriteLine($"WARN: Only MDL version 28 is supported. Version: {Version}");

        MainFlags = bs.Read1Byte();
        if (MainFlags != 1)
            Console.WriteLine($"WARN: Model flag 1 is missing.");

        ModelType = bs.Read1Byte();
        bs.Read1Byte();

        uint section1Size = bs.ReadUInt32();
        uint section2Size = bs.ReadUInt32();
        ushort materialNamesCount = bs.ReadUInt16();
        ushort flexVertAttributeCount = bs.ReadUInt16();
        byte flexVertInfoCount = bs.Read1Byte();
        byte lodCount = bs.Read1Byte();
        UnkFlags_0x16 = bs.Read1Byte();
        bs.Read1Byte();
        uint[] vBuffersOffsets = bs.ReadUInt32s(8);
        uint[] idxBuffersOffsets = bs.ReadUInt32s(8);
        uint[] vBuffersSizes = bs.ReadUInt32s(8);
        uint[] idxBuffersSizes = bs.ReadUInt32s(8);
        uint unkBuffer1Offset = bs.ReadUInt32();
        uint unkBuffer1Size = bs.ReadUInt32();
        uint unkBuffer1Offset2 = bs.ReadUInt32();
        uint unkBuffer2Size = bs.ReadUInt32();

        AttributeSets.Capacity = flexVertInfoCount;
        for (int i = 0; i < flexVertInfoCount; i++)
        {
            var attr = new MdlFlexVertexInfo();
            attr.Read(bs);
            AttributeSets.Add(attr);
        }

        Attributes.Capacity = flexVertAttributeCount;
        for (int i = 0; i < flexVertAttributeCount; i++)
        {
            var attr = new MdlFlexVertexAttribute();
            attr.Read(bs);
            Attributes.Add(attr);
        }

        BoundingBox = bs.ReadSingles(8);
        List<NamePointer> materialNamesPtrs = [];
        for (int i = 0; i < materialNamesCount; i++)
        {
            var namePtr = new NamePointer();
            namePtr.Read(bs);
            materialNamesPtrs.Add(namePtr);
        }

        if ((UnkFlags_0x16 & 2) != 0)
            UnknownEntries = bs.ReadBytes(0x10);

        long nameTableStart = bs.Position;
        for (int i = 0; i < materialNamesCount; i++)
        {
            bs.Position = nameTableStart + (int)materialNamesPtrs[i].Offset;
            MaterialFileNames.Add(bs.ReadString(StringCoding.ZeroTerminated));
        }

        bs.Position = MainHeaderSize + section1Size;
        ReadModelData(bs);

        vBuffers = ReadBuffers(bs, vBuffersOffsets, vBuffersSizes);
        idxBuffers = ReadBuffers(bs, idxBuffersOffsets, idxBuffersSizes);

        if (unkBuffer1Size > 0)
        {
            bs.Position = MainHeaderSize + unkBuffer1Offset;
            UnknownBuffer1 = new ModelBuffer()
            {
                Data = bs.ReadBytes((int)unkBuffer1Size),
            };
        }

        if (unkBuffer2Size > 0)
        {
            bs.Position = MainHeaderSize + unkBuffer1Offset2;
            UnknownBuffer2 = new ModelBuffer()
            {
                Data = bs.ReadBytes((int)unkBuffer2Size),
            };
        }
    }

    private ModelBuffer[] ReadBuffers(SmartBinaryStream bs, uint[] offsets, uint[] sizes)
    {
        ModelBuffer[] buffers = new ModelBuffer[sizes.Length];
        for (int i = 0; i < sizes.Length; i++)
        {
            if (sizes[i] == 0)
                continue;

            bs.Position = offsets[i] + MainHeaderSize;
            buffers[i] = new ModelBuffer()
            {
                Data = bs.ReadBytes((int)sizes[i]),
            };
        }
        return buffers;
    }

    private void ReadModelData(SmartBinaryStream bs)
    {
        SpecsHeader.Read(bs);
        LODModels = bs.ReadStructArray<MdlLODModelInfo>(SpecsHeader.LODModelCount);
        MeshInfos = bs.ReadStructArray<MdlMeshInfo>(SpecsHeader.SubmeshCount);
        SubDrawCalls = bs.ReadStructArray<SubDrawPart>(SpecsHeader.DrawPartCount);
        Joints = bs.ReadStructArray<JointEntry>(SpecsHeader.JointCount);
        List<NamePointer> materialNamePointers = bs.ReadStructArray<NamePointer>(SpecsHeader.MaterialCount);
        List<MdlFaceJointEntry> jointFaceNamePointers = bs.ReadStructArray<MdlFaceJointEntry>(SpecsHeader.FaceJointCount);
        JointMuscles = bs.ReadStructArray<MdlJointMuscleEntry>(SpecsHeader.MuscleJointCount);
        JointFacesEntries = bs.ReadStructArray<MdlUnkJointParam>(SpecsHeader.UnkJointParamCount);
        var AdditionalPartNamePointers = bs.ReadStructArray<NamePointer>(SpecsHeader.AdditionalPartCount);
        var OptionNamePointers = bs.ReadStructArray<NamePointer>(SpecsHeader.OptionCount);
        var VFXEntryNamePointers = bs.ReadStructArray<NamePointer>(SpecsHeader.VFXEntryCount);


        ExtraSection = bs.ReadBytes((int)SpecsHeader.ExtraSectionSize); //40 bytes when used
        bs.Align(0x04);

        // Read extra section bytes
        long mcexPosition = bs.Position;
        McexSection = bs.ReadBytes((int)SpecsHeader.ModelExternalContentSize);
        bs.Position = mcexPosition + (int)Utils.AlignValue(SpecsHeader.ModelExternalContentSize, 0x10);

        // Also try to read the structure
        long tempPos = bs.Position;
        ExternalContentHeader.Read(bs);
        bs.Position = tempPos;

        if (SpecsHeader.JointCount > 0)
            JointBoundings = bs.ReadStructArray<MdlJointBounding>(SpecsHeader.JointCount);

        if (SpecsHeader.JointCount > 0)
        {
            long basePos = bs.Position;
            JointMaxBounds = bs.ReadSingles(6);

            bs.Position = basePos + Utils.AlignValue(sizeof(float) * 6, 0x10);
        }

        long strTableOffset = bs.Position;
        for (int i = 0; i < SpecsHeader.MaterialCount; i++)
        {
            bs.Position = strTableOffset + materialNamePointers[i].Offset;

            string str = bs.ReadString(StringCoding.ZeroTerminated);
            if (!str.All(char.IsAscii))
                throw new Exception($"Material name '{str}' must be a valid ASCII with no special characters.");

            MaterialNames.Add(str);
        }

        for (int i = 0; i < SpecsHeader.JointCount; i++)
        {
            bs.Position = strTableOffset + Joints[i].NameOffset;

            string str = bs.ReadString(StringCoding.ZeroTerminated);
            if (!str.All(char.IsAscii))
                throw new Exception($"Joint name '{str}' must be a valid ASCII with no special characters.");

            JointNames.Add(str);
        }

        for (int i = 0; i < SpecsHeader.FaceJointCount; i++)
        {
            bs.Position = strTableOffset + jointFaceNamePointers[i].Offset;

            string str = bs.ReadString(StringCoding.ZeroTerminated);
            if (!str.All(char.IsAscii))
                throw new Exception($"Joint face name '{str}' must be a valid ASCII with no special characters.");

            JointFaceNames.Add(str);
        }

        for (int i = 0; i < SpecsHeader.MuscleJointCount; i++)
        {
            bs.Position = strTableOffset + JointMuscles[i].NameOffset;

            string str = bs.ReadString(StringCoding.ZeroTerminated);
            if (!str.All(char.IsAscii))
                throw new Exception($"Joint muscle name '{str}' must be a valid ASCII with no special characters.");

            JointMuscleNames.Add(str);
        }

        for (int i = 0; i < SpecsHeader.AdditionalPartCount; i++)
        {
            bs.Position = strTableOffset + AdditionalPartNamePointers[i].Offset;

            string str = bs.ReadString(StringCoding.ZeroTerminated);
            if (!str.All(char.IsAscii))
                throw new Exception($"Additional part '{str}' must be a valid ASCII with no special characters.");

            AdditionalParts.Add(str);
        }

        for (int i = 0; i < SpecsHeader.OptionCount; i++)
        {
            bs.Position = strTableOffset + OptionNamePointers[i].Offset;

            string str = bs.ReadString(StringCoding.ZeroTerminated);
            if (!str.All(char.IsAscii))
                throw new Exception($"Option name '{str}' must be a valid ASCII with no special characters.");

            Options.Add(str);
        }

        for (int i = 0; i < SpecsHeader.VFXEntryCount; i++)
        {
            bs.Position = strTableOffset + VFXEntryNamePointers[i].Offset;

            string str = bs.ReadString(StringCoding.ZeroTerminated);
            if (!str.All(char.IsAscii))
                throw new Exception($"VFX Entry '{str}' must be a valid ASCII with no special characters.");

            VFXEntries.Add(str);
        }

        bs.Position = strTableOffset + SpecsHeader.StringTableSize;
        bs.Align(0x10);

        if ((UnkFlags_0x16 & 1) != 0)
        {
            ExtraSection2 = bs.ReadBytes(0x18);
            bs.Align(0x10);
        }
    }

    #endregion

    #region Writing
    private long _ofsMeshInfoSavedPos; //for adjusting buffer offsets

    private void Write(SmartBinaryStream bs)
    {
        uint[] vBuffersSizes = new uint[8];
        uint[] idxBuffersSizes = new uint[8];

        for (int i = 0; i < vBuffers.Length; i++)
            if (vBuffers[i] != null)
                vBuffersSizes[i] = (uint)vBuffers[i].Data.Length;

        for (int i = 0; i < idxBuffers.Length; i++)
            if (idxBuffers[i] != null)
                idxBuffersSizes[i] = (uint)idxBuffers[i].Data.Length;

        bs.Write("MDL "u8);
        bs.Write(Version);
        bs.Write(MainFlags);
        bs.Write(ModelType);
        bs.Write((byte)0);
        bs.Write(0); //mat size later
        bs.Write(0); //mesh spec size later
        bs.Write((ushort)MaterialFileNames.Count);
        bs.Write((ushort)Attributes.Count);
        bs.Write((byte)AttributeSets.Count);
        bs.Write((byte)LODModels.Count); //LOD count
        bs.Write(UnkFlags_0x16);
        bs.Write((byte)0);

        //vertex buffer offsets
        long ofsVbufferPos = bs.Position;
        bs.Write(new uint[8]); //vertex buffer offsets saved later
        long ofsIbufferPos = bs.Position;
        bs.Write(new uint[8]); //index buffer offsets saved later
        bs.Write(vBuffersSizes);
        bs.Write(idxBuffersSizes);

        long ofsUnknownBuffers = bs.Position;

        bs.Write((uint)0); //unknown buffer 1 offset
        bs.Write((uint)UnknownBuffer1.Data.Length);
        bs.Write((uint)0); //unknown buffer 2 offset
        bs.Write((uint)UnknownBuffer2.Data.Length);

        long start_section1 = bs.Position;

        bs.WriteStructArray(AttributeSets);
        bs.WriteStructArray(Attributes);
        bs.Write(BoundingBox);

        uint nameOfs = 0;
        WriteNameStructs(bs, MaterialFileNames, ref nameOfs);

        if ((UnkFlags_0x16 & 2) != 0)
            bs.Write(UnknownEntries);

        bs.WriteStrings(MaterialFileNames);
        bs.Align(0x10);

        //size
        using (var seek = new Seek(bs, 0x08, SeekOrigin.Begin))
            bs.WriteUInt32((uint)(bs.Position - start_section1));

        long start_mesh_section = bs.Position;

        WriteMeshData(bs);

        bs.Align(16);

        //size
        using (var seek = new Seek(bs, 0x12, SeekOrigin.Begin))
            bs.WriteUInt32((uint)(bs.Position - start_mesh_section));

        //buffer data last
        for (int i = 0; i < vBuffers.Length; i++)
        {
            if (vBuffers[i] == null)
                continue;

            //mdl header offset
            bs.WriteUint32AtOffset(ofsVbufferPos + i * 4, start_section1);
            //mesh header offset
            bs.WriteUint32AtOffset(_ofsMeshInfoSavedPos + i * 64 + 16, start_section1);
            bs.Write(vBuffers[i].Data.Span);

            //mdl header offset
            bs.WriteUint32AtOffset(ofsIbufferPos + i * 4, start_section1);
            //mesh header offset
            bs.WriteUint32AtOffset(_ofsMeshInfoSavedPos + i * 64 + 20, start_section1);
            bs.Write(idxBuffers[i].Data.Span);
        }

        //2 unknown buffers
        bs.WriteUint32AtOffset(ofsUnknownBuffers, start_section1);
        bs.Write(UnknownBuffer1.Data.Span);

        bs.WriteUint32AtOffset(ofsUnknownBuffers + 8, start_section1);
        bs.Write(UnknownBuffer2.Data.Span);
    }

    private void WriteMeshData(SmartBinaryStream bs)
    {
        //Prepare spec header
        SpecsHeader.LODModelCount = (ushort)LODModels.Count;
        SpecsHeader.SubmeshCount = (ushort)MeshInfos.Count;
        SpecsHeader.JointCount = (uint)Joints.Count;
        SpecsHeader.MuscleJointCount = (ushort)JointMuscles.Count;
        SpecsHeader.FaceJointCount = (byte)JointFaceNames.Count;
        SpecsHeader.DrawPartCount = (ushort)SubDrawCalls.Count;
        SpecsHeader.UnkJointParamCount = (byte)JointFacesEntries.Count;
        SpecsHeader.MaterialCount = (byte)MaterialNames.Count;
        SpecsHeader.FlexVertexCount = (byte)Attributes.Count;
        SpecsHeader.OptionCount = (byte)Options.Count;
        SpecsHeader.AdditionalPartCount = (byte)AdditionalParts.Count;
        SpecsHeader.VFXEntryCount = (byte)VFXEntries.Count;

        SpecsHeader.StringTableSize = (uint)MaterialNames.Sum(x => Encoding.ASCII.GetByteCount(x) + 1) +
                                       (uint)JointNames.Sum(x => Encoding.ASCII.GetByteCount(x) + 1) +
                                       (uint)JointFaceNames.Sum(x => Encoding.ASCII.GetByteCount(x) + 1) +
                                       (uint)JointMuscleNames.Sum(x => Encoding.ASCII.GetByteCount(x) + 1) +
                                       (uint)Options.Sum(x => Encoding.ASCII.GetByteCount(x) + 1) +
                                       (uint)AdditionalParts.Sum(x => Encoding.ASCII.GetByteCount(x) + 1) +
                                       (uint)VFXEntries.Sum(x => Encoding.ASCII.GetByteCount(x) + 1);

        uint nameOfs = 0;

        //name offset setup for joints (after material names)
        uint jointNameOfs = (uint)MaterialNames.Sum(x => x.Length + 1);
        for (int i = 0; i < Joints.Count; i++)
        {
            Joints[i].NameOffset = jointNameOfs;
            jointNameOfs += (uint)JointNames[i].Length + 1;
        }
        //Joint face names
        ulong[] jointFaceNameOffsets = new ulong[JointFaceNames.Count];
        for (int i = 0; i < JointFaceNames.Count; i++)
        {
            jointFaceNameOffsets[i] = jointNameOfs;
            jointNameOfs += (uint)JointFaceNames[i].Length + 1;
        }
        //Joint muscle names
        for (int i = 0; i < JointMuscles.Count; i++)
        {
            JointMuscles[i].NameOffset = jointNameOfs;
            jointNameOfs += (uint)JointMuscleNames[i].Length + 1;
        }

        foreach (MdlLODModelInfo mesh in LODModels)
        {
            //get sub meshes
            List<MdlMeshInfo> subMeshes = [];
            for (int i = 0; i < mesh.MeshCount; i++)
                subMeshes.Add(MeshInfos[mesh.MeshIndex + i]);

            //Auto set vertex and index counters
            mesh.VertexCount = (uint)subMeshes.Sum(x => x.VertexCount);
            mesh.TriCount = (uint)subMeshes.Sum(x => x.FaceIndexCount) / 3;
        }

        SpecsHeader.Write(bs);

        _ofsMeshInfoSavedPos = bs.Position;
        bs.WriteStructArray(LODModels);
        bs.WriteStructArray(MeshInfos);
        bs.WriteStructArray(SubDrawCalls);
        bs.WriteStructArray(Joints);
        WriteNameStructs(bs, MaterialNames, ref nameOfs);

        bs.Write(jointFaceNameOffsets);
        bs.WriteStructArray(JointMuscles);
        bs.WriteStructArray(JointFacesEntries);

        nameOfs += (uint)JointNames.Sum(x => Encoding.ASCII.GetByteCount(x) + 1);
        nameOfs += (uint)JointFaceNames.Sum(x => Encoding.ASCII.GetByteCount(x) + 1);
        nameOfs += (uint)JointMuscleNames.Sum(x => Encoding.ASCII.GetByteCount(x) + 1);

        WriteNameStructs(bs, AdditionalParts, ref nameOfs);
        WriteNameStructs(bs, Options, ref nameOfs);
        WriteNameStructs(bs, VFXEntries, ref nameOfs);

        bs.Write(ExtraSection);
        bs.Align(0x04);

        long mcexPosition = bs.Position;
        bs.Write(McexSection);
        bs.Position = mcexPosition + (int)Utils.AlignValue((uint)McexSection.Length, 0x10);

        if (SpecsHeader.JointCount > 0)
            bs.WriteStructArray(JointBoundings);

        if (SpecsHeader.JointCount > 0)
        {
            long basePos = bs.Position;
            bs.Write(JointMaxBounds);
            bs.Position = basePos + Utils.AlignValue(sizeof(float) * 6, 0x10);
        }

        // TODO: String table
        bs.WriteStrings(MaterialNames);
        bs.WriteStrings(JointNames);
        bs.WriteStrings(JointFaceNames);
        bs.WriteStrings(JointMuscleNames);
        bs.WriteStrings(AdditionalParts);
        bs.WriteStrings(Options);
        bs.WriteStrings(VFXEntries);
        bs.Align(0x10);

        if ((UnkFlags_0x16 & 1) != 0)
        {
            bs.Write(ExtraSection2);
            bs.Align(0x10);
        }
    }

    private void WriteNameStructs(SmartBinaryStream bs, List<string> strings, ref uint offsetStart)
    {
        foreach (var name in strings)
        {
            bs.Write((ulong)offsetStart);
            bs.Write((ulong)0);
            offsetStart += (uint)name.Length + 1;
        }
    }

    public class NamePointer : ISerializableStruct
    {
        public uint Offset { get; set; }

        public void Read(SmartBinaryStream bs)
        {
            Offset = bs.ReadUInt32();
            bs.ReadCheckPadding(0x0C);
        }

        public void Write(SmartBinaryStream bs)
        {
            bs.Write(Offset);
            bs.WritePadding(0x0C);
        }

        public uint GetSize() => 0x10;
    }

    #endregion
}
