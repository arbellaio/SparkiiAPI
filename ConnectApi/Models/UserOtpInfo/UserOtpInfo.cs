using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectApi.Models.UserOtpInfo
{
    public class UserOtpInfo
    {
        public int Id { get; set; }
        public string OtpCode { get; set; }
        public int? UserId { get; set; }
        public string PhoneNumber  { get; set; }
        public DateTime CreatedDate { get; set; }
        public int NumberOfAttempts { get; set; }

    }
}
