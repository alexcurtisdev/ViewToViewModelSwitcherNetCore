using Microsoft;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;
using Microsoft.VisualStudio.Extensibility.Shell;
using Microsoft.VisualStudio.ProjectSystem.Query;
using System.Diagnostics;

namespace ViewToViewModelSwitcherNetCore
{
    [VisualStudioContribution]
    internal class Command1 : Command
    {
        private readonly TraceSource logger;
        public Command1(TraceSource traceSource)
        {
            this.logger = Requires.NotNull(traceSource, nameof(traceSource));
        }

        public override CommandConfiguration CommandConfiguration => new("%ViewToViewModelSwitcherNetCore.Command1.DisplayName%")
        {
            Icon = new(ImageMoniker.KnownValues.ToggleStackView, IconSettings.IconAndText),
            Placements = [
                            CommandPlacement.VsctParent(new Guid("d309f791-903f-11d0-9efc-00a0c911004f"), 0x02D0, 0x0000), // Right click context menu appears at the top of the Language section
                            CommandPlacement.VsctParent(new Guid("d309f791-903f-11d0-9efc-00a0c911004f"), 0x0133, 0x0000) // Main Navigation Bar top of 'View Menu'
                         ],
            VisibleWhen = ActivationConstraint.ClientContext(ClientContextKey.Shell.ActiveEditorFileName, "(?i)(View\\.xaml|view\\.xaml|ViewModel\\.cs|viewModel\\.cs)$"),
            EnabledWhen = ActivationConstraint.ClientContext(ClientContextKey.Shell.ActiveEditorFileName, "(?i)(View\\.xaml|view\\.xaml|ViewModel\\.cs|viewModel\\.cs)$"),
            Shortcuts = [
                            new(ModifierKey.ControlShiftLeftAlt, Key.M),
                            new(ModifierKey.ControlShiftLeftAlt, Key.V)
                        ],
        };

        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            return base.InitializeAsync(cancellationToken);
        }

        public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
        {

            try
            {
                var activeDocument = await context.GetActiveTextViewAsync(cancellationToken);

                if (activeDocument == null)
                {
                    await ShowPromptAsync("No active document found", cancellationToken);
                    return;
                }

                var activeFileName = Path.GetFileName(activeDocument.Uri.LocalPath);

                bool isView = IsViewFile(activeFileName);

                string targetDocumentName = GetTargetDocumentName(activeFileName, isView);

                if (string.IsNullOrEmpty(targetDocumentName))
                {
                    await ShowPromptAsync($"Corresponding View or ViewModel for {activeFileName} could not be found.", cancellationToken);
                    return;
                }

                if (await TryOpenMatchingDocumentAsync(targetDocumentName, cancellationToken))
                    return; // matching document was opened, don't proceed further

                await FindAndOpenMatchingFileAsync(context, targetDocumentName, cancellationToken);
            }
            catch (Exception ex)
            {
                await ShowPromptAsync($"An exception occurred: {ex.Message}", cancellationToken);
            }
        }

        private async Task ShowPromptAsync(string message, CancellationToken cancellationToken)
        {
            await Extensibility.Shell().ShowPromptAsync(message, PromptOptions.OK, cancellationToken);
        }

        private async Task<bool> TryOpenMatchingDocumentAsync(string targetDocumentName, CancellationToken cancellationToken)
        {
            DocumentsExtensibility documents = Extensibility.Documents();

            if (documents != null)
            {
                var openDocuments = await documents.GetOpenDocumentsAsync(cancellationToken);

                if (openDocuments != null)
                {
                    var openFileURIs = openDocuments.Select(x => x.Moniker).ToList();
                    if (openFileURIs.Any(u => u.AbsolutePath.EndsWith(targetDocumentName, StringComparison.OrdinalIgnoreCase)))
                    {
                        var targetDocument = openFileURIs.FirstOrDefault(u => u.AbsolutePath.EndsWith(targetDocumentName, StringComparison.OrdinalIgnoreCase));
                        if (targetDocument != null)
                        {
                            await documents.OpenDocumentAsync(targetDocument, cancellationToken);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private async Task FindAndOpenMatchingFileAsync(IClientContext context, string targetDocumentName, CancellationToken cancellationToken)
        {
            var matchingFile = await GetMatchingFileAsync(context, targetDocumentName, cancellationToken);

            if (matchingFile == null)
            {
                await ShowPromptAsync($"No matching View/ViewModel file exists with the name: {targetDocumentName}", cancellationToken);
                return;
            }

            await TryOpenDocumentAsync(matchingFile.Path, cancellationToken);
        }

        private async Task<IFileSnapshot> GetMatchingFileAsync(IClientContext context, string targetDocumentName, CancellationToken cancellationToken)
        {
            var activeProjectDirectoryFiles = await context.GetActiveProjectAsync(p => p.With(p => p.Files.With(f => f.FileName).With(f => f.Path)), cancellationToken);

            if (activeProjectDirectoryFiles != null && activeProjectDirectoryFiles.Files?.Any() == true)
            {
                if (activeProjectDirectoryFiles.Files.All(f => !string.IsNullOrEmpty(f.FileName)))
                    return activeProjectDirectoryFiles.Files.FirstOrDefault(f => string.Equals(f.FileName, targetDocumentName, StringComparison.OrdinalIgnoreCase));
            }

            return null;
        }

        private async Task TryOpenDocumentAsync(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                await OpenDocumentToSwitchToAsync(filePath, cancellationToken);
            }
            catch (Exception)
            {
                await this.Extensibility.Shell().ShowPromptAsync("Failed to open file", PromptOptions.OK, cancellationToken);
            }
        }

        private bool IsViewFile(string fileName) => fileName.EndsWith("View.xaml", StringComparison.OrdinalIgnoreCase);

        private static string GetTargetDocumentName(string fileName, bool isView)
        {
            return isView ? fileName.Replace(".xaml", "Model.cs") : fileName.Replace("Model.cs", ".xaml");
        }

        public async Task OpenDocumentToSwitchToAsync(string filePath, CancellationToken cancellationToken)
        {
            var fileUri = new Uri(filePath, UriKind.RelativeOrAbsolute);
            await Extensibility.Documents().OpenDocumentAsync(fileUri, cancellationToken);
        }
    }
}
