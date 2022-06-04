namespace PUXinterviewMVC.Models.WatchedFiles
{
    public class WatchedFile
    {
        public string FilePath { get; set; }
        public byte[] Hash { get; set; }
        public int Version { get; set; }

        public WatchedFile(string filePath, byte[] hash, int version = 1)
        {
            Version = version;
            FilePath = filePath;
            Hash = hash;
        }
    };

    
}
