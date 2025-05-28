namespace MZ.Domain.Interfaces
{
    public interface IInformationEncoder
    {
        public string Hash(string value);
        public bool Verify(string hashed, string raw);
    }
}
