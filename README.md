## View To ViewModel Switcher

Easily toggle between a View and a ViewModel, particularly useful for WPF MVVM projects.

### Shortcuts
- 'Ctrl + Shift + Alt, V'
- 'Ctrl + Shift + Alt, M'

### Placements
Accessible under the 'View' dropdown in the top navigation and within the context menu when right-clicking a C# or XAML file.

### Visibility/Enable Conditions
Active file must end with either 'view.xaml' or 'View.xaml' or 'viewmodel.cs' or 'ViewModel.cs'.

### How it Works
- Detects the active document and verifies if the filename ends with 'View.xaml' or 'Model.cs'.
- If it's a View or ViewModel, it first checks for any corresponding 'View' or 'ViewModel' files open that match the selected document. If found, it switches to that; otherwise, it searches the entire project for the matching target View/ViewModel.
- Dialog messages are provided in case of any processing issues.

## RELEASE NOTES

### New in 2.0.0.0 (Released 07/04/2024)
- Rewritten extension in .NET Core to make use of Microsoft's new extensibility.
- Updated key bindings: Now uses 'Ctrl + Shift + Alt, V' and 'Ctrl + Shift + Alt, M'. Verified these don't conflict with any Visual Studio commands or capitalisation conventions like before.
- Visibility/Enable Conditions: Active file must end with either 'view.xaml' or 'View.xaml' or 'viewmodel.cs' or 'ViewModel.cs'.

### New in 1.5.0.0 (Released 24/02/2023)
- Initial extension created in .NET Framework.
- Used 'Shift + V' as the shortcut.
