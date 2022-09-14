using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;
using static AutoBattle.Types;

// Kleber Ribeiro da Silva - 08/10/82
// Oct~Nov: Add an effect for each class that can somehow paralyze other characters (random chance);

namespace AutoBattle
{
    public class Character : IComparable // Adding this interface to sort them
    {
        public const int initialHealth = 100;
        public const int initialDamage = 20;

        // Defines an event to determine the character's death
        public delegate void CharacterDied(GridBox characterBox);
        public static CharacterDied OnCharacterDied;

        // Defines an event to update the character's health
        public delegate void HealthChanged(Character character);
        public static HealthChanged OnHealthChanged;

        // Every time the character's health value is updated, it calls the OnHealthChanged event to update the health bar in the UI.
        private int _health;
        public int Health
        {
            get { return _health; }
            set
            {
                if (_health != value)
                {
                    _health = value;
                    OnHealthChanged?.Invoke(this);

                    if (_health <= 0)
                    {
                        Die();
                    }
                }
            }
        }

        public int BaseDamage;
        public string Name { get; set; }
        public int playerIndex { get; set; }
        public int DamageMultiplier { get; set; } = 1;
        public bool Paralyzed { get; set; } = false;
        public Character Opponent { get; set; }
        public CharacterClass CharacterClass { get; set; }
        public GridBox currentBox;
        string CurrentCharacter => playerIndex == Program.characterSettings.playerIndex ? "Player" : "Enemy";
        string OpponentCharacter => playerIndex == Program.characterSettings.playerIndex ? "Enemy" : "Player";

        // Constructor of the character
        public Character(CharacterClass characterClass)
        {
            CharacterClass = characterClass;
            Opponent = null;
        }

        //Updates the character's health based on damage taken.
        public void TakeDamage(int amount)
        {
            Health = (Health - amount < 0) ? 0 : Health - amount;
        }

        // Call the event that determines the character's death and removes it from the grid.
        public void Die()
        {
            currentBox.ocupied = false;
            OnCharacterDied?.Invoke(currentBox);
        }

        public void StartTurn(Grid battlefield)
        {
            battlefield.DrawBattlefield();
            Console.ForegroundColor = GetClassColor(currentBox.characterClass);
            Console.Write($"\n{currentBox.characterClass.GetDisplayName()} {CurrentCharacter}'s turn.");

            // If the character is close to a target, attack it.
            if (IsAttackPossible(battlefield))
            {
                Attack(Opponent);
            }
            else
            {
                WalkTo(battlefield);
            }
        }

        // I made some changes on the WalkTo method. 
        // Removed the canWalk boolean and did the verification in a different way.
        public void WalkTo(Grid battlefield)
        {
            Predicate<GridBox> nextPosition = null ;
            int rowDistance = currentBox.row - Opponent.currentBox.row;
            int colDistance = currentBox.col - Opponent.currentBox.col;
            
            if (colDistance > 0)
            {
                if (rowDistance == 0)
                {
                    nextPosition = x => x.col == currentBox.col - 1 && x.row == currentBox.row;
                }
                else
                {
                    nextPosition = x => x.col == currentBox.col - 1 && x.row == currentBox.row + ((rowDistance > 0) ? -1 : 1);
                }
            }
            else if (colDistance < 0)
            {
                if (rowDistance == 0)
                {
                    nextPosition = x => x.col == currentBox.col + 1 && x.row == currentBox.row;
                }
                else
                {
                    nextPosition = x => x.col == currentBox.col + 1 && x.row == currentBox.row + ((rowDistance > 0) ? -1 : 1);
                }
            }
            else if (colDistance == 0)
            {
                if (rowDistance > 0)
                {
                    nextPosition = x => x.col == currentBox.col && x.row == currentBox.row - 1;
                }
                else if (rowDistance < 0)
                {
                    nextPosition = x => x.col == currentBox.col && x.row == currentBox.row + 1;
                }
            }

            if (battlefield.grids.Exists(nextPosition))
            {
                currentBox.ocupied = false;
                battlefield.grids[currentBox.index] = currentBox;

                Console.Write($"\nIt cannot do any damage because it is too far away.\nLooking for a new position.");
                Console.ReadKey();

                var newBox = battlefield.grids.Find(nextPosition);
                newBox.ocupied = true;
                newBox.characterClass = currentBox.characterClass;
                newBox.playerIndex = currentBox.playerIndex;
                battlefield.grids[newBox.index] = newBox;
                currentBox = newBox;
                battlefield.DrawBattlefield();
            }
        }

        // Checks if there is an opponent close enough to perform an attack
        bool IsAttackPossible(Grid battlefield)
        {
            // The position [P] is the cell where the character is.
            // I used a LINQ query to scan the cells around him to see if the opponent is in any of them.
            // If any of the [?] positions are true it is possible to start the attack.

            // [?]   [?]   [?]
            //     ↖  ↑  ↗
            // [?] ← [P] → [?]
            //     ↙  ↓  ↘
            // [?]   [?]   [?]

            return battlefield.grids.Where(n =>
                (n.col == currentBox.col - 1 && n.row == currentBox.row) ||     //←
                (n.col == currentBox.col - 1 && n.row == currentBox.row - 1) || //↖
                (n.row == currentBox.row - 1 && n.col == currentBox.col) ||     //↑
                (n.col == currentBox.col + 1 && n.row == currentBox.row - 1) || //↗
                (n.col == currentBox.col + 1 && n.row == currentBox.row) ||     //→
                (n.col == currentBox.col + 1 && n.row == currentBox.row + 1) || //↘
                (n.row == currentBox.row + 1 && n.col == currentBox.col) ||     //↓
                (n.col == currentBox.col - 1 && n.row == currentBox.row + 1)    //↙ 
                ).Any(x => x.ocupied);            
        }

        
        public void Attack(Character target)
        {
            int damageTaken = 0;

            if (Paralyzed)
            {
                Console.Write($"\nOh no, he is paralyzed and can't attack!");
                Console.ReadKey();
                Paralyzed = false;
            }
            else
            {
                Console.Write($" He will attack!");
                Console.ReadKey();

                int rand = Helper.GetRandomInt(0, 10);
                if (rand == 1)
                {
                    target.Paralyzed = true;
                }

                damageTaken = Helper.GetRandomInt(1, BaseDamage + 1);

                Console.WriteLine($"\n{OpponentCharacter} suffered a damage of {damageTaken} hits!");
                if (rand == 1)
                {
                    
                    Console.WriteLine($"{CurrentCharacter} has played a paralyzing spell. {OpponentCharacter} cannot attack the next turn.");
                }

                Console.ReadKey();
            }
            target.TakeDamage(damageTaken * DamageMultiplier);
        }

        // Returns default colors for each character class type
        public static ConsoleColor GetClassColor(CharacterClass characterClass)
        {
            ConsoleColor result = ConsoleColor.White;

            switch (characterClass)
            {
                case CharacterClass.Paladin:
                    result = ConsoleColor.Green;
                    break;
                case CharacterClass.Warrior:
                    result = ConsoleColor.Yellow;
                    break;
                case CharacterClass.Cleric:
                    result = ConsoleColor.Blue;
                    break;
                case CharacterClass.Archer:
                    result  = ConsoleColor.Red;
                    break;
                default:
                    result = ConsoleColor.White;
                    break;
            }
            return result;
        }

        // Implementation of the CompareTo() interface method to be able to use the AllPlayers.sort()
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            Character otherChar = obj as Character;
            return this.playerIndex.CompareTo(otherChar.playerIndex);
        }
    }
}
