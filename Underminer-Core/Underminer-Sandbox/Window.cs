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
using Underminer_Core.ECS.Components;
using Underminer_Core.Log;
using Underminer_Core.Maths;
using Underminer_Core.Rendering;
using Underminer_Core.Rendering.Resources;
using Underminer_Core.Tools;
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
        private CCamera _camera;

        // 创建窗口
        public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
        { Size = (width, height), Title = title }) { }

        // 窗口创建完成 第一次运行
        protected override void OnLoad()
        {
            _camera = new(Guid.NewGuid());
            _cTransform = new(Guid.NewGuid());
            _cTransform.LocationPosition += new Vector3(0, 0, -10);
            //// 序列化
            //CTransform c1 = new CTransform(Guid.NewGuid());
            //c1.LocationPosition += new Vector3(1, 1, 1);
            //SerializeHelper.Serialize(c1, """..\..\..\xml\test.xml""");
            //SerializeHelper.DeSerialize("""..\..\..\xml\test.xml""", out CTransform? c2);

            _myModel = Model.Create("""..\..\..\backpack\backpack.obj""");
            _texture01 = Texture2D.Create("""..\..\..\backpack\diffuse.jpg""");
            _shader = Shader.Create("""..\..\..\Shaders\Test.glsl""");
        }

        private double _totleTime = 0;      // 运行总时间
        private Matrix4 _model;
        private CTransform _cTransform;
        // 每帧渲染运行 Update
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.ClearColor(_camera.ClearColor);

            _shader.Bind();

            _model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians((float)(_totleTime * 10)));


            _shader.SetUniform("mainTex", 0);
            _texture01.Bind(0);
            _camera.UpdateMatrix(_cTransform, width, height);

            // 设置MVP矩阵
            _shader.SetUniform("model", _model);
            _shader.SetUniform("view", _camera.ViewMatrix);
            _shader.SetUniform("perspective", _camera.PerspectiveMatrix);

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
