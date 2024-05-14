using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace AvaloniaCurves;

public class TickControl : Control
{
    Eco eco;
    private DispatcherTimer timer;
    private Queue<double> grassValues;
    private Queue<int> bunnyValues;
    private Queue<int> wolfValues;

    static TickControl()
    {
        AffectsRender<TickControl>(AngleProperty);
    }

    // 26.04 - 20
    readonly int fieldSize = 90;
    // 23.04 - 80
    readonly int grassStart = 900;
    // 24.04 - 41
    readonly int bunnyStart = 441;
    readonly int wolfStart = 40;


    public TickControl()
    {
        int grassRndGrow = 0;
        eco = new Eco(fieldSize, fieldSize, grassStart, bunnyStart, wolfStart);

        grassValues = new Queue<double>();
        bunnyValues = new Queue<int>();
        wolfValues = new Queue<int>();

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1 / 30.0);
        timer.Tick += (sender, e) =>
        {
            grassRndGrow++;
            Angle += Math.PI / 360;
            eco.SimulateStep();

            if (grassValues.Count >= 150) grassValues.Dequeue();
            if (bunnyValues.Count >= 150) bunnyValues.Dequeue();
            if (wolfValues.Count >= 150) wolfValues.Dequeue();

            grassValues.Enqueue(eco.GrassSumValue);
            bunnyValues.Enqueue(eco.bunnies.Count);
            wolfValues.Enqueue(eco.wolves.Count);
            if (grassRndGrow % 50 == 0)
            {
                eco.CreateNewRndGrass(30);
            }

        };
    }

    public void StartTimer()
    {
        timer.Start();
    }

    public void PauseTimer()
    {
        timer.Stop();
    }

    public static readonly StyledProperty<double> AngleProperty =
        AvaloniaProperty.Register<TickControl, double>(nameof(Angle));

    public double Angle
    {
        get => GetValue(AngleProperty);
        set => SetValue(AngleProperty, value);
    }

    public override void Render(DrawingContext ctx)
    {
        double grassCounter = Math.Round(eco.GrassSumValue, 2);
        int bunnyCounter = eco.bunnies.Count;
        int wolfCounter = eco.wolves.Count;

        double dx = 700.0 / eco.Width;
        double dy = 700.0 / eco.Height;

        //рисуем траву
        for (int x = 0; x < eco.Width; x++)
            for (int y = 0; y < eco.Width; y++)
            {
                var value = eco.grass[x, y].Value;
                byte r, g, b;
                r = g = b = 255;
                if (value > 0)
                {
                    r = b = 0;
                    g = (byte)(200 - value / Eco.GRASS_LIMIT * 100);
                }

                var brush = new SolidColorBrush(Color.FromRgb(r, g, b), 1);

                ctx.DrawRectangle(brush, null, new Rect(dx * x, dy * y, dx, dy));
            }

        //рисуем кроликов
        int i = 0;
        foreach (var bunny in eco.bunnies)
        {
            if (i % 2 == 0)
            {
                var brush = Brushes.LightSlateGray;
                ctx.DrawEllipse(brush, null, new Rect(dx * bunny.X + 2, dy * bunny.Y + 2, dx - 3, dy - 3));
            }
            else
            {
                var brush = Brushes.LightGray;
                ctx.DrawEllipse(brush, null, new Rect(dx * bunny.X + 2, dy * bunny.Y + 2, dx - 3, dy - 3));
            }
            i++;
        }

        // рисуем волков
        foreach (var wolf in eco.wolves)
        {
            var brush = Brushes.Red;
            ctx.DrawEllipse(brush, null, new Rect(dx * wolf.X + 1, dy * wolf.Y + 1, dx - 2, dy - 2));
        }

        var text = $"Grass: {grassCounter}; Bunnies: {bunnyCounter}; Wolves: {wolfCounter}";
        var formattedText = new FormattedText(
            text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            16,
            Brushes.White
        );
        ctx.DrawText(formattedText, new Point(260, 700));

        // Рисуем графики
        for (int index = 0; index < grassValues.Count; index++)
        {
            double grassPercentage = grassValues.ElementAt(index) / eco.MAX_SUM_GRASS * 100;
            double grassHeight = grassPercentage * 7;
            ctx.DrawRectangle(new SolidColorBrush(Color.FromArgb(128, 0, 255, 0)), null, new Rect(710 + index * 5, 700 - grassHeight, 5, grassHeight));
        }

        for (int index = 0; index < bunnyValues.Count; index++)
        {
            ctx.DrawRectangle(new SolidColorBrush(Color.FromArgb(200, 128, 128, 128)), null, new Rect(710 + index * 5, 700 - bunnyValues.ElementAt(index) / 4, 5, bunnyValues.ElementAt(index) / 4));
        }

        for (int index = 0; index < wolfValues.Count; index++)
        {
            ctx.DrawRectangle(new SolidColorBrush(Color.FromArgb(160, 255, 0, 0)), null, new Rect(710 + index * 5, 700 - wolfValues.ElementAt(index) / 4, 5, wolfValues.ElementAt(index) / 4));
        }

    }
}
