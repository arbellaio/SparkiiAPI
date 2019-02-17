using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConnectApi.AppProperties;
using ConnectApi.Helpers.OneSignal;
using ConnectApi.Helpers.PasswordHasher;
using ConnectApi.Models.Contacts;
using ConnectApi.Models.UserOtpInfo;
using ConnectApi.Models.Users;
using ConnectApi.Services.AppDatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ConnectApi.Services.Users
{
    public class UserService : IUserService
    {
        private readonly AppDataContext _context;

        public UserService(AppDataContext context)
        {
            _context = context ?? throw new ArgumentNullException("DataContext Users");
        }


        public async Task<User> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email.Equals(email));
            if (user == null)
                return null;

            var passwordHasher = new Hasher();
            var checkPassword = passwordHasher.VerifyHashedPassword(user.Password, password);
            if (!checkPassword)
                return null;

            return user;
        }

        public async Task<User> Register(User user, string password)
        {
            if (user.UserName == null || user.Password == null || user.Email == null || user.Name == null)
            {
                return null;
            }

            // Password Hasher
            var passwordHasher = new Hasher();
            var hashedPassword = passwordHasher.HashPassword(password);
            user.Password = hashedPassword;

            user.CreatedDate = DateTime.UtcNow.Date;

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }


        public async Task<int> GetNumberOfAttempts(string phonenumber)
        {
            var userOtp = await GetUserOtpInfoByPhoneNumber(phonenumber);
            if (userOtp == null)
                return 0;
            return userOtp.NumberOfAttempts;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.UserName.Equals(username)))
                return true;

            return false;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email.Equals(email));
            return user;
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id.Equals(id));
            if (user == null)
                return null;
            return user;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByPhoneNumber(string phonenumber)
        {
            var userInDb = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber.Equals(phonenumber));
            return userInDb;
        }

        public async Task<UserOtpInfo> SaveGeneratedOtpAgainstPhoneNumber(string otpcode, string phonenumber,
            int numberOfAttempts, int? userId)
        {
            var userOtp = await GetUserOtpInfoByPhoneNumber(phonenumber);
            if (userOtp != null)
            {
                userOtp.OtpCode = otpcode;
                userOtp.PhoneNumber = phonenumber;
                userOtp.NumberOfAttempts = numberOfAttempts;
                userOtp.CreatedDate = DateTime.UtcNow;
                userOtp.UserId = userId;

                _context.UserOtpInfos.Update(userOtp);
                await _context.SaveChangesAsync();
                return userOtp;
            }

            userOtp = new UserOtpInfo
            {
                OtpCode = otpcode,
                PhoneNumber = phonenumber,
                CreatedDate = DateTime.UtcNow,
                NumberOfAttempts = numberOfAttempts,
                UserId = userId
            };
            await _context.UserOtpInfos.AddAsync(userOtp);
            await _context.SaveChangesAsync();
            return userOtp;
        }


        public async Task<bool> VerifyOtpCode(string code)
        {
            var userOtp = await _context.UserOtpInfos.SingleOrDefaultAsync(x => x.OtpCode.Equals(code));

            if (userOtp == null)
                return false;

            if (userOtp.CreatedDate.AddMinutes(AppConstants.DurationUntilOtpCodeActive) < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

        public async Task<UserOtpInfo> SaveResentOtpAgainstPhoneNumber(string otpcode, string phonenumber,
            int numberOfAttempts, int? userId)
        {
            var userOtp = await GetUserOtpInfoByPhoneNumber(phonenumber);


            userOtp.OtpCode = otpcode;
            userOtp.PhoneNumber = phonenumber;
            userOtp.NumberOfAttempts = numberOfAttempts;
            userOtp.UserId = userId;

            _context.UserOtpInfos.Update(userOtp);
            await _context.SaveChangesAsync();
            return userOtp;
        }

        public async Task<UserOtpInfo> GetUserOtpInfoByPhoneNumber(string phoneNumber)
        {
            var userOtp = await _context.UserOtpInfos.SingleOrDefaultAsync(x => x.PhoneNumber.Equals(phoneNumber));
            if (userOtp == null)
                return null;
            return userOtp;
        }

        public async Task<bool> CheckOtpTimeExpired(string code)
        {
            var userOtp = await _context.UserOtpInfos.SingleOrDefaultAsync(x => x.OtpCode.Equals(code));
            if (userOtp.CreatedDate.AddMinutes(AppConstants.DurationUntilOtpCodeActive) < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

        public async Task<string> SaveTokenInDb(int id, string token)
        {
            var user = await GetUserById(id);
            if (token != null && user.Token != null && user.Token.Equals(token))
                return token;

            if (token != null || user.Token == null)
                user.Token = token;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return token;
        }

        public bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public async Task<bool> SendPushNotificationToAllActiveContactsOnStatusActive(int userId)
        {
            if (userId != 0)
            {
                var user = await GetUserById(userId);
                var oneSignalDeviceIds = new List<string>();
                var userContacts = await GetAllActiveUserContactsForUser(userId);
                foreach (var userContact in userContacts)
                {
                    oneSignalDeviceIds.Add(userContact.OneSignalUserId);
                }

                if (oneSignalDeviceIds != null && oneSignalDeviceIds.Count != 0)
                {
                    var oneSignalHelper = new OneSignalPushNotificationHelper();
                    var responseOneSignal =
                        oneSignalHelper.SendOneSignalPushNotification(oneSignalDeviceIds, user, "I am online");
                    if (responseOneSignal != 0)
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        public async Task<List<User>> GetAllActiveUserContactsForUser(int userId)
        {
            if (userId != 0)
            {
                var users = new List<User>();
                var user = await GetUserById(userId);
                var contacts = await GetAllUserContacts(userId);
                if (contacts != null)
                {
                    foreach (var contact in contacts)
                    {
                        var userContact = await GetActiveUserByPhoneNumber(contact.PhoneNumber);
                        users.Add(userContact);
                    }

                    return users;
                }
            }

            return null;
        }

        public async Task<User> GetActiveUserByPhoneNumber(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber.Equals(phoneNumber));
                if (user.Status)
                {
                    return user;
                }

                return null;
            }

            return null;
        }

        public async Task<User> GetActiveUserByUserId(int userId)
        {
            if (userId != 0)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
                if (user.Status)
                {
                    return user;
                }

                return null;
            }

            return null;
        }

        public async Task<bool> ChangeStatusForAllContactsWhereUserIsSaved(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                var contacts = await _context.Contacts.Where(x => x.PhoneNumber.Equals(phoneNumber)).ToListAsync();
                if (contacts != null)
                {
                    foreach (var contact in contacts)
                    {
                        contact.Status = true;
                        _context.Contacts.Update(contact);
                    }

                    return true;
                }

                return false;
            }

            return false;
        }

        public async Task<List<Contact>> GetAllUserContacts(int userId)
        {
            if (userId != 0)
            {
                var contacts = await _context.Contacts.Where(x => x.UserId.Equals(userId)).ToListAsync();
                if (contacts != null)
                {
                    return contacts;
                }

                return new List<Contact>();
            }

            return null;
        }


        public async Task<bool> ChangeUserActiveStatusInDb(int userId)
        {
            var user = await GetUserById(userId);
            if (user != null)
            {
                user.Status = true;
                await UpdateUserInDb(user);
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateUserInDb(User user)
        {
            if (user != null)
            {
                var userInDb = await GetUserById(user.Id);
                if (userInDb != null)
                {
                    _context.Users.Update(userInDb);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}