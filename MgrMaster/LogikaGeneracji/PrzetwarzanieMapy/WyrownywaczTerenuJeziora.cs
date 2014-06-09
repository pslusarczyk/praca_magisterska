using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class WyrownywaczTerenuJeziora : BazaPrzetwarzacza
   {
      private Dictionary<IPunkt, List<IPunkt>> _grupy;
      private IEnumerable<IPunkt> _punktyJeziorne;
      private HashSet<IPunkt> _odwiedzone;

      public override void Przetwarzaj(IMapa mapa)
      {
         _grupy = new Dictionary<IPunkt, List<IPunkt>>();
         _punktyJeziorne = mapa.Komorki
            .Where(k => k.Dane.Typ == TypKomorki.Jezioro)
            .SelectMany(k => k.Rogi.Select(r => r.Punkt).Union(new[] {k.Punkt}))
            .ToList();
         _odwiedzone = new HashSet<IPunkt>();

         foreach (IPunkt punkt in _punktyJeziorne)
         {
            if (!_odwiedzone.Contains(punkt))
               Odwiedz(punkt, null);
         }

         foreach (List<IPunkt> grupa in _grupy.Values)
         {
            float minimalnaWysokosc = grupa.Min(p => p.Wysokosc);
            grupa.ToList().ForEach(p => p.Wysokosc = minimalnaWysokosc);
         }
      }

      private void Odwiedz(IPunkt odwiedzany, IPunkt reprezentantGrupy)
      {
         _odwiedzone.Add(odwiedzany);

         if (reprezentantGrupy == null)
         {
            reprezentantGrupy = odwiedzany;
            _grupy.Add(reprezentantGrupy, new List<IPunkt>());
         }
        _grupy[reprezentantGrupy].Add(odwiedzany);
         
         IEnumerable<IPunkt> sasiedzi = JeziorniSasiedziPunktu(_punktyJeziorne, odwiedzany).ToList();
         foreach (IPunkt sasiad in sasiedzi.Where(s => !_odwiedzone.Contains(s))) 
         {
               Odwiedz(sasiad, reprezentantGrupy);
         }
      }

      private IEnumerable<IPunkt> JeziorniSasiedziPunktu(
         IEnumerable<IPunkt> jeziorne, IPunkt punkt)
      {
         return punkt.Sasiedzi.Where(s => jeziorne.Contains(s));
      }
   }
}