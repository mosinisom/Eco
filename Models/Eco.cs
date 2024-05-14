using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

class Eco
{
    const double GRASS_START = 1.5;
    public double GrassSumValue = 0;
    const double GRASS_GROW = 0.8; // 0.6 надо
    public const double GRASS_LIMIT = 30;
    public readonly double MAX_SUM_GRASS;
    public double MAX_SESSION_GRASS;
    const double GRASS_BECOME_PARENT = 7;

    const double BUNNY_START = 15;
    const double BUNNY_SPEND = 1;
    const double BUNNY_DIVIDED = 95;
    const double BUNNY_EAT = 5;
    public int MAX_SESSION_BUNNIES;

    const double WOLF_START = 120;
    const double WOLF_SPEND = 2;
    const double WOLF_DIVIDED = 180;
    public int MAX_SESSION_WOLVES;

    public int Width { get; init; }
    public int Height { get; init; }
    public Grass[,] grass;
    public List<Bunny> bunnies;
    public List<Wolf> wolves;

    Random rnd = new Random();

    public Eco(int wd, int hg, int gs, int bs, int ws)
    {
        Width = wd;
        Height = hg;

        MAX_SUM_GRASS = wd * hg * GRASS_LIMIT;

        //создаем поле
        grass = new Grass[wd, hg];
        for (int x = 0; x < wd; x++)
            for (int y = 0; y < hg; y++)
                grass[x, y] = new Grass();

        //сажаем траву
        for (int i = 0; i < gs; i++)
            grass[rnd.Next(wd), rnd.Next(hg)].Value = GRASS_START;

        //раскидываем кроликов
        bunnies = new List<Bunny>();
        for (int i = 0; i < bs; i++)
            bunnies.Add(new Bunny() { Value = BUNNY_START, X = rnd.Next(wd), Y = rnd.Next(hg) });

        //раскидываем волков
        wolves = new List<Wolf>();
        for (int i = 0; i < ws; i++)
            wolves.Add(new Wolf() { Value = WOLF_START, X = rnd.Next(wd), Y = rnd.Next(hg) });

        MAX_SESSION_BUNNIES = bs;
        MAX_SESSION_WOLVES = ws;
        MAX_SESSION_GRASS = gs;
    }

    public void SimulateStep()
    {
        GrassSumValue = 0;



        //рост -------------------------------------------------------------------
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                // if (grass[x, y].Value > 0)
                if (grass[x, y].Value > 0 && grass[x, y].Value < GRASS_LIMIT)
                {
                    grass[x, y].Value += GRASS_GROW;
                }
            }

        //распространение травы
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (grass[x, y].Value <= 0 && HasParentGrassNeighbour(x, y))
                    grass[x, y].Value = GRASS_START;

        //травоядные --------------------------------------------------------------
        //питание и голод
        foreach (var b in bunnies)
        {
            if (grass[b.X, b.Y].Value > 0)
            {
                var v = Math.Min(grass[b.X, b.Y].Value, BUNNY_EAT);
                grass[b.X, b.Y].Value -= v;
                // GrassSumValue -= v;
                b.Value += v;
            }
            b.Value -= BUNNY_SPEND;
        }
        //смерть
        for (int i = bunnies.Count - 1; i >= 0; i--)
            if (bunnies[i].Value <= 0)
            {
                // grass[bunnies[i].X, bunnies[i].Y].Value += 0.0; не так весело с трупами удобрениями
                bunnies.RemoveAt(i);

            }
        //размножение
        for (int i = bunnies.Count - 1; i >= 0; i--)
            if (bunnies[i].Value >= BUNNY_DIVIDED)
            {
                var d = bunnies[i].Value / 2;
                bunnies[i].Value = d;
                // bunnies.Add(new Bunny() { Value = d, X = bunnies[i].X, Y = bunnies[i].Y });
                var x = bunnies[i].X + rnd.Next(-1, 2);
                var y = bunnies[i].Y + rnd.Next(-1, 2);
                if (x >= 0 && y >= 0 && x < Width && y < Height)
                    bunnies.Add(new Bunny() { Value = d, X = x, Y = y });

                if(bunnies.Count > MAX_SESSION_BUNNIES)
                    MAX_SESSION_BUNNIES = bunnies.Count;
            }
        //движение
        foreach (var b in bunnies)
        {
            int dx, dy;
            dx = rnd.Next(-1, 2);
            dy = rnd.Next(-1, 2);

            for (int i = 0; i < 5; i++)
            {
                dx = rnd.Next(-1, 2);
                dy = rnd.Next(-1, 2);
                if (b.X + dx >= 0 && b.Y + dy >= 0 && b.X + dx < Width && b.Y + dy < Height)
                {
                    if (grass[b.X + dx, b.Y + dy].Value > grass[b.X, b.Y].Value)
                    {
                        break;
                    }
                }
            }
            b.X += dx;
            b.Y += dy;

            if (b.X <= 0) b.X = 0;
            if (b.Y <= 0) b.Y = 0;
            if (b.X >= Width) b.X = Width - 1;
            if (b.Y >= Height) b.Y = Height - 1;
        }

