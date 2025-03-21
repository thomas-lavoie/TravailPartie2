using MongoDB.Driver;

bool keep_going = true;

while (keep_going)
{
    Console.WriteLine("Bibliotheque");
    Console.WriteLine();
    Console.WriteLine("   ------   ");
    Console.WriteLine();
    Console.WriteLine("Q: Quitter");

    ConsoleKey key = Console.ReadKey().Key;
    
    switch (key)
    {
        case ConsoleKey.Q:
            keep_going = false;
            break;
        default:
            break;
    }

    Console.Clear();
}