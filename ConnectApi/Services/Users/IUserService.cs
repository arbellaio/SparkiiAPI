using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConnectApi.Models.Contacts;
using ConnectApi.Models.UserOtpInfo;
using ConnectApi.Models.Users;

namespace ConnectApi.Services.Users
{
    public interface IUserService
    {
        Task<User> Login(string user, string password);
        Task<User> Register(User user, string password);
        Task<bool> UserExists(string username);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserById(int id);
        Task<List<User>> GetAllUsers();
        Task<int> GetNumberOfAttempts(string phonenumber);
        Task<User> GetUserByPhoneNumber(string phoneNumber);
        Task<UserOtpInfo> SaveGeneratedOtpAgainstPhoneNumber(string otpCode, string phoneNumber, int numberOfAttempts, int? userId);
        Task<bool> VerifyOtpCode(string code);
        Task<UserOtpInfo> SaveResentOtpAgainstPhoneNumber(string otpCode, string phoneNumber, int numberOfAttempts, int? userId);
        Task<UserOtpInfo> GetUserOtpInfoByPhoneNumber(string phoneNumber);
        Task<bool> CheckOtpTimeExpired(string code);
        Task<string> SaveTokenInDb(int id, string token);
        bool IsDigitsOnly(string str);
        Task<bool> SendPushNotificationToAllActiveContactsOnStatusActive(int userId);
        Task<List<Contact>> GetAllUserContacts(int userId);
        Task<List<User>> GetAllActiveUserContactsForUser(int userId);
        Task<User> GetActiveUserByPhoneNumber(string phoneNumber);
        Task<User> GetActiveUserByUserId(int userId);
        Task<bool> ChangeStatusForAllContactsWhereUserIsSaved(string phoneNumber);

        Task<bool> ChangeUserActiveStatusInDb(int userId,DateTime loginTime);
        Task<bool> ChangeUserInActiveStatusInDb(int userId, DateTime loginTime);
        Task<bool> UpdateUserInDb(User user);

        Task<bool> UserLogout(int userId);
        Task<bool> ChangeStatusInActiveForAllContactsWhereUserIsSaved(string phoneNumber);
        Task ChangeStatusForUsersWithActiveTime();
        Task MarkAsFriend(int userId);
        Task<bool> AddContacts(List<Contact> contacts, int userId);

    }
}