using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConnectApi.Models.Contacts;
using ConnectApi.Models.UserOtpInfo;
using ConnectApi.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace ConnectApi.Services.AppDatabaseContext
{
    public class AppDataContext : DbContext
    {
        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserOtpInfo> UserOtpInfos { get; set; }
        public DbSet<Contact> Contacts { get; set; }

    }
}
