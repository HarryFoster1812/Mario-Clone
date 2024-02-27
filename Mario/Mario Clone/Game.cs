using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Controls;
using System.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Mario_Clone
{

    public class Game
    {
        List<Sprite> Blocks = new List<Sprite>();
        public static MediaPlayer Music = new MediaPlayer();
        public static MediaPlayer SoundEffects = new MediaPlayer();
        static public List<Sprite> toRemove = new List<Sprite>();
        public static List<double[]> toAdd = new List<double[]>();
        Queue<char[]> outofframe = new Queue<char[]>();
        double cameraOffset = 0;
        double frameOffset = 0;
        public bool Up;
        public bool Down;
        public bool Left;
        public bool Right;
        public string path;
        private Dictionary<char, string> ImageDict = new Dictionary<char, string>();
        public static Canvas rootCanvas;
        Mario mario;
        Rectangle debugHit;

        public Game(Canvas canvas, string pathLevel, string musicPath = "main_theme.wav")
        {
            ImageDict.Add('?', "Lucky1");
            ImageDict.Add('m', "Lucky1");
            ImageDict.Add('=', "block1_16");
            ImageDict.Add('#', "block2_16");
            ImageDict.Add('N', "Ltop");
            ImageDict.Add('M', "Rtop");
            ImageDict.Add('L', "Ltop");
            ImageDict.Add('R', "Rtop");
            ImageDict.Add('l', "left");
            ImageDict.Add('r', "right");
            ImageDict.Add('E', "block2_16");

            rootCanvas = canvas;
            this.path = pathLevel;
            Music.Open(new Uri(Directory.GetCurrentDirectory() + "/assets/" + musicPath));
            Music.Play();
        }
        public void Load()
        {
            string file_content = File.ReadAllText(Directory.GetCurrentDirectory() + "/" + path);
            string[] filecontent = file_content.Split('\n');
            char[,] render = new char[18, 13];
            char[] temp = new char[14];

            mario = new Mario();

            debugHit = new Rectangle();
            debugHit.Height = 32;
            debugHit.Width = 16;
            debugHit.Stroke = Brushes.DarkGray;
            debugHit.StrokeThickness = 4;
            rootCanvas.Children.Add(debugHit);


            rootCanvas.Children.Add(mario.image);
            Canvas.SetLeft(mario.image, mario.X);
            Canvas.SetTop(mario.image, mario.Y);

            for (int i = 0; i < filecontent.Length; i++)
            {
                for (int j = 0; j < 18; j++)
                {
                    if (j < 18)
                        render[j, i] = filecontent[i][j];
                }
            }

            for (int i = 18; i < filecontent[0].Length - 1; i++)
            {
                for (int j = 0; j < filecontent.Length + 1; j++)
                {
                    if (j == 0)
                    {
                        temp[j] = Convert.ToChar(i);
                    }
                    else temp[j] = filecontent[j - 1][i];
                }
                outofframe.Enqueue(temp);
                temp = new char[14];
            }

            // vertical

            for (int i = 0; i < 18; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (render[i, j] != '-')
                    {

                        Sprite temp1;
                        switch (render[i, j])
                        {
                            case '?':
                                temp1 = new LuckyBlock(ImageDict[render[i, j]], i * 16, j * 16);
                                break;

                            case '#':
                                temp1 = new BreakableBlock(ImageDict[render[i, j]], i * 16, j * 16);
                                break;
                            case 'E':
                                temp1 = new BreakableBlock(ImageDict[render[i, j]], i * 16, j * 16);
                                break;

                            default:
                                temp1 = new NonBreakableBlock(ImageDict[render[i, j]], i * 16, j * 16);
                                break;
                        }


                        temp1.image.Tag = i;
                        Canvas.SetLeft(temp1.image, i * 16);
                        Canvas.SetTop(temp1.image, (j) * 16);
                        rootCanvas.Children.Add(temp1.image);
                        Blocks.Add(temp1);
                    }
                }
            }

            // convert both to objects
        }

        public void LoadNext_Column()
        {
            // push from queue
            if (outofframe.Count != 0)
            {
                char[] torender = outofframe.Dequeue();
                rootCanvas.Dispatcher.Invoke(() => {
                    double x = 0;
                    for (int j = 0; j < 14; j++)
                    {
                        if (j == 0)
                        {
                            Image lastblock = (Image)rootCanvas.Children[rootCanvas.Children.Count - 1];
                            int lastblocktag = Convert.ToInt32(lastblock.Tag);
                            double lastblockx = Canvas.GetLeft(rootCanvas.Children[rootCanvas.Children.Count - 1]);
                            int thistag = Convert.ToInt32((int)torender[j]);
                            x = lastblockx + (thistag - lastblocktag) * 16;
                        }
                        else if (torender[j] != '-')
                        {
                            Sprite temp1;

                            switch (torender[j])
                            {
                                case '?':
                                    temp1 = new LuckyBlock(ImageDict[torender[j]], x, (j - 1) * 16);
                                    break;

                                case '#':
                                    temp1 = new BreakableBlock(ImageDict[torender[j]], x, (j - 1) * 16);
                                    break;
                                case 'E':
                                    temp1 = new BreakableBlock(ImageDict[torender[j]], x, (j - 1) * 16);
                                    break;

                                case 'N':
                                    temp1 = new SecretPipe(ImageDict[torender[j]], x, (j - 1) * 16, "Secret.txt");
                                    break;

                                case 'M':
                                    temp1 = new SecretPipe(ImageDict[torender[j]], x, (j - 1) * 16, "Secret.txt");
                                    break;

                                case 'm':
                                    temp1 = new LuckyBlockPowerUp(ImageDict[torender[j]], x, (j - 1) * 16, 0);
                                    break;

                                case 'G':
                                    temp1 = new Goomba(x, (j - 1) * 16);
                                    break;

                                default:
                                    temp1 = new NonBreakableBlock(ImageDict[torender[j]], x, (j - 1) * 16);
                                    break;
                            }
                            temp1.image.Tag = Convert.ToInt32(torender[0]);
                            Canvas.SetLeft(temp1.image, x);
                            Canvas.SetTop(temp1.image, ((j - 1)) * 16);
                            rootCanvas.Children.Add(temp1.image);
                            Blocks.Add(temp1);
                        }
                    }
                    int i;
                    for (i = 1; i < Blocks.Count; i++)
                    {
                    
                        if (Blocks[i].X > -5)
                        {
                            break;
                        }
                    }
                    if (i != 1)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            rootCanvas.Children.Remove(Blocks[j].image);
                        }
                        Blocks.RemoveRange(0, i);
                    }
                });
            }
        }

        public void Tick()
        {
            while (true)
            {
                RemoveSprites();
                PhysicsTick();
                RenderTick();
                Thread.Sleep(8);
            }

        }

        private void RemoveSprites() {
            if (toRemove.Count > 0)
            {
                rootCanvas.Dispatcher.Invoke(new Action(() =>
                {
                    foreach (Sprite i in toRemove)
                    {
                        Blocks.Remove(i);
                        rootCanvas.Children.Remove(i.image);
                    }
                }));
                toRemove.Clear();
            }

            if (toAdd.Count > 0) {
                rootCanvas.Dispatcher.Invoke(() =>
                {
                    foreach (double[] i in toAdd)
                    {
                        if (i[0] == 0) {
                            Mushroom temp = new Mushroom(i[1], i[2]);
                            Blocks.Add(temp);
                            rootCanvas.Children.Insert(rootCanvas.Children.Count - 1, temp.image);
                        }
                    }
                });
                toAdd.Clear();
            }
        }

        public void PhysicsTick()
        {
            mario.tick(Up, Down, Left, Right, Blocks);

            if (mario.X >= 8 * 16) { cameraOffset = mario.velocity; mario.X = 8 * 16 - 1; }

            frameOffset += cameraOffset;
            if (frameOffset >= 16)
            {
                LoadNext_Column();
                frameOffset -= 16;
            }

            foreach (Sprite i in Blocks)
            {
                i.X -= cameraOffset;
                i.hitbox.x -= cameraOffset;
            }



            // loop through each moving sprite
            //for each change the x and y value accordingly
            //check for collisions
        }


        public void RenderTick()
        {
            ChangeMarioState();
            IncFrames();
            rootCanvas.Dispatcher.Invoke(() =>
            {
                foreach (Sprite i in Blocks)
                {
                    if (i.IsAnimated == true)
                    {
                        (i).tick();
                    }
                    else if (i is DynamicSprite)
                    {
                        DynamicSprite sprite = (DynamicSprite)i;
                        sprite.tick(Blocks);
                        Canvas.SetTop(i.image, i.Y);
                        if (sprite.IsDead) {
                            if((Environment.TickCount - sprite.deathTime) >= 500)
                            toRemove.Add(i);
                        }
                    }
                    Canvas.SetLeft(i.image, i.X);
                }

                Canvas.SetLeft(mario.image, mario.X);
                Canvas.SetTop(mario.image, mario.Y);

                Canvas.SetLeft(debugHit, mario.hitbox.x);
                Canvas.SetTop(debugHit, mario.hitbox.y);

                cameraOffset = 0;
            });
            // update all moving sprites eg mario and enemies
            // shift everything via the camera warp/update the screen
            // if the camera warp is greater than 16 then remove the first column and delete the data and add the next column to be rendered.
        }

        public void ChangeMarioState()
        {
            if (Right)
            {
                mario.Flipped = false;
                mario.State = Mario.PlayerAnimationState.Running;
            }

            else if (Left)
            {
                mario.Flipped = true;
                mario.State = Mario.PlayerAnimationState.Running;
            }
            else mario.State = Mario.PlayerAnimationState.Standing;


            if (Down)
            {
                mario.State = Mario.PlayerAnimationState.Crouch;
            }



        }

        private void IncFrames()
        {
            Sprite.Frames++;
            if (Sprite.Frames == 32 * 3 + 1)
            {
                Sprite.Frames = 0;
            }
        }

        private void EndGame()
        {

        }
    }
}
