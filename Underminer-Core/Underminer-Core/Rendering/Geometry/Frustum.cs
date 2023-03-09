using OpenTK.Mathematics;
using Underminer_Core.Maths;

namespace Underminer_Core.Rendering.Geometry
{
    public struct Frustum
    {
        public Plane NearPlane;
        public Plane FarPlane;
        public Plane LeftPlane;
        public Plane RightPlane;
        public Plane TopPlane;
        public Plane BottomPlane;

        public void CalculateFrustum(Camera camera,float aspect, Vector3 position, Quaternion rotation)
        {
            Vector3 front = rotation * Vector3.UnitZ;
            Vector3 right = rotation * Vector3.UnitX;
            Vector3 up = rotation * Vector3.UnitY;
            float halfVSide = camera.Far * (float)MathHelper.Tan(MathHelper.DegreesToRadians(camera.Fov / 2));
            float halfHSide = halfVSide * aspect;
            Vector3 frontMultFar = camera.Far * front;

            #region 视锥体六个面
            NearPlane = new Plane(position + camera.Near * front, front);
            FarPlane = new Plane(position + camera.Far * front, -front);
            RightPlane = new Plane(position, Vector3.Cross(frontMultFar - right * halfHSide, up));
            LeftPlane = new Plane(position, Vector3.Cross(up, frontMultFar + right * halfHSide));
            TopPlane = new Plane(position, Vector3.Cross(right, up * halfVSide + frontMultFar));
            BottomPlane = new Plane(position, Vector3.Cross(frontMultFar - up * halfVSide, right));
            #endregion
        }

        public bool IsSphereInFrustum(Sphere sphere)
        {
            return NearPlane.  DistanceToPlane(sphere.Position) >= -sphere.Radius &&
                   FarPlane.   DistanceToPlane(sphere.Position) >= -sphere.Radius &&
                   BottomPlane.DistanceToPlane(sphere.Position) >= -sphere.Radius &&
                   TopPlane.   DistanceToPlane(sphere.Position) >= -sphere.Radius &&
                   LeftPlane.  DistanceToPlane(sphere.Position) >= -sphere.Radius &&
                   RightPlane. DistanceToPlane(sphere.Position) >= -sphere.Radius;
        }

        public bool IsBoundingSphereInFrustum(Quaternion rotation, Vector3 scale, Sphere sphere)
        {
            // 获得使得最大的缩放轴 以得到最大的包围球
            float radius = MathHelper.Max(scale.X, MathHelper.Max(scale.Y, scale.Z)) * sphere.Radius;
            // 得到旋转后的中心点
            Vector3 centerPos = rotation * sphere.Position;

            return IsSphereInFrustum(new Sphere(centerPos, radius));
        }

    }
}
