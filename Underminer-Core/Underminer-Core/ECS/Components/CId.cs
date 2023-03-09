using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.ECS.Components
{
    [DataContract] public class CId : IComponent
    {
        [DataMember] private Guid _id;
        public Guid Id => _id;

        public CId()
        {
            _id = Guid.NewGuid();        // Guid视为全局唯一
        }
    }
}
