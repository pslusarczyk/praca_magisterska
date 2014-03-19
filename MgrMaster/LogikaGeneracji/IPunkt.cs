using System.Collections.Generic;

namespace LogikaGeneracji
{
    public interface IPunkt
    {
        IEnumerable<IPunkt> Sasiedzi { get; set; }
        IEnumerable<IPunkt> NajnizszySasiad { get; set; }
    }
}