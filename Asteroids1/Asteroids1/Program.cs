using System;
using System.Windows.Forms;
// создаем шаблон приложения, где подключаем модули
namespace Asteroids1
{
    class Program
    {
        static void Main(string[] args)
        {
            Form form = new Form();
            form.Width = 800;
            form.Height = 600;
            Game.Init(form);
            form.Show();
            Game.Draw();
            Application.Run(form);
        }
    }
}
