using System.Globalization;
using System.Text.RegularExpressions;
using SportApp;

// ================== Lab-1. Класи, об'єкти ==================
// Предметна область: спорт. Клас Athlete (спортсмен).
// Усі перевірки обмежень виконуються в основній програмі (Program.cs).

const int MinNameLength = 2;
const int MaxNameLength = 20;
const double MinWeight = 30.0;
const double MaxWeight = 200.0;
const int MaxMedals = 500;
DateTime minBirthDate = new DateTime(1950, 1, 1);

Random random = new Random();

Console.WriteLine("=== SPORT CLUB: athletes register (Lab-1) ===");
int capacity = ReadCapacity();
Athlete[] athletes = new Athlete[capacity];
int count = 0;

bool exit = false;
while (!exit)
{
    PrintMenu();
    string choice = Console.ReadLine()?.Trim();
    Console.WriteLine();
    switch (choice)
    {
        case "1": AddAthlete(); break;
        case "2": ShowAll(); break;
        case "3": FindAthlete(); break;
        case "4": DemonstrateBehavior(); break;
        case "5": DeleteAthlete(); break;
        case "0":
            exit = true;
            Console.WriteLine("Goodbye!");
            break;
        default:
            Console.WriteLine("Unknown menu item. Try again.");
            break;
    }
}

// ======================= MENU =======================

void PrintMenu()
{
    Console.WriteLine();
    Console.WriteLine("---------------- MENU ----------------");
    Console.WriteLine("1 - Add athlete");
    Console.WriteLine("2 - Show all athletes");
    Console.WriteLine("3 - Find athlete");
    Console.WriteLine("4 - Demonstrate behavior");
    Console.WriteLine("5 - Delete athlete");
    Console.WriteLine("0 - Exit");
    Console.Write("Your choice ---> ");
}

int ReadCapacity()
{
    while (true)
    {
        Console.Write("Enter max number of athletes N (1..100) ---> ");
        string input = Console.ReadLine();
        if (int.TryParse(input, out int n) && n > 0 && n <= 100)
            return n;
        Console.WriteLine("Error: N must be an integer from 1 to 100.");
    }
}

// ======================= 1. ADD =======================

void AddAthlete()
{
    if (count >= capacity)
    {
        Console.WriteLine($"Error: the register is full ({capacity} of {capacity}). Delete an athlete first.");
        return;
    }

    Console.WriteLine("1 - Manual input");
    Console.WriteLine("2 - Automatic (random) input");
    Console.Write("Choose mode ---> ");
    string mode = Console.ReadLine()?.Trim();

    Athlete athlete;
    if (mode == "1") athlete = CreateManually();
    else if (mode == "2") athlete = CreateRandomly();
    else
    {
        Console.WriteLine("Unknown mode. Athlete is not added.");
        return;
    }

    athletes[count] = athlete;
    count++;
    Console.WriteLine($"Athlete {athlete.name} is added ({count} of {capacity}).");
}

Athlete CreateManually()
{
    Athlete athlete = new Athlete();
    athlete.name = ReadName("Enter name");
    athlete.birthDate = ReadBirthDate();
    athlete.sport = ReadSport();
    athlete.weight = ReadWeight();
    athlete.medals = ReadMedals();
    athlete.isProfessional = ReadIsProfessional();
    return athlete;
}

Athlete CreateRandomly()
{
    (string Name, SportType Sport)[] samples =
    {
        ("Andriy Shevchenko", SportType.Football), ("Mykhailo Mudryk", SportType.Football),
        ("Artem Pustovyi", SportType.Basketball), ("Alex Len", SportType.Basketball),
        ("Marta Kostyuk", SportType.Tennis), ("Dayana Yastremska", SportType.Tennis),
        ("Mykhailo Romanchuk", SportType.Swimming), ("Vasyl Lomachenko", SportType.Boxing),
        ("Yaroslava Mahuchikh", SportType.Athletics), ("Bohdan Bondarenko", SportType.Athletics)
    };
    var sample = samples[random.Next(samples.Length)];
    Athlete athlete = new Athlete();
    athlete.name = sample.Name;
    athlete.birthDate = new DateTime(random.Next(1975, 2010), random.Next(1, 13), random.Next(1, 29));
    athlete.sport = sample.Sport;
    athlete.weight = Math.Round(50 + random.NextDouble() * 60, 1);
    athlete.medals = random.Next(0, 31);
    athlete.isProfessional = random.Next(2) == 1;
    Console.WriteLine("Athlete is generated automatically.");
    return athlete;
}

