using OpenTK.Mathematics;

namespace Underminer_Core.Maths
{
    /// <summary>
    /// 一点和一法向量确定一个平面
    /// </summary>
    public readonly struct Plane
    {
        public readonly Vector3 Position;
        public readonly Vector3 Normal;

        public Plane(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }

        public readonly float DistanceToPlane(in Vector3 point)
        {
            return Vector3.Dot(point, Vector3.Normalize(Normal)) - Vector3.Distance(Position, Vector3.Zero);
        }
    }
}
