using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace thegame
{
    class MapParser
    {

        private static bool initialized;

        static Dictionary<string, TypeValidatorEntry> supportedTypes;

        private static void initialize()
        {
            if (initialized)
                return;

            supportedTypes = new Dictionary<string, TypeValidatorEntry>();

            supportedTypes.Add("Texture", new TypeValidatorEntry("Texture", true, 1));
            supportedTypes.Add("Point", new TypeValidatorEntry("Point", true, 3));
            supportedTypes.Add("Tile", new TypeValidatorEntry("Tile", true, 2));
            supportedTypes.Add("Grid", new TypeValidatorEntry("Grid", true, 5));
            supportedTypes.Add("Cell", new TypeValidatorEntry("Cell", false, 5));
            supportedTypes.Add("Number", new TypeValidatorEntry("Number", true, 1));
            supportedTypes.Add("Boolean", new TypeValidatorEntry("Boolean", true, 1));
            supportedTypes.Add("String", new TypeValidatorEntry("String", true, 1));

            initialized = true;
        }

        public static void Parse(string fileName, DataManager manager, ContentManager content)
        {
            initialize();
            List<MapEntry> entries = parseFile(fileName);
            List<string> errors = CheckTypes(entries);
            if (errors.Count <= 0)
                translateEntries(entries, manager, content);
            else
                throw new Exception("There are errors!");

        }

        private static void translateEntries(List<MapEntry> entries, DataManager manager, ContentManager content)
        {
            Dictionary<string, EPoint> points = new Dictionary<string, EPoint>();
            Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
            Dictionary<string, ETile> tiles = new Dictionary<string, ETile>();
            Dictionary<string, Grid> grids = new Dictionary<string, Grid>();
            Dictionary<string, ECell> cells = new Dictionary<string, ECell>();
            Dictionary<string, double> numbers = new Dictionary<string, double>();
            Dictionary<string, bool> booleans = new Dictionary<string, bool>();
            Dictionary<string, string> strings = new Dictionary<string, string>();

            foreach (MapEntry entry in entries)
            {
                if (entry.TypeName == "Texture")
                {
                    Texture2D texture = content.Load<Texture2D>(entry[0]);
                    textures.Add(entry.EntryName, texture);
                }
                else if (entry.TypeName == "Point")
                {
                    EPoint point = new EPoint(
                        int.Parse(entry[0]),
                        int.Parse(entry[1]),
                        int.Parse(entry[2]));
                    points.Add(entry.EntryName, point);
                }
                else if (entry.TypeName == "Tile")
                {
                    ETile tile = new ETile();
                    tile.Texture = textures[entry[0]];
                    tile.Solid = bool.Parse(entry[1]);

                    tiles.Add(entry.EntryName, tile);
                }
                else if (entry.TypeName == "Grid")
                {
                    EPoint gridSize = points[entry[0]];
                    EPoint gridOrigin = points[entry[1]];
                    EPoint gridTileOffset = points[entry[2]];
                    EPoint gridTileSize = points[entry[3]];
                    EPoint playerStart = points[entry[4]];

                    Grid grid = new Grid(
                        gridSize.X,
                        gridSize.Y,
                        gridSize.Z,
                        null,
                        gridTileOffset.X,
                        gridTileOffset.Y,
                        gridTileSize.X,
                        gridTileSize.Y,
                        gridTileOffset.Z,
                        gridOrigin.X,
                        gridOrigin.Y);

                    grid.ShadowEast = content.Load<Texture2D>("Block Shadow\\Shadow East");
                    grid.ShadowEast = content.Load<Texture2D>("Block Shadow\\Shadow North East");
                    grid.ShadowEast = content.Load<Texture2D>("Block Shadow\\Shadow North West");
                    grid.ShadowEast = content.Load<Texture2D>("Block Shadow\\Shadow North");
                    grid.ShadowEast = content.Load<Texture2D>("Block Shadow\\Shadow West");
                    grid.ShadowEast = content.Load<Texture2D>("Block Shadow\\Shadow South East");
                    grid.ShadowEast = content.Load<Texture2D>("Block Shadow\\Shadow South West");
                    grid.ShadowEast = content.Load<Texture2D>("Block Shadow\\Shadow South");
                    grid.ShadowEast = content.Load<Texture2D>("Block Shadow\\Shadow East");

                    grids.Add(entry.EntryName, grid);
                }
                else if (entry.TypeName == "Cell")
                {
                    ECell cell = new ECell();

                    cell.Point = new EPoint(int.Parse(entry[0]), int.Parse(entry[1]), int.Parse(entry[2]));     
                    cell.Grid = grids[entry[3]];
                    cell.Tile = tiles[entry[4]];

                    cell.Grid.setTile(
                        cell.Point.X,
                        cell.Point.Y,
                        cell.Point.Z,
                        cell.Tile.Texture,
                        cell.Tile.Solid);

                    if (!string.IsNullOrEmpty(entry.EntryName))
                        cells.Add(entry.EntryName, cell);
                }
                else if (entry.TypeName == "Number")
                {
                    numbers.Add(entry.EntryName, double.Parse(entry[0]));
                }
                else if (entry.TypeName == "Boolean")
                {
                    booleans.Add(entry.EntryName, bool.Parse(entry[0]));
                }
                else if (entry.TypeName == "String")
                {
                    strings.Add(entry.EntryName, entry[0]);
                }
            }

            foreach (string key in grids.Keys)
            {
                manager.AddItem(key, grids[key]);
            }

            foreach (string key in textures.Keys)
            {
                manager.AddItem(key, textures[key]);
            }
        }

        private static List<MapEntry> parseFile(string fileName)
        {
            StreamReader stream = new StreamReader(fileName);
            List<MapEntry> entries = new List<MapEntry>();

            int lineCount = -1;
            while (!stream.EndOfStream)
            {
                string line = stream.ReadLine().Trim();
                lineCount++;

                if (line.StartsWith("#"))
                    continue;

                if (line == "")
                    continue;

                MapEntry entry = parseLine(line);
                entry.LineNumber = lineCount;
                entries.Add(parseLine(line));
            }

            return entries;
        }
        private static MapEntry parseLine(string line)
        {
            MapEntry entry = new MapEntry();

            int mode = 0;
            //00: TypeName
            //01: Name
            string current = "";
            foreach (char c in line)
            {
                if (mode == 0)
                {
                    if (c != '[')
                        current += c;
                    else
                    {
                        entry.TypeName = current.Trim();
                        current = "";
                        mode = 1;
                    }
                }
                else if (mode == 1)
                {
                    if (c != ']')
                        current += c;
                    else
                    {
                        entry.EntryName = current.Trim();
                        current = "";
                        mode = 2;
                    }
                }
                else if (mode == 2)
                {
                    if (c != '|')
                        current += c;
                    else
                    {
                        entry.Values.Add(current.Trim());
                        current = "";
                    }
                }
            }

            entry.Values.Add(current.Trim());
            return entry;
        }

        private static List<string> CheckTypes(List<MapEntry> entries)
        {
            List<string> errors = new List<string>();

            foreach (MapEntry entry in entries)
            {
                if (!supportedTypes.ContainsKey(entry.TypeName))
                {
                    errors.Add(string.Format("[{0}] {1}", entry.LineNumber, "Invalid type."));
                    continue;
                }
                
                if(supportedTypes[entry.TypeName].RequiresName && (entry.EntryName == string.Empty))
                {
                    errors.Add(string.Format("[{0}] {1}", entry.LineNumber, "This type requires a name."));
                }

                if (supportedTypes[entry.TypeName].ArgumentCount != entry.Values.Count)
                {
                    errors.Add(string.Format("[{0}] {1}", entry.LineNumber, "Invalid number of values."));

                }                
            }

            return errors;
        }

        private class MapEntry
        {
            public string TypeName { get; set; }
            public string EntryName { get; set; }
            public List<string> Values { get; private set; }
            public int LineNumber { get; set; }

            public string this[int index]
            {
                get
                {
                    return Values[index];
                }
            }

            public MapEntry()
            {
                Values = new List<string>();
            }
        }

        private class TypeValidatorEntry
        {
            public string TypeName { get; private set; }
            public bool RequiresName { get; private set; }
            public int ArgumentCount { get; private set; }


            public TypeValidatorEntry(string typeName, bool requiresName, int valueCount)
            {
                TypeName = typeName;
                RequiresName = requiresName;
                ArgumentCount = valueCount;
            }
        }

        private class EPoint
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }

            public EPoint(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public EPoint()
            {

            }
        }

        private class ETile
        {
            public Texture2D Texture { get; set; }
            public bool Solid { get; set; }            
        }

        private class ECell
        {
            public ETile Tile { get; set; }
            public EPoint Point { get; set; }
            public Grid Grid { get; set; }
        }


        

    }
}
