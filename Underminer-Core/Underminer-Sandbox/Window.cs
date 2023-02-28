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

namespace Underminer_Sandbox
{
    internal class Window : GameWindow
    {

        float[] _vertices =
        {
        //     ---- 位置 ----       ---- 颜色 ----      - 纹理坐标 -
             0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,   1.0f, 1.0f,   // 右上
             0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,   1.0f, 0.0f,   // 右下
            -0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f,   0.0f, 0.0f,   // 左下
            -0.5f,  0.5f, 0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 1.0f    // 左上
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
            layout.AddElement(new VertexBufferLayoutElement(0, 3),
                              new VertexBufferLayoutElement(1, 3), 
                              new VertexBufferLayoutElement(2, 2));
            _vbo.AddLayout(layout);
            _ibo = new IndexBufferObject(_indices);         // _ibo在VertexArrayObject中绑定
            _vao = new VertexArrayObject(_ibo, _vbo);
            _vao.Bind();
            _shader = new Shader("""D:\GameEngine\Underminer-Core\Underminer-Sandbox\Test.glsl""");

            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            int warpS = (int)TextureWrapMode.Repeat;
            int warpT = (int)TextureWrapMode.Repeat;
            int magFliter = (int)TextureMagFilter.Linear;       // 放大用线性过滤
            int minFilter = (int)TextureMagFilter.Nearest;      // 缩小用临近过滤
            GL.TextureParameterI(textureId, TextureParameterName.TextureWrapS, ref warpS);          // 配置横坐标
            GL.TextureParameterI(textureId, TextureParameterName.TextureWrapT, ref warpT);          // 配置纵坐标
            GL.TextureParameterI(textureId, TextureParameterName.TextureMagFilter, ref magFliter);      // 放大过滤
            GL.TextureParameterI(textureId, TextureParameterName.TextureMinFilter, ref minFilter);      // 缩小过滤

            // 对图片进行上下反转  加载时从左上角加载 stb纹理映射时从左下角映射
            StbImage.stbi_set_flip_vertically_on_load(1);

            ImageResult image = ImageResult.FromStream(File.OpenRead("""D:\GameEngine\Underminer-Core\Underminer-Sandbox\texture.png"""), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);


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
