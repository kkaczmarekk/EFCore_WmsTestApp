using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Items.SupportedTypes
{
    public record OwnerWithTypeDto
    {
        public OwnerWithTypeDto(Owner owner, OwnerType type)
        {
            Owner = owner;
            Type = type;
        }

        public Owner Owner { get; }
        public OwnerType Type { get; }
    }
}
