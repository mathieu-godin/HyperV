using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtelierXNA;


namespace HyperV
{
    public class HeightMap : PrimitiveDeBase
    {
        const int NB_TRIANGLES_PAR_TUILE = 2;
        const int NB_SOMMETS_PAR_TRIANGLE = 3;
        const float MAX_COULEUR = 255f;

        const int AUCUN_D�CALAGE_DE_SOMMET = 0;
        const int AVANT_PREMIER_SOMMET = -1;
        const int NB_TRIANGLES = 2;
        const int COTE_NULLE = 0;
        const int PREMIERS_SOMMETS_DU_STRIP = 2;

        const int SOMMET_SUPPL�MENTAIRE_POUR_LIGNE = 1;
        const int COMPENSATION_NULLE = 0;
        const int DIVISEUR_DEMI_GRANDEUR = 2;
        const int NB_PTS_TEXTURE_POSSIBLE = 4;
        const int NB_SOMMETS_PAR_TUILE = 4;
        const int ORDONN�E_NULLE = 0;
        const int POSITION_TEXEL_DE_MOINS_PAR_RAPPORT_�_DIMENSION = 1;
        const int VALEUR_J_MAX_HAUT_DE_TUILE = 1;
        const int VALEUR_HAUT_DE_TUILE = 0;
        const int VALEUR_BAS_DE_TUILE = 1;

        Vector3 �tendue { get; set; }
        string NomCarteTerrain { get; set; }
        string[] NomTextureTerrain { get; set; }
        int NbNiveauTexture { get; set; }

        BasicEffect EffetDeBase { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Texture2D CarteTerrain { get; set; }
        Vector3 Origine { get; set; }

        // � compl�ter en ajoutant les propri�t�s qui vous seront n�cessaires pour l'impl�mentation du composant
        int NbRang�es { get; set; }
        int NbColonnes { get; set; }
        Color[] DataCarteTexture { get; set; }
        int LargeurTuile { get; set; }
        Vector3[,] PtsSommets { get; set; }
        Vector2[,] PtsTexture { get; set; }
        VertexPositionTexture[] Sommets { get; set; }
        Vector2 Delta { get; set; }
        //int NbTexels { get; set; }
        float[,] Heights { get; set; }

        Texture2D TextureCombin�e { get; set; }
        Texture2D TextureTerrainSable { get; set; }
        Texture2D TextureTerrainHerbe { get; set; }

        public HeightMap(Game jeu, float homoth�tieInitiale, Vector3 rotationInitiale,
                        Vector3 positionInitiale, Vector3 �tendue, string nomCarteTerrain,
                        string[] nomTextureTerrain)
            : base(jeu, homoth�tieInitiale, rotationInitiale, 
                   positionInitiale)
      {
            �tendue = �tendue;
            NomCarteTerrain = nomCarteTerrain;
            NomTextureTerrain = nomTextureTerrain;
        }

        void InitialiserDonn�esCarte()
        {
            CarteTerrain = GestionnaireDeTextures.Find(NomCarteTerrain);
            DataCarteTexture = new Color[CarteTerrain.Width * CarteTerrain.Height];
            CarteTerrain.GetData<Color>(DataCarteTexture);
        }

        void InitialiserDonn�esTexture()
        {
            TextureTerrainSable = GestionnaireDeTextures.Find(NomTextureTerrain[0]);
            TextureTerrainHerbe = GestionnaireDeTextures.Find(NomTextureTerrain[1]);
            LargeurTuile = (int)(TextureTerrainSable.Height / (float)NbNiveauTexture);
        }

        public override void Initialize()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;

            InitialiserDonn�esCarte();
            InitialiserDonn�esTexture();

            NbRang�es = CarteTerrain.Width - SOMMET_SUPPL�MENTAIRE_POUR_LIGNE;
            NbColonnes = CarteTerrain.Height - SOMMET_SUPPL�MENTAIRE_POUR_LIGNE;
            NbTriangles = NbRang�es * NbColonnes * NB_TRIANGLES_PAR_TUILE;
            NbSommets = NbTriangles * NB_SOMMETS_PAR_TRIANGLE;
            Heights = new float[CarteTerrain.Width, CarteTerrain.Height];
            Origine = new Vector3(/*-�tendue.X / DIVISEUR_DEMI_GRANDEUR, 0, -�tendue.Z / DIVISEUR_DEMI_GRANDEUR*/0, 0, 0); //pour centrer la primitive au point (0,0,0)##########################Moins � Z ajout�

            AllouerTableaux();
            AffecterTableauPtsSommets(); 
            AffecterTableauPtsTexture();
            Cr�erTextureCombin�e();
            InitialiserSommets();

            base.Initialize();
        }

        void AllouerTableaux()
        {
            Sommets = new VertexPositionTexture[NbSommets];
            PtsTexture = new Vector2[CarteTerrain.Width, CarteTerrain.Height];
            PtsSommets = new Vector3[CarteTerrain.Width, CarteTerrain.Height];
        }

