using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Underminer_Core.ECS.Components
{
    public interface IComponent
    {
        /// <summary>
        /// component 所属的Actor(Entity)的Id
        /// </summary>
        Guid Id { get; }
    }
}
