using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AtelierXNA;


namespace HyperV
{
   public class Epee : ModeleRamassable
    {
        bool CoupDEpee { get; set; }
        public bool ContinuerCoupDEpee { get; private set; }
        float t { get; set; }
        InputManager GestionInput { get; set; }
        GamePadManager GestionGamePad { get; set; }
        float DiffAngleX { get; set; }
        float DiffAngleY { get; set; }
        int Attack { get; set; }

        public Epee(Game jeu, string nomMod�le, float �chelleInitiale,
                    Vector3 rotationInitiale, Vector3 positionInitiale, int attack)
            : base(jeu, nomMod�le, �chelleInitiale, rotationInitiale, positionInitiale)
        {
            Attack = attack;
        }

        public override void Initialize()
        {
            base.Initialize();
            CoupDEpee = false;
            ContinuerCoupDEpee = false;
            t = 0;
            DiffAngleX = 0;
            DiffAngleY = 0;
        }

        Boss Boss { get; set; }
        List<Enemy> Enemy { get; set; }

        protected override void LoadContent()
        {
            base.LoadContent();
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionGamePad = Game.Services.GetService(typeof(GamePadManager)) as GamePadManager;
            Boss = Game.Services.GetService(typeof(Boss)) as Boss;
            Enemy = Game.Services.GetService(typeof(List<Enemy>)) as List<Enemy>;
        }


        protected override void CalculerAngles()
        {
            base.CalculerAngles();

            if (CoupDEpee)
            {
                ContinuerCoupDEpee = true;
            }

            if (ContinuerCoupDEpee)
            {
                if (t < 30)
                {
                    DiffAngleX -= 0.03f;
                    DiffAngleY += 0.05f;
                }
                else
                {
                    DiffAngleX += 0.03f;
                    DiffAngleY -= 0.05f;

                    if (t > 60)
                    {
                        ContinuerCoupDEpee = false;
                        DiffAngleX = 0;
                        DiffAngleY = 0;
                        t = 0;
                        if (Boss != null)
                        {
                            Boss.CheckForAttack(Attack);
                        }
                        if (Enemy.Count > 0)
                        {
                            foreach (Enemy e in Enemy)
                            {
                                e.CheckForAttack(Attack);
                            }
                        }
                    }
                }
                angleX += DiffAngleX;
                angleY += DiffAngleY;
                ++t;
            }

        }

        public override void Update(GameTime gameTime)
        {
            CoupDEpee = (GestionInput.EstEnfonc�e(Keys.T) || GestionInput.EstNouveauClicDroit() || GestionGamePad.EstNouveauBouton(Buttons.B) )&& EstRamass�e;

            base.Update(gameTime);
        }
    }
}
