/*
TexteTournoyant.cs
------------------

Par Mathieu Godin

R�le : Composant s'occupant d'afficher
       un message tournoyant personnalis�
       par une cha�ne de charact�res
       repr�sentant ce message, sa position,
       sa couleur et un intervalle de mise
       � jour repr�sentant aussi la vitesse
       auquel le message tourne 

Cr�� : 29 ao�t 2016
*/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AtelierXNA
{
    /// <summary>
    /// Composant qui permet d'afficher un texte tournoyant personnalis�
    /// </summary>
    public class TexteTournoyant : Microsoft.Xna.Framework.DrawableGameComponent
    {
        /// <summary>
        /// Constructeur qui permet de sauvegarder les personnalisations pass�es en param�tres
        /// </summary>
        /// <param name="game">Jeu qui a appel� ce composant</param>
        /// <param name="message">Cha�ne rerp�sentant le message que l'on veut afficher</param>
        /// <param name="position">Position du message dans l'�cran par rapport � son centre</param>
        /// <param name="couleur">Couleur voulue pour le message tournoyant en question</param>
        /// <param name="intervalleMAJ">Intervalle de mise � jour de l'angle et de l'�chelle du message, une grande valeur fera tourner et agrandir le message plus rapidement</param>
        public TexteTournoyant(Game game, string message, Vector2 position, Color couleur, float intervalleMAJ) : base(game)
        {
            Message = message;
            Position = position;
            Couleur = couleur;
            IntervalleMAJ = intervalleMAJ;
        }

        /// <summary>
        /// Initialise les m�caniques de temps, d'angle et d'�chelle du message tournoyant
        /// </summary>
        public override void Initialize()
        {
            Temps�coul�DepuisMAJ = AUCUN_TEMPS_�COUL�;
            �chelle = �CHELLE_D�PART;
            AngleRotation = ANGLE_D�PART;
            base.Initialize();
        }

        /// <summary>
        /// Charge la police et la gestion des sprites et fait le calcul de l'origine du message tournoyant
        /// </summary>
        protected override void LoadContent()
        {
            ArialFont = Game.Content.Load<SpriteFont>("Fonts/Arial");
            InitialiserOrigine();
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
        }

        /// <summary>
        /// Initialise l'origine
        /// </summary>
        void InitialiserOrigine()
        {
            Vector2 dimension = ArialFont.MeasureString(Message);
            Origine = new Vector2(dimension.X / 2, dimension.Y / 2);
        }

        /// <summary>
        /// Met � jour l'angle et l'�chelle du message tournoyant, se d�sactive apr�s deux tours
        /// </summary>
        /// <param name="gameTime">Donne des informations sur le temps du jeu</param>
        public override void Update(GameTime gameTime)
        {
            Temps�coul�DepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            V�rifierSiIncr�mentationN�cessaire();
        }

        /// <summary>
        /// V�rifie si l'intervalle de mise � jour a �t� �coul� pour incr�menter l'angle et l'�chelle du texte tournoyant si c'est le cas
        /// </summary>
        void V�rifierSiIncr�mentationN�cessaire()
        {
            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                Temps�coul�DepuisMAJ = AUCUN_TEMPS_�COUL�;
                AngleRotation += INCR�MENT_ANGLE;
                �chelle += INCR�MENT_ZOOM;
                V�rifierSiD�sactivationN�cessaire();
            }
        }

        /// <summary>
        /// V�rifie si le texte tournoyant a fait deux tours afin de d�sactiver son tournage et son grossissement si c'est le cas
        /// </summary>
        void V�rifierSiD�sactivationN�cessaire()
        {
            if (AngleRotation > DEUX_TOURS)
            {
                AngleRotation = DEUX_TOURS;
                this.Enabled = false;
            }
        }

        /// <summary>
        /// Dessine le message tournoyant � l'�cran
        /// </summary>
        /// <param name="gameTime">Donne des informations sur le temps du jeu</param>
        public override void Draw(GameTime gameTime)
        {
            GestionSprites.DrawString(ArialFont, Message, Position, Couleur, MathHelper.ToRadians(AngleRotation), Origine, �chelle, SpriteEffects.None, AUCUNE_COUCHE_DE_PROFONDEUR);
        }

        string Message { get; set; }
        Vector2 Position { get; set; }
        Color Couleur { get; set; }
        float IntervalleMAJ { get; set; }
        SpriteFont ArialFont { get; set; }
        float �chelle { get; set; }
        float AngleRotation { get; set; }
        Vector2 Origine { get; set; }
        SpriteBatch GestionSprites { get; set; }
        float Temps�coul�DepuisMAJ { get; set; }

        const float INCR�MENT_ANGLE = 3.6F;
        const float INCR�MENT_ZOOM = 0.01F;
        const float DEUX_TOURS = 720.0F;
        const float ANGLE_D�PART = 0.0F;
        const float �CHELLE_D�PART = 0.01F;
        const float AUCUN_TEMPS_�COUL� = 0.0F;
        const float AUCUNE_COUCHE_DE_PROFONDEUR = 0.0F;
    }
}
