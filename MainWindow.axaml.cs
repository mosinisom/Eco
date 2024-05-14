using Avalonia.Controls;
using Avalonia.Layout;

namespace AvaloniaCurves;

public partial class MainWindow : Window
{
    public enum Mode { Grass, Bunny, Wolf }
    public Mode currentMode = Mode.Grass;

    public MainWindow()
    {
        InitializeComponent();
        this.WindowState = WindowState.Maximized;

        // Создание кнопок
        var startButton = new Button { Content = "Start Game" };
        var pauseButton = new Button { Content = "Pause Game" };
        var modeButton = new Button { Content = "Switch Mode" };

        // Создание TickControl
        var tickControl = new TickControl();

        // Привязка событий Click к методам, которые будут управлять таймером
        startButton.Click += (sender, e) => tickControl.StartTimer();
        pauseButton.Click += (sender, e) => tickControl.PauseTimer();
        modeButton.Click += (sender, e) =>
        {
            switch (currentMode)
            {
                case Mode.Grass:
                    currentMode = Mode.Bunny;
                    break;
                case Mode.Bunny:
                    currentMode = Mode.Wolf;
                    break;
                case Mode.Wolf:
                    currentMode = Mode.Grass;
                    break;
            }
            modeButton.Content = $"Mode: {currentMode}";
        };

        // Добавление кнопок на экран в горизонтальном ряду
        var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };
        buttonPanel.Children.Add(startButton);
        buttonPanel.Children.Add(pauseButton);
        buttonPanel.Children.Add(modeButton);

        // Добавление кнопок на экран
        var mainPanel = new StackPanel();
        mainPanel.Children.Add(buttonPanel);
        mainPanel.Children.Add(tickControl);
        this.Content = mainPanel;
    }

}