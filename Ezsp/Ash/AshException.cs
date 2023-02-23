namespace Ezsp.Ash
{
    public class AshException: Exception
    {
        public byte Version { get; }
        public byte ErrorCode { get; }

        public AshException(byte version, byte errorCode)
        {
            Version = version;
            ErrorCode = errorCode;
        }
    }
}
