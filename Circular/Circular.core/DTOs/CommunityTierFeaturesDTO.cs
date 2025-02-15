using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public class CommunityTierFeaturesDTO
    {
        public decimal Price { get; set; }
        public string? AdditionalText { get; set; }

        public string? FeatureCode { get; set; }
    }
}
