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
using Underminer_Core.Log;
using Underminer_Core.Maths;
using Underminer_Core.Rendering;
using Underminer_Core.Rendering.Resources;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace Underminer_Sandbox
{
    internal class Window : GameWindow
    {
        private float width;
        private float height;

        private Shader _shader;
        private Texture2D _texture01;
        private Model _myModel;

        // 创建窗口
        public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
        { Size = (width, height), Title = title }) { }

        // 窗口创建完成 第一次运行
        protected override void OnLoad()
        {
            _myModel = Model.Create("""..\..\..\backpack\backpack.obj""");
            _texture01 = Texture2D.Create("""..\..\..\backpack\diffuse.jpg""");
            _shader = Shader.Create("""..\..\..\Shaders\Test.glsl""");
        }

        private double _totleTime = 0;      // 运行总时间
        private Matrix4 _model;
        private Matrix4 _view;
        private Matrix4 _perspective;
        // 每帧渲染运行 Update
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.ClearColor(new Color4(0.1f, 0.1f, 0.1f, 1.0f));

            _shader.Bind();
            // 先缩放 后旋转 再平移 
            _model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians((float)(_totleTime*10)));
            _view = Matrix4.LookAt(new Vector3(0, 0, -10), Vector3.Zero, Vector3.UnitY);
            _perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), width / height, 0.1f, 1000);

            _shader.SetUniform("mainTex", 0);
            _texture01.Bind(0);

            // 设置MVP矩阵
            _shader.SetUniform("model", _model);
            _shader.SetUniform("view", _view);
            _shader.SetUniform("perspective", _perspective);

            // 开始渲染
            foreach (var mesh in _myModel.Meshes)
            {
                mesh.Bind();
                GL.Enable(EnableCap.DepthTest);
                if (mesh.IndexCount > 3)
                {
                    GL.DrawElements(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
                }
                else
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.VertexCount);
                }
            }

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
