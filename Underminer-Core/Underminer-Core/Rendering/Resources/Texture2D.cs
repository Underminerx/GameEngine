using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.Rendering.Resources
{
    public class Texture2D : IDisposable
    {
        public int Id { get; }
        public string? Path { get; }
        public TextureWrapMode WrapModeS { get; set; }
        public TextureWrapMode WrapModeT { get; set; }
        public TextureMagFilter TextureMagFilter { get; set; }
        public TextureMinFilter TextureMinFilter { get; set; }
        public bool IsMipmap { get; set; }
        public bool IsDestroy { get; private set; } = false;
        private Texture2D(string path, TextureWrapMode wrapModeS, TextureWrapMode wrapModeT,
                          TextureMagFilter magFilter, TextureMinFilter minFilter, bool isMipmap = false)
        {
            ImageResult? image = LoadTexture2DFromDisk(path);
            if (image != null)
            {
                Path = path;
                Id = GL.GenTexture();
                GL.ActiveTexture(TextureUnit.Texture0);     // 激活0号纹理插槽
                GL.BindTexture(TextureTarget.Texture2D, Id);
                WrapModeS = wrapModeS;
                WrapModeT = wrapModeT;
                TextureMagFilter = magFilter;
                TextureMinFilter = minFilter;
                IsMipmap = isMipmap;
                int ws = (int)wrapModeS;
                int wt = (int)wrapModeT;
                int magF = (int)magFilter;
                int minF = (int)minFilter;
                GL.TextureParameterI(Id, TextureParameterName.TextureWrapS, ref ws);
                GL.TextureParameterI(Id, TextureParameterName.TextureWrapT, ref wt);
                GL.TextureParameterI(Id, TextureParameterName.TextureMagFilter, ref magF);
                GL.TextureParameterI(Id, TextureParameterName.TextureMinFilter, ref minF);

                GL.TexImage2D(TextureTarget.Texture2D, 0, 
                    PixelInternalFormat.CompressedRgbaS3tcDxt5Ext,      // 写死了 以后扩展需要改
                    image.Width, image.Height,
                    0, PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    image.Data);
                if (isMipmap)
                {
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                }

            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private Texture2D(Color4 color)
        {
            Id = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);     // 激活0号纹理插槽
            GL.BindTexture(TextureTarget.Texture2D, Id);
            WrapModeS = TextureWrapMode.Repeat;
            WrapModeT = TextureWrapMode.Repeat;
            TextureMagFilter = TextureMagFilter.Linear;
            TextureMinFilter = TextureMinFilter.Nearest;
            int ws = (int)WrapModeS;
            int wt = (int)WrapModeT;
            int magF = (int)TextureMagFilter;
            int minF = (int)TextureMinFilter;
            GL.TextureParameterI(Id, TextureParameterName.TextureWrapS, ref ws);
            GL.TextureParameterI(Id, TextureParameterName.TextureWrapT, ref wt);
            GL.TextureParameterI(Id, TextureParameterName.TextureMagFilter, ref magF);
            GL.TextureParameterI(Id, TextureParameterName.TextureMinFilter, ref minF);

            // 生成单像素颜色纹理
            GL.TexImage2D(TextureTarget.Texture2D, 0,PixelInternalFormat.CompressedRgbaS3tcDxt5Ext, 1, 1, 0,
                PixelFormat.Rgba, PixelType.Float, new[]{ color.R, color.G, color.B, color.A });
        }


        public static Texture2D Create(string path,
            TextureWrapMode wrapModeS = TextureWrapMode.Repeat,
            TextureWrapMode wrapModeT = TextureWrapMode.Repeat,
            TextureMagFilter magFilter = TextureMagFilter.Linear,
            TextureMinFilter minFilter = TextureMinFilter.Nearest,
            bool isMipmap = false) => new Texture2D(path, wrapModeS, wrapModeT, magFilter, minFilter);

        public static Texture2D Create(Color4 color) => new Texture2D(color);

        public void Bind(int solt = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + solt);
            GL.BindTexture(TextureTarget.Texture2D, Id);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private ImageResult? LoadTexture2DFromDisk(string path)
        {
            try
            {
                // 对图片进行上下反转  加载时从左上角加载 stb纹理映射时从左下角映射
                StbImage.stbi_set_flip_vertically_on_load(1);
                return ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private void ReleaseUnmanagedResources()
        {
            // 释放内存
            GL.DeleteTexture(Id);
        }

        public void Dispose()
        {
            if (!IsDestroy)
            {
                ReleaseUnmanagedResources();
                GC.SuppressFinalize(this);
                IsDestroy = true;
            }

        }

        ~Texture2D()
        {
            ReleaseUnmanagedResources();
        }
    }
}
