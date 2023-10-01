using FloraSaver.ViewModels;

namespace FloraSaver;

public partial class DatabaseImportPage : ContentPage
{
    public DatabaseImportPage(BackupRestoreViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}

    private static void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
    }
}

