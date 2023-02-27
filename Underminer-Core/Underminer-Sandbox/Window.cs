using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Underminer_Core.Rendering;
using static System.Net.Mime.MediaTypeNames;

namespace Underminer_Sandbox
{
    internal class Window : GameWindow
    {

        float[] _vertices =
        {
             0.5f,  0.5f, 0.0f,    // 右上角
             0.5f, -0.5f, 0.0f,    // 右下角
            -0.5f, -0.5f, 0.0f,    // 左下角
            -0.5f,  0.5f, 0.0f    // 左上角
        };

        uint[] _indices = {
            0, 1, 3, // 第一个三角形
            1, 2, 3  // 第二个三角形
        };

        private VertexArrayObject _vao;
        private VertexBufferObject _vbo;
        private IndexBufferObject _ibo;
        private Shader _shader;

        // 创建窗口
        public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
        { Size = (width, height), Title = title })
        {

        }

        // 窗口创建完成 第一次运行
        protected override void OnLoad()
        {
            // 绘制线框
            // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            
            _vbo = new VertexBufferObject(_vertices);
            VertexBufferLayout layout = new VertexBufferLayout();
            layout.AddElement(new VertexBufferLayoutElement(0, 3));
            _vbo.AddLayout(layout);
            _ibo = new IndexBufferObject(_indices);         // _ibo在VertexArrayObject中绑定
            _vao = new VertexArrayObject(_ibo, _vbo);

            _vao.Bind();

            _shader = new Shader("""D:\GameEngine\Underminer-Core\Underminer-Sandbox\Test.glsl""");

        }

        private double _totleTime = 0;      // 运行总时间
        // 每帧渲染运行 Update
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(new Color4(0.1f, 0.1f, 0.1f, 1.0f));


            _vao.Bind();
            _shader.Bind();
            Vector3 color = new Vector3(MathF.Sin((float)_totleTime), MathF.Cos((float)_totleTime), MathF.Acos((float)_totleTime));
            _shader.SetUniform("color", color);

            //if (_vao.IndexBufferObject == null) 
            //    // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            //else
            GL.DrawElements(PrimitiveType.Triangles, _ibo.Length, DrawElementsType.UnsignedInt, 0);

            _totleTime += args.Time;        // args.Time每帧运行的时间

            SwapBuffers();
        }

        // FixedUpdate
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // 每帧检测输入
            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }


}
