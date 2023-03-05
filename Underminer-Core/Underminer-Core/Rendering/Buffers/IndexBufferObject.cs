using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.Rendering.Buffers
{
    // Buffer是非托管内存 类消亡时系统不会自动释放内存 因此 需要继承IDisposable接口
    public class IndexBufferObject : IDisposable
    {
        private readonly int _id;       // buffer id
        public int Length { get; }

        public IndexBufferObject(uint[] indices)
        {
            Length = indices.Length;
            _id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _id);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);

        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _id);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
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

        ~IndexBufferObject()
        {
            // 此函数自动释放
            ReleaseUnmanagedResources();
        }
    }
}
