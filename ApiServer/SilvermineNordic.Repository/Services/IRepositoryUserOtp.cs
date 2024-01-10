using SilvermineNordic.Models;
using System;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public interface IRepositoryUserOtp
    {
        public Task<UserOtp?> GetUserOtpAsync(Guid otp);
        public Task<User?> GetUserOtpByAuthKeyAsync(Guid authKey);
        public Task<UserOtp> AddUserOtpAsync(int userId);
        public void UpdateUserOtpAsync(UserOtp otp);
        public void DeleteUserOtpAsync(UserOtp otp);
    }
}
