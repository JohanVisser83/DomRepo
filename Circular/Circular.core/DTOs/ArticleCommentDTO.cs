using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public class ArticleCommentDTO
    {
        public long ArticleId { get; set; }
        public long CustomerId { get; set; }
        public long CommunityId { get; set; }
        public long? ParentCommentId { get; set; }
        public string Comment { get; set; }
    }
    public class ArticleListRequestDTO
    {
        public long ArticleId { get; set; }
        public long CustomerId { get; set; } 

        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 100;

    }
}
