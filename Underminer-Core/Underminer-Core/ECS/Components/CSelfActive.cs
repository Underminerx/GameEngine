using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.ECS.Components
{
    [DataContract] public class CSelfActive : IComponent
    {
        [DataMember] private Guid _id;
        public Guid Id => _id;
        public bool IsSelfActive { get; set; }

        public CSelfActive(Guid id)
        {
            _id = id;
            IsSelfActive = true;
        }

        public CSelfActive(Guid id, bool isSelfActive)
        {
            _id = id;
            IsSelfActive = isSelfActive;
        }
    }
}
