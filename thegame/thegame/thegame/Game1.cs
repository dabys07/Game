using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace thegame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>    
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont TitleFont { get; set; }
        SpriteFont MenuFont { get; set; }
        Player player;
        Grid grid;

        TimeSpan inputDelayCurrent;
        TimeSpan inputDelayMax;


        const bool enablePlayer = true;


        DataManager DataManager { get; set; }

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

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 1024;
            graphics.ApplyChanges();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TitleFont = Content.Load<SpriteFont>("TitleFont");
            MenuFont = Content.Load<SpriteFont>("ButtonFont");

            player = new Player();
            player.X = 0;
            player.Y = 0;
            player.Z = 1;
            player.Texture = Content.Load<Texture2D>("Character\\Character Boy");

            inputDelayMax = new TimeSpan(0, 0, 0, 0, 100);
            inputDelayCurrent = new TimeSpan();

            DataManager = new thegame.DataManager();

            MapParser.Parse("maps\\demo.txt", DataManager, Content);
            grid = DataManager.getGrid("start");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            inputDelayCurrent += gameTime.ElapsedGameTime;


            int newX = player.X, newY = player.Y, newZ = player.Z;
            if (enablePlayer)
            {
                if (inputDelayCurrent >= inputDelayMax)
                {
                    inputDelayCurrent = new TimeSpan();


                    KeyboardState kState = Keyboard.GetState();


                    if (kState.IsKeyDown(Keys.Down))
                    {
                        newY++;
                        if (newY >= grid.Heigth)
                            newY--;
                    }
                    else if (kState.IsKeyDown(Keys.Up))
                    {
                        newY--;
                        if (newY < 0)
                            newY++;
                    }
                    else if (kState.IsKeyDown(Keys.Right))
                    {
                        newX++;
                        if (newX >= grid.Heigth)
                            newX--;
                    }
                    else if (kState.IsKeyDown(Keys.Left))
                    {
                        newX--;
                        if (newX < 0)
                            newX++;
                    }
                }




                if (!grid.isSolid(newX, newY, newZ))
                {
                    if (grid.isSolid(newX, newY, newZ - 1))
                    {
                        grid.setTile(player.X, player.Y, player.Z, null, false);
                        player.X = newX;
                        player.Y = newY;
                        player.Z = newZ;
                    }
                }


                grid.setTile(player.X, player.Y, player.Z, player.Texture, true);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            grid.Render(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
