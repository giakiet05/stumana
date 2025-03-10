using LearnHub.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Stumana.WPF.Helpers
{
    public static class PasswordHelper
    {
        
       public static bool IsHashedPassword(string password)
        {
            var _passwordHasher = new PasswordHasher<object>();
            if (string.IsNullOrWhiteSpace(password))
                return false;

            var dummyUser = new object(); // Dummy object for hasher
            var result = _passwordHasher.VerifyHashedPassword(dummyUser, password, "dummy");
            return result != PasswordVerificationResult.Failed;
        }
    }
}
