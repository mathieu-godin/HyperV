using AtelierXNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperV
{
    public class AmmunitionCatapulte : CreateurModele
    {
        bool EstTir� { get; set; }
        public bool EstAmmunition { get; set; }

        const float GRAVIT� = -9.81f;
        const float Poids = 0.1f;
        const float INTERVALLE_MAJ = 1 / 60f;

        int VitesseY { get; set; }
        float Temps�coul� = 0;
        float Temps�coul�MAJ = 0;
        Vector3 PositionInitiale { get; set; }

        float FrictionAir { get; set; }
        float Angle { get; set; }

        Vector3 D�placement { get; set; }
        Vector2 Vitesse { get; set; }
        SoundEffect TourD�truite { get; set; }
        RessourcesManager<SoundEffect> SoundManager { get; set; }



        public AmmunitionCatapulte(Game game, string modele3D, Vector3 position, float homoth�sie, float rotation)
            : base(game, modele3D, position, homoth�sie, rotation)
        { }

        public override void Initialize()
        {
            base.Initialize();
            D�placement = Vector3.Zero;
            PositionInitiale = Position;
            EstTir� = true;
            EstAmmunition = false;
            TourD�truite = SoundManager.Find("TourD�truite");
        }

        public override void Update(GameTime gameTime)
        {
            if (EstTir�)
            {
                Temps�coul�MAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Temps�coul�MAJ >= INTERVALLE_MAJ)
                {
                    Temps�coul� += INTERVALLE_MAJ;
                    Vector3 D�placement = PositionProjectile(Temps�coul�);
                    Position = PositionInitiale + D�placement;
                    base.Update(gameTime);
                    Temps�coul�MAJ = 0;
                }
            }
            if (Position.Y < -100)
            {
                Game.Components.Remove(this);
                //Game.Components.RemoveAt(0);  //remove le Afficheur3D qui va etre a la roche -1 ... 
            }
            if (EstAmmunition)
            {
                G�rerColision();
            }
        }

        protected override void LoadContent()
        {
            SoundManager = Game.Services.GetService(typeof(RessourcesManager<SoundEffect>)) as RessourcesManager<SoundEffect>;
            base.LoadContent();
        }

        public void TirerProjectile(float angle, Vector3 vitesse, int ModificateurVitesse)
        {
            EstTir� = true;
            Angle = angle;
            vitesse.Normalize();
            VitesseY = ModificateurVitesse;
            Vitesse = new Vector2((float)Math.Cos(vitesse.X), (float)Math.Sin(vitesse.Z)) * ModificateurVitesse;
        }

        private Vector3 PositionProjectile(float temps)
        {
            return new Vector3(D�placementX(Vitesse.X, temps), D�placementY(Angle, VitesseY, temps), D�placementZ(Vitesse.Y, temps));
        }

        private float D�placementX(float vitesse, float temps)
        {
            return vitesse * temps;
        }

        private float D�placementZ(float vitesse, float temps)
        {
            return vitesse * temps;
        }

        private float D�placementY(float angle, float vitesse, float temps)
        {
            return (vitesse * (float)Math.Sin(angle) * temps + 0.5f * GRAVIT� * temps * temps);
        }

        private void G�rerColision()
        {
            bool aDetruire = false;
            List<CreateurModele> modeleDetruire = new List<CreateurModele>();
            foreach (CreateurModele modele in Game.Components.Where(x => x is CreateurModele))
            {
                if (modele.EstTour)
                {
                    if (Position.X < modele.GetPosition().X + 8 && Position.X > modele.GetPosition().X - 8) //test X
                    {
                        if (Position.Z < modele.GetPosition().Z + 8 && Position.Z > modele.GetPosition().Z - 8) //test z
                        {
                            if (Position.Y < modele.GetPosition().Y + 120 && Position.Y > modele.GetPosition().Y) //test y
                            {
                                modeleDetruire.Add(modele);
                                aDetruire = true;
                                TourD�truite.Play();
                            }
                        }
                    }
                }
            }
            if (aDetruire)
            {
                Game.Components.Remove(this);
                foreach (CreateurModele modele in modeleDetruire)
                {
                    Game.Components.Remove(modele);
                }
            }
        }
    }
}
