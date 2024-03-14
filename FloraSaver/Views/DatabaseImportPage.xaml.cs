using FloraSaver.ViewModels;

namespace FloraSaver;

public partial class DatabaseImportPage : ContentPage
{
    public DatabaseImportPage(BackupRestoreViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        ((BackupRestoreViewModel)(this.BindingContext)).ReconfigureSpanForScreenSize(width - 40, height - 40);
        
    }

    private static void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
    }
}