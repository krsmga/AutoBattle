using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static AutoBattle.Types;
using static AutoBattle.EnumExtesions;

namespace AutoBattle
{
    public class Grid
    {
        public List<GridBox> grids = new List<GridBox>();

        // Setting the maximum size of rows and columns for the battlefield.
        public const int maxRows = 10;
        public const int maxCols = 10;

        // Set the default size of rows and columns for the battlefield.
        public const int defaultRows = 8;
        public const int defaultCols = 8;

        // Row and column size variables for control
        private int rowsSize = defaultRows;
        private int colsSize = defaultCols;

        // Saves the value of the characters' current health (Each turn)
        private int playerCurrentHealth;
        private int enemyCurrentHealth;

        // When the grid is created, it includes the listener events.
        public Grid(bool startEvents)
        {
            if (startEvents)
            {
                playerCurrentHealth = Character.initialHealth;
                enemyCurrentHealth = Character.initialHealth;

                Character.OnHealthChanged += HandleOnHealthChanged;
                Character.OnCharacterDied += HandleOnCharacterDeath;
            }
        }

        // When the grid is destroyed, it removes the listener events.
        ~Grid()
        {
            Character.OnHealthChanged -= HandleOnHealthChanged;
            Character.OnCharacterDied -= HandleOnCharacterDeath;
        }

        //I have changed the constructor method to a Create method. This way I can use the Rows & Columns        
        public void Create()
        {
            for (int i = 0; i < rowsSize; i++)
            {                    
                for(int j = 0; j < colsSize; j++)
                {
                    GridBox newBox = new GridBox(i+1, j+1, false, (colsSize * i + j));
                    //I moved grids.Add here to correct an intentional error.
                    grids.Add(newBox);                    
                }
            }
            DrawBattlefield();
        }

        // Must be used to set the value of battlefield rows, when the value is a string. (Overload)
        public bool SetRowsSize(string rows)
        {
            return SetRowsSize(Helper.GetIntParsed(rows));
        }

        // Must be used to set the value of battlefield rows, when the value is a int. (Overload)
        public bool SetRowsSize(int rows)
        {
            rowsSize = rows;
            bool isValid = IsValidGrid(GridCells.row);
            if (!isValid)
            {
                rowsSize = defaultRows;
            }
            return isValid;
        }

        // Used to get the size of rows. This way we keep the wrapping of the "rowsSize" variable which is now private.
        public int GetRowsSize()
        {
            return rowsSize;
        }

        // Must be used to set the value of battlefield columns, when the value is a string. (Overload)
        public bool SetColsSize(string cols)
        {
            return SetColsSize(Helper.GetIntParsed(cols));
        }

        // Must be used to set the value of battlefield columns, when the value is a int. (Overload)
        public bool SetColsSize(int cols)
        {
            colsSize = cols;
            bool isValid = IsValidGrid(GridCells.col);
            if (!isValid)
            {
                colsSize = defaultCols;
            }
            return isValid;
        }

        // Used to get the size of columns. This way we keep the wrapping of the "colsSize" variable which is now private.
        public int GetColsSize()
        {
            return colsSize;
        }

        // I modified the method because now I don't need the row and column parameters which are now encapsulated in the class. 
        public void DrawBattlefield()
        {
            Helper.PrintWellcome();

            // If no character has been chosen, we are creating the battlefield,
            // if one has already been chosen it shows the health information for each character.
            if (Program.characterSettings.playerIndex == -1)
            {
                Console.WriteLine($"A {this.rowsSize}x{this.colsSize} battlefield is ready!\n");
            }
            else
            {
                DrawCharacterHealthInfo("Player", Program.characterSettings.playerClass, playerCurrentHealth);
                DrawCharacterHealthInfo("Enemy", Program.characterSettings.enemyClass, enemyCurrentHealth);
                Console.Write(Environment.NewLine);
            }

            for (int i = 0; i < rowsSize; i++)
            {
                for (int j = 0; j < colsSize; j++)
                {
                    //Finds the grid index based on the values of (i) & (j)
                    int gridIndex = (i * colsSize) + j;
                    if (grids[gridIndex].ocupied)
                    {
                        if (grids[gridIndex].playerIndex == Program.characterSettings.playerIndex)
                        {
                            Console.ForegroundColor = Character.GetClassColor(Program.characterSettings.playerClass);
                            //As each class starts with a different letter I take the first letter of each.
                            Console.Write($"[{grids[gridIndex].characterClass.GetDisplayName()[0]}]\t");
                        }
                        else if (grids[gridIndex].playerIndex == Program.characterSettings.enemyIndex)
                        {
                            Console.ForegroundColor = Character.GetClassColor(Program.characterSettings.enemyClass);
                            //As each class starts with a different letter I take the first letter of each.
                            Console.Write($"[{grids[gridIndex].characterClass.GetDisplayName()[0]}]\t");
                        }
                    }
                    else
                    // Draw an empty battlefield
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("[ ]\t");
                    }
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }            
        }

        // Draws the health status information of the character for each turn
        public void DrawCharacterHealthInfo(string character, CharacterClass characterClass, int currentHealth)
        {
            int barCurrentHealth = currentHealth / 5;
            int barTotalHeatth = Character.initialHealth / 5;
            ConsoleColor classColor = Character.GetClassColor(characterClass);

            Console.ResetColor();
            Console.ForegroundColor = classColor;
            Console.Write($"{character}\t");
            Console.Write($"({characterClass.GetDisplayName()})\t");

            // Start printing each position of the battlefield grid
            Console.ForegroundColor = ConsoleColor.White;         
            Console.Write('[');
            Console.BackgroundColor = classColor;

            if (barCurrentHealth > 0)
            {
                for (int i = 0; i < barCurrentHealth; i++)
                {
                    Console.Write(' ');
                }
            }
            else if (currentHealth > 0 && barCurrentHealth == 0)
            {
                barTotalHeatth--;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = classColor;                
                Console.Write('|');                
            }

            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = barCurrentHealth; i < barTotalHeatth; i++)
            {
                Console.Write(' ');
            }

            Console.ResetColor();
            Console.Write(']');
            // End of printing each position of the battlefield grid

            // Character health value
            Console.Write($" {currentHealth}");

            // Prints the victory for the character that won
            if ((character == "Player" && enemyCurrentHealth <= 0) || (character == "Enemy" && playerCurrentHealth <= 0))
            {
                Console.ForegroundColor = classColor;
                Console.Write(" \\o/");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.Write(Environment.NewLine);
        }

        // Every time a character is attacked and its health value is changed you must update the UI.
        public void HandleOnHealthChanged(Character character)
        {
            if (character.playerIndex == Program.characterSettings.playerIndex)
            {
                playerCurrentHealth = character.Health;
            }
            else if (character.playerIndex == Program.characterSettings.enemyIndex)
            {
                enemyCurrentHealth = character.Health;
            }

            DrawBattlefield();
        }

        // Updates the battlefield grid to remove the dead character.
        public void HandleOnCharacterDeath(GridBox characterGridBox)
        {
            grids[characterGridBox.index] = characterGridBox;            
            DrawBattlefield();
        }

        // Validates whether the row or column value is valid for battlefield creation.
        public bool IsValidGrid(GridCells gridCell)
        {
            bool result = false;
            if (gridCell == GridCells.row)
            {
                result = rowsSize <= maxRows && rowsSize > 1;
            }
            else if (gridCell == GridCells.col)
            {
                result = colsSize <= maxCols && colsSize > 1;
            }
            return result;
        }
    }
}