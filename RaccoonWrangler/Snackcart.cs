using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RaccoonWrangler
{
    class Snackcart : DrawableGameComponent
    {
        protected Game1 game;
        protected Vector2 Position;
        protected Vector2 Direction;
        protected Texture2D foodTruck;
        protected int hp;

        protected Rectangle drawCart;
        protected SpriteBatch spriteBatch;
        protected float screenWidth;
        protected float screenHeight;

        
        public Snackcart(Game1 theGame, float ix, float iy) : base(theGame)
        {

            Position = new Vector2(ix, iy);
            game = theGame;

        }

        protected override void LoadContent()
        {
            spriteBatch = game.getSpriteBatch();
            foodTruck = game.getTexture(2);//load snackCart texture
            drawCart = new Rectangle((int)Position.X, (int)Position.Y, foodTruck.Width, foodTruck.Height);
            screenWidth = game.getScreenWidth();
            screenHeight = game.getScreenHeight();
            
        }

        public override void Draw(GameTime gameTime)
        {
            //spriteBatch.Draw(loseScreen, winloseRect, null, Color.White, 0f, new Vector2(winScreen.Width / 2, winScreen.Height / 2), 0, 1f);

            spriteBatch.Draw(foodTruck, drawCart, null, Color.White, 0f, Vector2.Zero, 0, 0.4f);

        }//end Draw

        

        public void takeSnack()
        {
            hp--;
        }

        public Vector2 getPosition()
        {
            return Position;
        }
    }
}
