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
    //DOIT UPDATER TON DLL POUR BON FONCTIONNEMENT
    public class ModeleRamassable : ObjetDeBase
    {
        public static bool Taken { get; set; }

        static ModeleRamassable()
        {
            Taken = false;
        }

        public bool Ramasser { get; set; }

        public bool EstRamass�e { get; set; }
        public bool Placed { get; set; }

        private float Rayon { get; set; }

        Cam�raJoueur Cam�raJoueur { get; set; }

        //Matrix MondeInitial { get; set; }

        public BoundingSphere Sph�reDeCollision
        {
            get { return new BoundingSphere(Position, Rayon); }
        }

        public float? EstEnCollision(Ray autreObjet)
        {
            return Sph�reDeCollision.Intersects(autreObjet);
        }

        public ModeleRamassable(Game jeu, string nomMod�le, float �chelleInitiale,
                                Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            EstRamass�e = false;
            Placed = false;
            Rayon = 4;
            //MondeInitial = base.GetMonde();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Cam�raJoueur = Game.Services.GetService(typeof(Cam�ra)) as Cam�raJoueur;
        }

        public override void Update(GameTime gameTime)
        {
            if (EstRamass�e)
            {
                Position = Cam�raJoueur.Position + 4 * Vector3.Normalize(Cam�raJoueur.Direction)
                            + 2.5f * Vector3.Normalize(Cam�raJoueur.Lat�ral)
                            - 1.5f * Vector3.Normalize(Vector3.Cross(Cam�raJoueur.Lat�ral, Cam�raJoueur.Direction));
                CalculerMonde();
                //Game.Window.Title = Position.ToString();
            }
            base.Update(gameTime);
        }

        private void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }


    }
}
