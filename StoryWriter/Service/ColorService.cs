using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoryWriter.Models;

namespace StoryWriter.Service
{
    public static class ColorService
    {
        public static List<PlayerColor> Colors = new List <PlayerColor>
        {
            new PlayerColor { Name = "RED", HexCode = "#F44336" },
            new PlayerColor { Name = "PINK", HexCode = "#E91E63" },
            new PlayerColor { Name = "PURPLE", HexCode = "#9C27B0" },
            new PlayerColor { Name = "DEEP_PURPLE", HexCode = "#673AB7" },
            new PlayerColor { Name = "INGIGO", HexCode = "#3F51B5" },
            new PlayerColor { Name = "BLUE", HexCode = "#2196F3" },
            new PlayerColor { Name = "LIGHT_BLUE", HexCode = "#03A9F4" },
            new PlayerColor { Name = "CYAN", HexCode = "#00BCD4" },
            new PlayerColor { Name = "TEAL", HexCode = "#009688" },
            new PlayerColor { Name = "GREEN", HexCode = "#4CAF50" },
            new PlayerColor { Name = "LIGHT_GREEN", HexCode = "#8BC34A" },
            new PlayerColor { Name = "LIME", HexCode = "#CDDC39" },
            new PlayerColor { Name = "YELLOW", HexCode = "#FFEB3B" },
            new PlayerColor { Name = "AMBER", HexCode = "#FFC107" },
            new PlayerColor { Name = "ORANGE", HexCode = "#FF9800" },
            new PlayerColor { Name = "DEEP_ORANGE", HexCode = "#FF5722" },
            new PlayerColor { Name = "BROWN", HexCode = "#795548" },
            new PlayerColor { Name = "GREY", HexCode = "#9E9E9E" },
            new PlayerColor { Name = "BLUE_GREY", HexCode =  "#607D8B" }
        };

        public static PlayerColor RandomColor (List<PlayerColor> ExcludedColors)
        {
            var shortList = Colors.Except(ExcludedColors);
            var rng = new System.Random();
            var randomColorIndex = rng.Next(0, shortList.Count() - 1);
            return shortList.ElementAt(randomColorIndex);
        }
    }
}