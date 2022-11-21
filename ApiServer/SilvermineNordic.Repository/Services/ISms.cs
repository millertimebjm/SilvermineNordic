
namespace SilvermineNordic.Repository.Services
{
    public interface ISms
    {
        Task<bool> SendSms(string to, string message);
    }
}
