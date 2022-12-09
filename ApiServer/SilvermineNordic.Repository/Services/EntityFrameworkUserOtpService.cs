using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public class EntityFrameworkUserOtpService : IRepositoryUserOtp
    {
        private readonly SilvermineNordicDbContext _dbContext;
        public EntityFrameworkUserOtpService(SilvermineNordicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserOtp> GetUserOtpAsync(Guid otp)
        {

            var userOtp = _dbContext.UserOtps.SingleOrDefault(_ => _.Otp == otp && !_.Exhausted);
            if (userOtp == null)
            {
                return null;
            }
            userOtp.Exhausted = true;
            userOtp.LastUsedDateTimestampUtc = DateTime.UtcNow;
            _dbContext.Update(userOtp);
            await _dbContext.SaveChangesAsync();
            return userOtp;
        }

        public async Task<User> GetUserOtpByAuthKeyAsync(Guid authKey)
        {
            var otp = await _dbContext.UserOtps.SingleOrDefaultAsync(_ => _.AuthKey == authKey);
            if (otp is not null)
            {
                return await _dbContext.Users.SingleOrDefaultAsync(_ => _.Id == otp.UserId);
            }
            return null;
        }

        public async Task<UserOtp> AddUserOtpAsync(int userId)
        {
            var userOtp = new UserOtp()
            {
                UserId = userId,
                AuthKey = Guid.NewGuid(),
                Exhausted = false,
                Otp = Guid.NewGuid(),
            };
            await _dbContext.AddAsync(userOtp);
            await _dbContext.SaveChangesAsync();
            return userOtp;
        }

        public async Task DeleteUserOtpAsync(UserOtp otp)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateUserOtpAsync(UserOtp otp)
        {
            throw new NotImplementedException();
        }
    }
}
