using System.Collections.Generic;
using ConnectApi.Models.Contacts;

namespace ConnectApi.Dtos.UserDto
{
    public class UploadContactsDto
    {
        public List<Contact> Contacts { get; set; }
        public int UserId { get; set; }
    }
}