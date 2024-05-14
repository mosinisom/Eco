using Avalonia.Controls;

namespace AvaloniaCurves;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.WindowState = WindowState.Maximized;

        // Создание кнопок
        var startButton = new Button { Content = "Start Game" };
        var pauseButton = new Button { Content = "Pause Game" };

        // Создание TickControl
        var tickControl = new TickControl();

        // Привязка событий Click к методам, которые будут управлять таймером
        startButton.Click += (sender, e) => tickControl.StartTimer();
        pauseButton.Click += (sender, e) => tickControl.PauseTimer();

        // Добавление кнопок и TickControl на экран
        var panel = new StackPanel();
        panel.Children.Add(startButton);
        panel.Children.Add(pauseButton);
        panel.Children.Add(tickControl);
        this.Content = panel;
    }
}