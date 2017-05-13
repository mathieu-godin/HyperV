using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AtelierXNA;

namespace HyperV
{
    //void RythmLevel()
    //{
    //    NiveauRythm� circuit = new NiveauRythm�(this, "Fil Electrique", "../../../Data3.txt",
    //                                            3, "Blanc", "Rouge",
    //                                            "Vert", "BleuBlancRouge", "Arial50",
    //                                            Color.Black, 15, 1,
    //                                            FpsInterval);
    //    Components.Add(circuit);
    //    Services.AddService(typeof(NiveauRythm�), circuit);
    //}


    //Faire enlever de la vie si pese touche pas colision
    public class NiveauRythm� : Microsoft.Xna.Framework.GameComponent
    {
        //CONSTRUCTEUR
        //Cylindre
        readonly string TextureCylindre,
                        NomFichierLecturePositionsCylindre;
        //Cube
        readonly float LongueurAr�teCube;
        readonly string TextureCubeBase,
                        TextureCube�chec,
                        TextureCubeR�ussite;
        //Sph�reRythm�e
        readonly string TextureSph�reRythm�e;
        //Pointage
        readonly string NomPolicePointage;
        readonly Color CouleurPointage;
        readonly int NbreBalle�R�ussir,
                     Difficult�e;

        readonly float IntervalleMAJ;


        //Initialize
        bool BoutonUn { get; set; }
        bool BoutonDeux { get; set; }
        bool BoutonTrois { get; set; }
        bool NiveauEstTermin� { get; set; }

        float Temps�coul�DepuisMAJ { get; set; }
        List<Vector3> Positions { get; set; }
        int i { get; set; }
        int j { get; set; }
        int nombreR�ussi { get; set; }
        public Vector3? PositionCubeRouge { get; set; }
        AfficheurPointage Pointage { get; set; }
        int BorneMaximale_i { get; set; }
        int BorneMaximale_j { get; set; }

        //ChargerContenu
        Random G�n�rateurAl�atoire { get; set; }
        InputManager GestionInput { get; set; }
        GamePadManager GestionGamePad { get; set; }
        List<UnlockableWall> Mur�Enlever { get; set; }
        List<Portal> ListePortails { get; set; }


        public NiveauRythm�(Game jeu, string textureCylindre, string nomFichierLecturePositionsCylindre,
                            float longueurAr�teCube, string textureCubeBase, string textureCube�chec,
                            string textureCubeR�ussite, string textureSph�reRythm�e, string nomPolicePointage,
                            Color couleurPointage, int nbreBalle�R�ussir, int difficult�e,
                            float intervalleMAJ)
            : base(jeu)
        {
            TextureCylindre = textureCylindre;
            NomFichierLecturePositionsCylindre = nomFichierLecturePositionsCylindre;

            LongueurAr�teCube = longueurAr�teCube;
            TextureCubeBase = textureCubeBase;
            TextureCube�chec = textureCube�chec;
            TextureCubeR�ussite = textureCubeR�ussite;

            TextureSph�reRythm�e = textureSph�reRythm�e;

            NomPolicePointage = nomPolicePointage;
            CouleurPointage = couleurPointage;
            NbreBalle�R�ussir = nbreBalle�R�ussir;
            Difficult�e = difficult�e;

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
            i = 0;
            j = 0;
            BorneMaximale_i = 120;
            BorneMaximale_j = 20;  // CONSTANTES ________________________________________________________
            Temps�coul�DepuisMAJ = 0;

            Positions = new List<Vector3>();
            InitialiserPositions();
            Pointage = new AfficheurPointage(Game, NomPolicePointage, CouleurPointage, IntervalleMAJ);
            ChargerContenu();
            InitialisationComposants();

        }

