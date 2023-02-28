using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace Underminer_Core.Rendering
{
    public class Shader : IDisposable
    {
        public int Id { get; private set; }
        public string Path { get; }

        private Dictionary<string, int> _cache = new Dictionary<string, int>();

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

        public void SetUniform(string name, int v) => GL.Uniform1(GetUniformLocation(name), v);
        public void SetUniform(string name, float v) => GL.Uniform1(GetUniformLocation(name), v);
        public void SetUniform(string name, Vector2 v) => GL.Uniform2(GetUniformLocation(name), v);
        public void SetUniform(string name, Vector3 v) => GL.Uniform3(GetUniformLocation(name), v);
        public void SetUniform(string name, Vector4 v) => GL.Uniform4(GetUniformLocation(name), v);
        //public void SetUniform(string name, Matrix3x4 v) => GL.UniformMatrix3x4(GetUniformLocation(name), true, ref v);
        //public void SetUniform(string name, Matrix2x4 v) => GL.UniformMatrix2x4(GetUniformLocation(name), true, ref v);
        //public void SetUniform(string name, Matrix2x3 v) => GL.UniformMatrix2x3(GetUniformLocation(name), true, ref v);
        public void SetUniform(string name, Matrix2 v) => GL.UniformMatrix2(GetUniformLocation(name), true, ref v);
        public void SetUniform(string name, Matrix3 v) => GL.UniformMatrix3(GetUniformLocation(name), true, ref v);
        public void SetUniform(string name, Matrix4 v) => GL.UniformMatrix4(GetUniformLocation(name), true, ref v);     // OpenGL 主列 OpenTK 主行


        private int GetUniformLocation(string name)
        {
            if (_cache.ContainsKey(name)) return _cache[name];      // 存在则直接返回
            int location = GL.GetUniformLocation(Id, name);
            _cache.Add(name, location);
            return location;
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
                Console.WriteLine($"error:{type.ToString()} \n {info}");
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
