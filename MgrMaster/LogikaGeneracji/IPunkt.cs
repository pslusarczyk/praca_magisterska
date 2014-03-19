using System.Collections.Generic;
using UnityEngine;

namespace LogikaGeneracji
{
    public interface IPunkt
    {
        IEnumerable<IPunkt> Sasiedzi { get; set; }
        IEnumerable<IPunkt> NajnizszySasiad { get; set; }
        Vector3 Pozycja { get; set; }
    }
}