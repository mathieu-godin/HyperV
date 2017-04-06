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
using AtelierXNA;


namespace HyperV
{
    public class Arc : ModeleRamassable
    {
        const float FPS_60_INTERVAL = 1f / 60f;

        bool LancerFleche { get; set; }
        InputManager GestionInput { get; set; }
        GamePadManager GestionGamePad { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }

        public Arc(Game jeu, string nomMod�le, float �chelleInitiale,
                    Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            LancerFleche = false;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionGamePad = Game.Services.GetService(typeof(GamePadManager)) as GamePadManager;
        }

        public override void Update(GameTime gameTime)
        {
            LancerFleche = (GestionInput.EstEnfonc�e(Keys.T) || GestionInput.EstNouveauClicDroit()) && EstRamass�e;

            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            if (Temps�coul�DepuisMAJ >= FPS_60_INTERVAL)
            {
                if (LancerFleche)
                {
                    Game.Components.Add(new Fleche(Game, "Robot", 0.002f, new Vector3(angleY, angleX + (float)Math.PI / 2, Rotation.Z),
                                                   Cam�raJoueur.Position, Cam�raJoueur.Direction));
                }
            }
            base.Update(gameTime);

        }
    }
}