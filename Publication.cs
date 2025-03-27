using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravailPartie2
{
    [BsonDiscriminator("publication")]
    public abstract class Publication
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }

        [BsonElement("titre")]
        [JsonProperty("titre")]
        public string Title { get; set; }

        [BsonElement("dispo")]
        [JsonProperty("dispo")]
        public bool Available { get; set; }

        [BsonElement("prix")]
        [JsonProperty("prix")]
        public double Price { get; set; }

        [BsonElement("type")]
        [JsonProperty("type")]
        public abstract string Type { get; }

        public Publication() { }

        public Publication(string title, bool available, double price)
        {
            Title = title;
            Available = available;
            Price = price;
        }
    }

    public class InvalidPublicationException : Exception;
}
