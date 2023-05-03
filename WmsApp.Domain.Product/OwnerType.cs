using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Items.SupportedTypes;

namespace WmsApp.Domain.Items
{
    public partial class OwnerType
    {
        private OwnerType() { }

        internal static OwnerType CreateOwnerType(int id, string name)
        {
            var status = new StatusValidatorHandler<OwnerType>();

            status.PropertyAttributeCheckByName<OwnerType>(name, nameof(Name));

            if (!status.IsValid)
                return null;

            var owe = new OwnerType()
            {
                Id = id,
                Name = name
            };

            status.SetResult(owe);

            return status.Result;
        }

        public static OwnerType GetDirectorType() => CreateOwnerType(1, "Director");
        public static OwnerType GetSupplyChainType() => CreateOwnerType(2, "Supply chain");
        public static OwnerType GetMerchantType() => CreateOwnerType(3, "Merchant");
        public static OwnerType GetCopywriterType() => CreateOwnerType(4, "Copywriter");
    }
}
