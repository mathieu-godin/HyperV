//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
//using AtelierXNA;


//namespace HyperV
//{
//    //DOIT UPDATER TON DLL POUR BON FONCTIONNEMENT
//    public class ModeleRamassable : ObjetDeBase
//    {
//        public static bool Taken { get; set; }

//        static ModeleRamassable()
//        {
//            Taken = false;
//        }

//        public bool Ramasser { get; set; }

//        public bool EstRamass�e { get; set; }
//        public bool Placed { get; set; }

//        private float Rayon { get; set; }

//        Cam�raJoueur Cam�raJoueur { get; set; }

//        //Matrix MondeInitial { get; set; }

//        public BoundingSphere Sph�reDeCollision
//        {
//            get { return new BoundingSphere(Position, Rayon); }
//        }

//        public float? EstEnCollision(Ray autreObjet)
//        {
//            return Sph�reDeCollision.Intersects(autreObjet);
//        }

//        public ModeleRamassable(Game jeu, string nomMod�le, float �chelleInitiale,
//                                Vector3 rotationInitiale, Vector3 positionInitiale)
//            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
//        {

//        }

//        public override void Initialize()
//        {
//            base.Initialize();
//            EstRamass�e = false;
//            Placed = false;
//            Rayon = 4;
//            //MondeInitial = base.GetMonde();
//        }

//        protected override void LoadContent()
//        {
//            base.LoadContent();
//            Cam�raJoueur = Game.Services.GetService(typeof(Cam�ra)) as Cam�raJoueur;
//        }

//        public override void Update(GameTime gameTime)
//        {
//            if (EstRamass�e)
//            {
//                Position = Cam�raJoueur.Position + 4 * Vector3.Normalize(Cam�raJoueur.Direction)
//                            + 2.5f * Vector3.Normalize(Cam�raJoueur.Lat�ral)
//                            - 1.5f * Vector3.Normalize(Vector3.Cross(Cam�raJoueur.Lat�ral, Cam�raJoueur.Direction));
//                CalculerMonde();
//                //Game.Window.Title = Position.ToString();
//            }
//            base.Update(gameTime);
//        }

//        private void CalculerMonde()
//        {
//            Monde = Matrix.Identity;
//            Monde *= Matrix.CreateScale(�chelle);
//            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
//            Monde *= Matrix.CreateTranslation(Position);
//        }


//    }
//}





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
    public class ModeleRamassable : ObjetDeBase
    {
        public static bool Taken { get; set; }

        static ModeleRamassable()
        {
            Taken = false;
        }
        public bool Placed { get; set; }
        public bool Ramasser { get; set; }
        //UP WAS COMMENTED

        public bool EstRamass�e { get; set; }

        private float Rayon { get; set; }

        protected Cam�raJoueur Cam�raJoueur { get; set; }

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
            EstRamass�e = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            //EstRamass�e = false;
            Rayon = 10;
            //Placed = false;
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
                CalculerAngles();
                CalculerMonde();
            }
            base.Update(gameTime);
        }

        protected float angleX { set; get; }

        protected float angleY { set; get; }

        protected virtual void CalculerAngles()
        {
            Vector3 DirectionXYZ = Vector3.Normalize(new Vector3(Cam�raJoueur.Direction.X, Cam�raJoueur.Direction.Y, Cam�raJoueur.Direction.Z));

            angleX = -(float)Math.PI / 2f +//********
                (float)Math.PI * 3 / 2f +
                (DirectionXYZ.Z >= 0 ? -1 : 1) *
                (float)Math.Acos(Vector2.Dot((new Vector2(DirectionXYZ.X, DirectionXYZ.Z)),
                                              new Vector2(1, 0)));
            angleY =
                MathHelper.Pi / 2f -
                (float)Math.Acos(Vector2.Dot(new Vector2(DirectionXYZ.X, DirectionXYZ.Y),
                                             new Vector2(0, 1)));
        }

        private void CalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(angleX, angleY, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);

            //Game.Window.Title = Cam�raJoueur.Direction.ToString() + "      " + MathHelper.ToDegrees(angleX).ToString() + "       " + MathHelper.ToDegrees(angleY).ToString().ToString();
        }
    }
}
