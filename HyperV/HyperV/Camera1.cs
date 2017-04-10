// By Mathieu Godin
using AtelierXNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace HyperV
{
    public class Camera1 : Cam�raJoueur
    {
        const float MAX_DISTANCE = 4.5f;

        Grass Grass { get; set; }
        Walls Walls { get; set; }
        List<Character> Characters { get; set; }
        List<Portal> Portals { get; set; }

        public Camera1(Game jeu, Vector3 positionCam�ra, Vector3 cible, Vector3 orientation, float intervalleMAJ, float renderDistance)
            : base(jeu, positionCam�ra, cible, orientation, intervalleMAJ, renderDistance)
        { }

        protected override void ChargerContenu()
        {
            base.ChargerContenu();
            Grass = Game.Services.GetService(typeof(Grass)) as Grass;
            Walls = Game.Services.GetService(typeof(Walls)) as Walls;
            Characters = Game.Services.GetService(typeof(List<Character>)) as List<Character>;
            Portals = Game.Services.GetService(typeof(List<Portal>)) as List<Portal>;
        }

        protected override void G�rerD�placement(float direction, float lat�ral)
        {
            base.G�rerD�placement(direction, lat�ral);

            if (Walls.CheckForCollisions(Position) || CheckForCharacterCollision() || CheckForPortalCollision())
            {
                Position -= direction * VitesseTranslation * Direction;
                Position += lat�ral * VitesseTranslation * Lat�ral;
            }
        }

        bool CheckForPortalCollision()
        {
            Game.Window.Title = Position.ToString();
            bool result = false;
            int i;

            for (i = 0; i < Portals.Count && !result; ++i)
            {
                result = Portals[i].CheckForCollisions(Position);
            }

            return result;
        }

        bool CheckForCharacterCollision()
        {
            bool result = false;
            int i;

            for (i = 0; i < Characters.Count && !result; ++i)
            {
                result = Vector3.Distance(Characters[i].GetPosition(), Position) < MAX_DISTANCE;
            }

            return result;
        }
    }
}
