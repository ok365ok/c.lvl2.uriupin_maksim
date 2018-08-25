﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Asteroids
{
    class Game
    {
        private static BufferedGraphicsContext _context;
        public static BufferedGraphics Buffer;
        private static Timer timer = new Timer();
        public static Random rnd = new Random();
        private static int Score = 0;
        private static int Level = 1;

        #region Game window settings
        public static int Width { get; set; }
        public static int Height { get; set; }
        #endregion


        #region Game objects
        private static BaseObject[] _objs;
        private static List<Asteroid> _asteroids;
        private static Bullet _bullet;
        private static Medicine _medicine;
        private static Ship _ship;
        #endregion

        #region Game settings
        private const int StarSpeed = 8;
        private const int MinStarSize = 15;
        private const int MaxStarSize = 30;
        private const int MinAsteroidSize = 30;
        private const int MaxAsteroidSize = 50;
        private const int AsteroidsCount = 10;
        private const int StarsCount = 15;
        private const int BulletSpeed = 15;
        private const int MedicinePower = 10;
        #endregion

        public Game()
        {

        }

        public static void Init(Form form)
        {
            Graphics g;
            _context = BufferedGraphicsManager.Current;
            g = form.CreateGraphics();
            Width = form.Width;
            Height = form.Height;
            Buffer = _context.Allocate(g, new Rectangle(0, 0, Width, Height));

            Ship.MessageDie += Finish;

            Load();

            form.KeyDown += Form_KeyDown;
            form.KeyPreview = true;

            timer.Interval = 100;
            timer.Start();
            timer.Tick += Timer_Tick;
        }

        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) _bullet = new Bullet(new Point(_ship.Rect.X + 60, _ship.Rect.Y + 34),
                                                                   new Point(BulletSpeed, 0), new Size(3, 3));
            if (e.KeyCode == Keys.Up) _ship.Up();
            if (e.KeyCode == Keys.Down) _ship.Down();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            _asteroids.RemoveAll(a => a.Power == 0);
            if(_asteroids.Count == 0)
            {
                Level++;
                GenerateAsteroids();
            }
            Draw();
            Update();
        }

        private static void GenerateAsteroids()
        {
            _asteroids = new List<Asteroid>();
            for (int i = 0; i < AsteroidsCount + Level - 1; i++)
            {
                int size = rnd.Next(MinAsteroidSize, MaxAsteroidSize);
                _asteroids.Add(new Asteroid(new Point(rnd.Next(Width / 2, Width), rnd.Next(0, Height)), new Point(rnd.Next(-5, 10), rnd.Next(-5, 10)), new Size(size, size)));
            }
        }

        public static void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);
            foreach (BaseObject obj in _objs)
                obj.Draw();
            foreach (Asteroid a in _asteroids)
                a.Draw();
            _bullet?.Draw();
            if (_ship != null)
                Buffer.Graphics.DrawString("Energy: " + _ship.Energy, SystemFonts.DefaultFont, Brushes.White, 0, 0);
            Buffer.Graphics.DrawString("Score: " + Score, SystemFonts.DefaultFont, Brushes.White, 0, 15);
            Buffer.Graphics.DrawString("Level: " + Level, SystemFonts.DefaultFont, Brushes.White, 0, 30);
            _ship.Draw();
            if (_medicine == null && rnd.Next(0, 50) < 10)
            {
                _medicine = new Medicine(new Point(rnd.Next(Width - 10), rnd.Next(0, Height)), new Point(10, 10), new Size(30, 30));
            }
            _medicine?.Draw();
            Buffer.Render();
        }

        public static void Update()
        {
            foreach (Asteroid asteroid in _asteroids)
            {
                asteroid.Update();
                if (_bullet != null && asteroid.Collision(_bullet))
                {
                    System.Media.SystemSounds.Beep.Play();
                    _bullet = null;
                    Score++;
                    int size = rnd.Next(MinAsteroidSize, MaxAsteroidSize);
                    asteroid.Power--;
                }
                if (!_ship.Collision(asteroid)) continue;
                _ship?.EnergyChange(1);
                System.Media.SystemSounds.Asterisk.Play();
                if (_ship.Energy <= 0) _ship?.Die();
            }
            if (_medicine != null && _ship.Collision(_medicine))
            {
                _ship?.EnergyChange(-1 * MedicinePower);
                System.Media.SystemSounds.Hand.Play();
                _medicine = null;
            }
            _medicine?.Update();
            _bullet?.Update();
        }

        public static void Load()
        {
            _ship = new Ship(new Point(15, Height / 2 - 60), new Point(5, 5), new Size(60, 70));
            _objs = new BaseObject[StarsCount + 1];
            for (int i = 0; i < StarsCount; i++)
            {
                int size = rnd.Next(MinStarSize, MaxStarSize);
                _objs[i] = new Star(new Point(rnd.Next(0, Width), rnd.Next(0, Height)), new Point(-1 * StarSpeed, 0), new Size(size, size));
            }
            _objs[_objs.Length - 1] = new Moon(new Point(Width, rnd.Next(0, Height)), new Point(-5, 0), new Size(100, 100));
            GenerateAsteroids();

        }

        public static void Finish()
        {
            timer.Stop();
            Buffer.Graphics.DrawString("The End", new Font(FontFamily.GenericSansSerif, 60, FontStyle.Underline), Brushes.White, 200, 100);
            Buffer.Render();
        }
    }
}