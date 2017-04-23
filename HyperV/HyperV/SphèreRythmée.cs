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
    public class Sph�reRythm�e : Sph�reTextur�e, ICollisionable
    {
        public bool �D�truire { get; set; }

        #region
        float Rayon { get; set; }

        public new bool EstEnCollision(object autreObjet)
        {
            BoundingSphere obj2 = (autreObjet as ICollisionable).Sph�reDeCollision;
            return obj2.Intersects(Sph�reDeCollision);
        }

        public new BoundingSphere Sph�reDeCollision
        {
            get
            {
                return new BoundingSphere(Position, Rayon);
            }
        }
        #endregion

        public Vector3 Extr�mit�1 { get; set; }
        Vector3 Extr�mit�2 { get; set; }
        Vector3 VecteurD�placement { get; set; }
        int i { get; set; }
        LifeBar[] LifeBars { get; set; }

        NiveauRythm� Niveau { get; set; }

        public Sph�reRythm�e(Game jeu, float homoth�tieInitiale, Vector3 rotationInitiale,
                       Vector3 positionInitiale, float rayon, Vector2 charpente,
                       string nomTexture, float intervalleMAJ, Vector3 extr�mit�2)
            : base(jeu, homoth�tieInitiale, rotationInitiale,
                   positionInitiale, rayon, charpente,
                   nomTexture, intervalleMAJ)
        {
            Rayon = rayon;//****

            Extr�mit�1 = positionInitiale;
            Extr�mit�2 = extr�mit�2;
        }

        public override void Initialize()
        {
            base.Initialize();
            �D�truire = false;
            i = 0;
            VecteurD�placement = Vector3.Normalize(Extr�mit�2 - Extr�mit�1);
            Niveau = Game.Services.GetService(typeof(NiveauRythm�)) as NiveauRythm�;
            LifeBars = Game.Services.GetService(typeof(LifeBar[])) as LifeBar[];

        }

        protected override void EffectuerMise�Jour()
        {
            base.EffectuerMise�Jour();

            Position += VecteurD�placement;
            CalculerMatriceMonde();
            i++;

            if(i > (Extr�mit�1 - Extr�mit�2).Length() + 3 || �D�truire)
            {
                if (!�D�truire)
                {
                    Niveau.PositionCubeRouge = Extr�mit�2;
                    LifeBars[0].Attack(10);
                }
                Game.Components.Remove(this);

                //faire enlever de la vie
            }

        }


    }
}
