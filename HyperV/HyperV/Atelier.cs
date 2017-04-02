// By Mathieu Godin
// Created on January 2017

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
using System.IO;
using System.Diagnostics;
using AtelierXNA;

namespace HyperV
{
    enum Language
    {
        French, English, Spanish, Japanese
    }

    enum Input
    {
        Controller, Keyboard
    }

    public class Atelier : Microsoft.Xna.Framework.Game
    {
        const float INTERVALLE_CALCUL_FPS = 1f;
        float FpsInterval { get; set; }
        GraphicsDeviceManager PériphériqueGraphique { get; set; }

        Caméra Camera { get; set; }
        Maze Maze { get; set; }
        InputManager InputManager { get; set; }
        GamePadManager GamePadManager { get; set; }

        //GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch SpriteBatch { get; set; }

        RessourcesManager<SpriteFont> FontManager { get; set; }
        RessourcesManager<Texture2D> TextureManager { get; set; }
        RessourcesManager<Model> ModelManager { get; set; }
        RessourcesManager<Song> SongManager { get; set; } 
        Song Song { get; set; }
        PressSpaceLabel PressSpaceLabel { get; set; }
        //Caméra CaméraJeu { get; set; }

        public Atelier()
        {
            PériphériqueGraphique = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = false;
            //PériphériqueGraphique.PreferredBackBufferHeight = 800;
            //PériphériqueGraphique.PreferredBackBufferWidth = 1500;
            PériphériqueGraphique.PreferredBackBufferHeight = 500;
            PériphériqueGraphique.PreferredBackBufferWidth = 1000;
        }

        Gazon Gazon { get; set; }
        Grass Grass { get; set; }

        RessourcesManager<Video> VideoManager { get; set; }
        CutscenePlayer CutscenePlayer { get; set; }
        Walls Walls { get; set; }
        Character Robot { get; set; }
        List<Character> Characters { get; set; }
        int SaveNumber { get; set; }
        int Level { get; set; }
        Vector3 Position { get; set; }
        Vector3 Direction { get; set; }
        TexteCentré Loading { get; set; }
        TexteCentré GameOver { get; set; }
        TexteCentré Success { get; set; }
        TimeSpan TimePlayed { get; set; }
        Language Language { get; set; }
        int RenderDistance { get; set; }
        bool FullScreen { get; set; }
        Input Input { get; set; }
        Sprite Crosshair { get; set; }
        RessourcesManager<SoundEffect> SoundManager { get; set; }

        void LoadSettings()
        {
            //StreamReader reader = new StreamReader("F:/programmation clg/quatrième session/WPFINTERFACE/Launching Interface/Saves/Settings.txt");
            //StreamReader reader = new StreamReader("C:/Users/Mathieu/Source/Repos/WPFINTERFACE/Launching Interface/Saves/Settings.txt");
            StreamReader reader = new StreamReader("../../../WPFINTERFACE/Launching Interface/Saves/Settings.txt");
            string line = reader.ReadLine();
            string[] parts = line.Split(new string[] { ": " }, StringSplitOptions.None);
            MediaPlayer.Volume = int.Parse(parts[1]) / 100.0f;
            line = reader.ReadLine();
            parts = line.Split(new string[] { ": " }, StringSplitOptions.None);
            SoundEffect.MasterVolume = int.Parse(parts[1]) / 100.0f;
            line = reader.ReadLine();
            parts = line.Split(new string[] { ": " }, StringSplitOptions.None);
            Language = (Language)int.Parse(parts[1]);
            line = reader.ReadLine();
            parts = line.Split(new string[] { ": " }, StringSplitOptions.None);
            RenderDistance = int.Parse(parts[1]);
            if (Camera != null)
            {
                (Camera as CaméraJoueur).SetRenderDistance(RenderDistance);
            }
            line = reader.ReadLine();
            parts = line.Split(new string[] { ": " }, StringSplitOptions.None);
            FpsInterval = 1.0f / int.Parse(parts[1]);
            TargetElapsedTime = new TimeSpan((int)(FpsInterval * 10000000));
            line = reader.ReadLine();
            parts = line.Split(new string[] { ": " }, StringSplitOptions.None);
            FullScreen = int.Parse(parts[1]) == 1;
            if (FullScreen != PériphériqueGraphique.IsFullScreen)
            {
                //PériphériqueGraphique.ToggleFullScreen();
            }
            line = reader.ReadLine();
            parts = line.Split(new string[] { ": " }, StringSplitOptions.None);
            Input = (Input)int.Parse(parts[1]);
            reader.Close();
        }

