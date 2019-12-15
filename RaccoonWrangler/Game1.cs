using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;//to access Debug

namespace RaccoonWrangler
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        float screenWidth, screenHeight;
        public SpriteBatch spriteBatch;
        SpriteFont Font1;
        Texture2D Raccoon1;
        Texture2D foodTruck;
        Texture2D Grass1;
        Texture2D Fence1;
        Texture2D pivotCirc;
        Texture2D winScreen;
        Texture2D loseScreen;
        Rectangle winloseRect;
        protected Rectangle drawRaccoon;
        protected Rectangle drawBG1;
        protected Rectangle drawBG2;
        Vector2 mouse;
        Raccoon[] raccoonArray; // raccoon array
        Fence[] fenceArray;
        protected int maxRaccoon;
        protected int maxFence;
        bool grabRacc;
        int deterent;
        Vector2 avgFlockPos;
        Vector2 avgFlockLookDir;
        Random rand;
        Vector2 snackLoc;
        int snackCount;
        Vector2 fenceALoc;
        Snackcart snackcart;
        float elapsedTime;
        float eTime;
        float roundTime;
        Vector2 beanLoc;
        int whichPivot;
        int whichFence;
        float beanCD = 0f;
        bool grabbed = false;
        bool fenceGrabbed = false;
        bool canBeans = true;
        int gameState;
        //Fence fenceA;
        //input stuff
        double mouseX1 = 0;
        double mouseY1 = 0;
        double mouseX2 = 0;
        double mouseY2 = 0;

        MouseState mouseState;

        //sound
        private SoundEffect musicBG;
        private SoundEffectInstance musicBGinst;
        private SoundEffect chitter;
        private SoundEffectInstance chitterInst;

        bool hasClicked = false;

        MouseState prevMouseState;
        KeyboardState prevKeyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //load sounds
            musicBG = Content.Load<SoundEffect>("banjo_backgroundmusic");
            chitter = Content.Load<SoundEffect>("raccoon_chitter_sfc");
            //load images
            Raccoon1 = Content.Load<Texture2D>("images/raccoon01");
            Grass1 = Content.Load<Texture2D>("images/Grass1");
            foodTruck = Content.Load<Texture2D>("images/snackcart_90");
            Fence1 = Content.Load<Texture2D>("images/fence1");
            pivotCirc = Content.Load<Texture2D>("images/pivotCirc");
            winScreen = Content.Load<Texture2D>("images/winscreen");
            loseScreen = Content.Load<Texture2D>("images/losescreen");
            //setgfx bookkeeping
            rand = new Random((int)DateTime.Now.Ticks);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            screenWidth = (float)(Window.ClientBounds.Width);
            screenHeight = (float)(Window.ClientBounds.Height);
            grabRacc = false;
            //instantiate various game objects
            maxRaccoon = 10;
            maxFence = 1;
            gameState = 0;
            deterent = 1;

            elapsedTime = 0;
            eTime = 0;
            roundTime = 60;

            fenceALoc.X = (screenWidth / 2);
            fenceALoc.Y = (screenHeight / 2);
            snackLoc.X = 0;
            snackLoc.Y = (screenHeight / 2) - (foodTruck.Height / 2);
            
            raccoonArray = new Raccoon[maxRaccoon];
            snackcart = new Snackcart(this, snackLoc.X, snackLoc.Y);
            fenceArray = new Fence[maxFence];
            //gamemode

            snackCount = 100;

            //fenceA = new Fence(this, fenceALoc.X, fenceALoc.Y);

            //adds to drawable
            Components.Add(snackcart);
            

            for (int i = 0; i < maxRaccoon; i++)
            {
                raccoonArray[i] = new Raccoon(this, 600, 200);
                Components.Add(raccoonArray[i]);//adds to drawable
            }
            for (int i = 0; i < maxFence; i++)
            {
                fenceArray[i] = new Fence(this, fenceALoc.X, fenceALoc.Y + i*30);
                Components.Add(fenceArray[i]);
            }
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);
            IsMouseVisible = true;
            Font1 = Content.Load<SpriteFont>("Impact");
            //drawBG1 = new Rectangle((int)0, (int)0, Grass1.Width/4, Grass1.Height/4);
            drawBG1 = new Rectangle((int)0, (int)0, (int)screenWidth*4, (int)screenHeight*4);
            drawBG2 = new Rectangle((int)Grass1.Width/4, (int)0, Grass1.Width/4, Grass1.Height/4);
            winloseRect = new Rectangle((int)screenWidth/2, (int)screenHeight/2, (int)winScreen.Width, (int)winScreen.Height);
            updateFlock();
            musicBGinst = musicBG.CreateInstance();
            musicBGinst.IsLooped = true;
            musicBGinst.Play();

            chitterInst = chitter.CreateInstance();
            chitterInst.Play();

            
            
            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            //calculate total Flock position and direction so that it only needs to be done once per frame instead of duplicated for every raccoon.
            updateFlock();
            //update m/kb inputs
            updateMouseInput();
            updateKeyboardInput();
            //update gamestate
            winLose();
            if (gameState == 0)
            {
                elapsedTime = gameTime.ElapsedGameTime.Ticks;

                // convert ticks to seconds

                eTime += elapsedTime / TimeSpan.TicksPerSecond;
                

                if (canBeans == false)
                {
                    beanCD += elapsedTime / TimeSpan.TicksPerSecond;
                    if(beanCD > 5)
                    {
                        canBeans = true;
                        beanCD = 0f;
                    }
                }

                // reset everybody
                for (int it = 0; it < maxRaccoon; it++)
                {
                    raccoonArray[it].Reset();
                }

                //iterate through each array each frame

                for (int it = 0; it < maxRaccoon; it++)//column
                {
                    for (int ot = 0; ot < maxRaccoon; ot++)//row
                    {
                        if (it != ot)//don't check self
                        {
                            //compare me to everyone else and update
                            raccoonArray[it].Compare(raccoonArray[ot]);

                        }
                    }

                }


                // calculate separation direction
                for (int it = 0; it < maxRaccoon; it++)
                {
                    raccoonArray[it].Seperate();
                    raccoonArray[it].Align();
                    raccoonArray[it].Cohesion();
                    raccoonArray[it].seekSnack();
                    raccoonArray[it].fenceCollider(fenceArray[0].getPivot1(), fenceArray[0].getPivot2());
                    
                }
            }
           



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.LinearWrap, null, null);
            // TODO: Add your drawing code here
            Vector2 end = new Vector2(screenHeight, screenWidth);
            spriteBatch.Draw(Grass1, Vector2.Zero, drawBG1, Color.White, 0, Vector2.Zero, .25F, SpriteEffects.None, 1f); //.25f texture scale, must make rect 4x as large and use linearWrap
            spriteBatch.DrawString(Font1, "Survival Time..." + eTime.ToString("f0"), Vector2.Zero, Color.Blue);
            spriteBatch.DrawString(Font1, "Remaining Snacks: " + snackCount.ToString(), new Vector2(0, 30), Color.Red);

            if (gameState == 2)
            {
                spriteBatch.Draw(winScreen, winloseRect, null,  Color.White, 0f, new Vector2(winScreen.Width/2, winScreen.Height/2), 0, 0f);
                //spriteBatch.Draw(fence1, drawFence, null, Color.White, 3.14159f * 2, fenceOrigin, 0, 1f);
            }
            if(gameState == 1)
            {
                spriteBatch.Draw(loseScreen, winloseRect, null, Color.White, 0f, new Vector2(winScreen.Width / 2, winScreen.Height / 2), 0, 0f);

            }
            //spriteBatch.Draw(Grass1, drawBG2, Color.White);
            //drawBG2.X += Grass1.Width;
            //spriteBatch.Draw(Grass1, drawBG2, Color.White);
            //spriteBatch.Draw(Raccoon1, drawRaccoon, Color.White);

            base.Draw(gameTime);

            spriteBatch.End();
        }

        public void updateFlock()
        {
            Vector2 sumFlockPos = Vector2.Zero;
            Vector2 sumFlockDir = Vector2.Zero;

            for (int i = 0; i < maxRaccoon; i++)
            {
                //add up and normalize flock dir and pos divided by number of flock
                //totAvgDir = sumDirection / totalFlock;
                sumFlockPos += raccoonArray[i].getPosition();
                sumFlockDir += raccoonArray[i].getDirection();
            }

            if (maxRaccoon > 0)
            {
                avgFlockPos = sumFlockPos / maxRaccoon;
                avgFlockLookDir = sumFlockDir / maxRaccoon;
            }
 
        }

        void winLose()
        {
            if (snackCount < 1)
            {
                //you lose
                snackCount = 0;
                gameState = 1;
            }

            if (eTime > roundTime)
            {
                //you win
                gameState = 2;
            }
        }

        public int getState()
        {
            return gameState;
        }

        public void takeSnack()
        {
            snackCount--;
        }

        public SpriteBatch getSpriteBatch()
        {
            return spriteBatch;
        }

        public Texture2D getTexture(int i)
        {
            switch (i)
            {
                case 1:
                    return Raccoon1;
                    
                case 2:
                    return foodTruck;

                case 3:
                    return Fence1;

                case 4:
                    return pivotCirc;
                    
                default:
                    return Raccoon1;
                    
            }
            
        }

        void updateMouseInput()
        {
            mouseState = Mouse.GetState();

            
            mouse = new Vector2(mouseState.X, mouseState.Y);
            // add code here to handle mouse window selection
            if (mouseState.LeftButton == ButtonState.Pressed  && grabbed == false)
            {
                //mouse = new Vector2(mouseState.X, mouseState.Y);
                for (int i = 0; i<maxFence; i++)
                {
                    whichPivot = fenceArray[i].checkPivot(mouse);
                    
                    if (whichPivot == 1 || whichPivot == 2)
                    {
                        whichFence = i;
                        grabbed = true;
                    } else if (fenceArray[i].grabbedFence(mouse))
                    {
                            whichFence = i;
                            grabbed = true;
                    }
                }
            }

            if (mouseState.LeftButton == ButtonState.Pressed && grabbed == true)
            {
                fenceArray[whichFence].setPivotLoc(whichPivot, mouse);

                if (whichPivot != 1 && whichPivot != 2)
                {
                    fenceArray[whichFence].updatePosition(mouse);
                }
            }

            if (mouseState.LeftButton == ButtonState.Released && grabbed == true)
            {
                whichPivot = -1;
                whichFence = 0;
                grabbed = false;
            }
            if (deterent == 1)
            {
                if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton != ButtonState.Pressed)
                {
                    //use deterent
                    if(canBeans == true)
                    {
                        Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
                        for (int i = 0; i < maxRaccoon; i++)
                        {
                            raccoonArray[i].setAwayBeans(mousePos);
                        }
                    }
                    canBeans = false;
                    
                }
            
            }
            else if (deterent == 2)
            {
                if (mouseState.RightButton == ButtonState.Pressed && grabRacc == false)
                {
                    grabRacc = true;
                    //mouse = new Vector2(mouseState.X, mouseState.Y);
                    for (int i = 0; i < maxRaccoon; i++)
                    {
                        bool gotGrabbed = raccoonArray[i].checkDrag(mouse);
                        if (gotGrabbed)
                        {
                            chitterInst.Play();
                        }
                        
                    }
                }

                if (mouseState.RightButton == ButtonState.Released && grabRacc == true)
                {
                    grabRacc = false;
                    for (int i = 0; i < maxRaccoon; i++)
                    {
                        raccoonArray[i].letGo();
                    }
                    
                }
            }

            prevMouseState = mouseState;
        }


        void updateKeyboardInput()
        {

            KeyboardState keyboardState = Keyboard.GetState();

            if ((keyboardState.IsKeyDown(Keys.R)) && (!prevKeyboardState.IsKeyDown(Keys.R)))
            {

                if (gameState != 0 )
                {
                    gameReset();
                }

            }
            if ((keyboardState.IsKeyDown(Keys.NumPad1)) && (!prevKeyboardState.IsKeyDown(Keys.NumPad1)))
            {

                deterent = 1;

            }
            if ((keyboardState.IsKeyDown(Keys.NumPad2)) && (!prevKeyboardState.IsKeyDown(Keys.NumPad2)))
            {

                deterent = 2;

            }

            prevKeyboardState = keyboardState;
        }

        public void useBeans()
        {
            //apply beans to every raccoon
            //within radius r
            //beans sets a location vector of mouse input
            //each raccoon calculates how to run the opposite way of it


        }

        public void grab()
        {

        }

        public Vector2 getMouse()
        {
            
            return new Vector2(mouseState.X, mouseState.Y);
        }

        void gameReset()
        {
            snackCount = 100;
            eTime = 0;
            for (int it = 0; it < maxRaccoon; it++)
            {
                raccoonArray[it].setPosition(new Vector2(600, 200));
                //todo: reset fence
                
            }
            gameState = 0;
        }

        public float getScreenWidth()
        {
            return screenWidth;
        }

        public float getScreenHeight()
        {
            return screenHeight;
        }

        public Random getRand()
        {
            return rand;
        }

        public int getMaxRaccoon()
        {
            return maxRaccoon;
        }

        public Vector2 getAvgFlockPos()
        {
            return avgFlockPos;
        }
        public Vector2 getAvgFlockLookDir()
        {
            return avgFlockLookDir;
        }

        public Vector2 getSnackLoc()
        {
            return snackLoc;
        }

    }
}
