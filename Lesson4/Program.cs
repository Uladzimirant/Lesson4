// See https://aka.ms/new-console-template for more information

namespace Lesson4 {
    public class Program
    {
        public static void Main(string[] args)
        {
            CMDHandler handler = new CMDHandler();

            //These are console commands that will execute functions
            handler.RegisterCommand("matrix_test", () => { throw new NotImplementedException("There will be some matrix method"); });

            handler.Run();
        }
    }
}

