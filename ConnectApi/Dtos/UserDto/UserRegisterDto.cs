using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConnectApi.Models.Contacts;

namespace ConnectApi.Dtos.UserDto
{
    public class UserRegisterDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public int ActiveShowTime { get; set; }
        public string TwilioUserId { get; set; }
        public string OneSignalUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Status { get; set; }
        public virtual List<Contact> Contacts { get; set; }
    }
}
