using System;
using System.Windows.Forms;
using System.Drawing;

namespace Asteroids1
{
    static class Game
    {
        private static BufferedGraphicsContext _context;
        public static BufferedGraphics Buffer;
        // свойства
        // ширина и высота игрового поля
        public static int Width { get; set; }
        public static int Height { get; set; }
        static Game()
        {
        }

        public static void Init (Form form)
        {
            //графическое устройство для вывода графики
            Graphics g;
            //предоставляет доступ к главному буферу графического контекста для текущего приложения
            _context = BufferedGraphicsManager.Current;
            g = form.CreateGraphics();
            // создаем объект (поверхность рисования) и связываем его с формой
            // запоминаем размеры формы
            Width = form.Width;
            Height = form.Height;
            // связываем буфер в памяти с графическим объектом, чтобы рисовать в буфере
            Buffer = _context.Allocate(g, new Rectangle(0, 0, Width, Height));
            // таймер
            Timer timer = new Timer { Interval = 100 };
            timer.Start();
            timer.Tick += Timer_Tick;
            // метод вызова Load
            Load();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            // обработчик таймера
            Draw();
            Update();
        }

        public static void Draw()
        {
            // проверяем вывод графики
            Buffer.Graphics.Clear(Color.Black);
            Buffer.Graphics.DrawRectangle(Pens.White, new Rectangle(100, 100, 200, 200));
            Buffer.Graphics.FillEllipse(Brushes.Wheat, new Rectangle(100, 100, 200, 200));
            Buffer.Graphics.FillEllipse(Brushes.Wheat, new Rectangle(100, 100, 200, 200));
            Buffer.Render();

            // вывод объектов на экран
            Buffer.Graphics.Clear(Color.Black);
            foreach (BaseObject obj in _objs)
                obj.Draw();
            Buffer.Render();
        }

        public static void Update()
        {
            // изменение состояний объектов
            foreach (BaseObject obj in _objs)
                obj.Update();
        }

        // создаем массив
        public static BaseObject[] _objs;

        // иниализируем объекты
        public static void Load()
        {
            _objs = new BaseObject[30];
            for (int i = 0; i < _objs.Length / 2; i++)
                _objs[i] = new BaseObject(new Point(600, i * 20), new Point(-i, -i), new Size(10, 10));
            for (int i = _objs.Length / 2; i < _objs.Length; i++)
                _objs[i] = new Star(new Point(600, i * 20), new Point(-i, 0), new Size(5, 5));
            for (int i = _objs.Length / 2; i < _objs.Length; i++)
                _objs[i] = new Planet(new Point(600, i * 20), new Point(-i, i), new Size(20, 20));
        }
    }
}