using PhotoSelector2.Services;
using CommunityToolkit.Maui.Storage;

namespace PhotoSelector2
{
    public partial class MainPage : ContentPage
    {
        private readonly FileManagerService _fileManager;
        private string? _sourceFolder;
        private const string LastSourceFolderKey = "LastSourceFolder";

        public MainPage()
        {
            InitializeComponent();
            _fileManager = new FileManagerService();
            LoadLastSourceFolder();
        }

        private void LoadLastSourceFolder()
        {
            var lastFolder = Preferences.Get(LastSourceFolderKey, string.Empty);
            if (!string.IsNullOrEmpty(lastFolder) && Directory.Exists(lastFolder))
            {
                _sourceFolder = lastFolder;
                SourceFolderEntry.Text = _sourceFolder;

                var destinationPath = Path.Combine(_sourceFolder, "selected");
                DestinationFolderLabel.Text = $"Destination: {destinationPath}";
                DestinationFolderLabel.TextColor = Colors.Green;
            }
        }

        private void SaveLastSourceFolder(string folderPath)
        {
            Preferences.Set(LastSourceFolderKey, folderPath);
        }

        private async void OnSelectSourceFolderClicked(object? sender, EventArgs e)
        {
            try
            {
                var result = await FolderPicker.Default.PickAsync(default);
                if (result.IsSuccessful && result.Folder != null)
                {
                    _sourceFolder = result.Folder.Path;
                    SourceFolderEntry.Text = _sourceFolder;
                    SaveLastSourceFolder(_sourceFolder);

                    // Update destination label to show where files will be moved
                    var destinationPath = Path.Combine(_sourceFolder, "selected");
                    DestinationFolderLabel.Text = $"Destination: {destinationPath}";
                    DestinationFolderLabel.TextColor = Colors.Green;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to select folder: {ex.Message}", "OK");
            }
        }

        private async void OnMoveFilesClicked(object? sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(_sourceFolder))
            {
                await DisplayAlert("Input Error", "Please select a source folder.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(FileNamesEditor.Text))
            {
                await DisplayAlert("Input Error", "Please enter file names.", "OK");
                return;
            }

            // Auto-generate destination folder as "selected" subfolder
            var destinationFolder = Path.Combine(_sourceFolder, "selected");

            // Show loading indicator
            MoveFilesBtn.IsEnabled = false;
            MoveFilesBtn.Text = "Processing...";

            try
            {
                // Execute file move operation
                var result = await _fileManager.MoveMatchingFilesAsync(
                    FileNamesEditor.Text,
                    _sourceFolder,
                    destinationFolder);

                // Display results
                ResultFrame.IsVisible = true;
                ResultLabel.Text = result.Message;
                ResultLabel.TextColor = result.Success ? Colors.Green : Colors.Red;

                // Show moved files
                if (result.MovedFiles.Any())
                {
                    MovedFilesHeaderLabel.IsVisible = true;
                    MovedFilesScrollView.IsVisible = true;
                    MovedFilesLabel.Text = string.Join("\n", result.MovedFiles);
                }
                else
                {
                    MovedFilesHeaderLabel.IsVisible = false;
                    MovedFilesScrollView.IsVisible = false;
                }

                // Show not found files
                if (result.NotFoundFiles.Any())
                {
                    NotFoundFilesHeaderLabel.IsVisible = true;
                    NotFoundFilesScrollView.IsVisible = true;
                    NotFoundFilesLabel.Text = string.Join("\n", result.NotFoundFiles);
                }
                else
                {
                    NotFoundFilesHeaderLabel.IsVisible = false;
                    NotFoundFilesScrollView.IsVisible = false;
                }

                // Show completion message
                if (result.Success)
                {
                    await DisplayAlert("Complete", $"{result.Message}\n\nFiles moved to: {destinationFolder}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error moving files: {ex.Message}", "OK");
                ResultFrame.IsVisible = true;
                ResultLabel.Text = $"Error: {ex.Message}";
                ResultLabel.TextColor = Colors.Red;
            }
            finally
            {
                // Reset button
                MoveFilesBtn.IsEnabled = true;
                MoveFilesBtn.Text = "Move Files";
            }
        }
    }
}
