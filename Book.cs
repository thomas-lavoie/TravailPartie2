﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravailPartie2
{
    public class Book : Publication
    {
        public override string Type => "livre";

        [BsonElement("exemplaires")]
        public string[] Copies { get; set; }

        [BsonElement("détails")]
        public BookDetails Details { get; set; }

        public Book()
        {
            Details = new BookDetails();
        }

        public Book(string title, bool available, double price, string[] copies, string year, string publisher, string author) : base(title, available, price)
        {
            Copies = copies;
            Details = new BookDetails(year, publisher, author);
        }
    }

    public class BookDetails
    {
        [BsonElement("année")]
        public string Year { get; set; }

        [BsonElement("maison d'édition")]
        public string Publisher { get; set; }

        [BsonElement("auteur")]
        public string Author { get; set; }

        public BookDetails() { }

        public BookDetails(string year, string publisher, string author)
        {
            Year = year;
            Publisher = publisher;
            Author = author;
        }
    }
}
