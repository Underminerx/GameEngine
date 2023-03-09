using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.ECS.Components
{
    [DataContract] public class CTag : IComponent
    {
        [DataMember] private Guid _id;
        [DataMember] public string Tag { get; set; }

        public Guid Id => _id;

        public CTag(Guid id)
        {
            _id = id;
            Tag = "";
        }

        public CTag(Guid id, string tag)
        {
            _id = id;
            Tag = tag;
        }


        // public bool Compare(string tag) => tag.Equals(Tag);


    }
}
