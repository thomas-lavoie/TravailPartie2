using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravailPartie2
{
    public class Menu
    {
        public string Titre { get; private set; }
        private List<string> Options { get; set; }
        private List<Action> Actions { get; set; }

        public Menu(string titre, List<string> options, List<Action> actions)
        {
            if (null == options || null == actions || options.Count != actions.Count)
                throw new ArgumentException("La liste d'option et/ou la liste d'action est invalide");

            Titre = titre;
            Options = options;
            Actions = actions;
        }

        public void Afficher() 
        { 
            bool retour = false;

            while (!retour)
            {
                Console.Clear();

                Console.WriteLine("+──────────────────────────────────────────────+");
                Console.WriteLine("│              TP NoSQL - Partie 2             │");
                Console.WriteLine("│                  28 mars 2025                │");
                Console.WriteLine("+──────────────────────────────────────────────+\n");

                Console.WriteLine("Équipe  :  Keven Bellavance-Boisclair &\n" +
                                  "           Thomas Lavoie\n");
      
                Console.WriteLine($"--- {Titre} ---\n");

                for (int i = 0; i < Options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {Options[i]}");
                }
                Console.WriteLine($"{Options.Count + 1}. Retour");
                Console.Write("\nChoix : ");

                if (int.TryParse(Console.ReadLine(), out int choix))
                {
                    if (choix == Options.Count + 1)
                    {
                        retour = true;
                    }
                    else if (choix >= 1 && choix <= Options.Count)
                    {
                        Actions[choix - 1].Invoke();
                    }
                    else
                    {
                        Console.WriteLine("Choix invalide ! Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                    }
                }
            }

        }
    }

    public class MenuManager
    {
        public Menu Inserer { get; private set; }
        public Menu Rechercher { get; private set; }
        public Menu Principal { get; private set; }

        public MenuManager()
        {
            Inserer = InitMenuInserer();
            Rechercher = InitMenuRechercher();
            Principal = InitMenuPrincipal();
        }

        private Menu InitMenuPrincipal()
        {
            List<string> options = new List<string>
            {
                "Insérer un nouvel ouvrage",
                "Rechercher un ouvrage",
                "Statistiques"
            };


            List<Action> actions = new List<Action>() 
            { 
                Inserer.Afficher,
                Rechercher.Afficher,
                AfficherStatistique
            };

            return new Menu("Menu", options, actions);
        }

        // -------------------------------------
        // Menu Insérer
        // -------------------------------------
        private Menu InitMenuInserer()
        {
            MongoClient dbClient = new MongoClient("mongodb://localhost:27017");
            var database = dbClient.GetDatabase("tp-partie-2");
            var collection = database.GetCollection<Publication>("ouvrages");

            List<string> menuInserer_options = new List<string>
            {
                "Insérer un périodique",
                "Insérer un livre",
                "Insérer une BD"
            };

            List<Action> menuInserer_actions = new List<Action>
            {
                () => InsererPeriodique(collection),
                () => InsererLivre(collection),
                () => InsererBD(collection)
            };

            return new Menu("Menu Insertion", menuInserer_options, menuInserer_actions);
        }

        private void InsererPeriodique(IMongoCollection<Publication> collection)
        {
            Console.Clear();

            Periodical periodique = new Periodical();

            Console.Write("Titre : ");
            periodique.Title = Console.ReadLine();
            Console.Write("Disponible (oui/non) : ");
            periodique.Available = Console.ReadLine() == "oui";
            Console.Write("Prix : ");
            periodique.Price = double.Parse(Console.ReadLine());
            Console.Write("Date : ");
            periodique.Details.Date = Console.ReadLine();
            Console.Write("Périodicité : ");
            periodique.Details.Frequency = Console.ReadLine();

            try
            {
                collection.InsertOne(periodique);
                Console.WriteLine("Insertion réussi!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Appuyer sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void InsererLivre(IMongoCollection<Publication> collection)
        {
            Console.Clear();

            Book livre = new Book();

            Console.Write("Titre : ");
            livre.Title = Console.ReadLine();
            Console.Write("Disponible (oui/non) : ");
            livre.Available = Console.ReadLine() == "oui";
            Console.Write("Prix : ");
            livre.Price = double.Parse(Console.ReadLine());
            Console.Write("Année : ");
            livre.Details.Year = Console.ReadLine();
            Console.Write("Maison d'édition : ");
            livre.Details.Publisher = Console.ReadLine();
            Console.Write("Auteur : ");
            livre.Details.Author = Console.ReadLine();

            try
            {
                collection.InsertOne(livre);
                Console.WriteLine("Insertion réussi!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Appuyer sur une touche pour continuer...");
            Console.ReadKey();
        }

        private void InsererBD(IMongoCollection<Publication> collection)
        {
            Console.Clear();

            Comic bd = new Comic();

            Console.Write("Titre : ");
            bd.Title = Console.ReadLine();
            Console.Write("Disponible (oui/non) : ");
            bd.Available = Console.ReadLine() == "oui";
            Console.Write("Prix : ");
            bd.Price = double.Parse(Console.ReadLine());
            Console.Write("Année : ");
            bd.Details.Year = Console.ReadLine();
            Console.Write("Maison d'édition : ");
            bd.Details.Publisher = Console.ReadLine();
            Console.Write("Auteur : ");
            bd.Details.Author = Console.ReadLine();
            Console.Write("Dessinateur : ");
            bd.Details.Illustrator = Console.ReadLine();

            try
            {
                collection.InsertOne(bd);
                Console.WriteLine("Insertion réussi!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Appuyer sur une touche pour continuer...");
            Console.ReadKey();
        }

        // -------------------------------------
        // Menu Rechercher
        // -------------------------------------
        private Menu InitMenuRechercher()
        {
            List<string> options = new List<string>
            {
                "Rechercher un ouvrage selon le type",
                "Rechercher une BD selon le dessinateur"
            };

            List<Action> actions = new List<Action>
            {
                RechercherOuvrageType,
                RechercherBdDessinateur
            };

            return new Menu("Menu Recherche", options, actions);
        }

        private void RechercherOuvrageType()
        {
            Console.WriteLine("\nRecherche d'un ouvrage selon le type... (à implémenter)");
            Console.ReadKey();
        }

        private void RechercherBdDessinateur()
        {
            Console.WriteLine("\nRecherche d'une BD selon le dessinateur... (à implémenter)");
            Console.ReadKey();
        }

        // -------------------------------------
        // Menu Statistique
        // -------------------------------------
        
        private void AfficherStatistique()
        {
            Console.WriteLine("\nAfficher le nombre d'ouvrage par type... (à implémenter)");
            Console.WriteLine("\nAfficher le prix moyen par type d'ouvrage... (à implémenter)");
            Console.ReadKey();
        }
    }
}
