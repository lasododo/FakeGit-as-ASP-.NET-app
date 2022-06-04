using Newtonsoft.Json;
using PUXinterviewMVC.Models.WatchedFiles;
using System.Security.Cryptography;

namespace PUXinterviewMVC.Services
{
    public class PathHashingService
    {

        public byte[] CalculateHash(string file)
        {
            using var md5 = MD5.Create();
            using var fs1 = File.Open(file, FileMode.Open);

            return md5.ComputeHash(fs1);
        }

        public List<WatchedFile> GetFiles(string path)
        {
            var editList = new List<WatchedFile>();

            foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                var hash = CalculateHash(file);
                editList.Add(new WatchedFile(file, hash));
            }
            return editList;
        }

    }
}
