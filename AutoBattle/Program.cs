using System;
using System.Collections.Generic;
using System.Linq;
using static AutoBattle.Types;
using static AutoBattle.Helper;
using static AutoBattle.Character;
using static AutoBattle.Grid;

namespace AutoBattle
{
    class Program
    {
        public static int defaultScreenSize = 60;

        // Stores each character's indices
        public static CharacterSettings characterSettings = new CharacterSettings(true);

        static void Main(string[] args)
        {
            // Não vou usar esta variável pq agora uso character settins
            // CharacterClass playerCharacterClass;

            Grid grid = new Grid(true);            
            GridBox PlayerCurrentLocation;
            GridBox EnemyCurrentLocation;
            Character PlayerCharacter;
            Character EnemyCharacter;
            List<Character> AllPlayers = new List<Character>();
            int currentTurn = 0;
            int numberOfPossibleTiles = grid.grids.Count;

            Setup(); 

            void Setup()
            {
                // Create a battleground with user choices
                CreateBattleField();

                GetPlayerChoice();
                StartGame();
            }

            // The idea here is to use steps so that the user can insert rows and columns.
            // 0 to insert rows
            // 1 to insert columns
            void CreateBattleField(int step = 0)
            {
                string value = null;

                // Wellcome print
                PrintWellcome();

                //When you are setting the row quantity
                if (step == 0)
                {
                    WriteLineCenter($"Press ENTER to create a standard battlefield ({defaultRows}x{defaultCols})");
                    WriteLineCenter("OR");
                    WriteLineCenter($"Choose the battlefield's number of Rows (max. {maxRows})");

                    value = Console.ReadLine();
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!grid.SetRowsSize(value))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            WriteLineCenter("Ops! The value of Rows is invalid! Press any key to try again.");
                            Console.ReadKey();
                            CreateBattleField();
                        }
                        CreateBattleField(1);
                    }
                }
                //When you are setting the column quantity
                else if (step == 1)
                {
                    WriteLineCenter($"Now, choose the battlefield's number of Columns (max. {maxCols})");
                    value = Console.ReadLine();
                    if (!grid.SetColsSize(value))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        WriteLineCenter("Ops! The value of Columns is invalid! Press any key to try again.");
                        Console.ReadKey();
                        CreateBattleField(1);
                    }                    
                }                
                grid.Create();
            }

            void GetPlayerChoice()
            {
                //asks for the player to choose between for possible classes via console.
                Console.WriteLine("Select a Class for your character:\n");
                Console.ForegroundColor = GetClassColor(CharacterClass.Paladin);
                Console.Write("[1] Paladin\t");
                Console.ForegroundColor = GetClassColor(CharacterClass.Warrior);
                Console.Write("[2] Warrior\t");
                Console.ForegroundColor = GetClassColor(CharacterClass.Cleric);
                Console.Write("[3] Cleric\t");
                Console.ForegroundColor = GetClassColor(CharacterClass.Archer);
                Console.Write("[4] Archer\n");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Your choice: ");

                // Store the player choice in a variable
                // I removed the existing switch to implement something with which I think it has less redundancy
                int choice = GetIntParsed(Console.ReadLine());
                if (choice > 0 && choice <=4)
                {
                    CreatePlayerCharacter(choice);
                }
                else
                {
                    grid.DrawBattlefield();
                    GetPlayerChoice();
                }                
            }

            

            void CreatePlayerCharacter(int classIndex)
            {
                //playerCharacterClass = (CharacterClass)classIndex;
                characterSettings.playerClass = (CharacterClass)classIndex;
                Console.WriteLine($"Player Class Choice: {characterSettings.playerClass}");
                PlayerCharacter = new Character(characterSettings.playerClass);
                PlayerCharacter.Name = $"{characterSettings.playerClass} Player";
                PlayerCharacter.Health = initialHealth;
                PlayerCharacter.BaseDamage = initialDamage;

                // Determines whether the player starts first or second. If it is 0 it starts the game.
                characterSettings.playerIndex = Helper.GetRandomInt(0, 2);
                PlayerCharacter.playerIndex = characterSettings.playerIndex;
                CreateEnemyCharacter();
            }

            void CreateEnemyCharacter()
            {
                //CharacterClass enemyClass = GetRandomEnemyClass();
                characterSettings.enemyClass = GetRandomEnemyClass();
                Console.WriteLine($"Enemy Class Choice: {characterSettings.enemyClass}");
                EnemyCharacter = new Character(characterSettings.enemyClass);
                EnemyCharacter.Name = $"{characterSettings.enemyClass} Enemy";
                EnemyCharacter.Health = initialHealth;
                EnemyCharacter.BaseDamage = initialDamage;

                // Calculation to find the value opposite the player's index
                characterSettings.enemyIndex = ((PlayerCharacter.playerIndex + 1) % 2);
                EnemyCharacter.playerIndex = characterSettings.enemyIndex;
            }

            // Choose a CharacterClass randomly that is not the same as the Player.
            CharacterClass GetRandomEnemyClass()
            {                
                int randomCharacterClass;
                int totalCharacterClass = Enum.GetValues(typeof(CharacterClass)).Length;
                do
                {
                    randomCharacterClass = (int)Helper.GetRandomInt(1, totalCharacterClass + 1);
                }
                while (randomCharacterClass == (int)characterSettings.playerClass);

                return (CharacterClass)randomCharacterClass;
            }

            void StartGame()
            {
                //populates the character variables and targets
                EnemyCharacter.Opponent = PlayerCharacter;
                PlayerCharacter.Opponent = EnemyCharacter;
                AllPlayers.Add(PlayerCharacter);
                AllPlayers.Add(EnemyCharacter);
                AlocatePlayers();
                StartTurn();
            }

            void StartTurn(){

                if (currentTurn == 0)
                {
                    AllPlayers.Sort();  
                }

                foreach(Character character in AllPlayers)
                {
                    // If either character reaches health <= 0, the turn ends
                    if (character.Health <=0)
                    {
                        break;
                    }
                    character.StartTurn(grid);
                }

                currentTurn++;
                HandleTurn();
            }

            void HandleTurn()
            {
                if(PlayerCharacter.Health <= 0) //Enemy is the winner
                {
                    PrintWinner(EnemyCharacter.Name);
                    return;
                }
                else if (EnemyCharacter.Health <= 0) //Player is the winner
                {
                    PrintWinner(PlayerCharacter.Name);
                    return;
                }
                else
                {
                    Console.Write(Environment.NewLine);
                    Console.WriteLine("Press any key to start the next turn!");
                    Console.Write(Environment.NewLine + Environment.NewLine);
                    Console.ReadKey();
                    StartTurn();
                }
            }

            void PrintWinner(string playerName)
            {
                Console.Write(Environment.NewLine + Environment.NewLine);
                Console.WriteLine("Game Over! The {0} wins!", playerName);
                Console.Write(Environment.NewLine + Environment.NewLine);
                Console.WriteLine("Press any key to continue!");
                Console.ReadKey();
            }

            // Removed GetRandomInt() that existed here
            // Transferred to Helper

            void AlocatePlayers()
            {
                AlocatePlayerCharacter();
            }

            void AlocatePlayerCharacter()
            {
                int random = Helper.GetRandomInt(0, grid.grids.Count());
                GridBox RandomLocation = grid.grids.ElementAt(random);
                if (!RandomLocation.ocupied)
                {
                    // PlayerCurrentLocation is declared in the global scope of the class,
                    // so I removed the local declaration.
                    PlayerCurrentLocation = RandomLocation;
                    PlayerCurrentLocation.ocupied = true;
                    PlayerCurrentLocation.playerIndex = PlayerCharacter.playerIndex;
                    PlayerCurrentLocation.characterClass = PlayerCharacter.CharacterClass;
                    grid.grids[random] = PlayerCurrentLocation;
                    PlayerCharacter.currentBox = grid.grids[random];
                    AlocateEnemyCharacter();
                } else
                {
                    AlocatePlayerCharacter();
                }
            }

            void AlocateEnemyCharacter()
            {
                int random = Helper.GetRandomInt(0, grid.grids.Count());
                GridBox RandomLocation = grid.grids.ElementAt(random);
                if (!RandomLocation.ocupied)
                {
                    EnemyCurrentLocation = RandomLocation;
                    EnemyCurrentLocation.ocupied = true;
                    EnemyCurrentLocation.playerIndex = EnemyCharacter.playerIndex;
                    EnemyCurrentLocation.characterClass = EnemyCharacter.CharacterClass;
                    grid.grids[random] = EnemyCurrentLocation;
                    EnemyCharacter.currentBox = grid.grids[random];
                    grid.DrawBattlefield();
                    Console.WriteLine("Press any key to Start Game!");
                    Console.ReadKey();
                }
                else
                {
                    AlocateEnemyCharacter();
                }                
            }
        }
    }
}