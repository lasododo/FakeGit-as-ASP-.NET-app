using Newtonsoft.Json;
using PUXinterviewMVC.Models.WatchedFiles;

namespace PUXinterviewMVC.Services
{
    public class FileHandlingService
    {
        private const string DirectoryName = ".fakegit";
        private const string LogFile = "logs.log";

        public async Task SerlializeToJSONAsync(List<WatchedFile> editList, string fakegit)
        {
            var json = JsonConvert.SerializeObject(editList);

            using (var writer = new StreamWriter(fakegit))
            {
                await writer.WriteAsync(json);
            }
        }

        public List<WatchedFile> GetJSONData(string fakegit)
        {
            var cachedFiles = new List<WatchedFile>();

            using (var reader = new StreamReader(fakegit))
            {
                var json22 = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<List<WatchedFile>>(json22) ?? new List<WatchedFile>();
            }
        }

        public Dictionary<string, List<(WatchedFile, WatchedFileStatus)>> CombineCachedAndCurrent(
            List<WatchedFile> cachedFiles,
            List<WatchedFile> newFiles)
        {
            var combined = new Dictionary<string, List<(WatchedFile, WatchedFileStatus)>>();

            foreach (var file in cachedFiles)
            {
                if (!combined.ContainsKey(file.FilePath))
                    combined[file.FilePath] = new List<(WatchedFile, WatchedFileStatus)>();

                combined[file.FilePath].Add((file, WatchedFileStatus.REMOVED));
            }

            foreach (var file in newFiles)
            {
                if (!combined.ContainsKey(file.FilePath))
                    combined[file.FilePath] = new List<(WatchedFile, WatchedFileStatus)>();

                combined[file.FilePath].Add((file, WatchedFileStatus.ADDED));
            }

            return combined;
        }

        public bool EnsureCreateded(string path, out string fakegit)
        {
            try
            {
                var dir = Path.Combine(path, DirectoryName);
                fakegit = Path.Combine(dir, LogFile);

                if (!Directory.Exists(dir) || !File.Exists(fakegit))
                {
                    Directory.CreateDirectory(dir);
                    File.Create(fakegit).Dispose();
                    return true;
                }
            }
            catch (Exception ex) 
            {
                throw new ArgumentException("Something went wrong when working with fakegit directory", ex);
            }
            return false;
        }

        public IEnumerable<WatchedFile> GetFiles(
            WatchedFileStatus fileStatusType,
            Dictionary<string, List<(WatchedFile, WatchedFileStatus)>> combined)
        {
            return fileStatusType switch
            {
                WatchedFileStatus.REMOVED => combined
                            .Where(file => file.Value.Count == 1 && file.Value.First().Item2 == WatchedFileStatus.REMOVED)
                            .Select(file => file.Value.First().Item1)
                            .ToList(),
                WatchedFileStatus.ADDED => combined
                                .Where(file => file.Value.Count == 1 && file.Value.First().Item2 == WatchedFileStatus.ADDED)
                                .Select(file => file.Value.First().Item1)
                                .ToList(),
                WatchedFileStatus.MODIFIED => combined
                                .Where(file => file.Value.Count == 2)
                                .Where(file => file.Value
                                    .Select(hash => BitConverter.ToString(hash.Item1.Hash))
                                    .Distinct()
                                    .Count() != 1)
                                .Select(file => { var edit = file.Value.First().Item1; edit.Hash = file.Value.Last().Item1.Hash; return edit; })
                                .Select(file => { file.Version++; return file; })
                                .ToList(),
                _ => throw new ArgumentException("Invalid WatchedFileStatus"),
            };
        }
    }
}
