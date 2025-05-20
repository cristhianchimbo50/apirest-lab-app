using System;
using System.Collections.Generic;

namespace ApiRest_LabWebApp.Models
{
    public partial class ExamenComposicion
    {
        public int IdExamenPadre { get; set; }
        public int IdExamenHijo { get; set; }

        public virtual Examen? ExamenPadre { get; set; }
        public virtual Examen? ExamenHijo { get; set; }
    }
}