        void AffecterTableauPtsTexture()
        {
            for (int i = 0; i < PtsTexture.GetLength(0); ++i)
            {
                for (int j = 0; j < PtsTexture.GetLength(1); ++j)
                {
                    PtsTexture[i, j] = new Vector2(i / (float)NbColonnes, -j / (float)NbRang�es);
                }
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParam�tresEffetDeBase();
        }

        void InitialiserParam�tresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureCombin�e;
        }

        private void AffecterTableauPtsSommets()
        {
            Delta = new Vector2(�tendue.X / NbRang�es, �tendue.Z / NbColonnes);
            for (int i = 0; i < PtsSommets.GetLength(0); ++i)
            {
                for (int j = 0; j < PtsSommets.GetLength(1); ++j)
                {
                    PtsSommets[i, j] = Origine + new Vector3(Delta.X * i, DataCarteTexture[j * PtsSommets.GetLength(1) + i].B / MAX_COULEUR * �tendue.Y, Delta.Y * j);
                }
            }
        }

        //
        // Cr�ation des sommets.
        // N'oubliez pas qu'il s'agit d'un TriangleList...
        //
        protected override void InitialiserSommets()
        {
            int cpt = -1;
            for (int j = 0; j < NbRang�es; ++j)
            {
                for (int i = 0; i < NbColonnes; ++i)
                {
                    Sommets[++cpt] = new VertexPositionTexture(PtsSommets[i, j], PtsTexture[i, j]);
                    Sommets[++cpt] = new VertexPositionTexture(PtsSommets[i + 1, j], PtsTexture[i + 1, j]);
                    Sommets[++cpt] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                    Sommets[++cpt] = new VertexPositionTexture(PtsSommets[i + 1, j], PtsTexture[i + 1, j]);
                    Sommets[++cpt] = new VertexPositionTexture(PtsSommets[i + 1, j + 1], PtsTexture[i + 1, j + 1]);
                    Sommets[++cpt] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // � compl�ter
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = Cam�raJeu.Vue;
            EffetDeBase.Projection = Cam�raJeu.Projection;
            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets, COMPENSATION_NULLE, Sommets.Length / NB_SOMMETS_PAR_TRIANGLE);
            }
        }

        public float GetHeight(Vector3 positionReceived)
        {
            Vector3 position = positionReceived - PositionInitiale;
            int i = (int)(position.X / Delta.X), j = (int)(position.Z / Delta.Y);
            Vector3 n;
            float height;
            if (i >= 0 && j >= 0 && i + 1 < /*Heights.GetLength(0)*/PtsSommets.GetLength(0) && j + 1 < PtsSommets.GetLength(1)/*Heights.GetLength(1)*/)
            {
                n = Vector3.Cross(PtsSommets[i + 1, j] - PtsSommets[i, j], PtsSommets[i, j + 1] - PtsSommets[i, j]);
                height = (n.X * PtsSommets[i, j].X + n.Y * PtsSommets[i, j].Y + n.Z * PtsSommets[i, j].Z - n.X * position.X - n.Z * position.Z) / n.Y + 5 + PositionInitiale.Y; // - PositionInitiale.Y is new !!!
                //height = Heights[i, j];
            }
            else
            {
                height = 5;//position.Y;
            }
            return height;
        }

        void Cr�erTextureCombin�e()
        {
            TextureCombin�e = new Texture2D(TextureTerrainSable.GraphicsDevice, PtsSommets.GetLength(0), PtsSommets.GetLength(1));
            int nbTexels = PtsSommets.GetLength(0) * PtsSommets.GetLength(0);
            Color[] texels = new Color[nbTexels];
            TextureTerrainSable.GetData(texels);

            Color[] texelsSable = new Color[TextureTerrainSable.Width * TextureTerrainSable.Height];
            TextureTerrainSable.GetData(texelsSable);

            Color[] texelsHerbe = new Color[TextureTerrainHerbe.Width * TextureTerrainHerbe.Height];
            TextureTerrainHerbe.GetData(texelsHerbe);

            for (int noTexel = 0; noTexel < nbTexels; ++noTexel)
            {
                float pourcent = GetPourcent(noTexel);

                texels[noTexel].R = (byte)((pourcent * (byte)texelsHerbe[noTexel].R) + (byte)((1 - pourcent) * (byte)texelsSable[noTexel].R));
                texels[noTexel].G = (byte)((pourcent * (byte)texelsHerbe[noTexel].G) + (byte)((1 - pourcent) * (byte)texelsSable[noTexel].G));
                texels[noTexel].B = (byte)((pourcent * (byte)texelsHerbe[noTexel].B) + (byte)((1 - pourcent) * (byte)texelsSable[noTexel].B));
            }

            TextureCombin�e.SetData<Color>(texels);
        }

        float GetPourcent(int noTexel)
        {
            return DataCarteTexture[noTexel].B / MAX_COULEUR;
        }
    }
}
