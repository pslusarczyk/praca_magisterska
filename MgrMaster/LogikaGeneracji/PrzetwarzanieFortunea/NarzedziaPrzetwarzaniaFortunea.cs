using UnityEngine;
using ZewnetrzneBiblioteki.FortuneVoronoi;

namespace LogikaGeneracji.PrzetwarzanieFortunea
{
    static internal class NarzedziaPrzetwarzaniaFortunea
    {
        public static Vector3 VectorNaVector3(Vector w)
        {
            return new Vector3((float)w[0], 0f, (float)w[1]);
        }
    }
}