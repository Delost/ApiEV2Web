using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiEv2.Models;

public partial class Usuario
{
    public string Usuario1 { get; set; } = null!;

    public string NomPersona { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int IdEstado { get; set; }


    [JsonIgnore]
    public virtual Estado? IdEstadoNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Salidum> Salida { get; set; } = new List<Salidum>();
}
