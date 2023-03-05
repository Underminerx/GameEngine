using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.Rendering.Buffers
{
    public class VertexArrayObject : IDisposable
    {
        private readonly int _id;
        public IndexBufferObject? IndexBufferObject { get; }

        // 指定格式
        public VertexArrayObject(IndexBufferObject? indexBufferObject, params VertexBufferObject[] vertexBufferObjects)
        {
            IndexBufferObject = indexBufferObject;
            _id = GL.GenVertexArray();
            GL.BindVertexArray(_id);

            foreach (var vbo in vertexBufferObjects)
            {

                vbo.Bind();
                int offset = 0;
                foreach (var element in vbo.Layout.Elements)
                {
                    GL.VertexAttribPointer(element.Location, element.Count, VertexAttribPointerType.Float, element.IsNormalized, vbo.Stride, offset);
                    GL.EnableVertexAttribArray(element.Location);
                    offset += element.Count * sizeof(float);
                }
            }
        }

        public void Bind()
        {
            GL.BindVertexArray(_id);
            IndexBufferObject?.Bind();
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        private void ReleaseUnmanagedResources()
        {
            GL.DeleteVertexArray(_id);
        }

        // 实现的IDisposable接口函数
        public void Dispose()
        {
            // 此函数手动释放
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);      // 避免析构函数再次释放
        }

        ~VertexArrayObject()
        {
            // 此函数自动释放
            ReleaseUnmanagedResources();
        }
    }
}
