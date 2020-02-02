using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EfEagerLoad.ConsoleTester.Extensions
{
    public static class MiscExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null) { return; }

            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        public static async Task RunInConsole(this Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                var currentException = ex;
                while (currentException != null)
                {
                    Console.WriteLine(currentException.Message);
                    Console.WriteLine(currentException.StackTrace);
                    currentException = currentException.InnerException;
                }
            }
            finally
            {
                Console.WriteLine("Finished - Press a key");
                Console.ReadLine();
            }
        }
    }
}
