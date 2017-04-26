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
    public class AfficheurPointage : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //Constantes
        const int MARGE_BAS = 200;
        //const int MARGE_DROITE = 700;
        const float AUCUNE_ROTATION = 0f;
        const float AUCUNE_HOMOTH�TIE = 1f;
        const float AVANT_PLAN = 0f;

        //Constructeur
        readonly string NomFont;
        readonly Color CouleurFPS;
        readonly float IntervalleMAJ;

        //Initialize
        float Temps�coul�DepuisMAJ { get; set; }
        public string Cha�ne { get; set; }
        Vector2 PositionGaucheCentre { get; set; }
        Vector2 PositionCha�ne { get; set; }

        //LoadContent
        SpriteBatch GestionSprites { get; set; }
        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        SpriteFont PoliceDeCaract�res { get; set; }
        

        public AfficheurPointage(Game game, string nomFont, Color couleurFPS, float intervalleMAJ)
           : base(game)
        {
            NomFont = nomFont;
            CouleurFPS = couleurFPS;
            IntervalleMAJ = intervalleMAJ;
        }

        public override void Initialize()
        {
            DrawOrder = 1000;
            Temps�coul�DepuisMAJ = 0;
            Cha�ne = "";
            PositionGaucheCentre = new Vector2(30,
                                            Game.Window.ClientBounds.Height - MARGE_BAS);
            PositionCha�ne = PositionGaucheCentre;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionSprites = new SpriteBatch(Game.GraphicsDevice);
            GestionnaireDeFonts = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            PoliceDeCaract�res = GestionnaireDeFonts.Find(NomFont);
        }

        public override void Update(GameTime gameTime)
        {
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                EffectuerMAJ();
                Temps�coul�DepuisMAJ = 0;
            }
        }

        void EffectuerMAJ()
        {
            //Vector2 dimension = PoliceDeCaract�res.MeasureString(Cha�neFPS);
            //PositionCha�ne = PositionGaucheCentre - dimension;
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.DrawString(PoliceDeCaract�res, Cha�ne, PositionCha�ne, CouleurFPS, AUCUNE_ROTATION,
                                      Vector2.Zero, AUCUNE_HOMOTH�TIE, SpriteEffects.None, AVANT_PLAN);
            GestionSprites.End();
        }
    }
}

