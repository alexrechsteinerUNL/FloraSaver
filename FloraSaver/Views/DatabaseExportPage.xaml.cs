using FloraSaver.ViewModels;

namespace FloraSaver;

public partial class DatabaseExportPage : ContentPage
{
    public DatabaseExportPage(BackupRestoreViewModel viewModel)
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

