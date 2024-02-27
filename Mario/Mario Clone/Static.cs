using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using System.Configuration;

namespace Mario_Clone
{



    public abstract class Sprite
    {
        static public int Frames;
        public Hitbox hitbox;
        public double X;
        public double Y;
        public Image image = new Image();
        public bool IsAnimated = false;

        public Sprite()
        {

        }

        public Sprite(string source, double x, double y)
        {
            image.Source = Utils.BitmapSourceFromPath("assets/" + source + ".png");
            image.Width = 16;
            image.Height = 16;
            this.X = x;
            this.Y = y;
            hitbox = new Hitbox(X, Y, 16, 16);
        }


        public virtual int CalculateSide(DynamicSprite Dynamic) {
            // calculate width and height of collision area
            //

            double AfterY = Dynamic.Y + Dynamic.jumpSpeed;
            double AfterX = Dynamic.X + Dynamic.velocity;


            // check if DynX > X + size/2
            if (Dynamic.X < X + 8)
            {
                if (Math.Abs((Dynamic.Y + Dynamic.hitbox.sizey) - Y) < Math.Abs((Dynamic.X + 16) - X)) // Top Collison?
                {
                    
                    return 0;
                }

                else if (Math.Abs((Dynamic.Y) - Y - Dynamic.hitbox.sizey) > 0 && (Math.Abs((Dynamic.Y) - (Y + Dynamic.hitbox.sizey)) < Math.Abs((Dynamic.X + 16) - X)))
                {
                    return 1;
                }


                else
                {
                    
                    return 2;

                }
            }

            else
            {
                if (Math.Abs((Dynamic.Y + Dynamic.hitbox.sizey) - Y) < Math.Abs((Dynamic.X) - X-16)) {

                    return 0;
                } // top right side

                else if (Math.Abs((Dynamic.Y) - Y - Dynamic.hitbox.sizey) > 0 && (Math.Abs((Dynamic.Y) - (Y + Dynamic.hitbox.sizey)) < Math.Abs((Dynamic.X ) - X-16))) {
                    
                    return 1;
                }


                else {
                    
                    return 3;
                }
                
            }
        }


        public virtual void Action(DynamicSprite Dynamic)
        {

        }

        public abstract void tick();
    }

    public abstract class DynamicSprite : Sprite
    {
        public double XSize = 16;
        public double YSize = 16;
        protected int frames;
        public double jumpSpeed = 0;
        public const double jumpAcc = 0.025;
        public double velocity = 0;
        public double acceleration = 0;
        public bool Flipped = false;
        public bool InAir = false;
        public bool IsDead = false;
        public int deathTime;

        public virtual void Action(Sprite source, int action)
        {
            
        }

        public virtual void tick(List<Sprite> Blocks)
        {

        }
    }

    public class LuckyBlock : Sprite
    {
        public LuckyBlock() { }

        public LuckyBlock(string image, double x, double y) : base(image, x, y)
        {
            this.IsAnimated = true;
        }

        public override void tick()
        {
            image.Dispatcher.Invoke(() =>
            {
                if (Frames % 32 == 0 && Frames != 0)
                {
                    image.Source = Utils.BitmapSourceFromPath("assets/" + "Lucky" + (Frames / 32).ToString() + ".png");
                }
            });
        }
        public override void Action(DynamicSprite Dynamic)
        {

            int value = base.CalculateSide(Dynamic);
            Dynamic.Action(this, value);

            switch (value) {
                case 1:
                    IsAnimated = false;
                    image.Dispatcher.Invoke(() =>
                    {
                        image.Source = Utils.BitmapSourceFromPath("assets/" + "Lucky4.png");
                    });
                    break;
            }
        }

    }

    public class LuckyBlockPowerUp : Sprite
    {
        int PowerUpType;
        public LuckyBlockPowerUp() { }

        public LuckyBlockPowerUp(string image, double x, double y, int type) : base(image, x, y)
        {
            this.IsAnimated = true;
            PowerUpType = type;
        }

        public override void tick()
        {
            image.Dispatcher.Invoke(() =>
            {
                if (Frames % 32 == 0 && Frames != 0)
                {
                    image.Source = Utils.BitmapSourceFromPath("assets/" + "Lucky" + (Frames / 32).ToString() + ".png");
                }
            });
        }
        public override void Action(DynamicSprite Dynamic)
        {

            int value = base.CalculateSide(Dynamic);
            Dynamic.Action(this, value);

            if (value == 1)
            {
                if (IsAnimated)
                {
                    IsAnimated = false;

                    Game.toAdd.Add(new double[] { PowerUpType, X, Y - 17 });

                    image.Dispatcher.Invoke(() =>
                    {
                        image.Source = Utils.BitmapSourceFromPath("assets/" + "Lucky4.png");
                    });
                }
            }
            
        }

    }

    public class BreakableBlock : Sprite
    {
        public BreakableBlock() { }

        public BreakableBlock(string image, double x, double y) : base(image, x, y)
        {
            IsAnimated = false;
        }

        public override void tick() { }

        public override void Action(DynamicSprite Dynamic)
        {
            int value = base.CalculateSide(Dynamic);
            Dynamic.Action(this, value);

            switch (value)
            {
                case 1:
                    if (Dynamic is Mario && ((Mario)Dynamic).MarioState == Mario.PlayerState.Giant)
                    {
                        Game.toRemove.Add(this);

                    }
                    break;
                default:
                    // move block up
                    break;
            }
        }

    }

    public class SecretPipe : Sprite
    {
        string secretPath;
        public SecretPipe() { }

        public SecretPipe(string image, double x, double y, string path) : base(image, x, y)
        {
            this.IsAnimated = false;
            this.secretPath = path;
        }

        public override void tick()
        {
        }
        public override void Action(DynamicSprite Dynamic)
        {
            int value = base.CalculateSide(Dynamic);
            Dynamic.Action(this, value);

            switch (value)
            {
                case 0:
                    if (Dynamic is Mario && ((Mario)Dynamic).State == Mario.PlayerAnimationState.Crouch)
                    {

                        MainWindow.rootCanvas.Dispatcher.Invoke(() =>
                        {
                            MainWindow.t.Abort();
                            MainWindow.rootCanvas.Children.Clear();
                            MainWindow.game = new Game(MainWindow.rootCanvas, secretPath, "Rick.wav");
                            MainWindow.game.Load();
                            MainWindow.t = new Thread(MainWindow.game.Tick);
                            MainWindow.t.SetApartmentState(ApartmentState.STA);
                            MainWindow.t.Start();
                        });
                    }
                    break;
            }
        }

}


    public class NonBreakableBlock : Sprite
    {
        public NonBreakableBlock() { }

        public NonBreakableBlock(string image, double x, double y) : base(image, x, y)
        {
            IsAnimated = false;
        }

        public override void tick() { }

        public override void Action(DynamicSprite Dynamic)
        {

            int value = base.CalculateSide(Dynamic);
            Dynamic.Action(this, value);
        }

    }

}