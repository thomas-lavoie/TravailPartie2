using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Driver;
using System.Threading;
using System.Text.RegularExpressions;

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

                Console.WriteLine("Équipe  :  Keven Bellavance Boisclair &\n" +
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
                "Statistiques",
                "Réinitialisation de la collection ouvrages"
            };


            List<Action> actions = new List<Action>() 
            { 
                Inserer.Afficher,
                Rechercher.Afficher,
                AfficherStatistique,
                () => {
                    BD.ResetData("ouvrages", "default-data.json");
                    Console.WriteLine("\nRéinitialisation effectuée ! Appuyez sur une touche pour continuer...");
                    Console.ReadKey();
                }
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

            Periodical periodique = new();

            try
            {
                Console.Write("Titre : ");
                periodique.Title = Console.ReadLine();

                Console.Write("Disponible (oui/non) : ");
                periodique.Available = Console.ReadLine().ToLower() == "oui";

                Console.Write("Prix : ");
                periodique.Price = double.Parse(Console.ReadLine());

                int année = DateTime.Now.Year;
                int mois = DateTime.Now.Month;
                int jour = DateTime.Now.Day;
                ConsoleKey? key = null;
                int selectedPortion = 0;
                while (key != ConsoleKey.Enter)
                {
                    Console.Write("\rDate : ");

                    if (selectedPortion == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.Write(année);
                    Console.ResetColor();

                    Console.Write("-");

                    if (selectedPortion == 1)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.Write(mois.ToString().PadLeft(2, '0'));
                    Console.ResetColor();

                    Console.Write("-");

                    if (selectedPortion == 2)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.Write(jour.ToString().PadLeft(2, '0'));
                    Console.ResetColor();

                    // Espace supplémentaire pour s'assurer
                    // que la ligne est bien effacé
                    Console.Write("     ");

                    key = Console.ReadKey().Key;
                    switch (key)
                    {
                        case ConsoleKey.LeftArrow:
                            if (selectedPortion > 0)
                                --selectedPortion;
                            break;
                        case ConsoleKey.RightArrow:
                            if (selectedPortion < 2)
                                ++selectedPortion;
                            break;
                        case ConsoleKey.DownArrow:
                            switch (selectedPortion)
                            {
                                case 0:
                                    --année;
                                    break;
                                case 1:
                                    mois = (mois - 1) % 13;
                                    if (mois < 1)
                                        mois = 12;
                                    break;
                                case 2:
                                    jour = (jour - 1) % (DateTime.DaysInMonth(année, mois) + 1);
                                    if (jour < 0)
                                        jour = DateTime.DaysInMonth(année, mois);
                                    break;
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            switch (selectedPortion)
                            {
                                case 0:
                                    if (année < 2025)
                                        ++année;
                                    break;
                                case 1:
                                    mois = (mois + 1) % 13;
                                    if (mois < 1)
                                        mois = 1;
                                    break;
                                case 2:
                                    jour = (jour + 1) % (DateTime.DaysInMonth(année, mois) + 1);
                                    break;
                            }
                            break;
                    }
                }
                string parsedDate = $"{année}-{mois.ToString().PadLeft(2, '0')}-{jour.ToString().PadLeft(2, '0')}";
                Console.WriteLine($"\rDate : {parsedDate}");
                periodique.Details.Date = $"{parsedDate}";

                Console.Write("Périodicité : ");
                periodique.Details.Frequency = Console.ReadLine();

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

            Book livre = new();

            try
            {
                Console.Write("Titre : ");
                livre.Title = Console.ReadLine();

                Console.Write("Disponible (oui/non) : ");
                livre.Available = Console.ReadLine().ToLower() == "oui";

                Console.Write("Prix : ");
                livre.Price = double.Parse(Console.ReadLine());

                int année = DateTime.Now.Year;
                ConsoleKey? key = null;
                while (key != ConsoleKey.Enter)
                {
                    Console.Write("\rAnnée : ");
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(année);
                    Console.ResetColor();

                    // Espace supplémentaire pour s'assurer
                    // que la ligne est bien effacé
                    Console.Write("     ");

                    key = Console.ReadKey().Key;
                    switch (key)
                    {
                        case ConsoleKey.DownArrow:
                            --année;
                            break;
                        case ConsoleKey.UpArrow:
                            if (année < 2025)
                                ++année;
                            break;
                    }
                }
                livre.Details.Year = année.ToString();
                Console.WriteLine($"\rAnnée : {année}");

                Console.Write("Maison d'édition : ");
                livre.Details.Publisher = Console.ReadLine();

                Console.Write("Auteur : ");
                livre.Details.Author = Console.ReadLine();

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

            Comic bd = new();

            try
            {
                Console.Write("Titre : ");
                bd.Title = Console.ReadLine();

                Console.Write("Disponible (oui/non) : ");
                bd.Available = Console.ReadLine().ToLower() == "oui";

                Console.Write("Prix : ");
                bd.Price = double.Parse(Console.ReadLine());

                int année = DateTime.Now.Year;
                ConsoleKey? key = null;
                while (key != ConsoleKey.Enter)
                {
                    Console.Write("\rAnnée : ");
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(année);
                    Console.ResetColor();

                    // Espace supplémentaire pour s'assurer
                    // que la ligne est bien effacé
                    Console.Write("     ");

                    key = Console.ReadKey().Key;
                    switch (key) {
                        case ConsoleKey.DownArrow:
                            --année;
                            break;
                        case ConsoleKey.UpArrow:
                            if (année < 2025)
                                ++année;
                            break;
                    }
                }
                bd.Details.Year = année.ToString();
                Console.WriteLine($"\rAnnée : {année}");

                Console.Write("Maison d'édition : ");
                bd.Details.Publisher = Console.ReadLine();

                Console.Write("Auteur : ");
                bd.Details.Author = Console.ReadLine();

                Console.Write("Dessinateur : ");
                bd.Details.Illustrator = Console.ReadLine();

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
            string message = "";
            List<BsonDocument> résultats = new List<BsonDocument>();

            var collection = BD.ObtenirCollectionRaw("ouvrages");
            var types = collection.Distinct<string>("type", FilterDefinition<BsonDocument>.Empty).ToList();

            if (types.Count == 0)
            {
                Console.Write("Aucun type trouvé ! Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\n--- Types d'ouvrages disponibles ---\n");
            for (int i = 0; i < types.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {types[i]}");
            }

            Console.Write("\nEntrez le numéro du type à rechercher : ");
            if (!int.TryParse(Console.ReadLine(), out int choix) || choix < 1 || choix > types.Count)
            {
                Console.Write("Choix invalide ! Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                return;
            }

            string typeChoisi = types[choix - 1];

            var filtre = Builders<BsonDocument>.Filter.Eq("type", typeChoisi);
            résultats = collection.Find(filtre).ToList();

            message += $"--- Résultats de la recherche (Ouvrages de type : {typeChoisi}) ---\n";

            foreach (var doc in résultats)
            {
                string titre = doc.GetValue("titre", "").AsString;
                double prix = doc.GetValue("prix", 0.0).ToDouble();
                bool dispo = doc.GetValue("dispo", false).ToBoolean();

                message += $"{titre} ({typeChoisi})\n";
                message += $"Prix       : {prix:F2} $\n";
                message += $"Disponible : {(dispo ? "Oui" : "Non")}\n";

                if (doc.Contains("détails") && doc["détails"].IsBsonDocument)
                {
                    var details = doc["détails"].AsBsonDocument;
                    foreach (var champ in details.Elements)
                    {
                        message += $"{char.ToUpper(champ.Name[0])}{champ.Name.Substring(1)} : {champ.Value}\n";
                    }
                }

                if (doc.Contains("exemplaires") && doc["exemplaires"].IsBsonArray)
                {
                    var exs = doc["exemplaires"].AsBsonArray;
                    if (exs.Count > 0)
                    {
                        message += $"Exemplaires : {string.Join(", ", exs.Select(e => e.ToString()))}\n";
                    }
                }

                message += "\n";
            }

            Tools.AfficherDansNotepad(message);
            Console.WriteLine("\nUne nouvelle fenêtre du Bloc-notes a été ouverte pour afficher les résultats. Vous pouvez la consulter dès maintenant.");
            Console.WriteLine("Appuyez sur une touche pour continue...");
            Console.ReadKey();
        }

        private void RechercherBdDessinateur()
        {
            string message = "";
            List<BsonDocument> résultats = new List<BsonDocument>();
            var collection = BD.ObtenirCollectionRaw("ouvrages");

            Console.Write("\nVeuillez inscrire le nom du dessinateur recherché : ");
            string rechercheDessinateur = Console.ReadLine();

            if (string.IsNullOrEmpty(rechercheDessinateur))
            {
                Console.Write("\nAucun valeur saisie ! Appuyez sur une touche pour recommencer...");
                Console.ReadKey();
                return;
            }

            string rechercheSécurisée = Regex.Escape(rechercheDessinateur);
            var filtreType = Builders<BsonDocument>.Filter.Eq("type", "BD");
            var filtreDessinateur = Builders<BsonDocument>.Filter.Regex(
                "détails.dessinateur",
                new BsonRegularExpression(rechercheSécurisée, "i") // non case sensitive
            );
            var filtreCombiné = Builders<BsonDocument>.Filter.And(filtreType, filtreDessinateur);

            résultats = collection.Find(filtreCombiné).ToList();

            message += $"--- Résultats de la recherche (par auteurs de BD) ---\n\n";

            if (résultats == null || résultats.Count() == 0)
            {
                Console.Write("\nAucune valeur trouvée ! Appuyez sur une touche pour recommencer...");
                Console.ReadKey();
                return;
            }

            foreach (var doc in résultats)
            {
                string titre = doc.GetValue("titre", "").AsString;
                double prix = doc.GetValue("prix", 0.0).ToDouble();
                bool dispo = doc.GetValue("dispo", false).ToBoolean();
                string type = doc.GetValue("type", "").AsString;

                message += $"{titre} ({type})\n";
                message += $"Prix       : {prix:F2} $\n";
                message += $"Disponible : {(dispo ? "Oui" : "Non")}\n";

                if (doc.Contains("détails") && doc["détails"].IsBsonDocument)
                {
                    var details = doc["détails"].AsBsonDocument;
                    foreach (var champ in details.Elements)
                    {
                        message += $"{char.ToUpper(champ.Name[0])}{champ.Name.Substring(1)} : {champ.Value}\n";
                    }
                }

                if (doc.Contains("exemplaires") && doc["exemplaires"].IsBsonArray)
                {
                    var exs = doc["exemplaires"].AsBsonArray;
                    if (exs.Count > 0)
                    {
                        message += $"Exemplaires : {string.Join(", ", exs.Select(e => e.ToString()))}\n";
                    }
                }

                message += "\n";
            }
            Tools.AfficherDansNotepad(message);
            Console.WriteLine("\nUne nouvelle fenêtre du Bloc-notes a été ouverte pour afficher les résultats. Vous pouvez la consulter dès maintenant.");
            Console.WriteLine("Appuyez sur une touche pour continue...");
            Console.ReadKey();
        }

        // -------------------------------------
        // Menu Statistique
        // -------------------------------------

        private void AfficherStatistique()
        {
            string message = "";
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$type" },
                    { "total", new BsonDocument("$sum", 1) },
                    { "prixMoyen", new BsonDocument("$avg", "$prix") }
                }),
                new BsonDocument("$sort", new BsonDocument("_id", 1))
            };

            var collection = BD.ObtenirCollection("ouvrages");
            var résultats = collection.Aggregate<BsonDocument>(pipeline).ToList();

            message += ("--- Statistiques par type d'ouvrage ---\n");

            foreach (var doc in résultats)
            {
                var type = doc["_id"].AsString;
                var total = doc["total"].AsInt32;
                var prixMoyen = doc["prixMoyen"].ToDouble();

                message += ($"\n{type} :\n");
                message += ($"--> Nb d'ouvrage(s) : {total}\n");
                message += ($"--> prix moyen      : {prixMoyen:F2} $\n");
            }

            Tools.AfficherDansNotepad(message);
            Console.WriteLine("\nUne nouvelle fenêtre du Bloc-notes a été ouverte pour afficher les résultats. Vous pouvez la consulter dès maintenant.");
            Console.WriteLine("Appuyez sur une touche pour continue...");
            Console.ReadKey();
        }
    }
}
