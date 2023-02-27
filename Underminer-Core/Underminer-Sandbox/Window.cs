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

namespace Underminer_Sandbox
{
    internal class Window : GameWindow
    {

        float[] _vertices =
        {
             0.5f,  0.5f, 0.0f, 1, 0, 0,    // 右上角
             0.5f, -0.5f, 0.0f, 0, 1, 0,    // 右下角
            -0.5f, -0.5f, 0.0f, 0, 0, 1,    // 左下角
            -0.5f,  0.5f, 0.0f, 1, 0, 1     // 左上角
        };

        uint[] _indices = {
            0, 1, 3, // 第一个三角形
            1, 2, 3  // 第二个三角形
        };


        private VertexArrayObject _vao;
        private VertexBufferObject _vbo;
        private IndexBufferObject _ibo;
        private int _program;   // 渲染管线id

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
            layout.AddElement(new VertexBufferLayoutElement(0, 3), new VertexBufferLayoutElement(1, 3));
            _vbo.AddLayout(layout);
            _ibo = new IndexBufferObject(_indices);         // _ibo在VertexArrayObject中绑定
            _vao = new VertexArrayObject(_ibo, _vbo);

            _vao.Bind();



            #region shader创建与绑定pipline

            // 顶点shader代码 (@转义)
            string vertexSource = @"
                #version 460 core
                layout (location = 0) in vec3 aPos;
                layout (location = 1) in vec3 aColor;

                layout (location = 0) out vec3 color;

                void main()
                {
                    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
                    color = aColor;
                }";

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            // 将shader内容与类型绑定
            GL.ShaderSource(vertexShader, vertexSource);
            // 编译shader
            GL.CompileShader(vertexShader);

            // 片段shader代码 
            string fragmentSource = @"
                #version 460 core
                out vec4 FragColor;
                layout (location = 0) in vec3 color;

                void main()
                {
                    FragColor = vec4(color, 1.0f);
                } ";

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            // 将shader内容与类型绑定
            GL.ShaderSource(fragmentShader, fragmentSource);
            // 编译shader
            GL.CompileShader(fragmentShader);

            // 创建渲染管线
            _program = GL.CreateProgram();
            // 把shader与管线绑定
            GL.AttachShader(_program, vertexShader);
            GL.AttachShader(_program, fragmentShader);
            GL.LinkProgram(_program);
            // 可以直接使用管线
            // GL.UseProgram( _program );

            #endregion


        }

        // 每帧渲染运行 Update
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(new Color4(0.1f, 0.1f, 0.1f, 1.0f));
            _vao.Bind();
            GL.UseProgram(_program);        // 使用shader

            // 未使用索引
            if (_vao.IndexBufferObject == null)
            {
                // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            }
            // 使用索引
            else
            {
                GL.DrawElements(PrimitiveType.Triangles, _ibo.Length, DrawElementsType.UnsignedInt, 0);
            }




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
