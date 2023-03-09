using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.ECS.Components
{
    [DataContract] public class CName : IComponent
    {
        [DataMember] private Guid _id;

        public Guid Id => _id;
        public string Name { get; set; }
    
        public CName(Guid id)
        {
            _id = id;
            Name = "GameObject";
        }
        public CName(Guid id, string name)
        {
            _id = id;
            Name = name;
        }
    }
}