        void LoadSave()
        {
            //StreamReader reader = new StreamReader("F:/programmation clg/quatrième session/WPFINTERFACE/Launching Interface/Saves/save.txt");
            //StreamReader reader = new StreamReader("C:/Users/Mathieu/Source/Repos/WPFINTERFACE/Launching Interface/Saves/save.txt");
            StreamReader reader = new StreamReader("../../../WPFINTERFACE/Launching Interface/Saves/save.txt");
            SaveNumber = int.Parse(reader.ReadLine());
            reader.Close();
            //reader = new StreamReader("F:/programmation clg/quatrième session/WPFINTERFACE/Launching Interface/Saves/save" + SaveNumber.ToString() + ".txt");
            //reader = new StreamReader("C:/Users/Mathieu/Source/Repos/WPFINTERFACE/Launching Interface/Saves/save" + SaveNumber.ToString() + ".txt");
            reader = new StreamReader("../../../WPFINTERFACE/Launching Interface/Saves/save" + SaveNumber.ToString() + ".txt");
            string line = reader.ReadLine();
            string[] parts = line.Split(new char[] { ' ' });
            Level = int.Parse(parts[1]);
            line = reader.ReadLine();
            parts = line.Split(new string[] { "n: " }, StringSplitOptions.None);
            Position = Vector3Parse(parts[1]);
            line = reader.ReadLine();
            parts = line.Split(new string[] { "n: " }, StringSplitOptions.None);
            Direction = Vector3Parse(parts[1]);
            line = reader.ReadLine();
            parts = line.Split(new string[] { "d: " }, StringSplitOptions.None);
            TimePlayed = TimeSpan.Parse(parts[1]);
            reader.Close();
            //parts = line.Split(separator);
            //int startInd = parts[1].IndexOf("X:") + 2;
            //float aXPosition = float.Parse(parts[1].Substring(startInd, parts[1].IndexOf(" Y") - startInd));
            //startInd = parts[1].IndexOf("Y:") + 2;
            //float aYPosition = float.Parse(parts[1].Substring(startInd, parts[1].IndexOf(" Z") - startInd));
            //startInd = parts[1].IndexOf("Z:") + 2;
            //float aZPosition = float.Parse(parts[1].Substring(startInd, parts[1].IndexOf("}") - startInd));
            //Position = new Vector3(aXPosition, aYPosition, aZPosition);
        }

        Vector3 Vector3Parse(string parse)
        {
            int startInd = parse.IndexOf("X:") + 2;
            float aXPosition = float.Parse(parse.Substring(startInd, parse.IndexOf(" Y") - startInd));
            startInd = parse.IndexOf("Y:") + 2;
            float aYPosition = float.Parse(parse.Substring(startInd, parse.IndexOf(" Z") - startInd));
            startInd = parse.IndexOf("Z:") + 2;
            float aZPosition = float.Parse(parse.Substring(startInd, parse.IndexOf("}") - startInd));
            return new Vector3(aXPosition, aYPosition, aZPosition);
        }

        void SelectWorld(bool usePosition)
        {
            switch (Level)
            {
                case 0:
                    Level0();
                    break;
                case 1:
                    Level1(usePosition);
                    break;
                case 2:
                    Level2(usePosition);
                    break;
                case 3:
                    Level3(usePosition);
                    break;
            }
            Save();
        }

        void Level3(bool usePosition)
        {
            Components.Add(new Afficheur3D(this));
            if (usePosition)
            {
                //Camera = new Camera2(this, Position, new Vector3(20, 0, 0), Vector3.Up, FpsInterval, RenderDistance);
                //(Camera as Camera2).InitializeDirection(Direction);
            }
            else
            {
                Camera = new Camera3(this, new Vector3(-26, 2, -3), new Vector3(20, 0, 0), Vector3.Up, FpsInterval);
            }
            Services.AddService(typeof(Caméra), Camera);
            Walls = new Walls(this, FpsInterval, "Briques", "../../../Monde3_Murs.txt");
            Components.Add(Walls);

            Components.Add(new Catapulte(this, "catapult", new Vector3(-28, -3.8f, -50), 0.03f, 0));
            AjouterModeles("../../../Monde3_Modeles.txt");
            AjouterArbres();
            AjouterTours();

            Grass = new Grass(this, 10f, Vector3.Zero, new Vector3(1000, -70, 0), new Vector2(100, 100), "Grass", FpsInterval);
            Components.Add(Grass);
            Components.Add(Camera);
            Components.Remove(Loading);
            Components.Add(FPSLabel);
            //Components.Add(new Skybox(this, "Texture_Skybox"));
        }

