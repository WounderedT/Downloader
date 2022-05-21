namespace Infra.Storage.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<Byte[]?> ReadBytesAsync(this Stream stream)
        {
            var bufferSize = await stream.ReadInt32Async();
            var buffer = new Byte[bufferSize];
            return await stream.ReadAsync(buffer, 0, bufferSize) == bufferSize ? buffer : null;
        }

        public static async Task<Int32> ReadInt32Async(this Stream stream)
        {
            var byteArray = new Byte[sizeof(Int32)];
            var result = await stream.ReadAsync(byteArray, 0, sizeof(Int32));
            if (result < sizeof(Int32))
            {
                throw new InvalidOperationException($"Stream has less than {sizeof(Int32)} bytes available");
            }

            return BitConverter.ToInt32(byteArray, 0);
        }

        public static async Task WriteBytesAsync(this Stream stream, Byte[] bytes)
        {
            await stream.WriteAsync(BitConverter.GetBytes(bytes.Length), 0, sizeof(Int32));
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
