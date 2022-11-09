namespace PhotoSorterEngine
{
    public class FileComparer : IFileComparer
    {
        public bool Compare(string fileName1, string fileName2)
        {
            using (var file1 = new FileStream(fileName1, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var file2 = new FileStream(fileName2, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (file1.Length != file2.Length)
                {
                    return false;
                }
                return Compare(file1, file2);
            };
        }

        public bool Compare(Stream stream1, Stream stream2)
        {
            const int bufferSize = 1024 * 8;
            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];
            while (true)
            {
                int count1 = stream1.Read(buffer1, 0, bufferSize);
                int count2 = stream2.Read(buffer2, 0, bufferSize);

                if (count1 != count2)
                    return false;

                if (count1 == 0 || count2 == 0)
                    return true;

                Span<byte> bytes1 = buffer1;
                Span<byte> bytes2 = buffer2;
                if (bytes1.SequenceEqual(bytes2)) return false;
            }
        }
    }
}