        private void AjouterModeles(string chemin)
        {
            StreamReader fichier = new StreamReader(chemin);
            fichier.ReadLine();
            while (!fichier.EndOfStream)
            {
                string ligneLu = fichier.ReadLine();
                string[] ligneSplit = ligneLu.Split(';');
                CreateurModele x = new CreateurModele(this, ligneSplit[0], new Vector3(int.Parse(ligneSplit[1]), int.Parse(ligneSplit[2]), int.Parse(ligneSplit[3])), int.Parse(ligneSplit[4]), int.Parse(ligneSplit[5]));
                Components.Add(new Afficheur3D(this));
                Components.Add(x);
            }
        }

        private void AjouterArbres()
        {
            Random generateur = new Random();
            const int NB_ARBRES = 150;
            for (int i = 0; i < NB_ARBRES; ++i)
            {
                Components.Add(new Afficheur3D(this));
                Components.Add(new CreateurModele(this, "Models_Tree", new Vector3(generateur.Next(-300, 300), -70, generateur.Next(-300, 300)), 10, generateur.Next(0, 360)));
            }
        }

        private void AjouterTours()
        {
            Random generateur = new Random();
            const int NB_Tours = 10;
            for (int i = 0; i < NB_Tours; ++i)
            {
                Components.Add(new Afficheur3D(this));
                CreateurModele x = new CreateurModele(this, "Models_Tower", new Vector3(generateur.Next(50, 300), -70, generateur.Next(-300, 300)), 0.05f, generateur.Next(0, 360));
                Components.Add(x);
                x.EstTour = true;
            }

        }

        void Save()
        {
            //StreamWriter writer = new StreamWriter("F:/programmation clg/quatrième session/WPFINTERFACE/Launching Interface/Saves/pendingsave.txt");
            //StreamWriter writer = new StreamWriter("C:/Users/Mathieu/Source/Repos/WPFINTERFACE/Launching Interface/Saves/pendingsave.txt");
            StreamWriter writer = new StreamWriter("../../../WPFINTERFACE/Launching Interface/Saves/pendingsave.txt");

            writer.WriteLine("Level: " + Level.ToString());
            if (Camera != null)
            {
                writer.WriteLine("Position: " + Camera.Position.ToString());
                if (Level != 3)
                {
                    writer.WriteLine("Direction: " + (Camera as CaméraJoueur).Direction.ToString());
                }
            }
            else
            {
                writer.WriteLine("Position: {X:5 Y:5 Z:5}");
                writer.WriteLine("Direction: {X:5 Y:5 Z:5}");
            }
            writer.WriteLine("Time Played: " + TimePlayed.ToString());
            writer.Close();
        }

        Boss Boss { get; set; }
        Mill Mill { get; set; }
        HeightMap HeightMap { get; set; }
        LifeBar[] LifeBars { get; set; }
        Afficheur3D Display3D { get; set; }
        Water Water { get; set; }
        Food Food { get; set; }
        Ennemy Ennemy { get; set; }

        void Level2(bool usePosition)
        {
            Components.Add(SpaceBackground);
            Display3D = new Afficheur3D(this);
            Components.Add(Display3D);
            Services.AddService(typeof(Afficheur3D), Display3D);
            if (usePosition)
            {
                Camera = new Camera2(this, Position, new Vector3(20, 0, 0), Vector3.Up, FpsInterval, RenderDistance);
                (Camera as Camera2).InitializeDirection(Direction);
                Services.AddService(typeof(LifeBar[]), LifeBars);
            }
            else
            {
                Camera = new Camera2(this, new Vector3(0, 4, 60), new Vector3(20, 0, 0), Vector3.Up, FpsInterval, RenderDistance);
            }
            //(Camera as Camera2).SetRenderDistance(RenderDistance);
            Services.AddService(typeof(Caméra), Camera);
            Maze = new Maze(this, 1f, Vector3.Zero, new Vector3(0, 0, 0), new Vector3(256, 5, 256), "GrassFence", FpsInterval, "Maze1");
            Components.Add(Maze);
            Services.AddService(typeof(Maze), Maze);
            Boss = new Boss(this, "Great Bison", 100, "Bison", "Gauge", "Dock", "Arial", FpsInterval, FpsInterval, 1, Vector3.Zero, new Vector3(300, 30, 200));
            Components.Add(Boss);
            Services.AddService(typeof(Boss), Boss);
            Mill = new Mill(this, 1, Vector3.Zero, new Vector3(300, 10, 100), new Vector2(50, 50), "Fence", FpsInterval);
            Components.Add(Mill);
            Mill.AddLabel();
            Services.AddService(typeof(Mill), Mill);
            Food = new Food(this, "Pringles", 1, Vector3.Zero, new Vector3(290, 5, 110), 10, FpsInterval);
            Components.Add(Food);
            Food.AddLabel();
            Ennemy = new Ennemy(this, "Robot", 0.05f, Vector3.Zero, new Vector3(250, 0, 110), 10, FpsInterval);
            Components.Add(Ennemy);
            //HeightMap = new HeightMap(this, 1, Vector3.Zero, Vector3.Zero, new Vector3(10000, 1000, 10000), "HeightMap", "Ceiling");
            //Components.Add(HeightMap);
            //Services.AddService(typeof(HeightMap), HeightMap);
            //Water = new Water(this, 1f, Vector3.Zero, new Vector3(10000, 300, 200), new Vector2(10000, 10000), FpsInterval);
            //Components.Add(Water);
            //Services.AddService(typeof(Water), Water);
            Boss.AddFireball();
            Boss.AddLabel();
            Components.Add(LifeBars[0]);
            Components.Add(LifeBars[1]);
            //Services.AddService(typeof(LifeBar[]), LifeBars);
            Components.Add(Camera);
            Components.Remove(Loading);
            Components.Add(Crosshair);
            Components.Add(FPSLabel);
            base.Initialize();
        }

