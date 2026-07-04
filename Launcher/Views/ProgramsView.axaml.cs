using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;        
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Launcher.Models;
using Launcher.Services;
using System.Linq;

namespace Launcher.Views;

public partial class ProgramsView : UserControl
{
    private MyView? _myView;
    private DatabaseService _databaseService = new DatabaseService();

    public ProgramsView()
    {
        InitializeComponent();

        var allGames = _databaseService.LoadGames();
        var programs = allGames.Where(g => g.Type == "program").ToList();

        foreach (var program in programs)
        {
            program.UpdateButtonState(_databaseService);
        }

        ProgramsItemsControl.ItemsSource = programs;
    }

    public void SetMyView(MyView myView)
    {
        _myView = myView;
    }

    public void UpdateAllButtons()
    {
        var allGames = _databaseService.LoadGames();
        var programs = allGames.Where(g => g.Type == "program").ToList();

        foreach (var program in programs)
        {
            program.UpdateButtonState(_databaseService);
        }

        ProgramsItemsControl.ItemsSource = null;
        ProgramsItemsControl.ItemsSource = programs;
    }

    private void OnAddToSelfClick(object? sender, RoutedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("=== OnAddToSelfClick (ProgramsView) ===");

        if (sender is Button btn)
        {
            System.Diagnostics.Debug.WriteLine($"sender is Button, Tag = {btn.Tag}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"sender is NOT Button! Type: {sender?.GetType()}");
            return;
        }

        if (btn.Tag is Game program)
        {
            System.Diagnostics.Debug.WriteLine($"Ïğîãğàììà: {program.Name}, Id={program.Id}, Type={program.Type}");

            if (program.Type != "program")
            {
                System.Diagnostics.Debug.WriteLine("Òèï íå program!");
                return;
            }

            if (_myView == null)
            {
                System.Diagnostics.Debug.WriteLine("_myView is NULL!");
                return;
            }

            if (!_databaseService.IsProgramAdded(program.Id))
            {
                _myView.AddGame(program);
                program.UpdateButtonState(_databaseService);
                System.Diagnostics.Debug.WriteLine("Ïğîãğàììà äîáàâëåíà!");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Ïğîãğàììà óæå äîáàâëåíà");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Tag is NOT Game! Type: {btn.Tag?.GetType()}");
        }
    }

    // ========== ÑÌÅÍÀ ÃËÀÂÍÎÉ ÊÀĞÒÈÍÊÈ ==========
    private void SmallImage_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Bitmap bitmap)
        {
            var parent = btn.Parent;
            while (parent != null)
            {
                if (parent is Border border && border.DataContext is Game game)
                {
                    var index = game.LoadedImages.IndexOf(bitmap);
                    if (index >= 0 && index < game.Images.Count)
                    {
                        game.SetMainImage(game.Images[index]);
                    }
                    break;
                }
                parent = parent.Parent;
            }
        }
    }

    // ========== ÎÒÊĞÛÒÈÅ ÊÀĞÒÎ×ÊÈ ÏĞÎÃĞÀÌÌÛ ==========
    private void OpenProgramList_Click(object? sender, PointerPressedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Îòêğûòèå êàğòî÷êè ïğîãğàììû");
    }
}