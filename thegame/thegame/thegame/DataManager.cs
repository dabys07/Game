using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace thegame
{
    class DataManager
    {
        private Dictionary<string, Grid> grids;
        private Dictionary<string, Texture2D> textures;

        public void AddItem(string name, Texture2D texture)
        {
            textures.Add(name, texture);
        }

        public void AddItem(string name, Grid grid)
        {
            grids.Add(name, grid);
        }

        public Grid getGrid(string name)
        {
            return grids[name];
        }

        public Texture2D getTexture(string name)
        {
            return textures[name];
        }

        public void Clear()
        {
            grids.Clear();
            textures.Clear();
        }

        public DataManager()
        {
            grids = new Dictionary<string, Grid>();
            textures = new Dictionary<string, Texture2D>();
        }
        
    }
}
