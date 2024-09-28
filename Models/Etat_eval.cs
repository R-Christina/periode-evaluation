using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace periode.Models
{
    public class Etat_eval
    {
        [Key]
        public int etat_id {get; set;}
        public required string etat_nom {get; set;}

        [JsonIgnore]
        public virtual ICollection<Evaluation>? evaluation { get; set; }
    }
}