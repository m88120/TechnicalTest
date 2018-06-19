using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalCore.Models
{
    public class ResponseModel<T>
    {
        public bool status { get; set; }

        public T Data { get; set; }
        public string message { get; set; }
    }
}
