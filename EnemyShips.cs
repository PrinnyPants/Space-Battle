using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Battle
{
    public class EnemyShips
    {
        private Model enemyShip;
        private Random rand = new Random();
        private Vector3 enemyLocation;
        private Matrix enemyWorld;
        public Vector3 Position
        {
            get { return enemyLocation; }
            set { enemyLocation = value; }
        }
        public EnemyShips(Model model, Vector3 location, float speed)
        { 
            this.enemyShip = model;
            this.enemyLocation = location;
        }
        public void Update(GameTime gameTime, EnemyShips[] otherEnemies)
        {
            enemyWorld = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                * Matrix.CreateTranslation(enemyLocation);

            foreach(var otherEnemy in otherEnemies)
            {
                if(otherEnemy != this && shipsOverlap(enemyShip, enemyShip, enemyLocation, otherEnemy.Position))
                {
                    float randZ = rand.Next(-17, 17);
                    enemyLocation = new Vector3(9, 0, randZ);
                    enemyWorld = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                        * Matrix.CreateTranslation(enemyLocation);
                }
            }
        }

        private bool shipsOverlap(Model ship1, Model ship2, Vector3 enemyPos, Vector3 otherEnemyPos)
        {
            bool overlapDetected = false;

            foreach (ModelMesh mesh1 in ship1.Meshes)
            {
                BoundingSphere bs1 = mesh1.BoundingSphere;
                bs1 = bs1.Transform(Matrix.CreateTranslation(enemyPos));
                foreach (ModelMesh mesh2 in enemyShip.Meshes)
                {
                    BoundingSphere bs2 = mesh2.BoundingSphere;
                    bs2 = bs2.Transform(Matrix.CreateTranslation(otherEnemyPos));

                    if (bs1.Intersects(bs2))
                    {
                        overlapDetected = true;
                    }
                }
            }
            return overlapDetected;
        }

        public void Draw(Matrix camera, Matrix projection, Vector3 color)
        {
            //--For custom ship model--//
            //enemyWorld = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-90)) *
            //   Matrix.CreateScale(0.25f) * Matrix.CreateTranslation(enemyLocation);
            enemyWorld = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationZ(MathHelper.ToRadians(90))
                * Matrix.CreateTranslation(enemyLocation);
            DrawModel(enemyShip, enemyWorld, camera, projection, color);
        }

        private void DrawModel(Model model, Matrix world, Matrix camera, Matrix projection, Vector3 color)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
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
