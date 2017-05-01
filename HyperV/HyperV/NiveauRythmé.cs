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
        const int NB_�_R�USSIR = 5;

        //Constructeur
        readonly string NomFichierLecture;
        readonly string NomTexture;
        readonly float IntervalleMAJ;

        //Initialize
        bool BoutonUn { get; set; }
        bool BoutonDeux { get; set; }
        bool BoutonTrois { get; set; }
        bool NiveauEstTermin� { get; set; }

        float Temps�coul�DepuisMAJ { get; set; }
        List<Vector3> Positions { get; set; }
        int cpt { get; set; }
        int nombreR�ussi { get; set; }
        public Vector3? PositionCubeRouge { get; set; }
        AfficheurPointage Pointage { get; set; }

        //ChargerContenu
        Random G�n�rateurAl�atoire { get; set; }
        InputManager GestionInput { get; set; }
        GamePadManager GestionGamePad { get; set; }
        List<UnlockableWall> Mur�Enlever { get; set; }
        List<Portal> ListePortails { get; set; }

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
            NiveauEstTermin� = false;
            PositionCubeRouge = null;
            nombreR�ussi = 0;
            cpt = 0;
            Temps�coul�DepuisMAJ = 0;

            Positions = new List<Vector3>();
            InitialiserPositions();
            Pointage = new AfficheurPointage(Game, "Arial50", Color.Black, IntervalleMAJ);
            ChargerContenu();
            InitialisationComposants();
            
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
            G�n�rateurAl�atoire = Game.Services.GetService(typeof(Random)) as Random;
            Mur�Enlever = Game.Services.GetService(typeof(List<UnlockableWall>)) as List<UnlockableWall>;
            ListePortails = Game.Services.GetService(typeof(List<Portal>)) as List<Portal>;
        }

        void InitialisationComposants()
        {
          
            Game.Components.Add(Pointage);
            Game.Components.Add(new Afficheur3D(Game));

            for(int i = 0; i < Positions.Count; i += 2)
            {
                Game.Components.Add(new CylindreTextur�(Game, 1, new Vector3(0, 0, 0),
                                    Vector3.Zero, new Vector2(1, 1), new Vector2(20, 20),
                                    "Fil Electrique", IntervalleMAJ, Positions[i],
                                    Positions[i+1]));

                Game.Components.Add(new CubeTextur�(Game, 1, Vector3.Zero, Positions[i+1],
                                    "Blanc", new Vector3(3, 3, 3), IntervalleMAJ));

                Game.Components.Add(new TuileTextur�e(Game, 1, new Vector3(0, -MathHelper.PiOver2, 0),
                                    Positions[i+1] - 1.65f * Vector3.UnitX, new Vector2(3, 3), (i/2+1).ToString()));
            }
        }

        public override void Update(GameTime gameTime)
        {
            RegarderTouches();

            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                if (!NiveauEstTermin�)
                {
                    EffectuerMAJ();
                }
                Temps�coul�DepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        void RegarderTouches()
        {
            BoutonUn = GestionInput.EstNouvelleTouche(Keys.NumPad1)|| 
                      GestionInput.EstNouvelleTouche(Keys.D1) ||
                      GestionGamePad.EstEnfonc�(Buttons.DPadLeft) || BoutonUn;
            BoutonDeux = GestionInput.EstNouvelleTouche(Keys.NumPad2) || 
                        GestionInput.EstNouvelleTouche(Keys.D2) ||
                      GestionGamePad.EstEnfonc�(Buttons.DPadDown) || BoutonDeux;
            BoutonTrois = GestionInput.EstNouvelleTouche(Keys.NumPad3) || 
                        GestionInput.EstNouvelleTouche(Keys.D3) ||
                      GestionGamePad.EstEnfonc�(Buttons.DPadRight) || BoutonTrois;
        }

        void EffectuerMAJ()
        {
            cpt++;

            foreach (CubeTextur� cube in Game.Components.Where(composant => composant is CubeTextur�))
            {
                if (SontVecteurs�gaux(PositionCubeRouge, cube.Position))
                {
                    cube.NomTextureCube = "Rouge";
                    cube.InitialiserParam�tresEffetDeBase();
                    PositionCubeRouge = null;
                }

                foreach (Sph�reRythm�e sp in Game.Components.Where(composant => composant is Sph�reRythm�e))
                {
                    if (sp.EstEnCollision(cube))
                    {
                        if (SontVecteurs�gaux(sp.Extr�mit�1, Positions[0]) && BoutonUn ||
                        SontVecteurs�gaux(sp.Extr�mit�1, Positions[2]) && BoutonDeux ||
                        SontVecteurs�gaux(sp.Extr�mit�1, Positions[4]) && BoutonTrois)
                        {
                            sp.�D�truire = true;
                            cube.NomTextureCube = "Vert";
                            cube.InitialiserParam�tresEffetDeBase();
                            ++nombreR�ussi;
                        }
                    }
                }
            }

            Pointage.Cha�ne = nombreR�ussi.ToString() + "/" + NB_�_R�USSIR.ToString();

            if(nombreR�ussi >= NB_�_R�USSIR)
            {
                NiveauEstTermin� = true;
                cpt = 121;
                Game.Components.Remove(Mur�Enlever[0]);
                ListePortails.Add(new Portal(Game, 1, new Vector3(0, 1.570796f, 0),
                                  new Vector3(170, -60, -10), new Vector2(40, 40), "Transparent",
                                  1, IntervalleMAJ));
                Game.Components.Add(ListePortails.Last());
            }

            if (cpt > 120 )
            {
                if (!NiveauEstTermin�)
                {
                    //int nbreBalles = G�n�rateurAl�atoire.Next(1, 4);
                    //for(int i = 0; i < nbreBalles; i++)
                    //{
                        int choixPente = G�n�rateurAl�atoire.Next(0, 3) * 2;
                        Game.Components.Add(new Afficheur3D(Game));
                        Game.Components.Add(new Sph�reRythm�e(Game, 1, Vector3.Zero,
                                            Positions[choixPente], 1, new Vector2(20, 20),
                                            "BleuBlancRouge", IntervalleMAJ, Positions[choixPente + 1]));
                    //}
                }

                cpt = 0;

                foreach (CubeTextur� cube in Game.Components.Where(composant => composant is CubeTextur�))
                {
                    cube.NomTextureCube = "Blanc";
                    cube.InitialiserParam�tresEffetDeBase();
                }
            }

            BoutonUn = false;
            BoutonDeux = false;
            BoutonTrois = false;
        }

        bool SontVecteurs�gaux(Vector3? a, Vector3 b)
        {
            bool �gaux;

            if(a == null)
            {
                �gaux = false;
            }
            else
            {
                Vector3 c = (Vector3)a - b;
                �gaux = (c.X < 1 && c.X > -1) && (c.Y < 1 && c.Y > -1) && (c.Z < 1 && c.Z > -1);
            }

            return �gaux;
        }
    }
}
