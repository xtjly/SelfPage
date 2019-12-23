using System;
using System.Security.Claims;

namespace SelfPage_TestWebAPI.Utilty
{
    public static class IdentiFind
    {
        public static string FindFistVlaue(this ClaimsIdentity identity, string claimName)
        {
            if (string.IsNullOrWhiteSpace(claimName))
            {
                return null;
            }
            var claim = identity.FindFirst(claimName);
            if (claim == null)
            {
                return null;
            }
            return claim.Value;
        }

        public static DateTime FindFirstTime(this ClaimsIdentity identity, string claimName)
        {
            var claimStr = identity.FindFistVlaue(claimName);
            if (!string.IsNullOrWhiteSpace(claimStr) && DateTime.TryParse(claimStr, out DateTime time))
            {
                return time;
            }
            return DateTime.MinValue;
        }

        public static long FindInt64(this ClaimsIdentity identity, string claimName)
        {
            var claimStr = identity.FindFistVlaue(claimName);
            if (!string.IsNullOrWhiteSpace(claimStr) && long.TryParse(claimStr, out long value))
            {
                return value;
            }
            return 0;
        }

        public static int FindInt32(this ClaimsIdentity identity, string claimName)
        {
            var claimStr = identity.FindFistVlaue(claimName);
            if (!string.IsNullOrWhiteSpace(claimStr) && int.TryParse(claimStr, out int value))
            {
                return value;
            }
            return 0;
        }
    }
}
