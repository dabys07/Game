using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace thegame
{
    class Grid
    {
        Texture2D[, ,] textures;
        Texture2D[, ,] shadows;
        Rectangle[, ,] tileBounds;
        Rectangle[, ,] walkBounds;
        bool[, ,] solidity;
        

        public int Width { get; private set; }
        public int Heigth { get; private set; }
        public int Depth { get; private set; }

        public int ZOffset { get; private set; }

        public int TopOffset { get; private set; }
        public int BottomOffset { get; private set; }

        public int CellWidth { get; private set; }
        public int CellHeigth { get; private set; }

        public int GridOriginX { get; private set; }
        public int GridOriginY { get; private set; }

        public bool EnableShadows { get; set; }

        public Texture2D ShadowNorth { get; set; }
        public Texture2D ShadowNorthEast { get; set; }
        public Texture2D ShadowEast { get; set; }
        public Texture2D ShadowSouthEast { get; set; }
        public Texture2D ShadowSouth { get; set; }
        public Texture2D ShadowSouthWest { get; set; }
        public Texture2D ShadowWest { get; set; }
        public Texture2D ShadowNorthWest { get; set; }


        public Grid(int gridSizeX, int gridSizeY, int gridSizeZ, Texture2D defaultTexture, int topOffset, int bottomOffset, int cellWidth, int cellHeigth, int zOffset, int originX, int originY)
        {
            Width = gridSizeX;
            Heigth = gridSizeY;
            Depth = gridSizeZ;

            CellHeigth = cellHeigth;
            CellWidth = cellWidth;

            GridOriginX = originX;
            GridOriginY = originY;

            ZOffset = zOffset;
            TopOffset = topOffset;
            BottomOffset = bottomOffset;

            textures = new Texture2D[Width, Heigth, Depth];
            tileBounds = new Rectangle[Width, Heigth, Depth];
            walkBounds = new Rectangle[Width, Heigth, Depth];
            solidity = new bool[gridSizeX, Heigth, Depth];
            shadows = new Texture2D[Width, Heigth, Depth];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Heigth; y++)
                {
                    for (int z = 0; z < Depth; z++)
                    {
                        setTile(x, y, z, defaultTexture);
                    }
                }
            }
        }

        

        public void setTile(int x, int y, int z, Texture2D texture)
        {
            setTile(x, y, z, texture, false);
        }

        public void setTile(int x, int y, int z, Texture2D texture, bool solid)
        {
            int offsetX = (x * CellWidth) + GridOriginX;
            int offsetY = (y * CellHeigth - (y * (TopOffset + BottomOffset))) + GridOriginY;
            int offsetZ = z * ZOffset;

            textures[x, y, z] = texture;
            tileBounds[x, y, z] = new Rectangle(offsetX, offsetY - offsetZ, CellWidth, CellHeigth);
            solidity[x, y, z] = solid;

            if (EnableShadows)
            {
                Point[] points = new Point[8];
                Point p;
                int zminus = z - 1;

                if (zminus >= 0)
                {

                    if (IsValidCell(x - 1, y + 1))
                    {
                        p = new Point(x - 1, y + 1);
                        if (textures[p.X, p.Y, zminus] != null)
                        {
                            shadows[p.X, p.Y, zminus] = ShadowSouthEast;
                        }
                    }

                    if (IsValidCell(x, y + 1))
                    {
                        p = (new Point(x, y + 1));
                        if (textures[p.X, p.Y, zminus] != null)
                        {
                            shadows[p.X, p.Y, zminus] = ShadowSouth;
                        }

                    }

                    if (IsValidCell(x + 1, y + 1))
                    {
                        p = (new Point(x + 1, y + 1));
                        if (textures[p.X, p.Y, zminus] != null)
                        {
                            shadows[p.X, p.Y, zminus] = ShadowSouthWest;
                        }
                    }

                    if (IsValidCell(x - 1, y))
                    {
                        p = (new Point(x - 1, y));
                        if (textures[p.X, p.Y, zminus] != null)
                        {
                            shadows[p.X, p.Y, zminus] = ShadowEast;
                        }
                    }

                    if (IsValidCell(x + 1, y))
                    {
                        p = (new Point(x + 1, y));
                        if (textures[p.X, p.Y, zminus] != null)
                        {
                            shadows[p.X, p.Y, zminus] = ShadowWest;
                        }
                    }

                    if (IsValidCell(x - 1, y - 1))
                    {
                        p = (new Point(x - 1, y - 1));
                        if (textures[p.X, p.Y, zminus] != null)
                        {
                            shadows[p.X, p.Y, zminus] = ShadowNorthEast;
                        }
                    }

                    if (IsValidCell(x, y - 1))
                    {
                        p = (new Point(x, y - 1));
                        if (textures[p.X, p.Y, zminus] != null)
                        {
                            shadows[p.X, p.Y, zminus] = ShadowSouth;
                        }
                    }


                    if (IsValidCell(x + 1, y - 1))
                    {
                        p = (new Point(x, y - 1));
                        if (textures[p.X, p.Y, zminus] != null)
                        {
                            shadows[p.X, p.Y, zminus] = ShadowNorthWest;
                        }
                    }
                }
            }
        }
      

        public bool IsValidCell(int x, int y)
        {
            if (x >= 0 && y >= 0)
                if (x < Width && y < Heigth)
                    return true;

            return false;
        }

        private List<Point> getAdjancentPoints(int x, int y)
        {
            List<Point> points = new List<Point>();

            

            if (IsValidCell(x + 1, y - 1))
                points.Add(new Point(x + 1, y - 1));


            return points;
        }

        public bool isSolid(int x, int y, int z)
        {
            return solidity[x, y, z];
        }

        public void Render(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Heigth; y++)
                {
                    for (int z = 0; z < Depth; z++)
                    {
                        if (textures[x, y, z] != null)
                            spriteBatch.Draw(textures[x, y, z], tileBounds[x, y, z], Color.White);
                        if(shadows[x,y,z] != null)
                            spriteBatch.Draw(shadows[x, y, z], tileBounds[x, y, z], Color.White);
                    }
                }
            }
        }

    }
}
