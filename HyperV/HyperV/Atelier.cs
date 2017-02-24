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
    public class Atelier : Microsoft.Xna.Framework.Game
    {
        const float INTERVALLE_CALCUL_FPS = 1f;
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        GraphicsDeviceManager P�riph�riqueGraphique { get; set; }

        Cam�raSubjective Cam�raJeu { get; set; }                
        InputManager GestionInput { get; set; }

        //GraphicsDeviceManager P�riph�riqueGraphique { get; set; }
        SpriteBatch GestionSprites { get; set; }

        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeMod�les { get; set; }
        //Cam�ra Cam�raJeu { get; set; }

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
            const float �CHELLE_OBJET = 0.01f;
            Vector3 positionObjet = new Vector3(0, -10, -50);
            Vector3 rotationObjet = new Vector3(0, MathHelper.PiOver2, 0);
            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeMod�les = new RessourcesManager<Model>(this, "Models");
            //Cam�raJeu = new Cam�raFixe(this, Vector3.Zero, positionObjet, Vector3.Up);
            Cam�raJeu = new Cam�raSubjective(this, Vector3.Zero, positionObjet, Vector3.Up, INTERVALLE_MAJ_STANDARD);
            GestionInput = new InputManager(this);
            Components.Add(GestionInput);
            //Components.Add(new Arri�rePlanSpatial(this, "Ciel�toil�", INTERVALLE_MAJ_STANDARD));
            Components.Add(Cam�raJeu);
            Components.Add(new Afficheur3D(this));
            Components.Add(new ObjetDeBase(this, "ship", �CHELLE_OBJET, rotationObjet, positionObjet));
            //Components.Add(new PlanTextur�(this, 1f, Vector3.Zero, new Vector3(4, 4, -5), new Vector2(20, 20), new Vector2(40, 40), "Grass", INTERVALLE_MAJ_STANDARD));
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Grass gazon = new Grass(this, 1f, Vector3.Zero, new Vector3(0, 0, 0), new Vector2(256, 256), "Grass", INTERVALLE_MAJ_STANDARD);
            //Components.Add(gazon);
            Components.Add(new Maze(this, 1f, Vector3.Zero, new Vector3(0, 0, 0), new Vector3(256, 5, 256), "Grass", INTERVALLE_MAJ_STANDARD, "test1"));
            Services.AddService(typeof(Grass), gazon);
            Components.Add(new AfficheurFPS(this, "Arial", Color.Tomato, INTERVALLE_CALCUL_FPS));
            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeMod�les);
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(Cam�ra), Cam�raJeu);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), GestionSprites);
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
            GraphicsDevice.Clear(Color.Orchid);
            base.Draw(gameTime);
        }
    }
}

