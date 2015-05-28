using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Linq;

namespace PropMan
{
    static class PropertyManager
    {
        public static void EnumProperties(string fileName)
        {
            ShellPropertyCollection properties = new ShellPropertyCollection(fileName);
            foreach (IShellProperty property in properties)
            {
                DisplayProperty(property);
                Console.WriteLine("-------------------------------------------");
            }
        }

        public static void SetProperty(string fileName, string propName, object value)
        {
            ShellObject shObj = ShellObject.FromParsingName(fileName);
            using (ShellPropertyWriter writer = shObj.Properties.GetPropertyWriter())
            {
                writer.WriteProperty(propName, value);
            }
        }

        public static void GetProperty(string fileName, string propName)
        {
            ShellPropertyCollection properties = new ShellPropertyCollection(fileName);

            IShellProperty property = properties.FirstOrDefault(p => p.CanonicalName != null && p.CanonicalName.Equals(propName));

            if (null == property)
            {
                Console.WriteLine("Property '{0}' was not found.", propName);
                return;
            }

            DisplayProperty(property);
        }

        private static void DisplayProperty(IShellProperty property)
        {
            object value = property.ValueAsObject;

            Console.WriteLine("Canonical name: {0}", property.CanonicalName);
            Console.WriteLine("Property key:   {0}", property.PropertyKey);
            Console.WriteLine("Description:    {0}", property.Description);
            Console.WriteLine("Value:          {0}", value != null ? value.ToString() : "(null)");
        }
    }
}
