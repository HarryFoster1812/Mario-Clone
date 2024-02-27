using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using static Mario_Clone.Mario;

namespace Mario_Clone
{
    internal class Goomba : DynamicSprite
    {

        public Goomba(double x, double y)
        {
            X = x;
            Y = y;
            image = new Image();
            image.Height = 16;
            image.Width = 16;
            velocity= -0.26;
            image.Source = Utils.BitmapSourceFromPath("assets/" + "goomba_0.png");
            hitbox = new Hitbox(X, Y, 16, 16);
        }

        public override void Action(DynamicSprite Dynamic)
        {
            Dynamic.Action(this, base.CalculateSide(Dynamic));
        }

        public override void Action(Sprite source, int action)
        {
            base.Action(source, action);
            if (source is Mario)
            {
                deathTime = Environment.TickCount;
                IsDead = true;
                velocity = 0;
                return;
            }
            switch (action)
            {
                case 0:
                    InAir = false;
                    jumpSpeed = 0;
                    Y = source.Y - 16;
                    hitbox.y = Y;
                    break;
                case 2:
                    X = source.X - 17;
                    velocity *= -1;
                    hitbox.x = X;
                    break;
                case 3:
                    X = source.X + 18;
                    velocity *= -1;
                    hitbox.x = X;
                    break;
            }

        }


        private void animate()
        {
            string path = "";

            image.Dispatcher.Invoke(() => {

                frames++;
                    if (frames % 16 == 0)
                    {
                        path = "assets/" + "goomba_" + (frames / 16).ToString() + ".png";
                        if (frames == 16)
                        {
                        frames = -15;
                        }
                    }
                if (IsDead) { 
                        path = "assets/" + "goomba_dead.png";
                }

                if (path != "")
                {
                    if (!Flipped) image.Source = Utils.BitmapSourceFromPath(path);

                    else image.Source = Utils.FlippedBitmapSourceFromPath(path);
                }
            });
        }

        public override void tick(List<Sprite> Blocks)
        {
            InAir = true;

            if (jumpSpeed != 0)
            {
                jumpSpeed += jumpAcc;
            }
            else
            {
                jumpSpeed = 0.25;
            }
            if (!IsDead)
            {
                X += velocity;



                foreach (Sprite i in Blocks)
                {
                    if (i != this)
                    {
                        if (hitbox.Collides(i.hitbox))
                        {
                            i.Action(this);
                        }
                    }
                }

                Y += jumpSpeed;
                hitbox.x = X;
                hitbox.y = Y;


                
            }
            animate();
        }

        public override void tick()
        {
            throw new NotImplementedException();
        }
    }
}
