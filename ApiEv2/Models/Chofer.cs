using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiEv2.Models;

public partial class Chofer
{
    public int IdChofer { get; set; }

    public string NomChofer { get; set; } = null!;

    public int IdEstado { get; set; }


    [JsonIgnore]
    public virtual Estado? IdEstadoNavigation { get; set; } = null!;
    
    [JsonIgnore]
    public virtual ICollection<Salidum> Salida { get; set; } = new List<Salidum>();
}
