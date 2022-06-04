using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PUXinterviewMVC.Models;
using PUXinterviewMVC.Models.WatchedFiles;
using PUXinterviewMVC.Services;
using System.Security.Cryptography;

// using File = System.IO.File;

namespace PUXinterviewMVC.Controllers
{
    public class PathController : Controller
    {
        private readonly PathHashingService _hashingService;
        private readonly FileHandlingService _fileHandlingService;

        public PathController(PathHashingService hashingService, FileHandlingService fileHandlingService)
        {
            _hashingService = hashingService;
            _fileHandlingService = fileHandlingService;
        }

        [HttpGet]
        public IActionResult Report()
        {
            return View(new PathViewModel());
        }

        [HttpPost, ActionName("Report")]
        public IActionResult ReportConfrim(PathViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("error-message", $"Please fill in everything required!");
                return View(model);
            }

            if (model.FilePath == null)
            {
                ModelState.AddModelError("error-message", $"Path should not be empty");
                return View(model);
            }

            if (Directory.Exists(Path.GetFullPath(model.FilePath)))
                return RedirectToAction(nameof(Check), new { path = Path.GetFullPath(model.FilePath).ToString() });

            ModelState.AddModelError("error-message", $"Path ({model.FilePath}) does not exist -> {Path.GetFullPath(model.FilePath)}");
            return View(model);
        }

        public async Task<IActionResult> Check(string path)
        {
            if (_fileHandlingService.EnsureCreateded(path, out string fakegit))
            {
                var editList = _hashingService.GetFiles(path);
                await _fileHandlingService.SerlializeToJSONAsync(editList, fakegit);

                return View(null);
            }

            List<WatchedFile> currentFiles = _fileHandlingService.GetJSONData(fakegit);
            List<WatchedFile> newFiles = _hashingService.GetFiles(path);
            var combined = _fileHandlingService.CombineCachedAndCurrent(currentFiles, newFiles);

            var result = CreateEmptyResultDict();

            foreach (WatchedFileStatus enumVal in Enum.GetValues(typeof(WatchedFileStatus)))
            {
                result[enumVal] = _fileHandlingService.GetFiles(enumVal, combined).ToList();
                switch (enumVal)
                {
                    case WatchedFileStatus.REMOVED:
                        currentFiles = currentFiles
                            .Except(result[WatchedFileStatus.REMOVED])
                            .ToList();
                        break;
                    case WatchedFileStatus.ADDED:
                        currentFiles.AddRange(result[WatchedFileStatus.ADDED]);
                        break;
                }
            }
            await _fileHandlingService.SerlializeToJSONAsync(currentFiles, fakegit);

            return View(result);
        }

        private static Dictionary<WatchedFileStatus, List<WatchedFile>> CreateEmptyResultDict()
        {
            var output = new Dictionary<WatchedFileStatus, List<WatchedFile>>();

            foreach (WatchedFileStatus enumVal in Enum.GetValues(typeof(WatchedFileStatus)))
                output[enumVal] = new List<WatchedFile>();
            return output;
        }
    }
}
