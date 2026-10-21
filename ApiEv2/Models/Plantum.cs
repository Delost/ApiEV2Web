using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiEv2.Models;

public partial class Plantum
{
    public int IdPlanta { get; set; }

    public string NomPlanta { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Camion> Camions { get; set; } = new List<Camion>();

    [JsonIgnore]
    public virtual ICollection<Salidum> Salida { get; set; } = new List<Salidum>();
}
