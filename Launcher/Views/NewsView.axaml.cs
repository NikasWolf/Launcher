using Avalonia.Controls;
using Launcher.Services;

namespace Launcher.Views;

public partial class NewsView : UserControl
{
    private DatabaseService _databaseService = new DatabaseService();

    public NewsView()
    {
        InitializeComponent();

        var news = _databaseService.LoadNews();
        NewsItemsControl.ItemsSource = news;
    }
}