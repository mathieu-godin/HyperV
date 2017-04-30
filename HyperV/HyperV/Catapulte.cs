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
    public class Catapulte : CreateurModele
    {
        const float INTERVALLE_MAJ = 1 / 60f;

        float Temps�coul�MAJ { get; set; }
        float Temps�coul�MAJ2 { get; set; }
        float CooldownTir { get; set; }
        InputManager GestionInput { get; set; }
        Camera2 Camera { get; set; }
        AmmunitionCatapulte Ammunition { get; set; }
        bool EstActiv�e { get; set; }
        RessourcesManager<SoundEffect> SoundManager { get; set; }
        SoundEffect CatapulteTir�e { get; set; }

        float angle_;
        float Angle
        {
            get { return angle_; }
            set
            {
                if (angle_ < 0)
                {
                    value = 0;
                }
                if (angle_ > 90)
                {
                    value = 90;
                }
                angle_ = value;
            }
        }

        int vitesse_;
        int Vitesse
        {
            get { return vitesse_; }
            set
            {
                if (vitesse_ < 10)
                {
                    value = 10;
                }
                if (vitesse_ > 150)
                {
                    value = 150;
                }
                vitesse_ = value;
            }
        }

        Vector2 AncienVecteur { get; set; }

        public Catapulte(Game game, string modele3D, Vector3 position, float homoth�sie, float rotation)
            : base(game, modele3D, position, homoth�sie, rotation)
        { }

        public override void Initialize()
        {
            CooldownTir = 5;
            Vitesse = 20;
            Angle = 45;
            base.Initialize();
            EstActiv�e = false;
            AncienVecteur = new Vector2(Camera.Direction.X, Camera.Direction.Z);
            CatapulteTir�e = SoundManager.Find("CatapulteTir�e");

        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Camera = Game.Services.GetService(typeof(Cam�ra)) as Camera2;
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            SoundManager = Game.Services.GetService(typeof(RessourcesManager<SoundEffect>)) as RessourcesManager<SoundEffect>;
        }

        public override void Update(GameTime gameTime)
        {
            if (GestionInput.EstNouveauClicGauche())
            {
                Camera.D�sactiverCam�ra();
                EstActiv�e = !EstActiv�e;
                Rotation = 0;
            }

            G�rerTrajectoire(gameTime);
            G�rerTir(gameTime);
        }

        private void TournerModele()
        {
            Vector2 NouveauVecteur = new Vector2(Camera.Direction.X, Camera.Direction.Z);
            if (NouveauVecteur != AncienVecteur)
            {
                Rotation -= Camera.D�placementSouris.X * MathHelper.Pi / 180 * 0.1f;
                AncienVecteur = NouveauVecteur;
            }
        }

        private void ModifierAngle()
        {
            if (GestionInput.EstEnfonc�e(Keys.W))
            {
                Angle += 1;
            }
            if (GestionInput.EstEnfonc�e(Keys.S))
            {
                Angle -= 1;
            }
        }
        private void ModifierVitesse()
        {
            if (GestionInput.EstEnfonc�e(Keys.A))
            {
                Vitesse -= 1;
            }
            if (GestionInput.EstEnfonc�e(Keys.D))
            {
                Vitesse += 1;
            }
        }

        private void G�rerTrajectoire(GameTime gameTime)
        {
            Temps�coul�MAJ2 += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Temps�coul�MAJ2 >= 0.5f)
            {
                if (EstActiv�e)
                {
                    AmmunitionCatapulte Trajectoire = new AmmunitionCatapulte(Game, "Models_Ammunition", new Vector3(Position.X, Position.Y + 15, Position.Z), 0.4f, 180);
                    Game.Components.Add(new Afficheur3D(Game));
                    Game.Components.Add(Trajectoire);
                    Trajectoire.TirerProjectile(MathHelper.ToRadians(Angle), Camera.Direction, Vitesse);
                }
                Temps�coul�MAJ2 = 0;
            }
        }

        private void G�rerTir(GameTime gameTime)
        {
            CooldownTir += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�MAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Temps�coul�MAJ >= INTERVALLE_MAJ)
            {
                if (EstActiv�e)
                {
                    TournerModele();
                    ModifierAngle();
                    ModifierVitesse();
                    if (GestionInput.EstEnfonc�e(Keys.Space))
                    {
                        if (CooldownTir >= 5)
                        {
                            AmmunitionCatapulte Ammunition = new AmmunitionCatapulte(Game, "Models_Ammunition", new Vector3(Position.X, Position.Y + 15, Position.Z), 2, 180);
                            Game.Components.Add(new Afficheur3D(Game));
                            Game.Components.Add(Ammunition);
                            Ammunition.TirerProjectile(MathHelper.ToRadians(Angle), Camera.Direction, Vitesse);
                            Ammunition.EstAmmunition = true;
                            CooldownTir = 0;
                            CatapulteTir�e.Play();
                        }
                    }
                }
                Temps�coul�MAJ = 0;
            }
        }
    }
}
