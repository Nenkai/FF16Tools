using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.Model;

public class GeneratedJointData
{
    public string Name { get; set; }
    public int Index { get; set; }
    public JointEntry Joint { get; set; }
    public MdlJointBounding Bounding { get; set; }

    public GeneratedJointData(string name, int index, JointEntry joint, MdlJointBounding bounding)
    {
        Name = name;
        Index = index;
        Joint = joint;
        Bounding = bounding;
    }
}
