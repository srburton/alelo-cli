using System;
using System.IO;

namespace Alelo.Console
{
   internal static class App
    {
        private static void Main()
        {
            var aleloHome = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ALELO_HOME"))
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".alelo")
                : Environment.GetEnvironmentVariable("ALELO_HOME");

            if (!Directory.Exists(aleloHome))
                Directory.CreateDirectory(aleloHome);
        }
    }
}
