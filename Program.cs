//Summary
//Start player receives ball
//Players rotate in multiples of 45 degrees clockwise after receiving ball
//When they find another player they throw ball and leave field
//If multiple in same direction it goes to the first one
//Keeps going until player can no longer make any valid throws
//Output no of throws made and last player index


//Inputs
//Number of test cases T <= 100 (line1)
//Number of Players 2 <= N <= 1000 (First line of testcase)
//Player coordinates -10^9 <= X,Y <= 10^9 (Next N lines)
//Direction first player receives ball from N,E,S,W,NW,NE,SW,SE (Next line)
//Index of first player 1 <= S <= N (last line of testcase)


//Explaination
//I define a grid of all the players positions for a testcase from data
//Then find out the start player position and direction
//From here I iterate over each direction
//If a new player is found the throw count is increased
//We then get the start direction again which will be opposite to the previous throw direction
//We iterate over directions again
//In all directions are coverd and no valid throws are found we output the number of throws and last person


//NOTE
//I was unable to get result because of a memory leak with larger datasets due to using a 2D array
//I didn't have time to fix this, but I might have tried using a flat array

using System.Text.Json;

namespace Dodgeball
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                List<TestCaseInput>? testCases = Helpers.ParseInputFile(); 
                List<int[]> results = new List<int[]>();

                if (testCases == null || testCases.Count == 0)
                {
                    Console.WriteLine("No file or file not valid");
                    return;
                }

                foreach (TestCaseInput testCase in testCases)
                {
                    testCase.NormaliseStartPositions(); //Normalise minimal position to 0 for positive interations

                    int[][] grid = testCase.CreateGrid(); //Create grid where player positions are marked by 1s

                    int throwCount = 0;
                    int currentPlayerIndex = testCase.startingPlayer - 1; //Convert data to list index

                    int receiveDirectionIndex = testCase.GetDirectionStartIndex(); //Get first players direction they received from
                    int directionIndex = (receiveDirectionIndex + 1) % testCase.directions.Count; //Set the first throw try direction

                    do
                    {
                        //Get start player position and current direction
                        int[] directionArray = testCase.directions[directionIndex].vector;
                        int[] startPosition = testCase.players[currentPlayerIndex];

                        int i = startPosition[1]; 
                        int j = startPosition[0];

                        while (i < grid.Length && i >= 0 && j < grid[i].Length && j >= 0)
                        {
                            i += directionArray[1]; //Get next positions
                            j += directionArray[0];

                            if (i >= grid.Length || i < 0 || j >= grid[i].Length || j < 0)
                            {
                                break;
                            }

                            if (grid[i][j] != 0)
                            {
                                throwCount++; //Add to count
                                currentPlayerIndex = testCase.players.FindIndex(p => p[0] == j && p[1] == i); //Get player index for position
                                receiveDirectionIndex = (directionIndex + 4) % testCase.directions.Count; //Get opposite direction for receiving
                                directionIndex = receiveDirectionIndex; //Set start index
                                grid[i][j] = 0; //Remove player
                                break;
                            }
                        }

                        directionIndex = (directionIndex + 1) % testCase.directions.Count; //Bump direction for next iteration

                    }
                    while (directionIndex != receiveDirectionIndex); //No valid throws found

                    results.Add(new int[] { throwCount, currentPlayerIndex + 1 }); //Add current test case results
                }

                Helpers.OutputResults(results); //Output to file
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    public class TestCaseInput
    {
        public List<int[]>? players { get; set; }
        public string? startingDirection { get; set; }
        public int startingPlayer { get; set; }

        public class Direction
        {
            public string name { get; set; }
            public int[] vector { get; set; }
        }

        public List<Direction> directions = new List<Direction>()
        {
            new Direction (){ name = "N", vector = new int[] { 0, 1 } },
            new Direction (){ name = "NE", vector = new int[] { 1, 1 } },
            new Direction (){ name = "E", vector = new int[] { 1, 0 } },
            new Direction (){ name = "SE", vector = new int[] { 1, -1 } },
            new Direction (){ name = "S", vector = new int[] { 0, -1 } },
            new Direction (){ name = "SW", vector = new int[] { -1, -1 } },
            new Direction (){ name = "W", vector = new int[] { -1, 0 } },
            new Direction (){ name = "NW", vector = new int[] { -1, 1 } },
        };


        public bool NormaliseStartPositions()
        {
            try
            {
                int minX = players.Min(p => p[0]);
                int minY = players.Min(p => p[1]);

                foreach (int[] p in players)
                {
                    p[0] -= minX;
                    p[1] -= minY;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to normalise coordinants");
                throw ex;
            }
        }


        public int[][] CreateGrid() //This causes memory leakage for the larger data sets.
        {
            try
            {
                int maxX = players.Max(p => p[0]);
                int maxY = players.Max(p => p[1]);

                int[][] grid = new int[maxY + 1][];

                for (int i = 0; i <= maxY; i++)
                {
                    grid[i] = new int[maxX + 1];
                }


                foreach (int[] player in players)
                {
                    grid[player[1]][player[0]] = 1;
                }

                return grid;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to create grid");
                throw ex;
            }
        }

        public int GetDirectionStartIndex()
        {
            try
            {
                return directions.IndexOf(directions.First(d => d.name.Equals(startingDirection.ToUpper())));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to get direction");
                throw ex;
            }
        }
    }

    public class Helpers
    {

        public static List<TestCaseInput>? ParseInputFile()
        {
            string inputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "input.json");

            List <TestCaseInput>? input = new List<TestCaseInput>();

            if (File.Exists(inputFilePath))
            {
                string jsonInput = File.ReadAllText(inputFilePath);

                input = JsonSerializer.Deserialize<List<TestCaseInput>>(jsonInput);
            }

            return input;
        }

        public static void OutputResults(List<int[]> results)
        {
            string outputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "output.out");
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (int[] result in results)
                {
                    writer.WriteLine(string.Join(" ", result));
                }
            }
        }
    }
}

