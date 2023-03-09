using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.ECS.Components
{
    [DataContract] public class CHierarchy : IComponent
    {
        [DataMember] private Guid _id;
        [DataMember] private Guid? _parentId;
        [DataMember] private List<Guid> _childrenId;

        public Guid Id => _id;
        public Guid? ParentId => _parentId;      // 父物体Id 层级中根物体无父物体
        public List<Guid> ChildrenId => _childrenId;

        public CHierarchy(Guid id)
        {
            _id = id;
            _parentId = null;
            _childrenId = new();

        }


    }
}
