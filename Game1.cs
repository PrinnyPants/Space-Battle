using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Space_Battle
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Model ships, enemyShip;
        Texture2D cosmos;
        SoundEffect explosion;
        Random rand = new Random();
        SpriteFont gameFont12, gameFont16, gameFont48;

        Vector3 location1, cameraPos;
        Vector3 enemyLocation;
        Matrix shipWorld1, camera, projection;

        float enemySpeed, width, height;
        bool gameStart = false;
        bool gameOver = false;
        bool playAgain = false;
        int lives = 0;
        float scoreTime = 0f;
        int highScore = 0;

        EnemyShips[] enemies = new EnemyShips[5]; 

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //--Main game things--//
            location1 = new Vector3(-9, 0, 0);
            width = _graphics.PreferredBackBufferWidth;
            height = _graphics.PreferredBackBufferHeight;
            cameraPos = new(-25, 0, 0);

            camera = Matrix.CreateRotationZ(MathHelper.ToRadians(90)) *
                Matrix.CreateLookAt(cameraPos, Vector3.Zero, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), width / height,
                0.1f, 100f);

            shipWorld1 = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90))
                * Matrix.CreateTranslation(location1);

            enemySpeed = 0.1f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ships = Content.Load<Model>("Ship");
            //--Attempted custom ship model but the mesh of the new model caused issues--//
            //enemyShip = Content.Load<Model>("Rocketship11");
            enemyShip = Content.Load<Model>("Ship");
            explosion = Content.Load<SoundEffect>("a_explode");
            cosmos = Content.Load<Texture2D>("cosmos");

            gameFont12 = Content.Load<SpriteFont>("gameFont12x");
            gameFont16 = Content.Load<SpriteFont>("gameFont");
            gameFont48 = Content.Load<SpriteFont>("gameFont48x");

            for (int i = 0; i < enemies.Length; i++)
            {
                float randZ = (float)rand.Next(-17, 17);
                enemyLocation = new Vector3(9, 0, randZ);
                enemies[i] = new EnemyShips(enemyShip, enemyLocation, enemySpeed);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

                //--The player's score is tied to the amount of time that they have played--//
                scoreTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (gameStart && !playAgain)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    location1 -= new Vector3(0, 0, 0.25f);
                    shipWorld1 = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90))
                    * Matrix.CreateTranslation(location1);
                    //--This ensures the player ship cannot go off screen to the left--//
                    if (location1.Z < -17)
                    {
                        location1 += new Vector3(0, 0, 0.25f);
                        shipWorld1 = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90))
                            * Matrix.CreateTranslation(location1);
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    location1 += new Vector3(0, 0, 0.25f);
                    shipWorld1 = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90))
                    * Matrix.CreateTranslation(location1);
                    //--This ensures the player ship cannot go off screen to the right--//
                    if (location1.Z > 17)
                    {
                        location1 -= new Vector3(0, 0, 0.25f);
                        shipWorld1 = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90))
                            * Matrix.CreateTranslation(location1);
                    }
                }

                foreach (var enemy in enemies)
                {
                    if ((int)scoreTime == 15)
                    {
                        enemySpeed = 0.2f;
                    }
                    else if ((int)scoreTime == 30)
                    {
                        enemySpeed = 0.3f;
                    }
                    else if ((int)scoreTime == 45)
                    {
                        enemySpeed = 0.4f;
                    }
                    else if ((int)scoreTime == 60)
                    {
                        enemySpeed = 0.5f;
                    }
                    else if ((int)scoreTime == 75)
                    {
                        enemySpeed = 0.6f;
                    }
                    else if ((int)scoreTime == 90)
                    {
                        enemySpeed = 0.7f;
                    }
                    else if ((int)scoreTime == 100)
                    {
                        enemySpeed = 1f;
                    }
                    enemy.Position -= new Vector3(enemySpeed, 0, 0);

                    if (enemy.Position.X < -11)
                    {
                        float randZ = rand.Next(-17, 17);
                        enemy.Position = new Vector3(9, 0, randZ);
                    }

                    if (shipsCollide(ships, enemyShip, shipWorld1, enemy.Position))
                    {
                        explosion.Play();
                        lives--;
                        float randZ = rand.Next(-17, 17);
                        enemy.Position = new Vector3(9, 0, randZ);
                    }

                    enemy.Update(gameTime, enemies);
                }
                if(lives == 0)
                {
                    highScore = (int)scoreTime;
                    scoreTime = 0;
                    enemySpeed = 0.1f;
                    gameStart = false;
                    playAgain = true;
                }
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    gameStart = true;
                }
            }

            if (playAgain)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    foreach(var enemy in enemies)
                    {
                        float randZ = rand.Next(-17, 17);
                        enemy.Position = new Vector3(9, 0, randZ);
                    }
                    lives = 5;
                    gameStart = true;
                    playAgain = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                    gameStart = false;
                    playAgain = false;
                    gameOver = true;
                }
            }
                
            
            base.Update(gameTime);

        }

        private bool shipsCollide(Model playerShip, Model enemyShip, Matrix shipWorld1, Vector3 enemyWorld)
        {
            bool collisionDetected = false;

            foreach(ModelMesh mesh1 in playerShip.Meshes)
            {
                BoundingSphere bs1 = mesh1.BoundingSphere;
                bs1 = bs1.Transform(shipWorld1);
                foreach(ModelMesh mesh2 in enemyShip.Meshes)
                {
                    BoundingSphere bs2 = mesh2.BoundingSphere;
                    bs2 = bs2.Transform(Matrix.CreateTranslation(enemyWorld));

                    if (bs1.Intersects(bs2))
                    {
                        collisionDetected = true;
                    }
                }
            }
            return collisionDetected;

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(cosmos, Vector2.Zero, Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin();
            if(!gameOver)
            {
                if(gameStart)
                {
                    _spriteBatch.DrawString(gameFont16, "Lives: " + lives, new Vector2(0, 25), Color.White);
                    _spriteBatch.DrawString(gameFont16, "Score: " + (int)scoreTime, new Vector2(0, 0), Color.White);

                    foreach (var enemy in enemies)
                    {
                        enemy.Draw(camera, projection, new Vector3(1,0,0));
                    }

                    DrawModel(ships, shipWorld1, camera, projection, new Vector3(0,1,0));
                }
                else if(!gameStart && playAgain)
                {
                    _spriteBatch.DrawString(gameFont16, "High Score: " + highScore, new Vector2(350, 0), Color.Black);
                    _spriteBatch.DrawString(gameFont16, "Press Space to Play Again", new Vector2(325, 200), Color.Black);
                    _spriteBatch.DrawString(gameFont16, "Press Q to Quit", new Vector2(325, 250), Color.Black);
                }
                else if(!gameStart)
                {
                    _spriteBatch.DrawString(gameFont48, "Space Battle", new Vector2(240, 100), Color.Black);
                    _spriteBatch.DrawString(gameFont16, "Press Space to Begin", new Vector2(325, 200), Color.Black);
                    _spriteBatch.DrawString(gameFont12, "Developed by", new Vector2(685, 435), Color.Black);
                    _spriteBatch.DrawString(gameFont12, "Nicholas Cordial", new Vector2(675, 455), Color.Black);
                }
                
            }
            else
            {
                _spriteBatch.DrawString(gameFont48, "Game Over", new Vector2(240, 100), Color.Black);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix camera, Matrix projection, Vector3 color)
        {
            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = world;
                    effect.View = camera;
                    effect.Projection = projection;

                    effect.DiffuseColor = color;
                }
                mesh.Draw();
            }
        }
    }
}
