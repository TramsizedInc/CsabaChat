using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OneMessenger.Core;

namespace OneMessenger.Core
{
    internal class Hashish : IHashish
    {
        public string HashPassword(string password)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);

            byte[] result = sha256.ComputeHash(bytes);

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                stringBuilder.Append(result[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }


        public int GenerateRandomNumber(int min, int max)
        {
            max++;
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomNumber = new byte[4];
                rng.GetBytes(randomNumber);

                int generatedNumber = BitConverter.ToInt32(randomNumber, 0);

                return new Random(generatedNumber).Next(min, max);
            }
        }
        public string GenerateRandomPassword()
        {
            int passwordLength = this.GenerateRandomNumber(6, 12);
            StringBuilder password = new StringBuilder();

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < passwordLength; i++)
                {
                    byte[] randomBytes = new byte[1];
                    rng.GetBytes(randomBytes);
                    byte b = randomBytes[0];

                    int type = b % 3;

                    switch (type)
                    {
                        case 0:
                            password.Append((char)(b % 26 + 65)); // Nagybetü
                            break;
                        case 1:
                            password.Append((char)(b % 26 + 97)); // Kisbetü
                            break;
                        case 2:
                            password.Append((char)(b % 10 + 48)); // Szám
                            break;
                    }
                }
            }
            return password.ToString();
        }

    }
}
