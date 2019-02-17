using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectApi.Dtos.OtpDto
{
    public class GetOTP
    {
        public string PhoneNumber { get; set; }
        public string OtpCode { get; set; }
    }
}
