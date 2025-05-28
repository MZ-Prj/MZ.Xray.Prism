using MZ.Domain.Interfaces;
using System.Text;
using System;
using System.Security.Cryptography;

namespace MZ.Domain.Auths
{
    public class Sha256InformationEncoder : IInformationEncoder
    {
        public string Hash(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool Verify(string hashed, string raw)
        {
            return Hash(raw) == hashed;
        }
    }
}
