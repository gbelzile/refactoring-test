using System;

namespace LegacyApp
{
    public static class ValidationExtensions
    {
        public static bool ValidateName(this string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        public static bool ValidateEmail(this string email)
        {
            return email.ValidateName() && email.Contains("@") && email.Contains(".");
        }

        public static void ThrowIfNull(this object param, string paramName)
        {
            if(param == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
