using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravailPartie2
{
    public class Comic : Publication
    {
        public override string Type => "BD";

        [BsonElement("détails")]
        public ComicDetails Details { get; set; }
        
        public Comic() { }

        public Comic(string title, bool available, double price, string year, string publisher, string author, string illustrator) : base(title, available, price)
        {
            Details = new ComicDetails(year, publisher, author, illustrator);
        }
    }

    public class ComicDetails
    {
        [BsonElement("année")]
        public string Year { get; set; }

        [BsonElement("maison d'édition")]
        public string Publisher { get; set; }

        [BsonElement("auteur")]
        public string Author { get; set; }

        [BsonElement("dessinateur")]
        public string Illustrator { get; set; }

        public ComicDetails() { }

        public ComicDetails(string year, string publisher, string author, string illustrator)
        {
            Year = year;
            Publisher = publisher;
            Author = author;
            Illustrator = illustrator;
        }
    }
}
