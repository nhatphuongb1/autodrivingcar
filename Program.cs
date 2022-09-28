using System.Linq;

public class Position
{
    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }

    public bool Equals(Position currentPosition)
    {
        return currentPosition == null ? false : currentPosition.X == X && currentPosition.Y == Y;
    }
    public Position Clone() => new(X, Y);
}

public enum Direction
{
    N,
    S,
    W,
    E
}

public class CarCommand
{
    public const string TurnLeft = "L";
    public const string TurnRight = "R";
    public const string MoveForward = "F";
}
public class CurrentPosition
{
    public CurrentPosition(Position position, Direction direction)
    {
        Position = position;
        Direction = direction;
    }

    public Position Position { get; set; }
    public Direction Direction { get; set; }

    public CurrentPosition Clone() => new CurrentPosition
    (
        Position = Position.Clone(),
        Direction = Direction
    );

}

public class CarData
{
    private readonly List<string> validCommands = new()
    {
            CarCommand.MoveForward,
            CarCommand.TurnLeft,
            CarCommand.TurnRight
     };

    public CarData(string title, CurrentPosition currentPosition, string commands)
    {
        Title = title;
        CurrentPosition = currentPosition;
        Commands = commands?.ToArray().Where(x => validCommands.Contains(x.ToString())).Select(x => x.ToString()).ToList() ?? new List<string>();
    }

    public string Title { get; set; }
    public CurrentPosition CurrentPosition { get; set; }
    public List<string> Commands { set; get; }

    public string PositionString()
    {
        return $"Car {Title} position at: {CurrentPosition.Position.X} {CurrentPosition.Position.Y} {CurrentPosition.Direction}";
    }
    public void PrintPosition()
    {
        Console.WriteLine(PositionString());
    }
}
public class CarProgram
{
    public static CurrentPosition Process(Position max, CurrentPosition current, string command)
    {
        var newPosition = current;

        switch (command)
        {
            case CarCommand.TurnLeft:
                newPosition = TurnLeft(current);
                break;
            case CarCommand.TurnRight:
                newPosition = TurnRight(current);
                break;
            case CarCommand.MoveForward:
                newPosition = Foward(current, max);
                break;
            default:
                break;
        }

        return newPosition;
    }

    public static CurrentPosition Foward(CurrentPosition current, Position max)
    {
        var newPosition = current.Clone();

        switch (newPosition.Direction)
        {
            case Direction.E:
                if (current.Position.X < max.X)
                    current.Position.X++;
                break;
            case Direction.W:
                if (current.Position.X > 0)
                    current.Position.X--;
                break;
            case Direction.S:
                if (current.Position.Y > 0)
                    current.Position.Y--;
                break;
            case Direction.N:
                if (current.Position.Y < max.Y)
                    current.Position.Y++;
                break;
            default:
                break;
        }

        return newPosition;
    }

    public static CurrentPosition TurnLeft(CurrentPosition current)
    {
        var newPosition = current.Clone();

        switch (newPosition.Direction)
        {
            case Direction.E:
                newPosition.Direction = Direction.N;
                break;
            case Direction.W:
                newPosition.Direction = Direction.S;
                break;
            case Direction.S:
                newPosition.Direction = Direction.E;
                break;
            case Direction.N:
                newPosition.Direction = Direction.W;
                break;
            default:
                break;
        }

        return newPosition;
    }

    public static CurrentPosition TurnRight(CurrentPosition current)
    {
        var newPosition = current.Clone();

        switch (newPosition.Direction)
        {
            case Direction.E:
                newPosition.Direction = Direction.S;
                break;
            case Direction.W:
                newPosition.Direction = Direction.N;
                break;
            case Direction.S:
                newPosition.Direction = Direction.W;
                break;
            case Direction.N:
                newPosition.Direction = Direction.E;
                break;
            default:
                break;
        }

        return newPosition;
    }

