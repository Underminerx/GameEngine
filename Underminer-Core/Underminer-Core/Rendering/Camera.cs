using OpenTK.Mathematics;
using Underminer_Core.Rendering.Enums;

namespace Underminer_Core.Rendering
{
    public struct Camera
    {
        public float Fov;
        public float Near;
        public float Far;
        public Color4 ClearColor;
        public bool IsFrustumGeometryCulling;
        public bool IsFrustumLightCulling;
        public EProjectionMode ProjectionMode;

        public Matrix4 ViewMatrix;
        public Matrix4 PerspectiveMatrix;

        public Camera()
        {
            Fov = 45;
            Near = 0.1f;
            Far = 1000;
            ClearColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
            IsFrustumGeometryCulling = true;
            IsFrustumLightCulling = false;
            ProjectionMode = EProjectionMode.Perspective;
        }

        /// <summary>
        /// 每帧更新相机相关矩阵
        /// </summary>
        /// <param name="position">相机位置</param>
        /// <param name="rotation">相机朝向</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void UpdateMatrices(Vector3 position, Quaternion rotation, float width, float height)
        {
            Vector3 unitZ = rotation * Vector3.UnitZ;
            ViewMatrix = Matrix4.LookAt(position, position + unitZ, Vector3.UnitY);
            PerspectiveMatrix =
                ProjectionMode == EProjectionMode.Perspective ? 
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Fov), width / height, Near, Far) : 
                Matrix4.CreatePerspectiveFieldOfView(width, height, Near, Far);
        }
    }
}
