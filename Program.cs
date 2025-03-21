using MongoDB.Driver;
using MongoDB.Bson;
using TravailPartie2;

MongoClient dbClient = new MongoClient("mongodb://localhost:27017");
var database = dbClient.GetDatabase("tp-partie-2");
var collection = database.GetCollection<Publication>("ouvrages");

bool keep_going = true;

while (keep_going)
{
    Console.WriteLine("Bibliotheque\n");
    Console.WriteLine("1: Insérer un ouvrage");
    Console.WriteLine("Q: Quitter");

    ConsoleKey key = Console.ReadKey().Key;
    
    switch (key)
    {
        case ConsoleKey.D1:
            try
            {
                collection.InsertOne(create());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Appuyer sur une touche pour continuer.");
            Console.ReadKey();
            break;
        case ConsoleKey.Q:
            keep_going = false;
            break;
        default:
            break;
    }

    Console.Clear();
}

// Retourne une publication si tout est valide
static Publication create()
{
    PublicationType? type = null;

    bool keep_going_type = true;
    while (keep_going_type)
    {
        Console.Clear();
        Console.WriteLine("Quel type de publication voulez-vous créer?\n");
        Console.WriteLine("1: périodique");
        Console.WriteLine("2: livre");
        Console.WriteLine("3: BD");
        ConsoleKey key = Console.ReadKey().Key;

        switch (key)
        {
            case ConsoleKey.D1:
                type = PublicationType.Periodical;
                break;
            case ConsoleKey.D2:
                type = PublicationType.Book;
                break;
            case ConsoleKey.D3:
                type = PublicationType.Comic;
                break;
            default:
                break;

        }
        if (type != null)
        {
            keep_going_type = false;
        }
    }

    Console.Clear();

    Console.Write("Titre : ");
    string title = Console.ReadLine();
    Console.Write("Disponible (0: faux, 1: vrai) : ");
    bool available = Console.ReadLine() == "1";
    Console.Write("Prix : ");
    double price = double.Parse(Console.ReadLine());
    
    switch (type)
    {
        case PublicationType.Periodical:
            Console.Write("Date : ");
            string datePeriodical = Console.ReadLine();
            Console.Write("Périodicité : ");
            string frequency = Console.ReadLine();
            return new Periodical(title, available, price, datePeriodical, frequency);
        case PublicationType.Book:
            Console.Write("Examplaires (nom des examplaires séparés par des virgules) : ");
            string copies = Console.ReadLine();
            string[] copiesTab = copies.Split(',');
            Console.Write("Année : ");
            string yearBook = Console.ReadLine();
            Console.Write("Maison d'édition : ");
            string publisherBook = Console.ReadLine();
            Console.Write("Auteur : ");
            string authorBook = Console.ReadLine();
            return new Book(title, available, price, copiesTab, yearBook, publisherBook, authorBook);
        case PublicationType.Comic:
            Console.Write("Année : ");
            string yearComic = Console.ReadLine();
            Console.Write("Maison d'édition : ");
            string publisherComic = Console.ReadLine();
            Console.Write("Auteur : ");
            string authorComic = Console.ReadLine();
            Console.Write("Dessinateur : ");
            string illustrator = Console.ReadLine();
            return new Comic(title, available, price, yearComic, publisherComic, authorComic, illustrator);
    }

    throw new InvalidPublicationException();
}