        // хищники ---------------------------------------------------------------
        // питание и голод. волки едят с соседних клеток и со своей
        foreach (var w in wolves)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    var x = w.X + dx;
                    var y = w.Y + dy;
                    if (x >= 0 && y >= 0 && x < Width && y < Height)
                    {
                        foreach (var b in bunnies)
                            if (b.X == x && b.Y == y)
                            {
                                w.Value += b.Value;
                                b.Value = 0;
                            }
                    }
                }
            w.Value -= WOLF_SPEND;
        }
        // смерть
        for (int i = wolves.Count - 1; i >= 0; i--)
            if (wolves[i].Value <= 0)
                wolves.RemoveAt(i);
        // размножение
        for (int i = wolves.Count - 1; i >= 0; i--)
            if (wolves[i].Value >= WOLF_DIVIDED)
            {
                var d = wolves[i].Value / 2;
                wolves[i].Value = d;
                wolves.Add(new Wolf() { Value = d, X = wolves[i].X, Y = wolves[i].Y });

                if(wolves.Count > MAX_SESSION_WOLVES)
                    MAX_SESSION_WOLVES = wolves.Count;
            }
        // движение
        foreach (var w in wolves)
        {
            var bunnyCell = HasBunnyNeighbour(w.X, w.Y);
            if (bunnyCell.Item1 == true)
            {
                w.X = bunnyCell.Item2;
                w.Y = bunnyCell.Item3;
            }
            else
            {
                int dx = rnd.Next(-1, 2);
                int dy = rnd.Next(-1, 2);

                w.X += dx;
                w.Y += dy;

                if (w.X <= 0) w.X = 0;
                if (w.Y <= 0) w.Y = 0;
                if (w.X >= Width) w.X = Width - 1;
                if (w.Y >= Height) w.Y = Height - 1;
            }
        }

        // подсчет количества травы
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                {
                    GrassSumValue += grass[x, y].Value;
                    if(GrassSumValue > MAX_SESSION_GRASS)
                        MAX_SESSION_GRASS = Math.Round(GrassSumValue, 2);
                }
    }

    bool HasParentGrassNeighbour(int x, int y)
    {
        return
            (GetGrass(x - 1, y)?.Value ?? 0) >= GRASS_BECOME_PARENT ||
            (GetGrass(x + 1, y)?.Value ?? 0) >= GRASS_BECOME_PARENT ||
            (GetGrass(x, y + 1)?.Value ?? 0) >= GRASS_BECOME_PARENT ||
            (GetGrass(x, y - 1)?.Value ?? 0) >= GRASS_BECOME_PARENT;
    }

    (bool, int, int) HasBunnyNeighbour(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                var xx = x + dx;
                var yy = y + dy;
                if (xx >= 0 && yy >= 0 && xx < Width && yy < Height)
                {
                    foreach (var b in bunnies)
                        if (b.X == xx && b.Y == yy)
                            return (true, xx, yy);
                }
            }
        return (false, 0, 0);
    }

    (bool, int, int) HasGrassNeighbour(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                var xx = x + dx;
                var yy = y + dy;
                if (xx >= 0 && yy >= 0 && xx < Width && yy < Height)
                {
                    if (grass[xx, yy].Value > 0)
                        return (true, xx, yy);
                }
            }
        return (false, 0, 0);
    }
    

    Grass GetGrass(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return null;
        return grass[x, y];
    }

    public void CreateNewRndGrass(int amount)
    {
        for (int i = 0; i < amount; i++)
            grass[rnd.Next(Width), rnd.Next(Height)].Value = GRASS_LIMIT;
    }

    public void AddGrass(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
            grass[x, y].Value = GRASS_LIMIT;
    }

    public void AddBunny(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
            bunnies.Add(new Bunny() { Value = BUNNY_START, X = x, Y = y });
    }

    public void AddWolf(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
            wolves.Add(new Wolf() { Value = WOLF_START, X = x, Y = y });
    }
}
