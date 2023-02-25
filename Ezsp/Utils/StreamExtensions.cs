namespace Ezsp.Utils
{
    public static class StreamExtensions
    {
        public static async Task<int> ReadByteAsync(this Stream stream, CancellationToken cancellationToken)
        {
            var array = new byte[1];
            var bytes = await stream.ReadAsync(array, 0, 1, cancellationToken);
            if (bytes == 0)
                return -1;
            return array[0];
        }

        public static async Task WriteByteAsync(this Stream stream, byte b, CancellationToken cancellationToken)
        {
            await stream.WriteAsync(new[] { b }, cancellationToken);
        }
    }
}
