using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace RaccoonWrangler
{
    public class Raccoon : DrawableGameComponent
    {
        protected Game1 game;
        protected Vector2 Position;
        protected Vector2 Direction;
        protected Texture2D Raccoon1;
        protected Vector2 fenceDir;
        protected Rectangle drawRaccoon;
        protected SpriteBatch spriteBatch;
        protected float screenWidth;
        protected float screenHeight;
        protected float speed;
        protected float currentTime, elapsedTime, eTime;
        protected float stealTime, resetTime, beanTime;
        protected Random rand;
        protected bool beanRun;
        bool grabRacc;
        bool fenceHit;
        
        
        

        //------raccoon counting-------
        int maxRaccoon;
        //Vector2 averageFlockPos;
        //Vector2 averageFlockDir;

        //seperation
        int tooClose;
        Vector2 totTooClosePos;
        Vector2 avgTooClosePos;
        Vector2 AvgSeperateDir;

        //align
        protected Vector2 avgFlockLookDir;

        //cohesion
        protected Vector2 avgFlockDir;

        //seekSnack
        protected Vector2 snackPos;
        protected Vector2 snackDir;
        protected Vector2 awayBeans;


        public Raccoon(Game1 theGame, float ix, float iy) : base(theGame)
        {
            Position = new Vector2(ix, iy);

            Direction = new Vector2();

            awayBeans = Vector2.Zero;

            game = theGame;

            grabRacc = false;

            fenceHit = false;

            beanRun = false;
        }//end default constructor

        public override void Initialize()
        {

            base.Initialize();

        }//end initialize


        protected override void LoadContent()
        {
            spriteBatch = game.getSpriteBatch();
            Raccoon1 = game.getTexture(1);//load raccoon texture
            drawRaccoon = new Rectangle((int)Position.X, (int)Position.Y, Raccoon1.Width/2, Raccoon1.Height/2);
            screenWidth = game.getScreenWidth();
            screenHeight = game.getScreenHeight();
            maxRaccoon = game.getMaxRaccoon();
            totTooClosePos = Vector2.Zero;
            avgTooClosePos = Vector2.Zero;
            AvgSeperateDir = Vector2.Zero;
            fenceDir = Vector2.Zero;
            avgFlockDir = Vector2.Zero;
            avgFlockLookDir = Vector2.Zero;
            snackPos = game.getSnackLoc();
            snackDir = Vector2.Zero;
            currentTime = 0;
            elapsedTime = 0;
            eTime = 0;
            speed = 75f;
            stealTime = 0;
            resetTime = 0;
            beanTime = 0;

            //----testing init direction----
            //framesPerDir = 60;  // set to your frames Per Direction
            rand = game.getRand();

            Vector2 dir;
            dir.Y = (float)rand.NextDouble() - 0.5f;
            dir.X = (float)rand.NextDouble() - 0.5f;

            setDirection(dir);

            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (game.getState() == 0)
            {

                // get ticks since last frame

                elapsedTime = gameTime.ElapsedGameTime.Ticks;

                // convert ticks to seconds

                eTime = elapsedTime / TimeSpan.TicksPerSecond;


                //resetTime = elapsedTime / TimeSpan.TicksPerSecond;
                resetTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //Add the elapsed seconds so far

                if (resetTime > 1)
                {
                    stealing(game.getSnackLoc(), 40f);
                    resetTime = 0;
                }

                if (beanRun == true)
                {
                    beanTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (beanTime > 2)
                    {
                        beanRun = false;
                        beanTime = 0;
                    }
                }
                //stealing(game.getSnackLoc(), 40f);

                //setDirection(.6f * Direction + .2f * AvgSeperateDir + .1f * totAvgDir + .1f * AvgFlockDir);  //add alignment, cohesion, fleeing?
                wallCollider();

                if (beanRun == true)
                {
                    //run from beans
                    setDirection(awayBeans);
                    wallCollider();
                }
                else if(grabRacc == true)
                {
                    Position = game.getMouse();
                }
                else
                {
                    //+.5f * fenceDir
                    if (fenceHit == false)
                    {
                        setDirection(.5f * Direction + .1f * AvgSeperateDir + .1f * avgFlockLookDir + .1f * avgFlockDir + .2f * snackDir);  //add alignment, cohesion, fleeing?
                        wallCollider();
                    }
                    else if (fenceHit == true)
                    {
                        setDirection(fenceDir);
                    }
                   
                }
                //Direction.Normalize();
                Position += Direction * speed * eTime;
                //Position += Direction;
                //D = v + 1/2 at2
                drawRaccoon.X = (int)Position.X;
                drawRaccoon.Y = (int)Position.Y;
            }//end checkstate
        }//end Update

        public void setDirection(Vector2 inDir)
        {

            inDir.Normalize();

            Direction = inDir;

        }
         public bool checkDrag(Vector2 mousePos)
        {
            
            float mouseDistP1 = ((mousePos.X - Position.X) * (mousePos.X - Position.X) +
                                (mousePos.Y - Position.Y) * (mousePos.Y - Position.Y));

            if (mouseDistP1 < 32*32)
            {
                grabRacc = true;
            }
            return grabRacc;
        }

        public void fenceCollider(Vector2 p1, Vector2 p2)
        {
            double distP1;
            double distP2;

            distP1 = Math.Sqrt((Position.X - p1.X) * (Position.X - p1.X) + (Position.Y - p1.Y) * (Position.Y - p1.Y));
            distP2 = Math.Sqrt((Position.X - p2.X) * (Position.X - p2.X) + (Position.Y - p2.Y) * (Position.Y - p2.Y));

            double fenceDist;//find the distance to the line from my position
            fenceDist = (  Math.Abs(  ((p2.Y - p1.Y) * Position.X) - ((p2.X - p1.X) * Position.Y) + (p2.X * p1.Y) - (p2.Y * p1.X) )  ) /
                (  Math.Sqrt(  (p2.Y-p1.Y)*(p2.Y-p1.Y)  + (p2.X - p1.X) * (p2.X - p1.X) ) );

            double fenceLength = Math.Sqrt((p2.Y - p1.Y) * (p2.Y - p1.Y) + (p2.X - p1.X) * (p2.X - p1.X));//length of fence at any given time.

            if (fenceDist < 10 && distP1 < fenceLength && distP2 < fenceLength) // && distP1 < fenceLength && distP2 < fenceLength
            {
                fenceHit = true;
                //ignoring square root since comparing p1 and p2
                if (distP1 < distP2)
                {
                    fenceDir = p1 - p2;
                    fenceDir.Normalize();
                }
                else if (distP2 < distP1)
                {
                    fenceDir = p2 - p1;
                    fenceDir.Normalize();
                }
            }
            else
            {
                fenceHit = false;
            }

        }
        public void drag(Vector2 mousePos)
        {
            Position = mousePos;
        }

        public void letGo()
        {
            grabRacc = false;
        }

        public void setSpeed(float newSpeed)
        {
            speed = newSpeed;
        }

        public Vector2 getPosition()
        {
            return Position;
        }

        public void setPosition(Vector2 pos)
        {
            Position = pos;
        }

        public Vector2 getDirection()
        {
            return Direction;
        }

        public void wallCollider()
        {
            if (drawRaccoon.X < 0 && Direction.X < 0)
            {
                Direction.X *= -1;
            }
            else if (drawRaccoon.X > (screenWidth - Raccoon1.Width / 2) && Direction.X > 0)
            {
                Direction.X *= -1;
            }
            if (drawRaccoon.Y < 0 && Direction.Y < 0)
            {
                Direction.Y *= -1;
            }
            else if (drawRaccoon.Y > (screenHeight - Raccoon1.Height / 2) && Direction.Y > 0)
            {
                Direction.Y *= -1;
            }
        }

        public void Compare(Raccoon other)
        {
            
            if (this.near(other, 30f))
            {
                tooClose += 1;
                totTooClosePos += other.Position;
            }

        }
        void stealing(Vector2 snacks, float distance)
        {
            Vector2 myPos;
            Vector2 snackOffset;

            myPos = this.Position;
            snackOffset = snacks;
            snackOffset.X += 100;
            snackOffset.Y += 125;

            float collider = (float)Math.Sqrt((snackOffset.X - myPos.X) * (snackOffset.X - myPos.X) + (snackOffset.Y - myPos.Y) * (snackOffset.Y - myPos.Y));

            if (collider <= distance)
            {
                game.takeSnack();
            }
            
        }

        public void setAwayBeans(Vector2 beanPos)
        {
            awayBeans = Position - beanPos;
            beanRun = true;
        }

        public bool near(Raccoon them, float distance)
        {
            Vector2 myPos;
            Vector2 theirPos;

            myPos = this.Position;
            theirPos = them.Position;

            //float distance = 100;
            float collider = (float)Math.Sqrt((theirPos.X - myPos.X) * (theirPos.X - myPos.X) + (theirPos.Y - myPos.Y) * (theirPos.Y - myPos.Y));

            if (collider <= distance)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void Reset()
        {
            totTooClosePos = Vector2.Zero;
            tooClose = 0;
        }

        public void Seperate()
        {
            if (tooClose > 0)
            {
                avgTooClosePos = totTooClosePos / tooClose;
                AvgSeperateDir = this.Position - avgTooClosePos;
                if(!(AvgSeperateDir == Vector2.Zero))
                {
                    AvgSeperateDir.Normalize();
                }
                

            }
        }
        
        public void Align()
        {
            if (maxRaccoon > 0)
            {
                avgFlockLookDir = game.getAvgFlockLookDir();
                avgFlockLookDir.Normalize();
            }
        }
        
        public void Cohesion()
        {
            if (maxRaccoon > 0)
            {
                avgFlockDir = this.Position + game.getAvgFlockPos();
                avgFlockDir.Normalize();
            }
        }

        public void seekSnack()
        {
            Vector2 offset = new Vector2(100, 150);
            if (maxRaccoon > 0)
            {
                snackDir = (snackPos + offset) - this.Position;
                snackDir.Normalize();
            }
        }
    

        public override void Draw(GameTime gameTime)
        {
            //spriteBatch.Draw(loseScreen, winloseRect, null, Color.White, 0f, new Vector2(winScreen.Width / 2, winScreen.Height / 2), 0, 1f);

            spriteBatch.Draw(Raccoon1, drawRaccoon, null, Color.White, (float)Math.Atan2((double)Direction.X, (double)Direction.Y), new Vector2(drawRaccoon.Width/2, drawRaccoon.Height/2), 0, 0.1f);


        }//end Draw



    }//end raccoon class
}//end namespace

