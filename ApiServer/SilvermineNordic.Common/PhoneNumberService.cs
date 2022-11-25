
namespace SilvermineNordic.Common
{
    public static class PhoneNumberService
    {
        public static bool ValidatePhoneNumber(string phoneNumber)
        {
            if (phoneNumber.First() == '+'
                && phoneNumber.Length == 12
                && phoneNumber.Substring(1, 11).All(char.IsNumber))
                return true;

            return false;
        }
    }
}
