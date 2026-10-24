using System;
using System.Linq;

namespace StatsLib
{
    public enum OperationType
    {
        Mean,       // Среднее
        Variance,   // Дисперсия
        StdDev      // Среднеквадратичное отклонение
    }

    public static class StatsCalculator
    {
        /// <summary>
        /// Выполняет статистическую операцию над списком чисел.
        /// </summary>
        /// <param name="numbers">Список чисел (до 100 элементов)</param>
        /// <param name="operation">Тип операции</param>
        /// <returns>Результат операции</returns>
        public static double Calculate(double[] numbers, OperationType operation)
        {
            if (numbers == null || numbers.Length == 0)
                throw new ArgumentException("Список чисел не может быть пустым.");

            if (numbers.Length > 100)
                throw new ArgumentOutOfRangeException("Количество чисел не должно превышать 100.");

            switch (operation)
            {
                case OperationType.Mean:
                    return CalculateMean(numbers);
                case OperationType.Variance:
                    return CalculateVariance(numbers);
                case OperationType.StdDev:
                    return CalculateStdDev(numbers);
                default:
                    throw new NotSupportedException($"Операция {operation} не поддерживается.");
            }
        }

        private static double CalculateMean(double[] numbers)
        {
            return numbers.Average();
        }

        private static double CalculateVariance(double[] numbers)
        {
            double mean = CalculateMean(numbers);
            double sumOfSquaredDifferences = numbers.Sum(x => Math.Pow(x - mean, 2));

            // Используем несмещенную оценку (деление на N-1), если N > 1. 
            // Если нужно просто среднее квадратов отклонений (N), замени (numbers.Length - 1) на numbers.Length
            int divisor = numbers.Length > 1 ? numbers.Length - 1 : 1;

            return sumOfSquaredDifferences / divisor;
        }

        private static double CalculateStdDev(double[] numbers)
        {
            return Math.Sqrt(CalculateVariance(numbers));
        }
    }
}
