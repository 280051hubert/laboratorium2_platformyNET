namespace MatrixBenchmark;

public class MatrixMultiplier
{
    public static Matrix Multiply(Matrix a, Matrix b, int threadCount)
    {
        if (a.Cols != b.Rows)
            throw new ArgumentException($"Bad dimensions: {a.Rows}x{a.Cols} * {b.Rows}x{b.Cols}");

        var result = new Matrix(a.Rows, b.Cols);

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = threadCount
        };

        Parallel.For(0, a.Rows, options, i =>
        {
            for (int j = 0; j < b.Cols; j++)
            {
                double sum = 0.0;
                for (int k = 0; k < a.Cols; k++)
                    sum += a[i, k] * b[k, j];
                result[i, j] = sum;
            }
        });

        return result;
    }
}
