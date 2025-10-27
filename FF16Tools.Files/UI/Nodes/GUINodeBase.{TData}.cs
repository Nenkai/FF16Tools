using FF16Tools.Files.UI.Nodes.Data;

using Syroot.BinaryData;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FF16Tools.Files.UI.Nodes;

public abstract class GUINodeBase<TData> : GUINodeBase
    where TData : GUINodeDataBase, new()
{
    public TData Data { get; set; } = new TData();
}