using Circular.Core.Entity;
using System.ComponentModel.DataAnnotations;

namespace CircularWeb.Models
{
    public class LoginModel
    {
        public LoginModel()
        {
        }

        public string? username { get; set; } = "";

        [DataType(DataType.Password)]
        public string? Password { get; set; } = "";

    }

}


