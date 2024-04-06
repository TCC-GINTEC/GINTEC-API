using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Util
{
    public class GenericResponse<T>
    {
        public string mensagem { get; set; }
        public T response { get; set; }
        public Exception? error { get; set; }
    }
}
