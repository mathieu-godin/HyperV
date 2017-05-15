using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
                    Game.Components.Add(new Fleche(Game, "shop", 0.0025f, new Vector3(angleY, 1.57f/*angleX + (float)Math.PI / 2*/, 1.57f/*Rotation.Z*/),
                                                   Cam�raJoueur.Position, Cam�raJoueur.Direction));
                }
            }
            base.Update(gameTime);

        }
    }
}
