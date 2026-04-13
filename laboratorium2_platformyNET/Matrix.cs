namespace MatrixBenchmark;

public class Matrix
{
    private readonly double[,] _data;

    public int Rows { get; }
    public int Cols { get; }

    public double this[int row, int col]
    {
        get => _data[row, col];
        set => _data[row, col] = value;
    }

    public Matrix(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        _data = new double[rows, cols];
    }

    public static Matrix Random(int rows, int cols, int seed = 64)
    {
        var rng = new Random(seed);
        var m = new Matrix(rows, cols);
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                m[i, j] = Math.Round(rng.NextDouble() * 10, 2);
        return m;
    }

    public void Print(string label = "", int maxDim = 6)
    {
        if (!string.IsNullOrEmpty(label))
            Console.WriteLine(label);

        int rowsToShow = Math.Min(Rows, maxDim);
        int colsToShow = Math.Min(Cols, maxDim);

        for (int i = 0; i < rowsToShow; i++)
        {
            Console.Write("[ ");
            for (int j = 0; j < colsToShow; j++)
                Console.Write($"{_data[i, j],6:F2} ");
            if (colsToShow < Cols)
                Console.Write("... ");
            Console.WriteLine("]");
        }
        if (rowsToShow < Rows)
            Console.WriteLine($"  ... ({Rows} rows x {Cols} cols total)");

        Console.WriteLine();
    }

    public bool ApproximatelyEquals(Matrix other, double tolerance = 1e-9)
    {
        if (Rows != other.Rows || Cols != other.Cols)
            return false;
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                if (Math.Abs(_data[i, j] - other[i, j]) > tolerance)
                    return false;
        return true;
    }
}
