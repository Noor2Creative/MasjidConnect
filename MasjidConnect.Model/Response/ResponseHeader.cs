using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasjidConnect.Model.Response
{
    public class ResponseHeader
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
    }
}
