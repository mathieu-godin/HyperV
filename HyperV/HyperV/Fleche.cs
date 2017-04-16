using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace HyperV
{
    public class Fleche : ModeleRamassable
    {
        const float FPS_60_INTERVAL = 1f / 60f;

        Vector3 Direction { get; set; }

        float Temps�coul�DepuisMAJ { get; set; }
        float TempsTotal { get; set; }
        Boss Boss { get; set; }
        List<Enemy> Enemy { get; set; }

        public Fleche(Game jeu, string nomMod�le, float �chelleInitiale,
                    Vector3 rotationInitiale, Vector3 positionInitiale, Vector3 direction)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
        {
            Direction = Vector3.Normalize(direction);
        }

        public override void Initialize()
        {
            TempsTotal = 0;
            base.Initialize();
            Boss = Game.Services.GetService(typeof(Boss)) as Boss;
            Enemy = Game.Services.GetService(typeof(List<Enemy>)) as List<Enemy>;
        }

        public override void Update(GameTime gameTime)
        {
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsTotal += Temps�coul�;
            Temps�coul�DepuisMAJ += Temps�coul�;
            if (Temps�coul�DepuisMAJ >= FPS_60_INTERVAL)
            {
                Boss.CheckForArrowAttack(Position, Direction, 1, this);
                if (Enemy.Count > 0)
                {
                    foreach (Enemy e in Enemy)
                    {
                        e.CheckForArrowAttack(Position, 1, this);
                    }
                }
                if (TempsTotal >= 5)
                {
                    Game.Components.Remove(this);
                }
                Position += Direction;
                CalculerMonde();
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        private void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);

           // Game.Window.Title = Cam�raJoueur.Direction.ToString() + "      " + MathHelper.ToDegrees(angleX).ToString() + "       " + MathHelper.ToDegrees(angleY).ToString().ToString();
        }
    }
}
