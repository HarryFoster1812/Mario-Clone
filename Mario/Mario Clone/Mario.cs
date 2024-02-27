using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Windows;

namespace Mario_Clone
{

    public class Mario : DynamicSprite
    {

        public enum PlayerAnimationState
        {
            Standing,
            Walking,
            Running,
            Dead,
            Crouch,
        }

        public enum PlayerState
        {
            Normal,
            Giant,
            Fire,
            Star,
        }


        public PlayerState MarioState;
        public PlayerAnimationState State;
        private bool isInvincible = false;
        private int isInvincibleTime;
        public Mario()
        {
            image = new Image();
            image.Height = 16;
            image.Width = 16;
            image.Source = Utils.BitmapSourceFromPath("assets/" + "mario_stand.png");
            X = 32;
            Y = 11 * 16;
            hitbox = new Hitbox(X, Y, 16, 16);
            uint Score= 0;
        }

        public override void Action(Sprite source, int action)
        {
            base.Action(source, action);
            if (source is Goomba) {
                if (jumpSpeed > 0 || isInvincible) {
                    jumpSpeed = -1;
                    Y = source.Y - hitbox.sizey;
                    ((Goomba)source).Action(this, 0);
                    return;
                }

                if (MarioState != PlayerState.Normal) { 
                    MarioState= PlayerState.Normal;
                    image.Dispatcher.Invoke(new Action(() => {
                        this.image.Width = 16;
                        this.image.Height = 16;
                        hitbox.sizey = 16;
                    }));
                    isInvincibleTime = Environment.TickCount;
                    return;
                }

                IsDead= true;
                jumpSpeed = -2;
                velocity= 0;
                acceleration= 0;
                return;
            }

            if (source is Mushroom)
            {
                ((Mushroom)source).Action(this, 0);
                MarioState = PlayerState.Giant;
                image.Dispatcher.Invoke(new Action(() => {
                    this.image.Width = 16;
                    this.image.Height = 32;
                    hitbox.sizey = 32;
                }));
                
                return;
            }

            

            switch (action) {
                case 0:
                    InAir = false;
                    jumpSpeed = 0;
                    Y = source.Y - hitbox.sizey;
                    hitbox.y = Y;
                    break;
                case 1:
                    jumpSpeed = 0.25;
                    Y = source.Y + hitbox.sizey+3;
                    break;
                 case 2:
                    X = source.X - 17;
                    acceleration = 0;
                    velocity = 0;
                    hitbox.x = X;
                    break;
                 case 3:
                    X = source.X + 17;
                    acceleration = 0;
                    velocity = 0;
                    hitbox.x = X;
                    break;
            }
            
        }

        private void animate()
        {
            string path = "";

            string MarioType = "mario";
            if (MarioState == PlayerState.Giant) {
                MarioType = "marioBig";
            }

            image.Dispatcher.Invoke(() => {
                if (State == PlayerAnimationState.Walking || State == PlayerAnimationState.Running)
                {
                    frames++;


                    if (frames % 16 == 0)
                    {
                        path = "assets/" + MarioType +"_run_" + (frames / 16).ToString() + ".png";
                        if (frames == 16 * 2)
                        {
                            frames = -15;
                        }
                    }

                }

                if (State == PlayerAnimationState.Standing) path = "assets/" + MarioType +"_stand.png";

                else if (State == PlayerAnimationState.Crouch) path = "assets/" + MarioType +"_stand.png";
                
                if (InAir) path = "assets/" + MarioType + "_jump.png";

                if (IsDead) path = "assets/mario_dead.png";
                
                if (path != "")
                {
                    if (!Flipped) image.Source = Utils.BitmapSourceFromPath(path);

                    else image.Source = Utils.FlippedBitmapSourceFromPath(path);
                }
            });
        }

        public override void tick()
        {
            throw new NotImplementedException();
        }

        public void tick(bool Up, bool Down, bool Left, bool Right, List<Sprite>Blocks)
        {
            InAir = true;

            if (isInvincible && (Environment.TickCount - isInvincibleTime) >= 100) { 
                isInvincible= false;
            }

            if (!Left && !Right)
            {

                if (Math.Abs(velocity) < 0.1) { velocity = 0; acceleration = 0; }
                else if (velocity > 0) acceleration = -0.0125;

                else if (velocity < 0) acceleration = 0.0125;

            }

            else if (velocity < 0 && !Left) acceleration = +0.025;

            else if (velocity > 0 && !Right) acceleration = -0.025;

            else if (Right) { acceleration = 0.01; if (velocity > 1) { acceleration = 0; velocity = 1; } }
            else if (Left) { acceleration = -0.01; if (velocity < -1) { acceleration = 0; velocity = -1; } }
            

                velocity += acceleration;

            

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
                if (velocity == 0)
                {
                    acceleration = 0;
                }
                else
                {
                    X += velocity;
                    if (X < 5) X = 5;
                }


                hitbox.x = X;
                hitbox.y = Y;

                foreach (Sprite i in Blocks)
                {
                    if (hitbox.Collides(i.hitbox))
                    {
                        if (MarioState == PlayerState.Giant)
                        {
                            bool a = true;
                        }
                        i.Action(this);
                    }
                }

                if (Up && !InAir)
                {
                    jumpSpeed = -2;
                    InAir = true;
                    image.Dispatcher.Invoke(() =>
                    {
                        Game.SoundEffects.Open(new Uri(Directory.GetCurrentDirectory() + "/assets/jumpsound.wav"));
                        Game.SoundEffects.Play();
                    });

                }
                
                if (Y > 12 * 16)
                {
                    // OnDeath();
                    IsDead = true;
                    velocity= 0;
                    acceleration= 0;
                    jumpSpeed= -2;
                }
            }

                Y += jumpSpeed;

                
            

            animate();

            
            // move mario
            // check for collision
            // handle collison
            // check death #emo (y<12*16)
            // change mario state based on collisions/ in air
        } 
    }
}