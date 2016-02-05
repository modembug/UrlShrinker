using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;

namespace UrlShrinker.Business
{
    public sealed class TokenService : ITokenService
    {
        private int _tokenLength = 6;

        private async Task<Int32> GetRandomNumber(int maxValue)
        {
            return await Task.Run(() =>
            {
                RandomNumberGenerator randomGen = RandomNumberGenerator.Create();

                Int64 diff = maxValue;

                byte[] buffer = new byte[4];

                while (true)
                {
                    randomGen.GetBytes(buffer);

                    UInt32 rand = BitConverter.ToUInt32(buffer, 0);

                    Int64 max = (1 + (Int64)UInt32.MaxValue);

                    Int64 remainder = max % diff;

                    if (rand < max - remainder)
                    {
                        return (Int32)((rand % diff));
                    }
                }
            });
        }

        private async Task<string> GetRandomCharacters(List<int> randomIntegers)
        {
            return await Task.Run(() =>
            {
                const string baseUrlChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

                return randomIntegers.ToList().Aggregate(string.Empty, (current, num) => current + baseUrlChars.Substring(num, 1));
            });
        }

        public async Task<string> GetToken()
        {
            return await Task.Run(async () =>
            {
                List<int> buffer = new List<int>();

                for (int i = 0; i < _tokenLength; i++)
                {
                    buffer.Add(await GetRandomNumber(62));
                }

                return await GetRandomCharacters(buffer);
            });
        }

    }
}