        Grass[,] GrassArray { get; set; }
        Ceiling[,] CeilingArray { get; set; }
        ArrièrePlanSpatial SpaceBackground { get; set; }
        AfficheurFPS FPSLabel { get; set; }
        Portal[] Portals { get; set; }

        void Level1(bool usePosition)
        {
            //Song = SongManager.Find("castle");
            //MediaPlayer.Play(Song);
            Components.Add(SpaceBackground);
            Components.Add(new Afficheur3D(this));
            Services.AddService(typeof(List<Character>), Characters);
            if (usePosition)
            {
                Camera = new Camera1(this, Position, new Vector3(20, 0, 0), Vector3.Up, FpsInterval, RenderDistance);
                (Camera as Camera1).InitializeDirection(Direction);
            }
            else
            {
                Camera = new Camera1(this, new Vector3(0, -16, 60), new Vector3(20, 0, 0), Vector3.Up, FpsInterval, RenderDistance);
            }
            //(Camera as Camera1).SetRenderDistance(RenderDistance);
            Services.AddService(typeof(Caméra), Camera);
            Robot = new Character(this, "Robot", 0.02f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-50, -20, 60), "../../../CharacterScripts/Robot.txt", "FaceImages/Robot", "ScriptRectangle", "Arial", FpsInterval);
            Characters.Add(Robot);
            Grass = new Grass(this, 1f, Vector3.Zero, new Vector3(20, -20, 50), new Vector2(40, 40), "Ceiling", FpsInterval);
            Components.Add(Grass);
            Services.AddService(typeof(Grass), Grass);
            Walls = new Walls(this, FpsInterval, "Rockwall", "../../../Data.txt");
            Components.Add(Walls);
            Services.AddService(typeof(Walls), Walls);
            //Components.Add(Camera);
            GrassArray = new Grass[11, 7];
            CeilingArray = new Ceiling[11, 7];
            for (int i = 0; i < 11; ++i)
            {
                for (int j = 0; j < 7; ++j)
                {
                    GrassArray[i, j] = new Grass(this, 1f, Vector3.Zero, new Vector3(100 - i * 40, -20, -30 + j * 40), new Vector2(40, 40), "Ceiling", FpsInterval);
                    Components.Add(GrassArray[i, j]);
                }
            }
            for (int i = 0; i < 11; ++i)
            {
                for (int j = 0; j < 7; ++j)
                {
                    CeilingArray[i, j] = new Ceiling(this, 1f, Vector3.Zero, new Vector3(100 - i * 40, 0, -30 + j * 40), new Vector2(40, 40), "Ceiling", FpsInterval);
                    Components.Add(CeilingArray[i, j]);
                }
            }
            Portals = new Portal[2];
            Portals[0] = new Portal(this, 1f, Vector3.Zero, new Vector3(-345, -10, 170), new Vector2(30, 20), "Garden", FpsInterval);
            Components.Add(Portals[0]);
            Portals[1] = new Portal(this, 1f, new Vector3(0, MathHelper.ToRadians(-90), 0), new Vector3(-225, -10, -25), new Vector2(30, 20), "BleuBlancRouge", FpsInterval);
            Components.Add(Portals[1]);
            Services.AddService(typeof(Portal[]), Portals);
            Components.Add(Robot);
            Robot.AddLabel();
            Components.Add(PressSpaceLabel);
            PressSpaceLabel.Visible = false;
            Components.Add(LifeBars[0]);
            Components.Add(LifeBars[1]);
            Services.AddService(typeof(LifeBar[]), LifeBars);
            Components.Add(Camera);
            Components.Remove(Loading);
            Components.Add(Crosshair);
            Components.Add(FPSLabel);
        }

