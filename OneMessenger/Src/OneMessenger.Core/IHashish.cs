using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneMessenger.Core
{
    public interface IHashish
    {
        string HashPassword(string password);
        int GenerateRandomNumber(int min, int max);
        string GenerateRandomPassword();
    }
}
