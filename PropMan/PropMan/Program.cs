using System;
using System.IO;
using System.Linq;

namespace PropMan
{
    class Program
    {
        private enum Operation
        {
            Get,
            Set,
            Enum
        }

        static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("Properties.exe command file [property name] [property value]");
            Console.WriteLine();
            Console.WriteLine("Command: enum | set | get");
        }

        static bool IsNumeric(string str)
        {
            return str.All(c => Char.IsDigit(c));
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
            if (args[0].Equals("get", StringComparison.OrdinalIgnoreCase))
            {
                operation = Operation.Get;
            }
            else if (args[0].Equals("set", StringComparison.OrdinalIgnoreCase))
            {
                operation = Operation.Set;
            }
            else if (args[0].Equals("enum", StringComparison.OrdinalIgnoreCase))
            {
                operation = Operation.Enum;
            }
            else
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
