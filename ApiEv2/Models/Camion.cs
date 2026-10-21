using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiEv2.Models;

public partial class Camion
{
    public string CodCamion { get; set; } = null!;

    public int IdPlanta { get; set; }

    public int IdEstado { get; set; }

    [JsonIgnore]
    public virtual Estado? IdEstadoNavigation { get; set; } = null!;
    
    [JsonIgnore]
    public virtual Plantum? IdPlantaNavigation { get; set; } = null!;
    
    [JsonIgnore]
    public virtual ICollection<Salidum> Salida { get; set; } = new List<Salidum>();
}