        void InitialiserPositions()
        {
            string ligneLue;
            int indicateurDebut;
            float composanteX, composanteY, composanteZ;

            StreamReader lecteurFichier = new StreamReader(NomFichierLecturePositionsCylindre);

            while (!lecteurFichier.EndOfStream)
            {
                ligneLue = lecteurFichier.ReadLine();
                // Faire fonction une ligne
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

            for (int i = 0; i < Positions.Count; i += 2)
            {
                // constantes ---------------------------------------------------------------------

                Game.Components.Add(new CylindreTextur�(Game, 1, Vector3.Zero,
                                    Vector3.Zero, new Vector2(1, 1), new Vector2(20, 20),
                                    TextureCylindre, IntervalleMAJ, Positions[i],
                                    Positions[i + 1]));

                Game.Components.Add(new CubeTextur�(Game, 1, Vector3.Zero, Positions[i + 1],
                                    TextureCubeBase, new Vector3(LongueurAr�teCube, LongueurAr�teCube, LongueurAr�teCube), IntervalleMAJ));

                Game.Components.Add(new TuileTextur�e(Game, 1, new Vector3(0, -MathHelper.PiOver2, 0),
                                    Positions[i + 1] - 1.65f * Vector3.UnitX, new Vector2(LongueurAr�teCube, LongueurAr�teCube), (i / 2 + 1).ToString()));
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
            BoutonUn = GestionInput.EstNouvelleTouche(Keys.NumPad1) ||
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
            i++;
            j++;

            foreach (CubeTextur� cube in Game.Components.Where(composant => composant is CubeTextur�))
            {
                RemettreCubesTextureInitiale(cube);
                G�rer�chec(cube);

                foreach (Sph�reRythm�e sp in Game.Components.Where(composant => composant is Sph�reRythm�e))
                {
                    if (sp.EstEnCollision(cube))
                    {
                        G�rerR�ussite(sp, cube);
                    }
                }
            }

            G�rerPointage();
            AjouterSph�res();

            BoutonUn = false;
            BoutonDeux = false;
            BoutonTrois = false;
        }

        void G�rerPointage()
        {
            Pointage.Cha�ne = nombreR�ussi.ToString() + "/" + NbreBalle�R�ussir.ToString();

            if (nombreR�ussi >= NbreBalle�R�ussir)
            {

                // constantes  ----------------------------------

                NiveauEstTermin� = true;
                i = 1000;
                Game.Components.Remove(Mur�Enlever[0]);
                ListePortails.Add(new Portal(Game, 1, new Vector3(0, MathHelper.PiOver2, 0),
                                  new Vector3(170, -60, -10), new Vector2(40, 40), "Transparent",
                                  1, IntervalleMAJ));
                Game.Components.Add(ListePortails.Last());
            }
        }

        void AjouterSph�res()
        {
            if (i > BorneMaximale_i)
            {
                // constantes ---------------------------------------------


                if (!NiveauEstTermin�)
                {
                    BorneMaximale_i = G�n�rateurAl�atoire.Next(30 / Difficult�e, 90 / Difficult�e);

                    int choixPente = G�n�rateurAl�atoire.Next(0, 3) * 2;
                    Game.Components.Add(new Afficheur3D(Game));
                    Game.Components.Add(new Sph�reRythm�e(Game, 1, Vector3.Zero,
                                        Positions[choixPente], 1, new Vector2(20, 20),
                                        TextureSph�reRythm�e, IntervalleMAJ, Positions[choixPente + 1]));

                }
                i = 0;
            }
        }

        void RemettreCubesTextureInitiale(CubeTextur� cube)
        {
            if (j > BorneMaximale_j / Difficult�e || NiveauEstTermin�)
            {
                cube.NomTextureCube = TextureCubeBase;
                cube.InitialiserParam�tresEffetDeBase();

                //j = 0;
            }
        }

        void G�rer�chec(CubeTextur� cube)
        {
            if (SontVecteurs�gaux(PositionCubeRouge, cube.Position))
            {
                cube.NomTextureCube = TextureCube�chec;
                cube.InitialiserParam�tresEffetDeBase();
                PositionCubeRouge = null;
                j = 0;
            }
        }

        void G�rerR�ussite(Sph�reRythm�e sp, CubeTextur� cube)
        {
            if (SontVecteurs�gaux(sp.Extr�mit�1, Positions[0]) && BoutonUn ||
                                    SontVecteurs�gaux(sp.Extr�mit�1, Positions[2]) && BoutonDeux ||
                                    SontVecteurs�gaux(sp.Extr�mit�1, Positions[4]) && BoutonTrois)
            {
                sp.�D�truire = true;
                cube.NomTextureCube = TextureCubeR�ussite;
                cube.InitialiserParam�tresEffetDeBase();
                ++nombreR�ussi;
                j = 0;
            }
        }

        bool SontVecteurs�gaux(Vector3? a, Vector3 b)
        {
            bool �gaux;

            if (a == null)
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
