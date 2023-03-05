﻿using Assimp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.Rendering.Geometry
{
    public class Model : IDisposable
    {
        public string Path { get; }
        public List<Mesh> Meshes { get; }
        public List<string> MaterialNames { get; }
        public Model(string path)
        {
            Path = path;
            (Meshes, MaterialNames) = LoadModel(path);
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
            // 先处理自身mesh 转换为自定义mesh
            foreach (var index in rootNode.MeshIndices)
            {
                var mesh = scene.Meshes[index];
                meshes.Add(ProcessMesh(mesh, transform));
            }
            // 根节点处理完后 递归处理子节点
            foreach (var node in rootNode.Children)
            {
                ProcessNode(scene, meshes, node, node.Transform);
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
            foreach (var mesh in Meshes)
            {
                mesh.Dispose();
            }
        }
    }
}