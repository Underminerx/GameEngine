using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Underminer_Sandbox
{
    internal class Window : GameWindow
    {

        float[] _vertices =
        {
            -0.5f, -0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
             0.0f,  0.5f, 0.0f
        };

        private int _vao;       // 顶点序列id
        private int _vbo;       // 顶点缓冲id
        private int _program;   // 渲染管线id

        public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
        { Size = (width, height), Title = title })
        {

        }

        // 窗口创建完成 第一次运行
        protected override void OnLoad()
        {
            // 绑定vao 为了指定layout
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            // 获取vbo
            _vbo = GL.GenBuffer();
            // 绑定vbo
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            // 缓冲区类型 缓冲区大小 要传的数据 传入类型 (static不动 Dynamic少动 Stream多动)
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            // 顶点shader代码 (@转义)
            string vertexSource = @"#version 330 core
                layout (location = 0) in vec3 aPos;
                layout (location = 1) in vec3 aColor;

                void main()
                {
                    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
                }";
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);


            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            // 将shader内容与类型绑定
            GL.ShaderSource(vertexShader, vertexSource);
            // 编译shader
            GL.CompileShader(vertexShader);

            // 片段shader代码 
            string fragmentSource = @"#version 330 core
                out vec4 FragColor;

                void main()
                {
                    FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
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


        }

        // 每帧渲染运行 Update
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.AntiqueWhite);
            GL.BindVertexArray(_vao);       // 绑定vao
            GL.UseProgram(_program);        // 使用shader
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

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
