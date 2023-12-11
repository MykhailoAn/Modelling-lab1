using System;

class Program
{
    static decimal f(decimal t, decimal x, decimal y)
    {
        decimal dxdt = t / y;
        decimal dydt = -t / x;
        return dxdt;
    }

    static decimal[] RungeKuttaStep(decimal t, decimal x, decimal y, decimal h)
    {
        decimal k1x, k1y, k2x, k2y, k3x, k3y, k4x, k4y;

        k1x = f(t, x, y);
        k1y = -t / x;

        k2x = f(t + h / 2, x + h / 2 * k1x, y + h / 2 * k1y);
        k2y = -t / (x + h / 2 * k1x);

        k3x = f(t + h / 2, x + h / 2 * k2x, y + h / 2 * k2y);
        k3y = -t / (x + h / 2 * k2x);

        k4x = f(t + h, x + h * k3x, y + h * k3y);
        k4y = -t / (x + h * k3x);

        decimal xNew = x + h / 6 * (k1x + 2 * k2x + 2 * k3x + k4x);
        decimal yNew = y + h / 6 * (k1y + 2 * k2y + 2 * k3y + k4y);

        return new decimal[] { xNew, yNew };
    }

    static decimal FindOptimalH(Func<decimal, decimal, decimal, decimal[]> f, decimal x0, decimal y0, decimal t0, decimal tEnd, decimal h, decimal epsilon)
    {
        decimal t = t0;
        decimal x = x0;
        decimal y = y0;
        while (t < tEnd)
        {
            decimal[] result1 = f(t, x, y);
            decimal x1 = x + h * result1[0];
            decimal y1 = y + h * result1[1];
            t += h;
            decimal[] result2 = f(t, x1, y1);
            decimal x2 = x + h * result2[0];
            decimal y2 = y + h * result2[1];
            decimal error = Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1));
            h *= Math.Min(2.0m, Math.Max(0.1m, (epsilon / error) * (decimal)Math.Pow((double)epsilon / (double)error, 0.5)));
            x = x1;
            y = y1;
        }
        return h;
    }

    static void Main()
    {
        Console.Write("Enter the value of t: ");
        decimal t = decimal.Parse(Console.ReadLine());
        Console.Write("Enter the value of h₀: ");
        decimal h0 = decimal.Parse(Console.ReadLine());
        Console.Write("Enter the value of ε: ");
        decimal eps = decimal.Parse(Console.ReadLine());

        decimal t0 = 0;
        decimal x0 = 1;
        decimal y0 = 1;
        decimal optimalH = FindOptimalH((tt, xx, yy) => RungeKuttaStep(tt, xx, yy, h0), x0, y0, t0, t, h0, eps);

        decimal[] tValues = { t0 };
        decimal[] xValues = { x0 };
        decimal[] yValues = { y0 };

        while (t0 < t)
        {
            decimal[] result = RungeKuttaStep(t0, x0, y0, optimalH);
            t0 += optimalH;
            x0 = result[0];
            y0 = result[1];
            Array.Resize(ref tValues, tValues.Length + 1);
            Array.Resize(ref xValues, xValues.Length + 1);
            Array.Resize(ref yValues, yValues.Length + 1);
            tValues[tValues.Length - 1] = t0;
            xValues[xValues.Length - 1] = x0;
            yValues[yValues.Length - 1] = y0;
        }

        int n = tValues.Length;
        for (int i = 2; i < n; i++)
        {
            decimal tn = tValues[i];
            decimal yn2 = yValues[i - 2];
            decimal yn1 = yValues[i - 1];

            decimal fn = f(tn, xValues[i], yValues[i]);

            decimal yNew = 1.3333333333m * yn1 - 0.3333333333m * yn2 + 0.6666666666m * optimalH * fn;

            yValues[i] = yNew;
        }

        Console.WriteLine($"Optimal h: {optimalH:F8}");
        Console.WriteLine("Approximate results:");
        for (int i = 0; i < n; i++)
        {
            decimal exactX = (decimal)Math.Exp((double)(tValues[i] * tValues[i] / 2));
            decimal exactY = (decimal)Math.Exp((double)(-tValues[i] * tValues[i] / 2));
            Console.WriteLine($"t = {tValues[i]:F5}, x = {xValues[i]:F8}, y = {yValues[i]:F8}, xₜ(t) = {exactX:F8}, yₜ(t) = {exactY:F8}");
        }
        Console.ReadLine();
    }
}
