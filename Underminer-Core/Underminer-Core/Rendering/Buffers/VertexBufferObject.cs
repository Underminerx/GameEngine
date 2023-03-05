using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.Rendering.Buffers
{
    #region 模型数据布局

    // 每种数据的内容
    public struct VertexBufferLayoutElement
    {
        public int Location;
        public int Count;
        public bool IsNormalized;

        public VertexBufferLayoutElement(int location, int count, bool isNormalized = false)
        {
            Location = location;
            Count = count;
            IsNormalized = isNormalized;
        }
    }

    // 所种数据的集合列表
    public struct VertexBufferLayout
    {
        internal List<VertexBufferLayoutElement> Elements;

        public VertexBufferLayout()
        {
            Elements = new List<VertexBufferLayoutElement>();
        }

        // params 可以使得传入n个数据
        public void AddElement(params VertexBufferLayoutElement[] elements)
        {
            Elements.AddRange(elements);
        }
    }
    #endregion

    public class VertexBufferObject : IDisposable
    {
        private readonly int _id;
        internal VertexBufferLayout Layout;
        public int Stride => GetStride();

        public VertexBufferObject(float[] vertices)
        {
            Layout = new VertexBufferLayout();      // 初始化Layout 以便后续传入赋值
            _id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _id);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _id);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void AddLayout(VertexBufferLayout layout)
        {
            Layout = layout;
        }

        private int GetStride()
        {
            int stride = 0;
            foreach (var vertexBufferLayoutElement in Layout.Elements)
            {
                stride += vertexBufferLayoutElement.Count * sizeof(float);
            }
            return stride;
        }


        private void ReleaseUnmanagedResources()
        {
            GL.DeleteBuffer(_id);
        }

        // 实现的IDisposable接口函数
        public void Dispose()
        {
            // 此函数手动释放
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);      // 避免析构函数再次释放
        }

        ~VertexBufferObject()
        {
            // 此函数自动释放
            ReleaseUnmanagedResources();
        }
    }
}
