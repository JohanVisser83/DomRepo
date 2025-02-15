using Circular.Core.Entity;
using System.ComponentModel.DataAnnotations;

namespace CircularHQ.Models
{
    public class HQLoginModel
    {
        public HQLoginModel()
        {
        }

        public string? username { get; set; } = "";

        [DataType(DataType.Password)]
        public string? Password { get; set; } = "";

    }

}


