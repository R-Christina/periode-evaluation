using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace periode.Models
{
    public class Evaluation
    {
        [Key]
        public int eval_id { get; set; }
        public int eval_annee { get; set; }
        public DateTime fixation_objectif { get; set; }
        public DateTime mi_parcours { get; set; }
        public DateTime final { get; set; }
        public int etat_id { get; set; }

        public Etat_eval? etat { get; set; }
    }
}