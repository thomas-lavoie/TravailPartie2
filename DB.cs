using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using MongoDB.Bson;
using System.Diagnostics;
using System.Text;



namespace TravailPartie2
{

    public static class BD
    {
        private static readonly MongoClient dbClient = new MongoClient("mongodb://localhost:27017");
        public static readonly IMongoDatabase database = dbClient.GetDatabase("tp-partie-2");

        public static IMongoCollection<Publication> ObtenirCollection(string collectionName)
        {
            return database.GetCollection<Publication>(collectionName);
        }

        public static IMongoCollection<BsonDocument> ObtenirCollectionRaw(string collectionName)
        {
            return database.GetCollection<BsonDocument>(collectionName);
        }

        public static void ResetData(string collectionName, string jsonFile)
        {
            SupprimerCollection(collectionName);
            var collection = database.GetCollection<Publication>(collectionName);
            ImporterJson(collection, jsonFile);
        }

        public static void SupprimerCollection(string collectionName)
        {
            var collections = database.ListCollectionNames().ToList();

            if (collections.Contains(collectionName))
            {
                database.DropCollection(collectionName);
            }
        }

        public static void ImporterJson(IMongoCollection<Publication> collection, string jsonFile)
        {
            if (!File.Exists(jsonFile))
            {
                return;
            }

            if (collection.CountDocuments(FilterDefinition<Publication>.Empty) > 0)
            {
                return;
            }

            try
            {
                var jsonText = File.ReadAllText(jsonFile);
                var data = JArray.Parse(jsonText);

                foreach (var item in data)
                {
                    string type = item["type"]?.ToString()?.ToLower();
                    string itemJson = item.ToString();

                    switch (type)
                    {
                        case "livre":
                            var book = JsonConvert.DeserializeObject<Book>(itemJson);
                            collection.InsertOne(book);
                            break;

                        case "périodique":
                            var periodical = JsonConvert.DeserializeObject<Periodical>(itemJson);
                            collection.InsertOne(periodical);
                            break;

                        case "bd":
                            var comic = JsonConvert.DeserializeObject<Comic>(itemJson);
                            collection.InsertOne(comic);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public static class Tools
    {
        public static void AfficherDansNotepad(string message)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "résultats.txt");
            File.WriteAllText(tempPath, message, Encoding.UTF8);
            Process.Start(new ProcessStartInfo
            {
                FileName = "notepad.exe",
                Arguments = $"\"{tempPath}\"",
                UseShellExecute = true
            });
        }

    }


}