    public static void ShowDirection(CarData carData, Position max)
    {
        var newCarData = carData;

        foreach (var command in carData.Commands)
        {
            newCarData.CurrentPosition = Process(max, newCarData.CurrentPosition, command);
        }

        Console.WriteLine($"Car {newCarData.Title} position at: {newCarData.CurrentPosition.Position.X} {newCarData.CurrentPosition.Position.Y} {newCarData.CurrentPosition.Direction}");
    }
    public static List<CurrentPosition> GetPathByCommands(CarData carData, Position max)
    {
        var path = new List<CurrentPosition>();

        var currentPosition = carData.CurrentPosition;
        path.Add(carData.CurrentPosition.Clone());
        carData.Commands.ForEach(command =>
        {
            currentPosition = Process(max, currentPosition, command);
            path.Add(currentPosition.Clone());
        });

        return path;
    }
    public static void ShowCheckCollisionResult(List<(string, string, bool, Position, int)> listResult)
    {
        Console.WriteLine("Collision:\n--------");

        listResult.ForEach(item =>
        {
            Console.WriteLine($"{item.Item1} {item.Item2}");
            if (item.Item3)
            {
                // Duplidate position
                Console.WriteLine($"{item.Item4.X} {item.Item4.Y}");
                Console.WriteLine(item.Item5);
            }
            else
            {
                Console.WriteLine("No Collision");
            }

            Console.WriteLine("--------");
        });
    }

    public static (bool, Position, int) CheckCollision(CarData carData1, CarData carData2, Position max)
    {
        var carPath1 = GetPathByCommands(carData1, max).ToArray();
        var carPath2 = GetPathByCommands(carData2, max).ToArray();

        var longerPath = carPath1.Length >= carPath2.Length ? carPath1 : carPath2;
        var shorterPath = carPath1.Length >= carPath2.Length ? carPath2 : carPath1;

        var collide = false;
        var numberOfSteps = 0;
        var collidedPosition = new Position(-1, -1);


        for (int index = 0; index < shorterPath.Length; index++)
        {
            if (shorterPath[index].Position.Equals(longerPath[index].Position))
            {
                numberOfSteps = index + 1;
                collidedPosition = longerPath[index].Position;
                collide = true;
                break;
            }
        }
        if (!collide)
        {
            var lastPositionOfShorterPath = shorterPath[shorterPath.Length - 1];
            for (int index = shorterPath.Length; index < longerPath.Length; index++)
            {
                if (lastPositionOfShorterPath.Position.Equals(longerPath[index]))
                {
                    numberOfSteps = index + 1;
                    collidedPosition = longerPath[index].Position;
                    collide = true;
                    break;
                }
            }
        }
        return (collide, collidedPosition, numberOfSteps);
    }

    public static List<(string, string, bool, Position, int)> CheckCollisions(List<CarData> carDatas, Position max)
    {
        var carDataDict = carDatas.ToDictionary(x => x.Title);
        var collisedCarDict = new Dictionary<string, List<string>>();
        var availiableCars = carDatas.Select(x => x.Title).ToList();
        var checkCollisionResults = new List<(string, string, bool, Position, int)>();

        foreach (var item in carDatas)
        {
            if (availiableCars.Count == 1) break;

            availiableCars.Remove(item.Title);

            collisedCarDict.Add(item.Title, availiableCars.ToList());
        }

        collisedCarDict.ToList().ForEach(item =>
        {
            item.Value.ForEach(x =>
            {
                var result = CheckCollision(carDataDict[item.Key], carDataDict[x], max);
                checkCollisionResults.Add((item.Key, x, result.Item1, result.Item2, result.Item3));
            });
        });

        return checkCollisionResults;
    }
    static void Main(string[] args)
    {
        Position max = new Position(10, 10);

        // part 1
        Console.WriteLine("Part 1");

        var car0 = new CarData("0", new CurrentPosition(new Position(1, 2), Direction.N), "FFRFFFRRLF");

        ShowDirection(car0, max);


        // part 2
        Console.WriteLine();
        Console.WriteLine("Part 2");

        var carA = new CarData("A", new CurrentPosition(new Position(1, 2), Direction.N), "FFRFFFFRRL");
        var carB = new CarData("B", new CurrentPosition(new Position(7, 8), Direction.W), "FFLFFFFFFF");
        var carC = new CarData("C", new CurrentPosition(new Position(2, 2), Direction.E), "FFRFRFFRRL");
        var carD = new CarData("D", new CurrentPosition(new Position(0, 0), Direction.S), "FFLFFFFFFF");
        var result1 = CheckCollision(carA, carB, max);

        var mutipleCars = CheckCollisions(new List<CarData> { carA, carB, carC, carD }, max);
        ShowCheckCollisionResult(mutipleCars);
        Console.ReadLine();
    }
}