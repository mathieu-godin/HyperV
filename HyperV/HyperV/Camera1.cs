using AtelierXNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace HyperV
{
    public class Camera1 : Cam�ra
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

        public Vector3 Direction { get; private set; }
        Vector3 Lat�ral { get; set; }
        Grass Grass { get; set; }
        Walls Walls { get; set; }
        float VitesseTranslation { get; set; }
        float VitesseRotation { get; set; }
        Point AnciennePositionSouris { get; set; }
        Point NouvellePositionSouris { get; set; }
        Vector2 D�placementSouris { get; set; }

        float IntervalleMAJ { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }
        InputManager GestionInput { get; set; }
        float Height { get; set; }
        List<Character> Characters { get; set; }

        public Camera1(Game jeu, Vector3 positionCam�ra, Vector3 cible, Vector3 orientation, float intervalleMAJ) : base(jeu)
        {
            IntervalleMAJ = intervalleMAJ;
            Cr�erVolumeDeVisualisation(OUVERTURE_OBJECTIF, DISTANCE_PLAN_RAPPROCH�, DISTANCE_PLAN_�LOIGN�);
            Cr�erPointDeVue(positionCam�ra, cible, orientation);
            Height = positionCam�ra.Y;
        }

        public override void Initialize()
        {
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
                FonctionsSouris();
                FonctionsClavier();

                G�rerHauteur();
                Cr�erPointDeVue();




                Game.Window.Title = Position.ToString();
                Position = new Vector3(Position.X, Height, Position.Z);
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        //Souris
        #region
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
            G�rerTangageSouris();
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

        //Clavier
        #region
        private void FonctionsClavier()
        {
            G�rerD�placement();
            G�rerRotationClavier();
        }

        private void G�rerD�placement()
        {
            float d�placementDirection = (G�rerTouche(Keys.W) - G�rerTouche(Keys.S)) * VitesseTranslation;
            float d�placementLat�ral = (G�rerTouche(Keys.A) - G�rerTouche(Keys.D)) * VitesseTranslation;

            Direction = Vector3.Normalize(Direction);
            Lat�ral = Vector3.Cross(Direction, OrientationVerticale);
            Position += d�placementDirection * Direction;
            Position -= d�placementLat�ral * Lat�ral;
            if (Walls.CheckForCollisions(Position) || CheckForCharacterCollision())
            {
                Position -= d�placementDirection * Direction;
                Position += d�placementLat�ral * Lat�ral;
            }
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

        private void G�rerRotationClavier()
        {
            G�rerLacetClavier();
            G�rerTangageClavier();
        }

        private void G�rerLacetClavier()
        {
            Matrix matriceLacet = Matrix.Identity;

            if (GestionInput.EstEnfonc�e(Keys.Left))
            {
                matriceLacet = Matrix.CreateFromAxisAngle(OrientationVerticale, DELTA_LACET * VITESSE_INITIALE_ROTATION);
            }
            if (GestionInput.EstEnfonc�e(Keys.Right))
            {
                matriceLacet = Matrix.CreateFromAxisAngle(OrientationVerticale, -DELTA_LACET * VITESSE_INITIALE_ROTATION);
            }

            Direction = Vector3.Transform(Direction, matriceLacet);
        }

        private void G�rerTangageClavier()
        {
            Matrix matriceTangage = Matrix.Identity;

            if (GestionInput.EstEnfonc�e(Keys.Down))
            {
                matriceTangage = Matrix.CreateFromAxisAngle(Lat�ral, -DELTA_TANGAGE * VITESSE_INITIALE_ROTATION);
            }
            if (GestionInput.EstEnfonc�e(Keys.Up))
            {
                matriceTangage = Matrix.CreateFromAxisAngle(Lat�ral, DELTA_TANGAGE * VITESSE_INITIALE_ROTATION);
            }

            Direction = Vector3.Transform(Direction, matriceTangage);
        }
        #endregion

        private void G�rerHauteur()
        {
            Position = Grass.GetPositionWithHeight(Position, HAUTEUR_PERSONNAGE);
        }

        private int G�rerTouche(Keys touche)
        {
            return GestionInput.EstEnfonc�e(touche) ? 1 : 0;
        }
    }
}
