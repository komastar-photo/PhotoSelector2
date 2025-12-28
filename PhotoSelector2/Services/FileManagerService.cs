namespace PhotoSelector2.Services
{
    public class FileManagerService
    {
        /// <summary>
        /// Moves files matching the given filenames (without extensions) from source to destination folder
        /// </summary>
        /// <param name="fileNamesInput">Comma-separated list of filenames (can include extensions)</param>
        /// <param name="sourceFolder">Source folder path to search for files</param>
        /// <param name="destinationFolder">Destination folder path to move files</param>
        /// <returns>Result message with count of moved files</returns>
        public Task<(bool Success, string Message, List<string> MovedFiles, List<string> NotFoundFiles)> MoveMatchingFilesAsync(
            string fileNamesInput,
            string sourceFolder,
            string destinationFolder)
        {
            var movedFiles = new List<string>();
            var notFoundFiles = new List<string>();

            try
            {
                // Validate input parameters
                if (string.IsNullOrWhiteSpace(fileNamesInput))
                {
                    return Task.FromResult((false, "파일 이름 목록이 비어있습니다.", movedFiles, notFoundFiles));
                }

                if (string.IsNullOrWhiteSpace(sourceFolder) || !Directory.Exists(sourceFolder))
                {
                    return Task.FromResult((false, "원본 폴더가 존재하지 않습니다.", movedFiles, notFoundFiles));
                }

                // Create destination folder if it doesn't exist
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                // Parse input filenames - split by comma and trim whitespace
                var inputFileNames = fileNamesInput
                    .Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => f.Trim())
                    .Where(f => !string.IsNullOrWhiteSpace(f))
                    .ToList();

                // Extract filenames without extensions from input
                var targetFileNamesWithoutExt = inputFileNames
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                // Get all files from source folder
                var sourceFiles = Directory.GetFiles(sourceFolder);

                // Find and move matching files
                foreach (var sourceFilePath in sourceFiles)
                {
                    var fileName = Path.GetFileName(sourceFilePath);
                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

                    if (targetFileNamesWithoutExt.Contains(fileNameWithoutExt))
                    {
                        var destFilePath = Path.Combine(destinationFolder, fileName);

                        // Handle file already exists in destination
                        if (File.Exists(destFilePath))
                        {
                            // Optional: You can skip, overwrite, or rename
                            // For now, we'll skip
                            notFoundFiles.Add($"{fileName} (이미 대상 폴더에 존재)");
                            continue;
                        }

                        // Move the file
                        File.Move(sourceFilePath, destFilePath);
                        movedFiles.Add(fileName);
                    }
                }

                // Find files that were requested but not found
                foreach (var targetName in targetFileNamesWithoutExt)
                {
                    var found = sourceFiles.Any(f =>
                        string.Equals(
                            Path.GetFileNameWithoutExtension(Path.GetFileName(f)),
                            targetName,
                            StringComparison.OrdinalIgnoreCase));

                    if (!found && !notFoundFiles.Any(nf => nf.StartsWith(targetName, StringComparison.OrdinalIgnoreCase)))
                    {
                        notFoundFiles.Add($"{targetName} (찾을 수 없음)");
                    }
                }

                var message = $"성공: {movedFiles.Count}개 파일 이동 완료";
                if (notFoundFiles.Count > 0)
                {
                    message += $"\n실패/누락: {notFoundFiles.Count}개";
                }

                return Task.FromResult((true, message, movedFiles, notFoundFiles));
            }
            catch (Exception ex)
            {
                return Task.FromResult((false, $"오류 발생: {ex.Message}", movedFiles, notFoundFiles));
            }
        }
    }
}
