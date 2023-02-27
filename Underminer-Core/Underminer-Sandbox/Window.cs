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
             0.5f,  0.5f, 0.0f, 1, 0, 0,    // 右上角
             0.5f, -0.5f, 0.0f, 0, 1, 0,    // 右下角
            -0.5f, -0.5f, 0.0f, 0, 0, 1,    // 左下角
            -0.5f,  0.5f, 0.0f, 1, 0, 1     // 左上角
        };

        uint[] _indices = {
            0, 1, 3, // 第一个三角形
            1, 2, 3  // 第二个三角形
        };


        private int _vao;       // 顶点序列id vertex  array  object
        private int _vbo;       // 顶点缓冲id vertex  buffer object
        private int _ebo;       // 元素缓冲id element buffer object / ibo 索引缓冲id Index buffer object
        private int _program;   // 渲染管线id

        public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
        { Size = (width, height), Title = title })
        {

        }

        // 窗口创建完成 第一次运行
        protected override void OnLoad()
        {
            // 绘制线框
            // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            
            // 绑定vao 为了指定layout
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            // 获取vbo
            _vbo = GL.GenBuffer();
            // 绑定vbo
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            // 缓冲区类型 缓冲区大小 要传的数据 传入类型 (static不动 Dynamic少动 Stream多动)
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            // location位置0 指向aPos aPos内容格式
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            // location位置1 指向aColor
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

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


            // 创建索引缓冲对象
            _ebo = GL.GenBuffer();
            // 顶点位置数据
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

        }

        // 每帧渲染运行 Update
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.AntiqueWhite);
            GL.BindVertexArray(_vao);       // 绑定vao
            GL.UseProgram(_program);        // 使用shader
            // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);


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
