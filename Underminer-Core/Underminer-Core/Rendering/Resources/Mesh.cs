using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Underminer_Core.Maths;
using Underminer_Core.Rendering.Buffers;
using Underminer_Core.Rendering.Geometry;

namespace Underminer_Core.Rendering.Resources
{
    public class Mesh : IDisposable
    {
        public int VertexCount { get; }
        public int IndexCount { get; }
        public int MaterialIndex { get; }
        public string MeshName { get; }
        public Sphere BoundingSphere { get; private set; }

        // 渲染物体需要vbo vao
        private IndexBufferObject? _indexBufferObject;          // 若顶点数据少于3 无需创建ibo
        private VertexBufferObject _vertexBufferObject;
        private VertexArrayObject _vertexArrayObject;

        // 顶点信息 顶点索引 材质索引
        public Mesh(List<Vertex> vertices, List<uint> indices, int materialIndex, string meshName) 
        {
            VertexCount = vertices.Count;
            IndexCount = indices.Count;
            MaterialIndex = materialIndex;
            MeshName = meshName;

            CreatBuffer(vertices, indices);
            CreateBoundingSphere(vertices);
        }

        public void CreatBuffer(List<Vertex> vertices, List<uint> indices)
        {
            // 按照Vertex结构加载模型数据
            List<float> vertexData = new();
            foreach (var vertex in vertices)
            {
                vertexData.Add(vertex.Position.X);
                vertexData.Add(vertex.Position.Y);
                vertexData.Add(vertex.Position.Z);

                vertexData.Add(vertex.Normal.X);
                vertexData.Add(vertex.Normal.Y);
                vertexData.Add(vertex.Normal.Z);

                vertexData.Add(vertex.TexCoords.X);
                vertexData.Add(vertex.TexCoords.Y);

                vertexData.Add(vertex.Tangent.X);
                vertexData.Add(vertex.Tangent.Y);
                vertexData.Add(vertex.Tangent.Z);

                vertexData.Add(vertex.BiTangent.X);
                vertexData.Add(vertex.BiTangent.Y);
                vertexData.Add(vertex.BiTangent.Z);
            }
            
            _vertexBufferObject = new VertexBufferObject(vertexData.ToArray());
            VertexBufferLayout layout = new VertexBufferLayout();
            // shader书写顺序需要与以下layout一致
            layout.AddElement(new VertexBufferLayoutElement(0, 3),      // 1Position    
                              new VertexBufferLayoutElement(1, 3),      // 2Color
                              new VertexBufferLayoutElement(2, 2),      // 3Texture
                              new VertexBufferLayoutElement(3, 3),      // 4Tangent
                              new VertexBufferLayoutElement(4, 3));     // 5BiTangent
            _vertexBufferObject.AddLayout(layout);

            if (indices.Count >= 3)
            {
                _indexBufferObject = new IndexBufferObject(indices.ToArray());
            }

            _vertexArrayObject = new VertexArrayObject(_indexBufferObject, _vertexBufferObject);
        }

        // 根据所有顶点创建包围球
        private void CreateBoundingSphere(List<Vertex> vertices)
        {
            // 最小与最大取中点
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;

            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            foreach (var vertex in vertices)
            {
                // 找到最小的xyz值
                minX = MathHelper.Min(minX, vertex.Position.X);
                minY = MathHelper.Min(minY, vertex.Position.Y);
                minZ = MathHelper.Min(minZ, vertex.Position.Z);

                // 找到最大的xyz值
                maxX = MathHelper.Max(maxX, vertex.Position.X);
                maxY = MathHelper.Max(maxY, vertex.Position.Y);
                maxZ = MathHelper.Max(maxZ, vertex.Position.Z);
            }
            Vector3 position = new Vector3(minX + maxX, minY + maxY, minZ + maxZ) / 2;

            // 映射 把vertices集合里的值进行处理再保存   取最大距离为半径
            float radius = MathHelper.InverseSqrtFast(vertices.Select(v => Vector3.DistanceSquared(position, v.Position)).Max());
            BoundingSphere = new Sphere(position, radius);
        }

        // 绘制时绑定Mesh 事实上就是绑定_vertexArrayObject
        public void Bind() => _vertexArrayObject.Bind();
    
        public void Unbind() => _vertexArrayObject.Unbind();

        public void Dispose()
        {
            _indexBufferObject?.Dispose();
            _vertexBufferObject.Dispose();
            _vertexArrayObject.Dispose();
        }
    }

}
