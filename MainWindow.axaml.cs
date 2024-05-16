using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AvaloniaCurves;

public partial class MainWindow : Window
{
    public enum Mode { Grass, Bunny, Wolf }
    public Mode currentMode = Mode.Grass;
    public bool isRandomGrassEnabled = false;

    public MainWindow()
    {
        InitializeComponent();
        this.WindowState = WindowState.Maximized;

        var startButton = new Button { Content = "Start Game" };
        var pauseButton = new Button { Content = "Pause Game" };
        var modeButton = new Button { Content = "Mode: Grass", Background = Brushes.Green };
        var randomGrassButton = new Button { Content = "Enable Random Grass" };
        var clearButton = new Button { Content = "Clear" };

        var tickControl = new TickControl();

        startButton.Click += (sender, e) => tickControl.StartTimer();
        pauseButton.Click += (sender, e) => tickControl.PauseTimer();
        modeButton.Click += (sender, e) =>
        {
            switch (currentMode)
            {
                case Mode.Grass:
                    currentMode = Mode.Bunny;
                    modeButton.Background = Brushes.Gray;
                    break;
                case Mode.Bunny:
                    currentMode = Mode.Wolf;
                    modeButton.Background = Brushes.Red;
                    break;
                case Mode.Wolf:
                    currentMode = Mode.Grass;
                    modeButton.Background = Brushes.Green;
                    break;
            }
            modeButton.Content = $"Mode: {currentMode}";
        };

        randomGrassButton.Click += (sender, e) =>
        {
            isRandomGrassEnabled = !isRandomGrassEnabled;
            randomGrassButton.Content = isRandomGrassEnabled ? "Disable Random Grass" : "Enable Random Grass";
        };
        clearButton.Click += (sender, e) =>
        {
            tickControl.Clear();
        };


        var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };
        buttonPanel.Children.Add(startButton);
        buttonPanel.Children.Add(pauseButton);
        buttonPanel.Children.Add(modeButton);
        buttonPanel.Children.Add(randomGrassButton);
        buttonPanel.Children.Add(clearButton);

        // Добавление кнопок на экран
        var mainPanel = new StackPanel();
        mainPanel.Children.Add(buttonPanel);
        mainPanel.Children.Add(tickControl);
        this.Content = mainPanel;
    }

}