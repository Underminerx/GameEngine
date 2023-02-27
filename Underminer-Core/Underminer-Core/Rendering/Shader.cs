using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.Rendering
{
    public class Shader : IDisposable
    {
        public int Id { get; private set; }

        public string Path { get; }

        public Shader(string path)
        {
            Path = path;
            (string vertexSource, string fragmentSource) = LoadShaderFromDisk(path);
            CreatProgram(vertexSource, fragmentSource);
        }

        public Shader(string vertexShaderSource, string fragmentShaderSource)
        {
            CreatProgram(vertexShaderSource, fragmentShaderSource);
        }

        private void CreatProgram(string vertexShaderSource, string fragmentShaderSource)
        {
            int vertexShader = CreatShader(ShaderType.VertexShader, vertexShaderSource);
            int fragmentShader = CreatShader(ShaderType.FragmentShader, fragmentShaderSource);

            Id = GL.CreateProgram();
            GL.AttachShader(Id, vertexShader);
            GL.AttachShader(Id, fragmentShader);
            GL.LinkProgram(Id);
            GL.GetProgram(Id, GetProgramParameterName.LinkStatus, out int succeed);
            if (succeed == 0)
            {
                GL.GetShaderInfoLog(Id, out string info);
                Console.WriteLine(info);
            }
            // 绑定之后释放shader内存
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Bind()
        {
            GL.UseProgram(Id);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        private static (string vertexShaderSource, string fragmentShaderSource) LoadShaderFromDisk(string path)
        {
            string[] lines = File.ReadAllLines(path);       // 读取每一行 存入数组
            int vertexIndex = Array.IndexOf(lines, "#shader vertex");
            int fragmentIndex = Array.IndexOf(lines, "#shader fragment");
            string[] vertexLines = lines.Skip(vertexIndex + 1).Take(fragmentIndex - vertexIndex - 1).ToArray();
            string[] fragmentLines = lines.Skip(fragmentIndex + 1).ToArray();
            string vertexShader = string.Join("\n", vertexLines);
            string fragmentShader = string.Join("\n", fragmentLines);
            return (vertexShader, fragmentShader);
        }


        private static int CreatShader(ShaderType type, string source)
        {
            int id = GL.CreateShader(type);
            GL.ShaderSource(id, source);
            GL.CompileShader(id);
            GL.GetShader(id, ShaderParameter.CompileStatus, out int succeed);
            if (succeed == 0)
            {
                GL.GetShaderInfoLog(id, out string info);
                Console.WriteLine(info);
            }

            return id;
        }

        private void ReleaseUnmanagedResources()
        {
            // 释放program内存
            GL.DeleteProgram(Id);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Shader()
        {
            ReleaseUnmanagedResources();
        }
    }
}
