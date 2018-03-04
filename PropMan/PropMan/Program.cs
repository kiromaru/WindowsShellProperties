using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PropMan
{
    class Program
    {
        private enum Operation
        {
            Invalid,
            Get,
            Set,
            Enum
        }

        private static class Commands
        {
            public static readonly string Get = "get";
            public static readonly string Set = "set";
            public static readonly string Enum = "enum";
        }

        static void ShowUsage()
        {
            Assembly exeAssembly = Assembly.GetEntryAssembly();
            string exeName = Path.GetFileName(exeAssembly.Location);

            Console.WriteLine("Usage:");
            Console.WriteLine($"{exeName} command file [property name] [property value]");
            Console.WriteLine();
            Console.WriteLine("Command: enum | set | get");
        }

        private static bool IsNumeric(string str) => str.All(c => Char.IsDigit(c));

        private static bool IsDate(string propertyValue) => DateTime.TryParse(propertyValue, out DateTime result);

        static Operation DetermineOperation(string command)
        {
            Operation operation = Operation.Invalid;

            if (Commands.Get.Equals(command, StringComparison.OrdinalIgnoreCase))
            {
                operation = Operation.Get;
            }
            else if (Commands.Set.Equals(command, StringComparison.OrdinalIgnoreCase))
            {
                operation = Operation.Set;
            }
            else if (Commands.Enum.Equals(command, StringComparison.OrdinalIgnoreCase))
            {
                operation = Operation.Enum;
            }

            return operation;
        }

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                ShowUsage();
                return;
            }

            Operation operation;
            string fileName = args[1];
            string propertyName = null;
            string propertyValue = null;

            // Determine the operation requested.
            operation = DetermineOperation(args[0]);
            if (operation == Operation.Invalid)
            {
                ShowUsage();
                return;
            }


            if (args.Length >= 3)
            {
                propertyName = args[2];
            }

            if (args.Length >= 4)
            {
                propertyValue = args[3];
            }

            // We need 3 parameters for a Get: command, file name, property name
            if (operation == Operation.Get && null == propertyName)
            {
                ShowUsage();
                return;
            }

            // We need 4 parameters for a Set: command, file name, property name, value
            if (operation == Operation.Set && null == propertyValue)
            {
                ShowUsage();
                return;
            }

            // The file must exist
            if (!File.Exists(fileName))
            {
                Console.WriteLine("File '{0}' does not exist.", fileName);
                return;
            }
            fileName = Path.GetFullPath(fileName);

            try
            {
                switch (operation)
                {
                    case Operation.Enum:
                        PropertyManager.EnumProperties(fileName);
                        break;

                    case Operation.Set:
                        {
                            object value;
                            if (IsNumeric(propertyValue))
                            {
                                value = int.Parse(propertyValue);
                            }
                            if (IsDate(propertyValue))
                            {
                                value = DateTime.Parse(propertyValue);
                            }
                            else
                            {
                                value = propertyValue;
                            }

                            PropertyManager.SetProperty(fileName, propertyName, value);
                        }
                        break;

                    case Operation.Get:
                        PropertyManager.GetProperty(fileName, propertyName);
                        break;

                    default:
                        Console.WriteLine("Unrecognized operation: {0}", operation.ToString());
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while running command:");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
