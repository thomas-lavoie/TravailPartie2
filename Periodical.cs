using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravailPartie2
{
    public class Periodical : Publication
    {
        public override string Type => "périodique";

        [BsonElement("détails")]
        [JsonProperty("détails")]
        public PeriodicalDetails Details { get; set; }

        public Periodical()
        {
            Details = new PeriodicalDetails();
        }

        public Periodical(string title, bool available, double price, string date, string frequency) : base(title, available, price)
        {
            Details = new PeriodicalDetails(date, frequency);
        }
    }

    public class PeriodicalDetails
    {
        [BsonElement("date")]
        [JsonProperty("date")]
        public string Date { get; set; }

        [BsonElement("périodicité")]
        [JsonProperty("périodicité")]
        public string Frequency { get; set; }

        public PeriodicalDetails() { }

        public PeriodicalDetails(string date, string frequency)
        {
            Date = date;
            Frequency = frequency;
        }
    }
}
