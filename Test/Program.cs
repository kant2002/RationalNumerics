global using System.Diagnostics;
global using System.Numerics.Rational;
global using rat = System.Numerics.Rational.NewRational;
//global using old = Test.BigRational;
using System.Buffers;
using System.Drawing.Imaging;
using System.Numerics;

namespace Test
{
  internal static class Program
  {
    [STAThread]
    static void Main()
    {
      ApplicationConfiguration.Initialize(); test();
      //Application.EnableVisualStyles();
      //Application.SetCompatibleTextRenderingDefault(false);
      //Application.SetHighDpiMode(HighDpiMode.SystemAware);
      Application.Run(new MainFrame());
    }
    static void test()
    {
      //var cpu = rat.task_cpu;
      //cpu.push(0xffffffff);
      //cpu.sqr(); cpu.pop();
      //cpu.push(0x100000000);
      //cpu.sqr(); cpu.pop();
    }
  }
}