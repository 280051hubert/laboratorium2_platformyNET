using System.Diagnostics;

namespace MatrixBenchmark;

public class BenchmarkRunner
{
    private readonly int _runsPerConfig;
    private readonly int[] _matrixSizes;
    private readonly int[] _threadCounts;

    public BenchmarkRunner(int[] matrixSizes, int[] threadCounts, int runsPerConfig = 5)
    {
        _matrixSizes = matrixSizes;
        _threadCounts = threadCounts;
        _runsPerConfig = runsPerConfig;
    }

    public void Run()
    {
        Console.WriteLine($"{"Rozmiar",-8} {"Watki",-9} {"Sr w ms",10} {"Przyspieszenie",10}");
        Console.WriteLine(new string('-', 42));

        foreach (int size in _matrixSizes)
        {
            var a = Matrix.Random(size, size, seed: 0.1);
            var b = Matrix.Random(size, size, seed: 0.2);
            double baselineMs = MeasureAvg(a, b, threadCount: 1);

            foreach (int threads in _threadCounts)
            {
                double avgMs = (threads == 1) ? baselineMs : MeasureAvg(a, b, threads);
                double speedup = baselineMs / avgMs;

                Console.WriteLine($"{size + "x" + size,-8} {threads,-9} {avgMs,10:F1} {speedup,10:F2}x");
            }

            Console.WriteLine();
        }
    }

    private double MeasureAvg(Matrix a, Matrix b, int threadCount)
    {
        MatrixMultiplier.Multiply(a, b, threadCount);

        var sw = new Stopwatch();
        long totalMs = 0;

        for (int run = 0; run < _runsPerConfig; run++)
        {
            sw.Restart();
            MatrixMultiplier.Multiply(a, b, threadCount);
            sw.Stop();
            totalMs += sw.ElapsedMilliseconds;
        }

        return (double)totalMs / _runsPerConfig;
    }

    public static bool VerifyCorrectness(int size, int threadCount)
    {
        var a = Matrix.Random(size, size, seed: 1);
        var b = Matrix.Random(size, size, seed: 2);

        var seq = MatrixMultiplier.Multiply(a, b, threadCount: 1);
        var par = MatrixMultiplier.Multiply(a, b, threadCount);

        return seq.ApproximatelyEquals(par);
    }
}
