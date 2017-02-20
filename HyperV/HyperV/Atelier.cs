using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using AtelierXNA;

namespace HyperV
{
    public class Atelier : Microsoft.Xna.Framework.Game
    {
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        GraphicsDeviceManager P�riph�riqueGraphique { get; set; }

        Cam�raJoueur Cam�raJeu { get; set; }                
        InputManager GestionInput { get; set; }

        public Atelier()
        {
            P�riph�riqueGraphique = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            P�riph�riqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            GestionInput = new InputManager(this);
            Components.Add(GestionInput);
            Cam�raJeu = new Cam�raJoueur(this, Vector3.Zero, Vector3.One, Vector3.Up, INTERVALLE_MAJ_STANDARD);
            Components.Add(Cam�raJeu);
            Components.Add(new Afficheur3D(this));
            Components.Add(new AfficheurFPS(this, "Arial", Color.Red, INTERVALLE_CALCUL_FPS));
            Components.Add(new Jeu(this));
            Components.Add(new Skybox(this, "Texture_Skybox"));

            Services.AddService(typeof(Random), new Random());

            Services.AddService(typeof(RessourcesManager<SpriteFont>), new RessourcesManager<SpriteFont>(this, "Fonts"));
            Services.AddService(typeof(RessourcesManager<SoundEffect>), new RessourcesManager<SoundEffect>(this, "Sounds"));
            Services.AddService(typeof(RessourcesManager<Song>), new RessourcesManager<Song>(this, "Songs"));
            Services.AddService(typeof(RessourcesManager<Texture2D>), new RessourcesManager<Texture2D>(this, "Textures"));
            Services.AddService(typeof(RessourcesManager<TextureCube>), new RessourcesManager<TextureCube>(this, "Textures"));
            Services.AddService(typeof(RessourcesManager<Model>), new RessourcesManager<Model>(this, "Models"));
            Services.AddService(typeof(RessourcesManager<Effect>), new RessourcesManager<Effect>(this, "Effects"));

            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(Cam�raJoueur), Cam�raJeu);
            Services.AddService(typeof(SpriteBatch), new SpriteBatch(GraphicsDevice));
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            G�rerClavier();
            base.Update(gameTime);         
        }

        private void G�rerClavier()
        {
            if (GestionInput.EstEnfonc�e(Keys.Escape))
            {
                Exit();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }
}

