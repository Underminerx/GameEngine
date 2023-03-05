using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.IO;
using Underminer_Core.Log;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace Underminer_Core.Rendering.Resources
{
    public class Shader : IDisposable
    {
        public int Id { get; private set; }
        public string Path { get; }

        private Dictionary<string, int> _cache = new Dictionary<string, int>();
        public List<UniformInfo> UniformInfos { get; private set; } = new List<UniformInfo>();      // 保存当前shader内的所有uniform值

        public bool IsDestroy { get; private set; } = false;

        public static Shader? Create(string path)
        {
            Shader? shader = null;

            try
            {
                shader = new Shader(path);
            }
            catch (Exception e)
            {
                UmLog.ErrorLogCore(e.Message);
            }

            return shader;

        }  
        public static Shader Create(string vertexShaderSource, string fragmentShaderSource) => 
            new Shader(vertexShaderSource, fragmentShaderSource);



        public void Bind()
        {
            GL.UseProgram(Id);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        // 为了修改shader后更新内存中的shader 先编译
        public void ReCompile()
        {
            if (Path != null)                   // 从字符串加载的不会重新编译
            {
                GL.DeleteProgram(Id);           // 删除之前的
                (string vertexSource, string fragmentSource) = LoadShaderFromDisk(Path);
                CreatProgram(vertexSource, fragmentSource);     // 重新获取新shader 更新Id
            }
        }

        #region 设置shader中uniform变量的值
        public void SetUniform(string name, int v) => GL.Uniform1(GetUniformLocation(name), v);
        public void SetUniform(string name, float v) => GL.Uniform1(GetUniformLocation(name), v);
        public void SetUniform(string name, Vector2 v) => GL.Uniform2(GetUniformLocation(name), v);
        public void SetUniform(string name, Vector3 v) => GL.Uniform3(GetUniformLocation(name), v);
        public void SetUniform(string name, Vector4 v) => GL.Uniform4(GetUniformLocation(name), v);
        //public void SetUniform(string name, Matrix3x4 v) => GL.UniformMatrix3x4(GetUniformLocation(name), false, ref v);
        //public void SetUniform(string name, Matrix2x4 v) => GL.UniformMatrix2x4(GetUniformLocation(name), false, ref v);
        //public void SetUniform(string name, Matrix2x3 v) => GL.UniformMatrix2x3(GetUniformLocation(name), false, ref v);
        public void SetUniform(string name, Matrix2 v) => GL.UniformMatrix2(GetUniformLocation(name), false, ref v);
        public void SetUniform(string name, Matrix3 v) => GL.UniformMatrix3(GetUniformLocation(name), false, ref v);
        public void SetUniform(string name, Matrix4 v) => GL.UniformMatrix4(GetUniformLocation(name), false, ref v);     // OpenGL主列 MVP左乘 OpenTK主行 MVP右乘
        #endregion

        #region 获取shdaer中uniform变量的值
        public void GetUniform(string name, out int v) => GL.GetUniform(Id, GetUniformLocation(name), out v);
        public void GetUniform(string name, out float v) => GL.GetUniform(Id, GetUniformLocation(name), out v);
        public void GetUniform(string name, out Vector2 v)
        {
            float[] res = new float[2];
            GL.GetUniform(Id, GetUniformLocation(name), res);
            v = new Vector2(res[0], res[1]);
        }
        public void GetUniform(string name, out Vector3 v)
        {
            float[] res = new float[3];
            GL.GetUniform(Id, GetUniformLocation(name), res);
            v = new Vector3(res[0], res[1], res[2]);
        }
        public void GetUniform(string name, out Vector4 v)
        {
            float[] res = new float[4];
            GL.GetUniform(Id, GetUniformLocation(name), res);
            v = new Vector4(res[0], res[1], res[2], res[3]);
        }
        public void GetUniform(string name, out Matrix3 v)
        {
            float[] res = new float[9];
            GL.GetUniform(Id, GetUniformLocation(name), res);
            v = new Matrix3(res[0], res[1], res[2], 
                            res[3], res[4], res[5], 
                            res[6], res[7], res[8]);
        }
        public void GetUniform(string name, out Matrix4 v)
        {
            float[] res = new float[16];
            GL.GetUniform(Id, GetUniformLocation(name), res);
            v = new Matrix4(res[0],  res[1],  res[2],  res[3], 
                            res[4],  res[5],  res[6],  res[7],
                            res[8],  res[9],  res[10], res[11],
                            res[12], res[13], res[14], res[15]);
        }

        #endregion
        
        public void QueryUniforms()
        {
            UniformInfos.Clear();
            GL.GetProgram(Id, GetProgramParameterName.ActiveUniforms, out int uniformCount);        // 确定shader中有多少个uniform
            for (int i = 0; i < uniformCount; i++)
            {
                GL.GetActiveUniform(Id, i, 1024, out _, out _, out ActiveUniformType uniformType, out string name);
                object? value = null;
                switch (uniformType)
                {
                    case ActiveUniformType.Bool:
                        int v;
                        GetUniform(name, out v);
                        value = v;
                        break;
                    case ActiveUniformType.Int:
                        int v1;
                        GetUniform(name, out v1);
                        value = v1;
                        break;
                    case ActiveUniformType.Float:
                        int v2;
                        GetUniform(name, out v2);
                        value = v2;
                        break;
                    case ActiveUniformType.FloatVec2:
                        Vector2 v3 = new();
                        GetUniform(name, out v3);
                        value = v3;
                        break;
                    case ActiveUniformType.FloatVec3:
                        Vector3 v4 = new();
                        GetUniform(name, out v4);
                        value = v4;
                        break;
                    case ActiveUniformType.FloatVec4:
                        Vector4 v5 = new();
                        GetUniform(name, out v5);
                        value = v5;
                        break;
                    case ActiveUniformType.FloatMat3:
                        Matrix3 v6 = new();
                        GetUniform(name, out v6);
                        value = v6;
                        break;
                    case ActiveUniformType.FloatMat4:
                        Matrix4 v7 = new();
                        GetUniform(name, out v7);
                        value = v7;
                        break;
                    case ActiveUniformType.Sampler2D:
                        // 默认为None
                        break;
                }
                UniformInfos.Add(new UniformInfo(name, GetUniformLocation(name), uniformType, value));      // 存入数据结构
            }
        }

        private Shader(string path)
        {
            Path = path;

            (string vertexSource, string fragmentSource) = LoadShaderFromDisk(path);
            CreatProgram(vertexSource, fragmentSource);
            QueryUniforms();
        }

        private Shader(string vertexShaderSource, string fragmentShaderSource)
        {
            CreatProgram(vertexShaderSource, fragmentShaderSource);
            QueryUniforms();
        }

        private int GetUniformLocation(string name)
        {
            if (_cache.ContainsKey(name)) return _cache[name];      // 存在则直接返回
            int location = GL.GetUniformLocation(Id, name);
            _cache.Add(name, location);
            return location;
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
                UmLog.ErrorLogCore($"{info}");
            }
            // 绑定之后释放shader内存
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
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
                UmLog.ErrorLogCore($"{type.ToString()}\t{info}");
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
            if (!IsDestroy)
            {
                ReleaseUnmanagedResources();
                GC.SuppressFinalize(this);
                IsDestroy = true;
            }

        }

        ~Shader()
        {
            ReleaseUnmanagedResources();
        }
    }
}
