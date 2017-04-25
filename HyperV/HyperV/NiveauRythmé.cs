using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
    public class NiveauRythm� : Microsoft.Xna.Framework.GameComponent
    {
        //Constructeur
        readonly string NomFichierLecture;
        readonly string NomTexture;
        readonly float IntervalleMAJ;

        float Temps�coul�DepuisMAJ { get; set; }
        List<Vector3> Positions { get; set; }

        bool BoutonUn { get; set; }
        bool BoutonDeux { get; set; }
        bool BoutonTrois { get; set; }

        int i { get; set; }
        Random G�n�rateurAl�atoire { get; set; }

        InputManager GestionInput { get; set; }
        GamePadManager GestionGamePad { get; set; }

        public Vector3? PositionCubeRouge { get; set; }

        public NiveauRythm�(Game jeu, string nomFichierLecture, string nomTexture, float intervalleMAJ)
            : base(jeu)
        {
            NomFichierLecture = nomFichierLecture;
            NomTexture = nomTexture;
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            base.Initialize();

            BoutonUn = false;
            BoutonDeux = false;
            BoutonTrois = false;

            PositionCubeRouge = null;

            G�n�rateurAl�atoire = new Random();
            i = 0;
            Temps�coul�DepuisMAJ = 0;
            Positions = new List<Vector3>();
            InitialiserPositions();

            TestInitialisation();
            ChargerContenu();
        }

        void InitialiserPositions()
        {
            string ligneLue;
            int indicateurDebut;
            float composanteX, composanteY, composanteZ;

            StreamReader lecteurFichier = new StreamReader(NomFichierLecture);

            while (!lecteurFichier.EndOfStream)
            {
                ligneLue = lecteurFichier.ReadLine();

                indicateurDebut = ligneLue.IndexOf("X:") + 2;
                composanteX = float.Parse(ligneLue.Substring(indicateurDebut, ligneLue.IndexOf(" Y") - indicateurDebut));

                indicateurDebut = ligneLue.IndexOf("Y:") + 2;
                composanteY = float.Parse(ligneLue.Substring(indicateurDebut, ligneLue.IndexOf(" Z") - indicateurDebut));

                indicateurDebut = ligneLue.IndexOf("Z:") + 2;
                composanteZ = float.Parse(ligneLue.Substring(indicateurDebut, ligneLue.IndexOf("}") - indicateurDebut));

                Positions.Add(new Vector3(composanteX, composanteY, composanteZ));
            }
            lecteurFichier.Close();
        }

        protected virtual void ChargerContenu()
        {
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionGamePad = Game.Services.GetService(typeof(GamePadManager)) as GamePadManager;
        }

        void TestInitialisation()
        {
            Game.Components.Add(new CylindreTextur�(Game, 1, new Vector3(0, 0, 0), 
                                Vector3.Zero, new Vector2(1, 1), new Vector2(20, 20), 
                                "Fil Electrique", IntervalleMAJ, Positions[0], 
                                Positions[1]));

            Game.Components.Add(new CylindreTextur�(Game, 1, new Vector3(0, 0, 0),
                                Vector3.Zero, new Vector2(1, 1), new Vector2(20, 20),
                                "Fil Electrique", IntervalleMAJ, Positions[2],
                                Positions[3]));

            Game.Components.Add(new CylindreTextur�(Game, 1, new Vector3(0, 0, 0),
                                Vector3.Zero, new Vector2(1, 1), new Vector2(20, 20),
                                "Fil Electrique", IntervalleMAJ, Positions[4],
                                Positions[5]));

           Game.Components.Add(new CubeTextur�(Game, 1, Vector3.Zero, Positions[1],
                              "Blanc", new Vector3(3, 3, 3), IntervalleMAJ));

            Game.Components.Add(new CubeTextur�(Game, 1, Vector3.Zero, Positions[3],
                              "Blanc", new Vector3(3, 3, 3), IntervalleMAJ));

            Game.Components.Add(new CubeTextur�(Game, 1, Vector3.Zero, Positions[5],
                              "Blanc", new Vector3(3, 3, 3), IntervalleMAJ));

            Game.Components.Add(new TuileTextur�e(Game, 1, new Vector3(0, -MathHelper.PiOver2, 0), Positions[1] - 1.65f * Vector3.UnitX, 
                                new Vector2(3, 3), "1"));
            Game.Components.Add(new TuileTextur�e(Game, 1, new Vector3(0, -MathHelper.PiOver2, 0), Positions[3] - 1.65f * Vector3.UnitX,
                    new Vector2(3, 3), "2"));
            Game.Components.Add(new TuileTextur�e(Game, 1, new Vector3(0, -MathHelper.PiOver2, 0), Positions[5] - 1.65f * Vector3.UnitX,
                    new Vector2(3, 3), "3"));
        }

        public override void Update(GameTime gameTime)
        {
            RegarderTouches();

            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                EffectuerMAJ();
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        void RegarderTouches()
        {
            BoutonUn = GestionInput.EstNouvelleTouche(Keys.NumPad1) || BoutonUn;
            BoutonDeux = GestionInput.EstNouvelleTouche(Keys.NumPad2) || BoutonDeux;
            BoutonTrois = GestionInput.EstNouvelleTouche(Keys.NumPad3) || BoutonTrois;
        }

        void EffectuerMAJ()
        {
            i++;

            foreach (CubeTextur� cube in Game.Components.Where(composant => composant is CubeTextur�))
            {
                if (SontVecteurs�gaux(PositionCubeRouge, cube.Position))
                {
                    cube.NomTextureCube = "Rouge";
                    cube.InitialiserParam�tresEffetDeBase();
                    PositionCubeRouge = null;
                }
                if (SontVecteurs�gaux(PositionCubeRouge, cube.Position))
                {
                    cube.NomTextureCube = "Rouge";
                    cube.InitialiserParam�tresEffetDeBase();
                    PositionCubeRouge = null;

                }
                if (SontVecteurs�gaux(PositionCubeRouge, cube.Position))
                {
                    cube.NomTextureCube = "Rouge";
                    cube.InitialiserParam�tresEffetDeBase();
                    PositionCubeRouge = null;

                }
            }

            if (i > 120)
            {
                int choixPente = G�n�rateurAl�atoire.Next(0, 3) * 2;
                //Game.Components.Add(new Afficheur3D(Game));
                Game.Components.Add(new Sph�reRythm�e(Game, 1, Vector3.Zero,
                                    Positions[choixPente], 1, new Vector2(20, 20),
                                    "BleuBlancRouge", IntervalleMAJ, Positions[choixPente + 1]));
                i = 0;

                foreach (CubeTextur� cube in Game.Components.Where(composant => composant is CubeTextur�))
                {
                    cube.NomTextureCube = "Blanc";
                    cube.InitialiserParam�tresEffetDeBase();
                }
            }

            foreach (CubeTextur� cube in Game.Components.Where(composant => composant is CubeTextur�))
            {
                foreach (Sph�reRythm�e sp in Game.Components.Where(composant => composant is Sph�reRythm�e))
                {
                    if (sp.EstEnCollision(cube))
                    {
                        if(SontVecteurs�gaux(sp.Extr�mit�1, Positions[0]) && BoutonUn)
                        {
                           sp.�D�truire = true;
                            cube.NomTextureCube = "Vert";
                            cube.InitialiserParam�tresEffetDeBase();
                        }
                        if (SontVecteurs�gaux(sp.Extr�mit�1, Positions[2]) && BoutonDeux)
                        {
                            sp.�D�truire = true;
                            cube.NomTextureCube = "Vert";
                            cube.InitialiserParam�tresEffetDeBase();
                        }
                        if (SontVecteurs�gaux(sp.Extr�mit�1, Positions[4]) && BoutonTrois)
                        {
                            sp.�D�truire = true;
                            cube.NomTextureCube = "Vert";
                            cube.InitialiserParam�tresEffetDeBase();
                        }

                    }
                }
            }

            BoutonUn = false;
            BoutonDeux = false;
            BoutonTrois = false;
        }

        bool SontVecteurs�gaux(Vector3? a, Vector3 b)
        {
            if(a == null)
            {
                return false;
            }

            Vector3 c = (Vector3)a - b;

            return (c.X < 1 && c.X > -1) && (c.Y < 1 && c.Y > -1) && (c.Z < 1 && c.Z > -1);
        }
    }
}
