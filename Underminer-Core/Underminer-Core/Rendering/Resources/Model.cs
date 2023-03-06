using Assimp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Underminer_Core.Log;
using Underminer_Core.Maths;
using Underminer_Core.Rendering.Geometry;

namespace Underminer_Core.Rendering.Resources
{
    public class Model : IDisposable
    {
        public string Path { get; }                     // 模型文件路径
        public List<Mesh> Meshes { get; }               // 模型网格列表
        public List<string> MaterialNames { get; }      // 模型材质列表
        public bool IsDestroy { get; private set; } = false;
        public Sphere BoundingSphere { get; private set; }

        public static Model? Create(string path) 
        { 
            Model? model = null;
            try
            {
                model = new Model(path);
            }
            catch (Exception e)
            {
                UmLog.ErrorLogCore(e.Message);
            }

            return model;
        } 
        public static (List<Mesh> meshes, List<string> materialNames) LoadModel(string path, PostProcessSteps postProcessSteps = PostProcessSteps.None)
        {
            AssimpContext assimp = new AssimpContext();
            Scene? scene = assimp.ImportFile(path, postProcessSteps);      // PostProcessSteps 三角形后处理方法 生成法线|细分网格|减少网格|翻转UV
            // 文件加载失败 
            if (scene is null || (scene.SceneFlags & SceneFlags.Incomplete) == SceneFlags.Incomplete || scene?.RootNode is null)
            {
                Console.WriteLine($"ERROR:ASSIMP");
                return (null, null);
            }

            List<string> materialNames = ProcessMaterials(scene);
            List<Mesh> meshes = new List<Mesh>();
            ProcessNode(scene, meshes, scene.RootNode, Assimp.Matrix4x4.Identity);
            return (meshes, materialNames);
        }
        
        private Model(string path)
        {
            Path = path;
            (Meshes, MaterialNames) = LoadModel(path);
            CreateBoundingSphere(Meshes);
        }

        private void CreateBoundingSphere(List<Mesh> meshes)
        {
            if (meshes.Count == 1)
            {
                BoundingSphere = meshes[0].BoundingSphere;
            }
            else if (meshes.Count > 1)
            {
                // 最小与最大取中点
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float minZ = float.MaxValue;

                float maxX = float.MinValue;
                float maxY = float.MinValue;
                float maxZ = float.MinValue;

                foreach (var mesh in meshes)
                {
                    // 找到最小的xyz值
                    minX = MathHelper.Min(minX, mesh.BoundingSphere.Position.X - mesh.BoundingSphere.Radius);
                    minY = MathHelper.Min(minY, mesh.BoundingSphere.Position.Y - mesh.BoundingSphere.Radius);
                    minZ = MathHelper.Min(minZ, mesh.BoundingSphere.Position.Z - mesh.BoundingSphere.Radius);

                    // 找到最大的xyz值
                    maxX = MathHelper.Max(maxX, mesh.BoundingSphere.Position.X + mesh.BoundingSphere.Radius);
                    maxY = MathHelper.Max(maxY, mesh.BoundingSphere.Position.Y + mesh.BoundingSphere.Radius);
                    maxZ = MathHelper.Max(maxZ, mesh.BoundingSphere.Position.Z + mesh.BoundingSphere.Radius);
                }
                Vector3 position = new Vector3(minX + maxX, minY + maxY, minZ + maxZ) / 2;
                // 映射 把顶点集合里的值进行处理再保存
                float radius = MathHelper.InverseSqrtFast(meshes.Select(m => Vector3.DistanceSquared(position, m.BoundingSphere.Position)).Max());
                BoundingSphere = new Sphere
                {
                    Position = position,
                    Radius = radius
                };
            }

        }
        private static List<string> ProcessMaterials(Scene scene)
        {
            List<string> materialNames = new List<string>();
            if (scene.HasMaterials)
            {
                foreach (var mat in scene.Materials)
                {
                    materialNames.Add(mat.HasName ? mat.Name : "??"); 
                }
            }
            return materialNames;
        }

        // 处理的结果直接存入meshes
        private static void ProcessNode(Scene scene, List<Mesh> meshes, Node rootNode, Assimp.Matrix4x4 transform)
        {
            var nodeTransform = rootNode.Transform * transform;     // 父节点变换右乘(transform行主式)当前结点变换 得到当前结点的世界变换
            // 先处理自身mesh 转换为自定义mesh
            foreach (var index in rootNode.MeshIndices)
            {
                var mesh = scene.Meshes[index];
                meshes.Add(ProcessMesh(mesh, nodeTransform));
            }
            // 根节点处理完后 递归处理子节点
            foreach (var node in rootNode.Children)
            {
                ProcessNode(scene, meshes, node, nodeTransform);
            }
            
        }

        // 把assimp的mesh结构转换为我们自定义的mesh结构
        private static Mesh ProcessMesh(Assimp.Mesh mesh, Assimp.Matrix4x4 transform)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            int materialIndex = mesh.MaterialIndex;
            string meshName = mesh.Name;

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                // 有数据则添加 无数据则默认 再根据父节点变化位置
                var position = transform * (mesh.HasVertices ? mesh.Vertices[i] : new Vector3D(0, 0, 0));
                var normal = transform * (mesh.HasNormals ? mesh.Normals[i] : new Vector3D(0, 0, 0));
                var texCoords = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Vector3D(0, 0, 0);// 只管第一套纹理
                var tangent = transform * (mesh.HasTangentBasis ? mesh.Tangents[i] : new Vector3D(0, 0, 0));
                var BiTangent = transform * (mesh.HasTangentBasis ? mesh.BiTangents[i] : new Vector3D(0, 0, 0));

                Vertex vertex = new Vertex();
                vertex.Position.X = position.X;
                vertex.Position.Y = position.Y;
                vertex.Position.Z = position.Z;
                
                vertex.Normal.X = normal.X;
                vertex.Normal.Y = normal.Y;
                vertex.Normal.Z = normal.Z;

                vertex.TexCoords.X = texCoords.X;
                vertex.TexCoords.Y = texCoords.Y;

                vertex.Tangent.X = tangent.X;
                vertex.Tangent.Y= tangent.Y;
                vertex.Tangent.Z = tangent.Z;

                vertex.BiTangent.X = BiTangent.X;
                vertex.BiTangent.Y = BiTangent.Y;
                vertex.BiTangent.Z = BiTangent.Z;

                vertices.Add(vertex);
            }

            if (mesh.HasFaces)
            {
                // 把每个面的
                foreach (var face in mesh.Faces)
                {
                    // 面索引存入indices
                    foreach (var faceIndex in face.Indices)
                    {
                        indices.Add((uint)faceIndex);
                    }

                }
            }

            return new Mesh(vertices, indices, materialIndex, meshName);
        }

        public void Dispose()
        {
            if (!IsDestroy)
            {
                foreach (var mesh in Meshes)
                {
                    mesh.Dispose();
                }
            }

            IsDestroy = true;
        }
    }
}