// ---------- введення з перевіркою обмежень ----------

// Ім'я: 2..20 символів, тільки латинські літери, пробіл або дефіс між словами
string ReadName(string prompt)
{
    while (true)
    {
        Console.Write($"{prompt} ({MinNameLength}-{MaxNameLength} Latin letters) ---> ");
        string value = Console.ReadLine()?.Trim() ?? "";
        if (value.Length < MinNameLength || value.Length > MaxNameLength)
        {
            Console.WriteLine($"Error: name length must be from {MinNameLength} to {MaxNameLength} characters.");
            continue;
        }
        if (!Regex.IsMatch(value, @"^[A-Za-z]+([ \-][A-Za-z]+)*$"))
        {
            Console.WriteLine("Error: name can contain only Latin letters (words separated by space or '-').");
            continue;
        }
        return value;
    }
}

// Дата народження: формат dd.MM.yyyy, не раніше 01.01.1950, вік не менше 10 років
DateTime ReadBirthDate()
{
    DateTime maxBirthDate = DateTime.Today.AddYears(-10);
    while (true)
    {
        Console.Write("Enter birth date (dd.MM.yyyy) ---> ");
        string value = Console.ReadLine()?.Trim();
        if (!DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture,
                                    DateTimeStyles.None, out DateTime date))
        {
            Console.WriteLine("Error: date must be in format dd.MM.yyyy.");
            continue;
        }
        if (date < minBirthDate || date > maxBirthDate)
        {
            Console.WriteLine($"Error: date must be from {minBirthDate:dd.MM.yyyy} to {maxBirthDate:dd.MM.yyyy} (age 10+).");
            continue;
        }
        return date;
    }
}

// Вид спорту: тільки значення з переліку SportType
SportType ReadSport()
{
    SportType[] values = Enum.GetValues<SportType>();
    while (true)
    {
        Console.WriteLine("Sports: " + string.Join(", ", values.Select((s, i) => $"{i + 1}-{s}")));
        Console.Write("Enter sport (number or name) ---> ");
        string value = Console.ReadLine()?.Trim();
        if (int.TryParse(value, out int number))
        {
            if (number >= 1 && number <= values.Length) return values[number - 1];
        }
        else if (Enum.TryParse(value, true, out SportType sport) && Enum.IsDefined(sport))
        {
            return sport;
        }
        Console.WriteLine("Error: there is no such sport in the list.");
    }
}

// Вага: дійсне число від 30 до 200 кг
double ReadWeight()
{
    while (true)
    {
        Console.Write($"Enter weight, kg ({MinWeight}..{MaxWeight}) ---> ");
        string value = Console.ReadLine()?.Trim().Replace(',', '.');
        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double weight))
        {
            Console.WriteLine("Error: weight must be a number.");
            continue;
        }
        if (weight < MinWeight || weight > MaxWeight)
        {
            Console.WriteLine($"Error: weight must be from {MinWeight} to {MaxWeight} kg.");
            continue;
        }
        return Math.Round(weight, 1);
    }
}

// Медалі: ціле число від 0 до 500
int ReadMedals()
{
    while (true)
    {
        Console.Write($"Enter number of medals (0..{MaxMedals}) ---> ");
        string value = Console.ReadLine()?.Trim();
        if (!int.TryParse(value, out int medals))
        {
            Console.WriteLine("Error: number of medals must be an integer.");
            continue;
        }
        if (medals < 0 || medals > MaxMedals)
        {
            Console.WriteLine($"Error: number of medals must be from 0 to {MaxMedals}.");
            continue;
        }
        return medals;
    }
}

