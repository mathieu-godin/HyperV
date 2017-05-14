using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AtelierXNA
{
    public class Piste : PrimitiveDeBaseAnim�e
   {
      Vector3[] PtsSommets { get; set; }
      BasicEffect EffetDeBase { get; set; }
      DataPiste Donn�esPiste { get; set; }
      TerrainAvecBase Terrain { get; set; }
      List<Vector2> BordureInt�rieure { get; set; }
      List<Vector2> BordureExt�rieure { get; set; }
      VertexPositionColor[] Sommets { get; set; }

      public Piste(Game jeu, float homoth�tieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float intervalleMAJ) : base(jeu, homoth�tieInitiale, rotationInitiale, positionInitiale, intervalleMAJ) { }

      protected override void InitialiserSommets()
      {
         for (int i = 0; i < NbSommets; ++i)
         {
            Sommets[i] = new VertexPositionColor(PtsSommets[i], Color.Black);
         }
      }

      public override void Initialize()
      {
         base.Initialize();
         BordureInt�rieure = Donn�esPiste.GetBordureInt�rieure();
         BordureExt�rieure = Donn�esPiste.GetBordureExt�rieure();
         NbTriangles = (BordureInt�rieure.Count - 1) * 2;
         NbSommets = BordureInt�rieure.Count + BordureExt�rieure.Count + 2;
         PtsSommets = new Vector3[NbSommets];
         Sommets = new VertexPositionColor[NbSommets];
         Cr�erTableauPoints();
         InitialiserSommets();
      }

        protected override void LoadContent()
        {
            Donn�esPiste = Game.Services.GetService(typeof(DataPiste)) as DataPiste;
            Terrain = Game.Services.GetService(typeof(TerrainAvecBase)) as TerrainAvecBase;
            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParam�tresEffetDeBase();
            base.LoadContent();
        }

        void InitialiserParam�tresEffetDeBase()
        {
            EffetDeBase.VertexColorEnabled = true;
        }


        void Cr�erTableauPoints()
        {
            for (int i = 0; i < BordureInt�rieure.Count; ++i)
            {
                PtsSommets[2 * i] = Terrain.GetPointSpatial((int)BordureInt�rieure[i].X, Terrain.NbRang�es - (int)BordureInt�rieure[i].Y);
                PtsSommets[2 * i + 1] = Terrain.GetPointSpatial((int)BordureExt�rieure[i].X, Terrain.NbRang�es - (int)BordureExt�rieure[i].Y);

                if (i == BordureInt�rieure.Count - 1)
                {
                    PtsSommets[2 * i - 1] = Terrain.GetPointSpatial((int)BordureExt�rieure[i].X, Terrain.NbRang�es - (int)BordureExt�rieure[i].Y);
                    PtsSommets[2 * i] = Terrain.GetPointSpatial((int)BordureInt�rieure[i].X, Terrain.NbRang�es - (int)BordureInt�rieure[i].Y);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = Cam�raJeu.Vue;
            EffetDeBase.Projection = Cam�raJeu.Projection;
            G�rerVisibilit�Piste();
            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, Sommets, 0, NbTriangles);
            }
        }

      void G�rerVisibilit�Piste()
      {
         DepthStencilState vieux = GraphicsDevice.DepthStencilState;
         DepthStencilState jeuDepthBufferState = new DepthStencilState();
         jeuDepthBufferState.DepthBufferFunction = vieux.DepthBufferFunction;
         jeuDepthBufferState.DepthBufferEnable = false;
         GraphicsDevice.DepthStencilState = jeuDepthBufferState;
      }
   }
}
