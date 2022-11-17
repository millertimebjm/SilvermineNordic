using SilvermineNordic.Api.Models;

namespace SilvermineNordic.Api.Services
{
    public interface IRepositoryUserOtp
    {
        public Task AddUserOtpAsync(UserOtp otp);
        public Task UpdateUserOtpAsync(UserOtp otp);
        public Task DeleteUserOtpAsync(UserOtp otp);
    }
}
