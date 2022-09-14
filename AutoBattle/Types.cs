using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoBattle
{
    public class Types
    {
        public struct CharacterSettings
        {
            public CharacterSettings(bool reset = true)
            {
                playerIndex = enemyIndex = reset ? -1 : 0;
                playerClass = enemyClass = 0;
            }

            public int playerIndex;
            public CharacterClass playerClass;
            public int enemyIndex;
            public CharacterClass enemyClass;
        }

        public struct GridBox
        {
            public int row;
            public int col;
            public bool ocupied;
            public int index;
            public int playerIndex;
            public CharacterClass characterClass;

            public GridBox(int row, int col, bool ocupied, int index)
            {
                this.row = row;
                this.col = col;
                this.ocupied = ocupied;
                this.index = index;
                playerIndex = -1;
                characterClass = 0;
            }

            public void Empty()
            {
                ocupied = false;
            }
        }

        public enum GridCells: uint
        {
            row = 0,
            col = 1
        }

        public enum CharacterClass : uint
        {
            [Display(Name = "Paladin")]            
            Paladin = 1,
            [Display(Name = "Warrior")]
            Warrior = 2,
            [Display(Name = "Cleric")]
            Cleric = 3,
            [Display(Name = "Archer")]
            Archer = 4
        }
    }
}
