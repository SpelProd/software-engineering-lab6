using System;
using System.Linq;

namespace StatsLib
{
    /**
     * @brief Перечисление OperationType - Список поддерживаемых статистических операций.
     * 
     * Определяет доступные алгоритмы обработки числовых массивов данных.
     */
    public enum OperationType
    {
        /** 
         * @brief Вычисление среднего арифметического значения числового ряда.
         */
        Mean,

        /** 
         * @brief Вычисление выборочной дисперсии (несмещенная оценка).
         */
        Variance,

        /** 
         * @brief Вычисление среднеквадратичного отклонения (СКО).
         */
        StdDev
    }

    /**
     * @brief Статический класс StatsCalculator - Математический движок расчетов.
     * 
     * Содержит низкоуровневую реализацию алгоритмов первичного статистического анализа 
     * для наборов вещественных чисел с жестким контролем входных ограничений.
     */
    public static class StatsCalculator
    {
        /**
         * @brief Calculate - Универсальная точка вызова статистических операций.
         * 
         * Принимает входной вектор чисел, валидирует его размер согласно ограничениям проекта, 
         * маршрутизирует вызов на соответствующий внутренний метод и возвращает результат вычислений.
         * 
         * @param numbers double[]: Входной массив числовых элементов. Не должен быть null или пустым.
         * @param operation OperationType: Идентификатор запрашиваемой математической операции.
         * @return double: Итоговое рассчитанное статистическое значение. Допустимые значения: любое вещественное число.
         * @exception ArgumentException Вызывается, если массив null или не содержит элементов.
         * @exception ArgumentOutOfRangeException Вызывается, если размерность массива превышает 100 элементов.
         * @exception NotSupportedException Вызывается при передаче неопознанного типа операции.
         */
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

        /**
         * @brief CalculateMean - Вычисление математического среднего арифметического.
         * 
         * Суммирует все элементы массива и делит полученную сумму на их общее количество.
         * 
         * @param numbers double[]: Валидированный массив чисел для расчета.
         * @return double: Среднее арифметическое значение элементов.
         */
        private static double CalculateMean(double[] numbers)
        {
            return numbers.Average();
        }

        /**
         * @brief CalculateVariance - Вычисление выборочной дисперсии.
         * 
         * Рассчитывает сумму квадратов отклонений каждого элемента от их общего среднего значения. 
         * Для обеспечения несмещенности статистической оценки применяется деление на (N - 1), где N — объем выборки.
         * 
         * @param numbers double[]: Валидированный массив чисел для расчета.
         * @return double: Значение выборочной дисперсии ряда данных. При N = 1 возвращает результат деления на 1.
         */
        private static double CalculateVariance(double[] numbers)
        {
            double mean = CalculateMean(numbers);
            double sumOfSquaredDifferences = numbers.Sum(x => Math.Pow(x - mean, 2));

            // Используем несмещенную оценку (деление на N-1), если N > 1. 
            int divisor = numbers.Length > 1 ? numbers.Length - 1 : 1;

            return sumOfSquaredDifferences / divisor;
        }

        /**
         * @brief CalculateStdDev - Вычисление среднеквадратичного отклонения.
         * 
         * Извлекает квадратный корень из ранее рассчитанной несмещенной выборочной дисперсии ряда данных.
         * 
         * @param numbers double[]: Валидированный массив чисел для расчета.
         * @return double: Итоговое среднеквадратичное отклонение выборки. Допустимые значения: >= 0.
         */
        private static double CalculateStdDev(double[] numbers)
        {
            return Math.Sqrt(CalculateVariance(numbers));
        }
    }
}