// Професіонал: y / n
bool ReadIsProfessional()
{
    while (true)
    {
        Console.Write("Is professional? (y/n) ---> ");
        string value = Console.ReadLine()?.Trim().ToLower();
        if (value == "y" || value == "yes") return true;
        if (value == "n" || value == "no") return false;
        Console.WriteLine("Error: enter 'y' or 'n'.");
    }
}

int ReadInt(string prompt, int min, int max)
{
    while (true)
    {
        Console.Write($"{prompt} ({min}..{max}) ---> ");
        if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
            return value;
        Console.WriteLine($"Error: enter an integer from {min} to {max}.");
    }
}

// ======================= 2. SHOW =======================

void ShowAll()
{
    if (count == 0)
    {
        Console.WriteLine("The register is empty. There are no athletes to show.");
        return;
    }
    Console.WriteLine($"All athletes ({count} of {capacity}):");
    PrintTable(athletes.Take(count).ToArray(), Enumerable.Range(1, count).ToArray());
}

void PrintTable(Athlete[] list, int[] numbers)
{
    string line = new string('-', 104);
    Console.WriteLine(line);
    Console.WriteLine($"| {"#",2} | {"Name",-20} | {"Birth date",-10} | {"Age",3} | {"Sport",-10} | {"Weight",6} | {"Medals",6} | {"Pro",3} | {"Energy",6} | {"Hours",5} |");
    Console.WriteLine(line);
    for (int i = 0; i < list.Length; i++)
    {
        Athlete a = list[i];
        Console.WriteLine($"| {numbers[i],2} | {a.name,-20} | {a.birthDate,-10:dd.MM.yyyy} | {a.GetAge(),3} | {a.sport,-10} | " +
                          $"{a.weight.ToString("F1", CultureInfo.InvariantCulture),6} | {a.medals,6} | {(a.isProfessional ? "yes" : "no"),3} | " +
                          $"{a.GetEnergy(),6} | {a.GetTrainingHours(),5} |");
    }
    Console.WriteLine(line);
}

// ======================= 3. FIND =======================

void FindAthlete()
{
    if (count == 0)
    {
        Console.WriteLine("The register is empty. Nothing to search.");
        return;
    }
    Console.WriteLine("Search by:");
    Console.WriteLine("1 - Name");
    Console.WriteLine("2 - Sport");
    Console.Write("Your choice ---> ");
    string choice = Console.ReadLine()?.Trim();

    List<Athlete> found = new List<Athlete>();
    List<int> numbers = new List<int>();

    if (choice == "1")
    {
        Console.Write("Enter name (or part of name) ---> ");
        string text = Console.ReadLine()?.Trim() ?? "";
        for (int i = 0; i < count; i++)
        {
            if (text.Length > 0 && athletes[i].name.Contains(text, StringComparison.OrdinalIgnoreCase))
            {
                found.Add(athletes[i]);
                numbers.Add(i + 1);
            }
        }
    }
    else if (choice == "2")
    {
        SportType sport = ReadSport();
        for (int i = 0; i < count; i++)
        {
            if (athletes[i].sport == sport)
            {
                found.Add(athletes[i]);
                numbers.Add(i + 1);
            }
        }
    }
    else
    {
        Console.WriteLine("Unknown search type.");
        return;
    }

    if (found.Count == 0)
    {
        Console.WriteLine("No athletes found.");
        return;
    }
    Console.WriteLine($"Found athletes: {found.Count}");
    PrintTable(found.ToArray(), numbers.ToArray());
}

// ======================= 4. BEHAVIOR =======================

void DemonstrateBehavior()
{
    if (count == 0)
    {
        Console.WriteLine("The register is empty. Add an athlete first.");
        return;
    }
    ShowAll();
    int number = ReadInt("Enter athlete number", 1, count);
    Athlete a = athletes[number - 1];

    bool back = false;
    while (!back)
    {
        Console.WriteLine();
        Console.WriteLine($"--- Behavior of {a.name} (energy {a.GetEnergy()}, hours {a.GetTrainingHours()}, medals {a.medals}) ---");
        Console.WriteLine("1 - Train(hours)");
        Console.WriteLine("2 - Rest(hours)");
        Console.WriteLine("3 - CanCompete()");
        Console.WriteLine("4 - Compete(opponentRating)");
        Console.WriteLine("5 - GetRating()");
        Console.WriteLine("6 - GetAge()");
        Console.WriteLine("7 - GetEnergy() / GetTrainingHours()");
        Console.WriteLine("0 - Back to main menu");
        Console.Write("Your choice ---> ");
        string choice = Console.ReadLine()?.Trim();
        switch (choice)
        {
            case "1":
                int hours = ReadInt("Training hours", 1, 10);
                if (a.Train(hours))
                    Console.WriteLine($"Training is done. Energy = {a.GetEnergy()}, total hours = {a.GetTrainingHours()}.");
                else
                    Console.WriteLine($"Not enough energy for {hours} h of training (energy = {a.GetEnergy()}). Have a rest!");
                break;
            case "2":
                int restHours = ReadInt("Rest hours", 1, 24);
                a.Rest(restHours);
                Console.WriteLine($"Athlete has rested. Energy = {a.GetEnergy()}.");
                break;
            case "3":
                Console.WriteLine(a.CanCompete() ? "Athlete is ready to compete." : "Athlete is too tired to compete.");
                break;
            case "4":
                if (!a.CanCompete())
                {
                    Console.WriteLine("Athlete is too tired to compete (energy < 30).");
                    break;
                }
                int opponent = ReadInt("Opponent rating", 0, 1000);
                int rating = a.GetRating();
                bool win = a.Compete(opponent);
                Console.WriteLine(win
                    ? $"VICTORY! Rating {rating} > {opponent}. Medals = {a.medals}."
                    : $"Defeat. Rating {rating} <= {opponent}. Medals = {a.medals}.");
                break;
            case "5":
                Console.WriteLine($"Rating = {a.GetRating()}");
                break;
            case "6":
                Console.WriteLine($"Age = {a.GetAge()}");
                break;
            case "7":
                Console.WriteLine($"Energy = {a.GetEnergy()}, training hours = {a.GetTrainingHours()}");
                break;
            case "0":
                back = true;
                break;
            default:
                Console.WriteLine("Unknown item.");
                break;
        }
    }
}

// ======================= 5. DELETE =======================

void DeleteAthlete()
{
    if (count == 0)
    {
        Console.WriteLine("The register is empty. Nothing to delete.");
        return;
    }
    Console.WriteLine("Delete by:");
    Console.WriteLine("1 - Number in the table");
    Console.WriteLine("2 - Sport (all athletes of this sport)");
    Console.Write("Your choice ---> ");
    string choice = Console.ReadLine()?.Trim();

    if (choice == "1")
    {
        ShowAll();
        Console.Write($"Enter number (1..{count}) ---> ");
        if (!int.TryParse(Console.ReadLine(), out int number) || number < 1 || number > count)
        {
            Console.WriteLine($"No athlete with such number found.");
            return;
        }
        string name = athletes[number - 1].name;
        RemoveAt(number - 1);
        Console.WriteLine($"Athlete #{number} ({name}) is deleted.");
    }
    else if (choice == "2")
    {
        SportType sport = ReadSport();
        int removed = 0;
        for (int i = count - 1; i >= 0; i--)
        {
            if (athletes[i].sport == sport)
            {
                RemoveAt(i);
                removed++;
            }
        }
        Console.WriteLine(removed == 0
            ? $"No athletes of sport {sport} found."
            : $"Deleted athletes: {removed}.");
    }
    else
    {
        Console.WriteLine("Unknown delete type.");
    }
}

void RemoveAt(int index)
{
    for (int i = index; i < count - 1; i++)
        athletes[i] = athletes[i + 1];
    athletes[count - 1] = null;
    count--;
}
