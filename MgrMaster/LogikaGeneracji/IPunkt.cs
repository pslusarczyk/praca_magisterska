using System.Collections.Generic;
using UnityEngine;

namespace LogikaGeneracji
{
    public interface IPunkt
    {
        IList<IPunkt> Sasiedzi { get; set; }
        IPunkt NajnizszySasiad { get; set; }
        Vector3 Pozycja { get; set; }
    }
}