using System;
namespace M3uParser
{
    public static class Logger
    {
        public static void Exception(Exception exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);
        }

        public static void Info(string message)
        {
            Console.WriteLine(message);
        }
    }
}
