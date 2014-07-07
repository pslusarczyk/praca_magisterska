using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace LogikaGeneracji.PrzetwarzanieFortunea
{
    static internal class NarzedziaPrzetwarzaniaFortunea
    {
       public static MojVector3 VectorNaMojVector3(Vector w)
        {
            return new MojVector3((float)w[0], 0f, (float)w[1]);
        }
    }
}