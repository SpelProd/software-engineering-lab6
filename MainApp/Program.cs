using System;
using System.IO;
using StatsLib;

namespace StatsApp
{
    /**
     * @brief Класс Program - Главный класс приложения StatsApp.
     * 
     * Обеспечивает парсинг аргументов командной строки, чтение данных из файлов,
     * валидацию ограничений и вывод результатов вычислений.
     */
    class Program
    {
        /**
         * @brief Main - Точка входа в программу.
         * 
         * Координирует весь жизненный цикл выполнения: парсит аргументы,
         * проверяет файлы на существование, считывает до 100 чисел, вызывает
         * библиотеку расчетов и записывает/выводит финальный результат.
         * 
         * @param args string[]: Массив аргументов командной строки, переданных при запуске.
         * @return void: Функция не возвращает значения.
         */
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

        /**
         * @brief ParseArguments - Метод для парсинга и валидации аргументов командной строки.
         * 
         * Последовательно разбирает флаги (-i, -o, -op, --help) и заполняет структуру настроек.
         * 
         * @param args string[]: Массив переданных строковых аргументов.
         * @return Options: Объект класса Options с заполненными параметрами запуска и флагом валидности.
         */
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

        /**
         * @brief ShowHelp - Вывод справочной информации о программе в консоль.
         * 
         * Печатает синтаксис использования приложения, поддерживаемые ключи,
         * установленные ограничения на размер файла и практические примеры запуска.
         * 
         * @return void: Функция не возвращает значения.
         */
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

    /**
     * @brief Класс Options - Контейнер для хранения параметров конфигурации запуска.
     * 
     * Хранит распарсенные пути к файлам, тип выбранной операции и состояние валидности.
     */
    class Options
    {
        /**
         * @brief Переключатель отображения справки.
         * Тип: bool. Допустимые значения: true (показать справку), false (обычный запуск).
         */
        public bool ShowHelp { get; set; } = false;

        /**
         * @brief Путь к исходному текстовому файлу с числами.
         * Тип: string. Допустимые значения: валидный путь к файлу.
         */
        public string InputFile { get; set; } = "";

        /**
         * @brief Путь к итоговому файлу для сохранения отчета.
         * Тип: string. Допустимые значения: пустая строка или путь для записи.
         */
        public string OutputFile { get; set; } = "";

        /**
         * @brief Тип выполняемого статистического расчета.
         * Тип: OperationType? (nullable-перечисление). Допустимые значения: mean, variance, stddev, null.
         */
        public OperationType? Operation { get; set; }

        /**
         * @brief Флаг корректности переданных аргументов командной строки.
         * Тип: bool. Допустимые значения: true (ошибок нет), false (обнаружены некорректные аргументы).
         */
        public bool IsValid { get; set; } = true;

        /**
         * @brief Сообщение о конкретной ошибке, возникшей при парсинге.
         * Тип: string. Допустимые значения: текст ошибки, описывающий причину провала валидации.
         */
        public string ErrorMessage { get; set; } = "";
    }
}
