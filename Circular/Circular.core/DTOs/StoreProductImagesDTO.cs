
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public class StoreProductImagesDTO : BaseEntityDTO
    {
        public long ProductId { get; set; }
        public string ImagePath { get; set; }
        public string filename { get; set; }
    }
}
