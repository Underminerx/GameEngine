using OpenTK.Mathematics;

namespace Underminer_Core.Maths
{
    public struct Plane
    {
        public Vector3 Position;
        public Vector3 Normal;

        public float DistanceToPlane(Vector3 point)
        {
            return Vector3.Dot(point, Normal.Normalized()) - Vector3.Distance(Position, Vector3.Zero);
        }
    }
}
