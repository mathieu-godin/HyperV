using AtelierXNA;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace HyperV
{
    public class Camera3 : Cam�ra
    {
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        const float ACC�L�RATION = 0.001f;
        const float VITESSE_INITIALE_ROTATION = 5f;
        const float VITESSE_INITIALE_ROTATION_SOURIS = 0.1f;
        const float VITESSE_INITIALE_TRANSLATION = 0.5f;
        const float DELTA_LACET = MathHelper.Pi / 180; // 1 degr� � la fois
        const float DELTA_TANGAGE = MathHelper.Pi / 180; // 1 degr� � la fois
        const float DELTA_ROULIS = MathHelper.Pi / 180; // 1 degr� � la fois
        const float RAYON_COLLISION = 1f;
        const int HAUTEUR_PERSONNAGE = -6;

        bool EnColision { get; set; }
        bool placerJoueur { get; set; }
        public Vector3 Direction { get; private set; }
        Vector3 Lat�ral { get; set; }
        Grass Grass { get; set; }
        Walls Walls { get; set; }
        float VitesseTranslation { get; set; }
        float VitesseRotation { get; set; }
        Point AnciennePositionSouris { get; set; }
        Point NouvellePositionSouris { get; set; }
        public Vector2 D�placementSouris { get; set; }
        Vector3 PositionSauvegard�e { get; set; }

        float IntervalleMAJ { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }
        InputManager GestionInput { get; set; }
        float Height { get; set; }
        List<Character> Characters { get; set; }
        bool D�sactiverD�placement { get; set; }

        public Camera3(Game jeu, Vector3 positionCam�ra, Vector3 cible, Vector3 orientation, float intervalleMAJ) : base(jeu)
        {
            IntervalleMAJ = intervalleMAJ;
            Cr�erVolumeDeVisualisation(OUVERTURE_OBJECTIF, DISTANCE_PLAN_RAPPROCH�, DISTANCE_PLAN_�LOIGN�);
            Cr�erPointDeVue(positionCam�ra, cible, orientation);
            Height = positionCam�ra.Y;
            Position = positionCam�ra;
        }

        public override void Initialize()
        {
            EnColision = false;
            D�sactiverD�placement = false;
            VitesseRotation = VITESSE_INITIALE_ROTATION;
            VitesseTranslation = VITESSE_INITIALE_TRANSLATION;
            Temps�coul�DepuisMAJ = 0;
            base.Initialize();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            Grass = Game.Services.GetService(typeof(Grass)) as Grass;
            Walls = Game.Services.GetService(typeof(Walls)) as Walls;
            Characters = Game.Services.GetService(typeof(List<Character>)) as List<Character>;
            NouvellePositionSouris = new Point(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
            AnciennePositionSouris = new Point(NouvellePositionSouris.X, NouvellePositionSouris.Y);
            Mouse.SetPosition(NouvellePositionSouris.X, NouvellePositionSouris.Y);
        }

        protected override void Cr�erPointDeVue()
        {
            Vector3.Normalize(Direction);
            Vector3.Normalize(OrientationVerticale);
            Vector3.Normalize(Lat�ral);

            Vue = Matrix.CreateLookAt(Position, Position + Direction, OrientationVerticale);
        }

        protected override void Cr�erPointDeVue(Vector3 position, Vector3 cible, Vector3 orientation)
        {
            Position = position;
            Cible = cible;
            OrientationVerticale = orientation;

            Direction = cible - Position;
            //Direction = cible;

            Vector3.Normalize(Cible);

            Cr�erPointDeVue();
        }

        public override void Update(GameTime gameTime)
        {
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                List<CreateurModele> Models = new List<CreateurModele>();
                foreach (CreateurModele modele in Game.Components.Where(x => x is ICollisionable))
                {
                    Models.Add(modele);
                }
                //if (!EnColision)
                {
                    if (!D�sactiverD�placement)
                    {
                        if (placerJoueur)
                        {
                            placerJoueur = false;
                            Position = new Vector3(-27, 2, -28);
                        }
                        FonctionsClavier();
                    }
                    if (D�sactiverD�placement)
                    {
                        PositionSauvegard�e = Position;
                        Position = new Vector3(-57, 15, -52);
                        placerJoueur = true;
                    }
                    FonctionsSouris();

                    G�rerHauteur();
                    Cr�erPointDeVue();
                    
                    Position = new Vector3(Position.X, Height, Position.Z);
                }

                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
        
        #region Souris
        private void FonctionsSouris()
        {
            AnciennePositionSouris = NouvellePositionSouris;
            NouvellePositionSouris = GestionInput.GetPositionSouris();
            D�placementSouris = new Vector2(NouvellePositionSouris.X - AnciennePositionSouris.X,
                                            NouvellePositionSouris.Y - AnciennePositionSouris.Y);

            G�rerRotationSouris();

            NouvellePositionSouris = new Point(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
            Mouse.SetPosition(NouvellePositionSouris.X, NouvellePositionSouris.Y);

        }

        private void G�rerRotationSouris()
        {
            G�rerLacetSouris();
            if (!D�sactiverD�placement)
            {
                G�rerTangageSouris();
            }
        }

        private void G�rerLacetSouris()
        {
            Matrix matriceLacet = Matrix.Identity;

            matriceLacet = Matrix.CreateFromAxisAngle(OrientationVerticale, DELTA_LACET * VITESSE_INITIALE_ROTATION_SOURIS * -D�placementSouris.X);

            Direction = Vector3.Transform(Direction, matriceLacet);
        }

        private void G�rerTangageSouris()
        {
            Matrix matriceTangage = Matrix.Identity;

            matriceTangage = Matrix.CreateFromAxisAngle(Lat�ral, DELTA_TANGAGE * VITESSE_INITIALE_ROTATION_SOURIS * -D�placementSouris.Y);

            Direction = Vector3.Transform(Direction, matriceTangage);
        }
        #endregion
        
        #region Clavier
        private void FonctionsClavier()
        {
            G�rerD�placement();
        }

        private void G�rerD�placement()
        {
            float d�placementDirection = (G�rerTouche(Keys.W) - G�rerTouche(Keys.S)) * VitesseTranslation;
            float d�placementLat�ral = (G�rerTouche(Keys.A) - G�rerTouche(Keys.D)) * VitesseTranslation;

            Direction = Vector3.Normalize(Direction);
            Lat�ral = Vector3.Cross(Direction, OrientationVerticale);
            Position += d�placementDirection * Direction;
            Position -= d�placementLat�ral * Lat�ral;
            //if (Walls.CheckForCollisions(Position) || CheckForCharacterCollision())
            //{
            //    Position -= d�placementDirection * Direction;
            //    Position += d�placementLat�ral * Lat�ral;
            //}
        }

        const float MAX_DISTANCE = 4.5f;

        bool CheckForCharacterCollision()
        {
            bool result = false;
            int i;

            for (i = 0; i < Characters.Count && !result; ++i)
            {
                result = Vector3.Distance(Characters[i].GetPosition(), Position) < MAX_DISTANCE;
            }

            return result;
        }

        #endregion

        private void G�rerHauteur()
        {
            //Position = Grass.GetPositionWithHeight(Position, HAUTEUR_PERSONNAGE);
        }

        private int G�rerTouche(Keys touche)
        {
            return GestionInput.EstEnfonc�e(touche) ? 1 : 0;
        }

        public void D�sactiverCam�ra()
        {
            D�sactiverD�placement = !D�sactiverD�placement;
            Direction = new Vector3(1, 0, 0);
        }

        private List<Vector3> CreerBoite()
        {
            List<Vector3> Liste = new List<Vector3>();
            Liste.Add(new Vector3(Position.X, Position.Y +0.01f, Position.Z));
            Liste.Add(new Vector3(Position.X, Position.Y - 0.01f, Position.Z));
            return Liste;
        }
    }
}
