using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace ParticleOnTextCursor
{
    class Particle
    {
        public int x;
        public int y;
        public float vx;
        public float vy;
        public Color color;
        public int life;

        public Particle(int x, int y, float vx, float vy, Color color, int life)
        {
            this.x = x;
            this.y = y;
            this.vx = vx;
            this.vy = vy;
            this.color = color;
            this.life = life;
        }
    }

    class ParticleEmitter
    {
        public List<Color> AvailableColors
        {
            get
            {
                return availableColors;
            }
        }
        private List<Color> availableColors;
        private Random random = new Random();

        public List<Particle> Particles
        {
            get
            {
                return particles;
            }
        }
        private List<Particle> particles = new List<Particle>();

        private List<Color> GetAllColors()
        {
            List<Color> allColors = new List<Color>();

            foreach (PropertyInfo property in typeof(Color).GetProperties())
            {
                if (property.PropertyType == typeof(Color))
                {
                    allColors.Add((Color)property.GetValue(null));
                }
            }
            return allColors;
        }

        private List<Color> GetParticleColors()
        {
            // http://www.flounder.com/csharp_color_table.htm
            List<Color> colors = new List<Color>
            {
                Color.Blue, 
                Color.CornflowerBlue,
                Color.Cyan,
                Color.Crimson,
                Color.DeepPink,
                Color.DeepSkyBlue,
                Color.MediumSpringGreen,

            };
            return colors;
        }


        public ParticleEmitter()
        {
            //availableColors = GetAllColors();
            availableColors = GetParticleColors();
        }

        public Color SelectColor()
        {
            int idx = random.Next(availableColors.Count);
            return availableColors[idx];
        }

        public int SelectLifeInterval()
        {
            int minLife = 80;
            int maxLife = 130;
            return random.Next(minLife, maxLife);
        }

        public Point SelectVelocity()
        {
            float minVelocity = 500;
            float maxVelocity = 890;
            float velocity = random.Next((int)minVelocity, (int)maxVelocity);

            int minAngle = 0;
            int maxAngle = 30;
            int angle = random.Next(minAngle, maxAngle);

            double vy = - Math.Cos((Math.PI / 180) * angle) * velocity;
            double vx = Math.Sin((Math.PI / 180) * angle) * velocity;

            int sign = random.Next(100) > 50 ? 1 : -1;
            vx = vx * sign;
            return new Point((int)vx, (int)vy);
        }

        public void Emit(int x, int y)
        {
            Color color = SelectColor();
            for(int i = 0; i < 5; ++i)
            {
                EmitSingleParticle(x, y, color);
            }
        }

        public void EmitSingleParticle(int x, int y, Color color)
        {
            Point velocity = SelectVelocity();
            float vx = velocity.X;
            float vy = velocity.Y;
            int life = SelectLifeInterval();
            Particle p = new Particle(x, y, vx, vy, color, life);
            particles.Add(p);
        }

        public void Update(int ms)
        {
            float gravity = 10.0f;
            for(int i = 0; i < particles.Count; ++i)
            {
                Particle p = particles[i];
                p.life -= ms;

                p.x += (int)(p.vx * ms / 1000.0f);
                p.y += (int)(p.vy * ms / 1000.0f);

                p.vy += gravity * ms;
            }
            particles.RemoveAll(p => p.life <= 0);
        }
    }
}
