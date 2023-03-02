using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.Rendering.Geometry
{
    public class Model : IDisposable
    {
        public List<Mesh> Meshes { get; }
        public Dictionary<int, string> MaterialDic { get; }
        public Model(List<Mesh> meshes, Dictionary<int, string> materialDic)
        {
            Meshes = meshes;
            MaterialDic = materialDic;
        }

        public void Dispose()
        {
            foreach (var mesh in Meshes)
            {
                mesh.Dispose();
            }
        }
    }
}
