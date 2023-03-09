using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Underminer_Core.Rendering;
using Underminer_Core.Rendering.Enums;

namespace Underminer_Core.ECS.Components
{
    [DataContract]
    public class CCamera : IComponent
    {
        [DataMember] private Guid _id;
        public Guid Id => _id;

        private Camera _camera;
        public CCamera(Guid id)
        {
            _id = id;
            _camera = new();
        }

        [DataMember]
        public float Fov
        {
            get => _camera.Fov;
            set => _camera.Fov = value;
        }

        [DataMember]
        public float Near
        {
            get => _camera.Near;
            set => _camera.Near = value;
        }

        [DataMember]
        public float Far
        {
            get => _camera.Far;
            set => _camera.Far = value;
        }

        [DataMember]
        public Color4 ClearColor
        {
            get => _camera.ClearColor;
            set => _camera.ClearColor = new Color4(value.R, value.G, value.B, 255);
        }

        [DataMember]
        public bool IsFrustumGeometryCulling
        {
            get => _camera.IsFrustumGeometryCulling;
            set => _camera.IsFrustumGeometryCulling = value;
        }

        [DataMember]
        public bool IsFrustumLightCulling
        {
            get => _camera.IsFrustumLightCulling;
            set => _camera.IsFrustumLightCulling = value;
        }
        [DataMember]
        public EProjectionMode ProjectionMode
        {
            get => _camera.ProjectionMode;
            set => _camera.ProjectionMode = value;
        }

        public Matrix4 ViewMatrix => _camera.ViewMatrix;
        public Matrix4 PerspectiveMatrix => _camera.PerspectiveMatrix;

        /// <summary>
        /// 利用位置信息 屏幕宽高更新相机的VP矩阵
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void UpdateMatrix(CTransform transform, float width, float height) =>
            _camera.UpdateMatrices(transform.WorldPosition, transform.WorldRotation, width, height);
    }
}
