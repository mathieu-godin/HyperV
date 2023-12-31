using System;
using Microsoft.Xna.Framework;


namespace AtelierXNA
{
    public class CubeTexturéFlottant : CubeTexturé
    {
        float TempsÉcouléDepuisMAJ { get; set; }
        float IntervalleVariation { get; set; }
        float Variation { get; set; }

        public CubeTexturéFlottant(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, string nomTextureCube,
            Vector3 dimension, float intervalleMAJ) : base(game, homothétieInitiale, rotationInitiale, positionInitiale, nomTextureCube, dimension, intervalleMAJ)
        {
            IntervalleVariation = intervalleMAJ;
        }
        public override void Initialize()
        {
            Lacet = true;
            Variation = 0;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            if (TempsÉcouléDepuisMAJ >= IntervalleVariation)
            {
                VariationCube();
                TempsÉcouléDepuisMAJ = 0;
            }
        }
        private void VariationCube()
        {
            Position = new Vector3(Position.X, PositionInitiale.Y + (float)Math.Sin(Variation), Position.Z);
            Variation += 0.02f;
        }
    }
}
