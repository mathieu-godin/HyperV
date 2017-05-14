using System;
using Microsoft.Xna.Framework;


namespace AtelierXNA
{
    public class CubeTextur�Flottant : CubeTextur�
    {
        float Temps�coul�DepuisMAJ { get; set; }
        float IntervalleVariation { get; set; }
        float Variation { get; set; }

        public CubeTextur�Flottant(Game game, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, string nomTextureCube,
            Vector3 dimension, float intervalleMAJ) : base(game, homoth�tieInitiale, rotationInitiale, positionInitiale, nomTextureCube, dimension, intervalleMAJ)
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
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += Temps�coul�;
            if (Temps�coul�DepuisMAJ >= IntervalleVariation)
            {
                VariationCube();
                Temps�coul�DepuisMAJ = 0;
            }
        }
        private void VariationCube()
        {
            Position = new Vector3(Position.X, PositionInitiale.Y + (float)Math.Sin(Variation), Position.Z);
            Variation += 0.02f;
        }
    }
}
