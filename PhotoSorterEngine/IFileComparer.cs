﻿namespace PhotoSorterEngine
{
    public interface IFileComparer
    {
        bool Compare(string fileName1, string fileName2);
        bool Compare(Stream stream1, Stream stream2);
    }
}