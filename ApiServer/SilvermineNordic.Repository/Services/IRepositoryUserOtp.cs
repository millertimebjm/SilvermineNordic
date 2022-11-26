using SilvermineNordic.Models;

namespace SilvermineNordic.Repository.Services
{
    public interface IRepositoryUserOtp
    {
        public Task<UserOtp> GetUserOtpAsync(Guid otp);
        public Task<User> GetUserOtpByAuthKeyAsync(Guid authKey);
        public Task<UserOtp> AddUserOtpAsync(int userId);
        public Task UpdateUserOtpAsync(UserOtp otp);
        public Task DeleteUserOtpAsync(UserOtp otp);
    }
}
