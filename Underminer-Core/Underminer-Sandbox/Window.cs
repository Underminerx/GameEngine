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
using StbImageSharp;
using Underminer_Core.Rendering;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace Underminer_Sandbox
{
    internal class Window : GameWindow
    {

        float[] _vertices =
        {
        // ---- 位置----       - 纹理坐标 -    /*---- 颜色 ----*/
        -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,  /*0.0f, 0.0f, 1.0f,*/
         0.5f, -0.5f, -0.5f,  1.0f, 0.0f,  /*0.0f, 1.0f, 0.0f,*/
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,  /*1.0f, 0.0f, 0.0f,*/
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,  /*1.0f, 0.0f, 0.0f,*/
        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,  /*1.0f, 1.0f, 0.0f,*/
        -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,  /*0.0f, 0.0f, 1.0f,*/

        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  /*0.0f, 0.0f, 1.0f,*/
         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,  /*0.0f, 1.0f, 0.0f,*/
         0.5f,  0.5f,  0.5f,  1.0f, 1.0f,  /*1.0f, 0.0f, 0.0f,*/
         0.5f,  0.5f,  0.5f,  1.0f, 1.0f,  /*1.0f, 0.0f, 0.0f,*/
        -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,  /*1.0f, 1.0f, 0.0f,*/
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  /*0.0f, 0.0f, 1.0f,*/

        -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,  /*0.0f, 1.0f, 0.0f,*/
        -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,  /*1.0f, 0.0f, 0.0f,*/
        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,  /*1.0f, 1.0f, 0.0f,*/
        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,  /*1.0f, 1.0f, 0.0f,*/
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  /*0.0f, 0.0f, 1.0f,*/
        -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,  /*0.0f, 1.0f, 0.0f,*/

         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,  /*0.0f, 1.0f, 0.0f,*/
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,  /*1.0f, 0.0f, 0.0f,*/
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f,  /*1.0f, 1.0f, 0.0f,*/
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f,  /*1.0f, 1.0f, 0.0f,*/
         0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  /*0.0f, 0.0f, 1.0f,*/
         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,  /*0.0f, 1.0f, 0.0f,*/

        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,  /*1.0f, 1.0f, 0.0f,*/
         0.5f, -0.5f, -0.5f,  1.0f, 1.0f,  /*1.0f, 0.0f, 0.0f,*/
         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,  /*0.0f, 1.0f, 0.0f,*/
         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,  /*0.0f, 1.0f, 0.0f,*/
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  /*0.0f, 0.0f, 1.0f,*/
        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,  /*1.0f, 1.0f, 0.0f,*/

        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,  /*1.0f, 1.0f, 0.0f,*/
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,  /*1.0f, 0.0f, 0.0f,*/
         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,  /*0.0f, 1.0f, 0.0f,*/
         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,  /*0.0f, 1.0f, 0.0f,*/
        -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,  /*0.0f, 0.0f, 1.0f,*/
        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f   /*1.0f, 1.0f, 0.0f*/
        };

        uint[] _indices = {
            0, 1, 3, // 第一个三角形
            1, 2, 3  // 第二个三角形
        };

        private float width;
        private float height;

        private VertexArrayObject _vao;
        private VertexBufferObject _vbo;
        private IndexBufferObject _ibo;
        private Shader _shader;
        private Texture2D _texture01;

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
            layout.AddElement(new VertexBufferLayoutElement(0, 3),
                              new VertexBufferLayoutElement(1, 2));

            _vbo.AddLayout(layout);
            _ibo = new IndexBufferObject(_indices);         // _ibo在VertexArrayObject中绑定

            _vao = new VertexArrayObject(null, _vbo);
            _shader = new Shader("""D:\GameEngine\Underminer-Core\Underminer-Sandbox\Test.glsl""");
            _texture01 = new Texture2D("""D:\GameEngine\Underminer-Core\Underminer-Sandbox\texture01.png""");
        }

        private double _totleTime = 0;      // 运行总时间
        private Matrix4 _model;
        private Matrix4 _view;
        private Matrix4 _perspective;
        // 每帧渲染运行 Update
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(new Color4(0.1f, 0.1f, 0.1f, 1.0f));

            _vao.Bind();
            _shader.Bind();

            _model = Matrix4.Identity * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(30));
            _view = Matrix4.LookAt(new Vector3(0, 0, -3), Vector3.Zero, Vector3.UnitY);
            _perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), width / height, 0.1f, 1000);

            _shader.SetUniform("mainTex", 0);
            _texture01.Bind(0);

            // 设置MVP矩阵
            _shader.SetUniform("model", _model);
            _shader.SetUniform("view", _view);
            _shader.SetUniform("perspective", _perspective);

            GL.Enable(EnableCap.DepthTest);
            // GL.DrawElements(PrimitiveType.Triangles, _ibo.Length, DrawElementsType.UnsignedInt, 0);     //draw call   与GPU通信
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

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
            width = e.Width;
            height = e.Height;
        }
    }


}
