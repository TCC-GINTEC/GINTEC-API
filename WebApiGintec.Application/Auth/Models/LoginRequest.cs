using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Auth.Models
{
    public class LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
