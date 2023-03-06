using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.ECS.Components
{
    public class CTransform : IComponent
    {
        public Guid Id { get; }
        public Vector3 LocationPosition
        {
            get => GetLocalPosition();
            set => SetLocalPosition(value);
        }
        public Quaternion LocalRotation
        {
            get => GetLocalRotation();
            set => SetLocalRotation(value);
        }

        public Vector3 LocalScale
        {
            get => GetLocalScale();
            set => SetLocalScale(value);
        }

        public Vector3 WorldPosition => GetWorldPosition();
        public Quaternion WorldRotation => GetWorldRotation();
        public Vector3 WorldScale => GetWorldScale();

        public Vector3 LocalForward => LocalRotation * Vector3.UnitZ;
        public Vector3 LocalUp => LocalRotation * Vector3.UnitY;
        public Vector3 LocalRight => LocalRotation * Vector3.UnitX;

        public Vector3 WorldForward => WorldRotation * Vector3.UnitZ;
        public Vector3 WorldUp => WorldRotation * Vector3.UnitY;
        public Vector3 WorldRight => WorldRotation * Vector3.UnitX;

        public CTransform(Guid id)
        {
            Id = id;

            _localPosition = new Vector3();
            _localRotation = Quaternion.Identity;
            _localScale = Vector3.One;
            _parentMatrix = Matrix4.Identity;
            _localMatrix = Matrix4.Identity;
            _worldMatrix = Matrix4.Identity;
            _isDirty = false;
        }

        private Vector3 _localPosition;
        private Quaternion _localRotation;
        private Vector3 _localScale;
        private Matrix4 _parentMatrix;
        private Matrix4 _localMatrix;
        private Matrix4 _worldMatrix;
        private bool _isDirty;      // 判断局部信息是否更改

        public void SetLocalPosition(Vector3 localPosition)
        {
            _localPosition = localPosition;
            _isDirty = true;
        }

        public Vector3 GetLocalPosition()
        {
            return _localPosition;
        }

        public void SetLocalRotation(Quaternion localRotation)
        {
            _localRotation = localRotation;
            _isDirty = true;
        }

        public Quaternion GetLocalRotation()
        {
            return _localRotation;
        }

        public void SetLocalScale(Vector3 localScale)
        {
            _localScale = localScale;
            _isDirty = true;
        }

        public Vector3 GetLocalScale()
        {
            return _localScale;
        }

        public Matrix4 GetWorldMatrix()
        {
            if (_isDirty)
                UpdateMatrices();
            return _worldMatrix;
        }

        public void SetLocalMatrix(Matrix4 localTransform) 
        {
            _localMatrix = localTransform;
            _worldMatrix = _parentMatrix * _localMatrix;
            ApplyTransform();
        }

        public Matrix4 GetLocalMatrix()
        {
            if (_isDirty)
                UpdateMatrices();
            return _localMatrix;
        }

        public Vector3 GetWorldPosition()
        {
            if (_isDirty)
                UpdateMatrices();
            return _worldMatrix.ExtractTranslation();
        }
        public Quaternion GetWorldRotation()
        {
            if (_isDirty)
                UpdateMatrices();
            return _worldMatrix.ExtractRotation();
        }
        public Vector3 GetWorldScale()
        {
            if (_isDirty)
                UpdateMatrices();
            return _worldMatrix.ExtractScale();
        }

        private void ApplyTransform()
        {
            // 局部矩阵分解
            _localPosition = _localMatrix.ExtractTranslation();
            _localRotation = _localMatrix.ExtractRotation();
            _localScale = _localMatrix.ExtractScale();
        
        }

        /// <summary>
        /// 将当前矩阵信息传递给子物体
        /// </summary>
        /// <param name="parentTransform">父物体</param>
        internal void SetWorldMatrix(Matrix4 parentTransform)
        {
            if (_isDirty)
                UpdateMatrices();

            _parentMatrix = parentTransform;
            _worldMatrix = parentTransform * _localMatrix;
        }

        private void UpdateMatrices()
        {
            // 缩放 -> 旋转 -> 平移   行主式 右乘矩阵
            _localMatrix = Matrix4.CreateScale(_localScale) *
                           Matrix4.CreateFromQuaternion(_localRotation) *
                           Matrix4.CreateTranslation(_localPosition);

            _worldMatrix = _parentMatrix * _localMatrix;

            _isDirty = false;
        }
    }
}
