using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RaccoonWrangler
{
    class Fence : DrawableGameComponent
    {
        protected Vector2 Position;
        protected Game1 game;
        protected Texture2D fence1;
        protected Texture2D pivotCirc;

        protected Rectangle drawFence;
        protected Rectangle drawPivot1;
        protected Rectangle drawPivot2;
        protected float pivotCenterOffset;
        protected SpriteBatch spriteBatch;
        protected float screenWidth;
        protected float screenHeight;
        protected float fenceAngle;

        protected float length;
        

        //protected Vector2 Direction; //prolly dont need direction

        //pivot stuff
        Vector2 mouse;
        Vector2 fenceOrigin;
        Vector2 pivot1Origin;//default left
        Vector2 pivot2Origin;//default right
        float pivotR = 20f;
        int whichPivot = -1;


        public Fence(Game1 theGame, float ix, float iy) : base(theGame)
        {
            Position = new Vector2(ix, iy);
            game = theGame;
            length = 200f;
        }//end constructor

        protected override void LoadContent()
        {
            spriteBatch = game.getSpriteBatch();
            fence1 = game.getTexture(3);//load fence texture
            pivotCirc = game.getTexture(4);//load raccoon texture
            drawPivot1 = new Rectangle((int)(Position.X), (int)(Position.Y + length/2.0f), pivotCirc.Width, pivotCirc.Height);
            drawPivot2 = new Rectangle((int)(Position.X), (int)(Position.Y - length/2.0f), pivotCirc.Width, pivotCirc.Height);
            drawFence = new Rectangle((int)Position.X, (int)Position.Y, fence1.Width, fence1.Height);
            screenWidth = game.getScreenWidth();
            screenHeight = game.getScreenHeight();
            pivotCenterOffset = pivotCirc.Height / 2;
            fenceAngle = 3.1415f/2; //in radians
            //pivot1 = new Vector2((Position.X), Position.Y-(fence1.Height/2));
            //pivot2 = new Vector2((Position.X + fence1.Width), Position.Y - (fence1.Height / 2));
            fenceOrigin = new Vector2(fence1.Width / 2, fence1.Height / 2);
            pivot1Origin = new Vector2(pivotCirc.Width / 2, pivotCirc.Height / 2);//default left
            pivot2Origin = new Vector2(pivotCirc.Width / 2, pivotCirc.Height / 2);//default right


        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
            

        public int checkPivot(Vector2 mousePos)
        {
            float mouseDistP1 = ((mousePos.X - drawPivot1.X) * (mousePos.X - drawPivot1.X) + 
                                (mousePos.Y - drawPivot1.Y) * (mousePos.Y - drawPivot1.Y));

            float mouseDistP2 = ((mousePos.X - drawPivot2.X ) * (mousePos.X - drawPivot2.X) +
                                (mousePos.Y - drawPivot2.Y) * (mousePos.Y - drawPivot2.Y));

            whichPivot = -1;

            
            if ((pivotR * pivotR) > mouseDistP1)
            {
                whichPivot = 1;
                
            }

            if ((pivotR * pivotR) > mouseDistP2)
            {
                whichPivot = 2;
                
            }

            return whichPivot;
        }

        public bool grabbedFence(Vector2 mousePos)
        {
            double distP1;
            double distP2;

            distP1 = Math.Sqrt((mousePos.X - drawPivot1.X) * (mousePos.X - drawPivot1.X) + (mousePos.Y - drawPivot1.Y) * (mousePos.Y - drawPivot1.Y));
            distP2 = Math.Sqrt((mousePos.X - drawPivot2.X) * (mousePos.X - drawPivot2.X) + (mousePos.Y - drawPivot2.Y) * (mousePos.Y - drawPivot2.Y));

            double fenceDist;//find the distance to the line from my position
            fenceDist = (Math.Abs(((drawPivot2.Y - drawPivot1.Y) * mousePos.X) - ((drawPivot2.X - drawPivot1.X) * mousePos.Y) + (drawPivot2.X * drawPivot1.Y) - (drawPivot2.Y * drawPivot1.X))) /
                (Math.Sqrt((drawPivot2.Y - drawPivot1.Y) * (drawPivot2.Y - drawPivot1.Y) + (drawPivot2.X - drawPivot1.X) * (drawPivot2.X - drawPivot1.X)));

            double fenceLength = Math.Sqrt((drawPivot2.Y - drawPivot1.Y) * (drawPivot2.Y - drawPivot1.Y) + (drawPivot2.X - drawPivot1.X) * (drawPivot2.X - drawPivot1.X));//length of fence at any given time.

            if (fenceDist < 10 && distP1 < fenceLength && distP2 < fenceLength) // && distP1 < fenceLength && distP2 < fenceLength
            {
                return true;
            }

            return false;
        }

        public void setPivotLoc(int pivotIndex, Vector2 mouse)
        {
            Vector2 pToMouse;
            Vector2 p1 = new Vector2(drawPivot1.X, drawPivot1.Y);
            Vector2 p2 = new Vector2(drawPivot2.X, drawPivot2.Y);
            Vector2 correctedDist;
            if (pivotIndex == 1)
            {
                pToMouse = mouse - p2;
                pToMouse.Normalize();

                correctedDist = p2 + (length * pToMouse);
                

                //drawPivot1 = new Rectangle((int)(correctedDist.X), (int)(correctedDist.Y), pivotCirc.Width, pivotCirc.Height);
                drawPivot1.X = (int)correctedDist.X;
                drawPivot1.Y = (int)correctedDist.Y;

                Vector2 midpoint = p2 + (length*0.5f * pToMouse);
                drawFence.X = (int)midpoint.X;
                drawFence.Y = (int)midpoint.Y;

                fenceAngle = (float)Math.Atan2(drawPivot1.Y- drawPivot2.Y , drawPivot1.X- drawPivot2.X);
            }
            if (pivotIndex == 2)
            {
                pToMouse = mouse - p1;
                pToMouse.Normalize();

                correctedDist = p1 + (length * pToMouse);

                //drawPivot2 = new Rectangle((int)(correctedDist.X), (int)(correctedDist.Y), pivotCirc.Width, pivotCirc.Height);
                drawPivot2.X = (int)correctedDist.X;
                drawPivot2.Y = (int)correctedDist.Y;

                Vector2 midpoint = p1 + (length * 0.5f * pToMouse);
                drawFence.X = (int)midpoint.X;
                drawFence.Y = (int)midpoint.Y;

                fenceAngle = (float)Math.Atan2(drawPivot2.Y - drawPivot1.Y , drawPivot2.X - drawPivot1.X);
            }
        }

        public void updatePosition(Vector2 mouse)
        {
            Vector2 p1 = new Vector2(drawPivot1.X, drawPivot1.Y);
            Vector2 p2 = new Vector2(drawPivot2.X, drawPivot2.Y);

            Vector2 v = p2 - p1;
            v.Normalize();

            Vector2 newp2 = mouse + (length * .5f * v);
            Vector2 newp1 = mouse - (length * .5f * v);

            drawPivot1.X = (int)newp1.X;
            drawPivot1.Y = (int)newp1.Y;

            drawPivot2.X = (int)newp2.X;
            drawPivot2.Y = (int)newp2.Y;

            drawFence.X = (int)mouse.X;
            drawFence.Y = (int)mouse.Y;
        }

        public Vector2 getPivot1()
        {
            return new Vector2(drawPivot1.X, drawPivot1.Y);
        }

        public Vector2 getPivot2()
        {
            return new Vector2(drawPivot2.X, drawPivot2.Y);
        }


        public override void Draw(GameTime gameTime)
        {
            
            spriteBatch.Draw(fence1, drawFence, null, Color.White, fenceAngle, fenceOrigin, 0, 0.25f);
            spriteBatch.Draw(pivotCirc, drawPivot1, null, Color.White, 0f, pivot1Origin, 0, 0.2f);
            spriteBatch.Draw(pivotCirc, drawPivot2, null, Color.White, 0f, pivot2Origin, 0, 0.2f);

        }//end Draw


    }//end class
}//end namespace
