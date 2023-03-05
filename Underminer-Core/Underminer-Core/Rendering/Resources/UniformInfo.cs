using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Underminer_Core.Rendering.Resources
{
    public struct UniformInfo
    {
        public string Name;
        public int Location;
        public ActiveUniformType Type;
        public object? value;

        public UniformInfo(string name, int location, ActiveUniformType type, object? value)
        {
            Name = name;
            Location = location;
            Type = type;
            this.value = value;
        }
    }
}
