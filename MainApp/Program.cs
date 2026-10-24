using System;
using System.IO;
using StatsLib;

namespace StatsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = ParseArguments(args);

            if (!options.IsValid)
            {
                Console.WriteLine(options.ErrorMessage);
                ShowHelp();
                return;
            }

            if (options.ShowHelp)
            {
                ShowHelp();
                return;
            }

            try
            {
                if (string.IsNullOrEmpty(options.InputFile))
                {
                    Console.WriteLine("Ошибка: Не указан входной файл (-i).");
                    ShowHelp();
                    return;
                }

                if (!File.Exists(options.InputFile))
                {
                    Console.WriteLine($"Ошибка: Файл '{options.InputFile}' не найден.");
                    return;
                }

                string content = File.ReadAllText(options.InputFile);
                var parts = content.Split(new char[] { ' ', ',', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0)
                {
                    Console.WriteLine("Ошибка: Файл пуст или не содержит чисел.");
                    return;
                }

                double[] numbers = new double[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                {
                    if (!double.TryParse(parts[i], out numbers[i]))
                    {
                        Console.WriteLine($"Ошибка: Не удалось распознать число в позиции {i + 1}: '{parts[i]}'");
                        return;
                    }
                }

                if (numbers.Length > 100)
                {
                    Console.WriteLine($"Ошибка: Количество чисел ({numbers.Length}) превышает лимит в 100.");
                    return;
                }

                // ВАЖНО: используем не-nullable значение, потому что IsValid уже гарантирует, что Operation задан
                double result = StatsCalculator.Calculate(numbers, options.Operation.Value);

                string outputText = $"Операция: {options.Operation}\nКоличество чисел: {numbers.Length}\nРезультат: {result:F6}";

                if (string.IsNullOrEmpty(options.OutputFile))
                {
                    Console.WriteLine(outputText);
                }
                else
                {
                    File.WriteAllText(options.OutputFile, outputText);
                    Console.WriteLine($"Результаты успешно записаны в файл: {options.OutputFile}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла критическая ошибка: {ex.Message}");
            }
        }

        private static Options ParseArguments(string[] args)
        {
            var opts = new Options();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i].ToLower();

                switch (arg)
                {
                    case "--help":
                        opts.ShowHelp = true;
                        break;

                    case "-i":
                    case "--input":
                        if (i + 1 >= args.Length)
                        {
                            opts.IsValid = false;
                            opts.ErrorMessage = "Отсутствует путь к входному файлу после ключа -i.";
                            return opts;
                        }
                        opts.InputFile = args[++i];
                        break;

                    case "-o":
                    case "--output":
                        if (i + 1 >= args.Length)
                        {
                            opts.IsValid = false;
                            opts.ErrorMessage = "Отсутствует путь к выходному файлу после ключа -o.";
                            return opts;
                        }
                        opts.OutputFile = args[++i];
                        break;

                    case "-op":
                    case "--operation":
                        if (i + 1 >= args.Length)
                        {
                            opts.IsValid = false;
                            opts.ErrorMessage = "Не указана операция после ключа -op.";
                            return opts;
                        }

                        string opStr = args[++i].ToLower();
                        // ВАЖНО: сначала в локальную переменную, потом присваиваем nullable
                        if (!Enum.TryParse<OperationType>(opStr, true, out var parsedOp))
                        {
                            opts.IsValid = false;
                            opts.ErrorMessage = $"Неизвестная операция: '{opStr}'. Доступные: mean, variance, stddev.";
                            return opts;
                        }

                        opts.Operation = parsedOp;
                        break;

                    default:
                        opts.IsValid = false;
                        opts.ErrorMessage = $"Неизвестный аргумент: {arg}";
                        return opts;
                }
            }

            // Если help не запрошен, а операция не задана — ошибка
            if (!opts.ShowHelp && opts.Operation == null)
            {
                opts.IsValid = false;
                opts.ErrorMessage = "Необходимо указать операцию (-op mean/variance/stddev).";
            }

            return opts;
        }

        private static void ShowHelp()
        {
            Console.WriteLine(@"
Программный комплекс (Вариант 3): Статистика списка чисел.

Использование: StatsApp.exe [ключи]

Обязательные ключи:
  -i, --input <file>      Путь к входному файлу с числами.
  -op, --operation <op>   Операция: mean (среднее), variance (дисперсия), stddev (СКО).

Необязательные ключи:
  -o, --output <file>     Путь к выходному файлу (если не указан, результат выводится в консоль).
  --help                  Показать эту справку.

Ограничения:
  - В файле не более 100 чисел.
  - Числа могут быть дробными.

Примеры:
  StatsApp.exe -i data.txt -op mean
  StatsApp.exe -i data.txt -op stddev -o result.txt
");
        }
    }

    class Options
    {
        public bool ShowHelp { get; set; } = false;
        public string InputFile { get; set; } = "";
        public string OutputFile { get; set; } = "";
        public OperationType? Operation { get; set; }
        public bool IsValid { get; set; } = true;
        public string ErrorMessage { get; set; } = "";
    }
}