        void Level0()
        {
            CutscenePlayer = new CutscenePlayer(this, "test1", false, "Arial");
            Components.Add(CutscenePlayer);
        }

        protected override void Initialize()
        {
            Sleep = false;
            FirstGameOver = true;
            FpsInterval = 1f / 60f;
            SongManager = new RessourcesManager<Song>(this, "Songs");
            Services.AddService(typeof(RessourcesManager<Song>), SongManager);
            TextureManager = new RessourcesManager<Texture2D>(this, "Textures");
            Services.AddService(typeof(RessourcesManager<Texture2D>), TextureManager);
            ModelManager = new RessourcesManager<Model>(this, "Models");
            Services.AddService(typeof(RessourcesManager<Model>), ModelManager);
            FontManager = new RessourcesManager<SpriteFont>(this, "Fonts");
            SpaceBackground = new ArrièrePlanSpatial(this, "CielÉtoilé", FpsInterval);
            FPSLabel = new AfficheurFPS(this, "Arial", Color.Tomato, INTERVALLE_CALCUL_FPS);
            Loading = new TexteCentré(this, "Loading . . .", "Arial", new Rectangle(Window.ClientBounds.Width / 2 - 200, Window.ClientBounds.Height / 2 - 40, 400, 80), Color.White, 0);
            GameOver = new TexteCentré(this, "Game Over", "Arial", new Rectangle(Window.ClientBounds.Width / 2 - 200, Window.ClientBounds.Height / 2 - 40, 400, 80), Color.White, 0);
            Success = new TexteCentré(this, "Success!", "Arial", new Rectangle(Window.ClientBounds.Width / 2 - 200, Window.ClientBounds.Height / 2 - 40, 400, 80), Color.White, 0);
            InputManager = new InputManager(this);
            Components.Add(InputManager);
            Services.AddService(typeof(RessourcesManager<SpriteFont>), FontManager);
            Services.AddService(typeof(InputManager), InputManager);
            GamePadManager = new GamePadManager(this);
            Components.Add(GamePadManager);
            Services.AddService(typeof(GamePadManager), GamePadManager);
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), SpriteBatch);
            VideoManager = new RessourcesManager<Video>(this, "Videos");
            Services.AddService(typeof(RessourcesManager<Video>), VideoManager);
            SoundManager = new RessourcesManager<SoundEffect>(this, "Sounds");
            Services.AddService(typeof(RessourcesManager<SoundEffect>), SoundManager);
            Characters = new List<Character>();
            PressSpaceLabel = new PressSpaceLabel(this);
            LifeBars = new LifeBar[2];
            LifeBars[0] = new LifeBar(this, 300, "Gauge", "Dock", new Vector2(30, Window.ClientBounds.Height - 70), FpsInterval);
            LifeBars[1] = new LifeBar(this, 300, "StaminaGauge", "TiredGauge", "WaterGauge", "Dock", new Vector2(30, Window.ClientBounds.Height - 130), FpsInterval);
            Crosshair = new Sprite(this, "crosshair", new Vector2(Window.ClientBounds.Width / 2 - 18, Window.ClientBounds.Height / 2 - 18));
            
            LoadSave();
            LoadSettings();
            //Level = 2;
            SelectWorld(true);

            //const float ÉCHELLE_OBJET = 0.02f;
            //Vector3 positionObjet = new Vector3(-50, -20, 60);
            //Vector3 rotationObjet = new Vector3(0, MathHelper.PiOver2, 0);
            ////CaméraJeu = new CaméraFixe(this, Vector3.Zero, positionObjet, Vector3.Up);
            ////CaméraJeu = new CaméraSubjective(this, new Vector3(0, 0, 0), positionObjet, Vector3.Up, INTERVALLE_MAJ_STANDARD);
            //Components.Add(new ArrièrePlanSpatial(this, "CielÉtoilé", INTERVALLE_MAJ_STANDARD));
            ////Components.Add(new ObjetDeBase(this, "Robot", ÉCHELLE_OBJET, rotationObjet, positionObjet));
            
            //Robot = new Character(this, "Robot", ÉCHELLE_OBJET, rotationObjet, positionObjet, "../../../CharacterScripts/Robot.txt", "FaceImages/Robot", "ScriptRectangle");
            //Components.Add(Robot);
            //Characters.Add(Robot);
            //Services.AddService(typeof(List<Character>), Characters);
            ////Components.Add(new PlanTexturé(this, 1f, Vector3.Zero, new Vector3(4, 4, -5), new Vector2(20, 20), new Vector2(40, 40), "Grass", INTERVALLE_MAJ_STANDARD));
            
            //Grass grass = new Grass(this, 1f, Vector3.Zero, new Vector3(20, -20, 50), new Vector2(20, 20), "Grass", INTERVALLE_MAJ_STANDARD);
            //Components.Add(grass);
            //for (int i = 0; i < 15; ++i)
            //{
            //    for (int j = 0; j < 15; ++j)
            //    {
            //        Components.Add(new Grass(this, 1f, Vector3.Zero, new Vector3(60 - i * 20, -20, 10 + j * 20), new Vector2(20, 20), "Grass", INTERVALLE_MAJ_STANDARD));
            //    }
            //}
            //for (int i = 0; i < 15; ++i)
            //{
            //    for (int j = 0; j < 15; ++j)
            //    {
            //        Components.Add(new Ceiling(this, 1f, Vector3.Zero, new Vector3(60 - i * 20, 0, 10 + j * 20), new Vector2(20, 20), "Grass", INTERVALLE_MAJ_STANDARD));
            //    }
            //}
            //Services.AddService(typeof(RessourcesManager<TextureCube>), new RessourcesManager<TextureCube>(this, "Textures"));
            //Services.AddService(typeof(RessourcesManager<Effect>), new RessourcesManager<Effect>(this, "Effects"));
            //Maze = new Maze(this, 1f, Vector3.Zero, new Vector3(0, 0, 0), new Vector3(256, 5, 256), "GrassFence", INTERVALLE_MAJ_STANDARD, "Maze");
            //Walls = new Walls(this, INTERVALLE_MAJ_STANDARD, "Rockwall", "../../../Data.txt");
            //Components.Add(Walls);
            //Services.AddService(typeof(Walls), Walls);
            ////Components.Add(Maze);
            ////Services.AddService(typeof(Maze), Maze);
            //Services.AddService(typeof(Grass), grass);
            //Camera = new CaméraJoueur(this, new Vector3(0, -16, 60), new Vector3(20, 0, 0), Vector3.Up, INTERVALLE_MAJ_STANDARD);
            //Services.AddService(typeof(Caméra), Camera);
            //Components.Add(Camera);
            //Services.AddService(typeof(RessourcesManager<Model>), ModelManager);
            //////Components.Add(new Skybox(this, "Texture_Skybox"));

            //Components.Add(new AfficheurFPS(this, "Arial", Color.Tomato, INTERVALLE_CALCUL_FPS));
            //Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            //Services.AddService(typeof(InputManager), GestionInput);
            //GestionSprites = new SpriteBatch(GraphicsDevice);
            //Services.AddService(typeof(SpriteBatch), GestionSprites);
            //VideoManager = new RessourcesManager<Video>(this, "Videos");
            //Services.AddService(typeof(RessourcesManager<Video>), VideoManager);
            //CutscenePlayer = new CutscenePlayer(this, "test1");
            ////Components.Add(CutscenePlayer);
            base.Initialize();
        }

        float Timer { get; set; }

        protected override void Update(GameTime gameTime)
        {
            if (!Sleep)
            {
                ManageKeyboard(gameTime);
                Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                TimePlayed = TimePlayed.Add(gameTime.ElapsedGameTime);
                //Window.Title = Input.ToString();
                if (Timer >= FpsInterval)
                {
                    switch (Level)
                    {
                        case 0:
                            CheckForCutscene();
                            break;
                        case 1:
                            CheckForPortal0();
                            //CheckForPortal1();
                            //CheckForGameOver1();
                            break;
                        case 2:
                            CheckForGameOver2();
                            break;
                    }
                    Timer = 0;
                }
                //Window.Title = CaméraJeu.Position.ToString();
                base.Update(gameTime);
            }
        }

        void CheckForGameOver1()
        {
            if (LifeBars[0].Dead)
            {
                //Components.Add(Loading);
                Components.Add(GameOver);
                //++Level;
                MediaPlayer.Stop();
                Robot.RemoveLabel();
                Components.Remove(Camera);
                Services.RemoveService(typeof(Caméra));
                Components.Remove(Grass);
                Services.RemoveService(typeof(Grass));
                Components.Remove(Walls);
                Services.RemoveService(typeof(Walls));
                for (int i = 0; i < 11; ++i)
                {
                    for (int j = 0; j < 7; ++j)
                    {
                        Components.Remove(GrassArray[i, j]);
                    }
                }
                for (int i = 0; i < 11; ++i)
                {
                    for (int j = 0; j < 7; ++j)
                    {
                        Components.Remove(CeilingArray[i, j]);
                    }
                }
                Components.Remove(Portals[0]);
                Components.Remove(Portals[1]);
                Services.RemoveService(typeof(Portal[]));
                Characters.Remove(Robot);
                Services.RemoveService(typeof(List<Character>));
                Components.Remove(Robot);
                Components.Remove(SpaceBackground);
                Components.Remove(PressSpaceLabel);
                Components.Remove(LifeBars[0]);
                Components.Remove(LifeBars[1]);
                Services.RemoveService(typeof(LifeBar[]));
                Components.Remove(Crosshair);
                Components.Remove(FPSLabel);
                //SelectWorld(false);
            }
        }

        bool FirstGameOver { get; set; }

        void CheckForGameOver2()
        {
            if (LifeBars[0].Dead && FirstGameOver)
            {
                FirstGameOver = false;
                Components.Add(GameOver);
                Components.Remove(SpaceBackground);
                Components.Remove(Display3D);
                Services.RemoveService(typeof(Afficheur3D));
                Services.RemoveService(typeof(Caméra));
                Components.Remove(Maze);
                Services.RemoveService(typeof(Maze));
                Boss.RemoveFireball();
                Boss.RemoveLabel();
                Components.Remove(Boss);
                Services.RemoveService(typeof(Boss));
                Mill.RemoveLabel();
                Mill.RemoveComponents();
                Components.Remove(Mill);
                Services.RemoveService(typeof(Mill));
                Services.RemoveService(typeof(HeightMap));
                Components.Remove(LifeBars[0]);
                Components.Remove(LifeBars[1]);
                Services.RemoveService(typeof(LifeBar[]));
                Components.Remove(Camera);
                Components.Remove(Crosshair);
                Components.Remove(FPSLabel);
                LaunchPause();
            }
            else if (Boss.Dead && FirstGameOver)
            {
                FirstGameOver = false;
                Components.Add(Success);
                Components.Remove(SpaceBackground);
                Components.Remove(Display3D);
                Services.RemoveService(typeof(Afficheur3D));
                Services.RemoveService(typeof(Caméra));
                Components.Remove(Maze);
                Services.RemoveService(typeof(Maze));
                Boss.RemoveFireball();
                Boss.RemoveLabel();
                Components.Remove(Boss);
                Services.RemoveService(typeof(Boss));
                Mill.RemoveLabel();
                Mill.RemoveComponents();
                Components.Remove(Mill);
                Services.RemoveService(typeof(Mill));
                Services.RemoveService(typeof(HeightMap));
                Components.Remove(LifeBars[0]);
                Components.Remove(LifeBars[1]);
                Services.RemoveService(typeof(LifeBar[]));
                Components.Remove(Camera);
                Components.Remove(Crosshair);
                Components.Remove(FPSLabel);
            }
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            Sleep = false;
            base.OnActivated(sender, args);
            if (Camera != null)
            {
                (Camera as CaméraJoueur).EstCaméraSourisActivée = true;
            }
            IsMouseVisible = false;
            LoadSettings();
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            Sleep = true;
            base.OnDeactivated(sender, args);
            if (Camera != null)
            {
                if (Level != 3)
                {
                    (Camera as CaméraJoueur).EstCaméraSourisActivée = false;
                }
            }
            IsMouseVisible = true;
        }

        void CheckForPortal0()
        {
            float? collision = Portals[0].Collision(new Ray(Camera.Position, (Camera as Camera1).Direction));
            if (collision < 30 && collision != null)
            {
                PressSpaceLabel.Visible = true;
                if (InputManager.EstEnfoncée(Keys.Space))
                {
                    Components.Add(Loading);
                    ++Level;
                    MediaPlayer.Stop();
                    Robot.RemoveLabel();
                    Components.Remove(Camera);
                    Services.RemoveService(typeof(Caméra));
                    Components.Remove(Grass);
                    Services.RemoveService(typeof(Grass));
                    Components.Remove(Walls);
                    Services.RemoveService(typeof(Walls));
                    for (int i = 0; i < 11; ++i)
                    {
                        for (int j = 0; j < 7; ++j)
                        {
                            Components.Remove(GrassArray[i, j]);
                        }
                    }
                    for (int i = 0; i < 11; ++i)
                    {
                        for (int j = 0; j < 7; ++j)
                        {
                            Components.Remove(CeilingArray[i, j]);
                        }
                    }
                    Components.Remove(Portals[0]);
                    Components.Remove(Portals[1]);
                    Services.RemoveService(typeof(Portal));
                    Characters.Remove(Robot);
                    Services.RemoveService(typeof(List<Character>));
                    Components.Remove(Robot);
                    Components.Remove(SpaceBackground);
                    Components.Remove(PressSpaceLabel);
                    Components.Remove(LifeBars[0]);
                    Components.Remove(LifeBars[1]);
                    //Services.RemoveService(typeof(LifeBar[]));
                    Components.Remove(Crosshair);
                    Components.Remove(FPSLabel);
                    SelectWorld(false);
                }
            }
            else
            {
                PressSpaceLabel.Visible = false;
            }
        }

        void CheckForPortal1()
        {
            float? collision = Portals[1].Collision(new Ray(Camera.Position, (Camera as Camera1).Direction));
            if (collision < 30 && collision != null)
            {
                PressSpaceLabel.Visible = true;
                if (InputManager.EstEnfoncée(Keys.Space))
                {
                    Components.Add(Loading);
                    Level = 3;
                    MediaPlayer.Stop();
                    Robot.RemoveLabel();
                    Components.Remove(Camera);
                    Services.RemoveService(typeof(Caméra));
                    Components.Remove(Grass);
                    Services.RemoveService(typeof(Grass));
                    Components.Remove(Walls);
                    Services.RemoveService(typeof(Walls));
                    for (int i = 0; i < 11; ++i)
                    {
                        for (int j = 0; j < 7; ++j)
                        {
                            Components.Remove(GrassArray[i, j]);
                        }
                    }
                    for (int i = 0; i < 11; ++i)
                    {
                        for (int j = 0; j < 7; ++j)
                        {
                            Components.Remove(CeilingArray[i, j]);
                        }
                    }
                    Components.Remove(Portals[0]);
                    Components.Remove(Portals[1]);
                    Services.RemoveService(typeof(Portal));
                    Characters.Remove(Robot);
                    Services.RemoveService(typeof(List<Character>));
                    Components.Remove(Robot);
                    Components.Remove(SpaceBackground);
                    Components.Remove(PressSpaceLabel);
                    Components.Remove(LifeBars[0]);
                    Components.Remove(LifeBars[1]);
                    Services.RemoveService(typeof(LifeBar[]));
                    Components.Remove(Crosshair);
                    Components.Remove(FPSLabel);
                    SelectWorld(false);
                }
            }
            else
            {
                PressSpaceLabel.Visible = false;
            }
        }

        void CheckForCutscene()
        {
            if (CutscenePlayer.CutsceneFinished)
            {
                ++Level;
                SelectWorld(false);
                CutscenePlayer.ResetCutsceneFinished();
            }
        }

        public void AddLoading()
        {
            Components.Add(Loading);
        }

        bool Sleep { get; set; }

        void ManageKeyboard(GameTime gameTime)
        {
            if (InputManager.EstNouvelleTouche(Keys.Escape))
            {
                LaunchPause();
            }
        }

        void LaunchPause()
        {
            Sleep = true;
            Save();
            TakeAScreenshot();
            //string path = "F:/programmation clg/quatrième session/WPFINTERFACE/Launching Interface/bin/Debug/Launching Interface.exe";
            //string path = "C:/Users/Mathieu/Source/Repos/WPFINTERFACE/Launching Interface/bin/Debug/Launching Interface.exe";
            string path = Path.Combine(Environment.CurrentDirectory, @"..\..\..\WPFINTERFACE\Launching Interface\bin\Debug\Launching Interface.exe");
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = path;
            p.WorkingDirectory = System.IO.Path.GetDirectoryName(path);
            Process.Start(p);
            //Process.Start(@"WPFINTERFACE\Launching Interface\bin\Debug\Launching Interface.exe");
            //Process.Start(Path.Combine(Environment.CurrentDirectory, @"WPFINTERFACE\Launching Interface\bin\Debug\Launching Interface.exe"));

            //(Camera as CaméraJoueur).EstCaméraSourisActivée = false;
            //Exit();
        }

        Texture2D Screenshot { get; set; }

        void TakeAScreenshot() 
        {
            int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int h = GraphicsDevice.PresentationParameters.BackBufferHeight;
            Draw(new GameTime());
            int[] backBuffer = new int[w * h];
            GraphicsDevice.GetBackBufferData(backBuffer);
            Screenshot = new Texture2D(GraphicsDevice, w, h, false, GraphicsDevice.PresentationParameters.BackBufferFormat);
            Screenshot.SetData(backBuffer);
            Stream stream;
            while (true)
            {
                try
                {
                    //stream = File.OpenWrite("F:/programmation clg/quatrième session/WPFINTERFACE/Launching Interface/Saves/pendingscreenshot.png");
                    //stream = File.OpenWrite("C:/Users/Mathieu/Source/Repos/WPFINTERFACE/Launching Interface/Saves/pendingscreenshot.png");
                    stream = File.OpenWrite("../../../WPFINTERFACE/Launching Interface/Saves/pendingscreenshot.png");
                }
                catch (IOException e)
                {
                    continue;
                }
                break;
            }
            Screenshot.SaveAsPng(stream, w, h);
            stream.Dispose();
            stream.Close();
            Screenshot.Dispose();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }
}



