using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static AutoBattle.Types;

namespace AutoBattle
{
    public class Grid
    {
        // Setting the maximum size of rows and columns for the battlefield.
        private const int maxRows = 10;
        private const int maxCols = 10;

        // Set the default size of rows and columns for the battlefield.
        private const int defaultRows = 8;
        private const int defaultCols = 8;

        // Row and column size variables for control
        private int rowsSize = defaultRows;
        private int colsSize = defaultCols;

        public List<GridBox> grids = new List<GridBox>();

        //I have changed the constructor method to a Create method. This way I can use the Rows & Columns
        //sets first and then run the battlefield creation.
        public void Create()
        {
            Console.WriteLine("The battle field has been created ({0}x{1})\n",rowsSize,colsSize);
            for (int i = 0; i < rowsSize; i++)
            {
                    
                for(int j = 0; j < colsSize; j++)
                {
                    GridBox newBox = new GridBox(j, i, false, (colsSize * i + j));
                    //I moved grids.Add here to correct an intentional error.
                    grids.Add(newBox);                    
                }
            }
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

        // Prints the matrix that indicates the tiles of the battlefield
        public void drawBattlefield(int Lines, int Columns)
        {
            for (int i = 0; i < Lines; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    GridBox currentgrid = new GridBox();
                    if (currentgrid.ocupied)
                    {
                        //if()
                        Console.Write("[X]\t");
                    }
                    else
                    {
                        Console.Write($"[ ]\t");
                    }
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
            Console.Write(Environment.NewLine + Environment.NewLine);
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
