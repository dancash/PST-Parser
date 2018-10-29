namespace PSTParse
{
    public class PSTRoot
    {
        public ulong FileSizeBytes { get; }

        public PSTRoot(ulong fileSizeBytes)
        {
            FileSizeBytes = fileSizeBytes;
        }
    }
}
