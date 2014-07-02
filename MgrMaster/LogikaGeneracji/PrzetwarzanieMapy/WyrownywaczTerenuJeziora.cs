using System.Collections.Generic;
using System.Linq;
using LogikaGeneracji.PrzetwarzanieMapy.Baza;

namespace LogikaGeneracji.PrzetwarzanieMapy
{
   public class WyrownywaczTerenuJeziora : BazaPrzetwarzacza
   {
      private Dictionary<IKomorka, List<IPunkt>> _grupy;
      private IEnumerable<IKomorka> _komorkiJeziorne;
      private HashSet<IKomorka> _odwiedzone;

      public override void Przetwarzaj(IMapa mapa)
      {
         _grupy = new Dictionary<IKomorka, List<IPunkt>>();
         _komorkiJeziorne = mapa.Komorki
            .Where(k => k.Dane.Typ == TypKomorki.Jezioro)
            .ToList();
         _odwiedzone = new HashSet<IKomorka>();

         foreach (IKomorka komorka in _komorkiJeziorne)
         {
            if (!_odwiedzone.Contains(komorka))
               Odwiedz(komorka, null);
         }
         
         foreach (List<IPunkt> grupa in _grupy.Values)
         {
            float minimalnaWysokosc = grupa.Min(k => k.Wysokosc);
            grupa.ToList().ForEach(k => k.Wysokosc = minimalnaWysokosc);
         }
      }

      private void Odwiedz(IKomorka odwiedzana, IKomorka reprezentantGrupy)
      {
         _odwiedzone.Add(odwiedzana);

         if (reprezentantGrupy == null)
         {
            reprezentantGrupy = odwiedzana;
            _grupy.Add(reprezentantGrupy, new List<IPunkt>());
         }
        _grupy[reprezentantGrupy].Add(odwiedzana.Punkt);
         foreach (var rog in odwiedzana.Rogi)
         {
            if(!_grupy[reprezentantGrupy].Contains(rog.Punkt) 
               && rog.Komorki.All(k=>k.Dane.Typ==TypKomorki.Jezioro))
               _grupy[reprezentantGrupy].Add(rog.Punkt);
         }
         
         IEnumerable<IKomorka> przylegle = JeziornePrzylegleKomorki(_komorkiJeziorne, odwiedzana).ToList();
         foreach (IKomorka przylegla in przylegle.Where(s => !_odwiedzone.Contains(s))) 
         {
               Odwiedz(przylegla, reprezentantGrupy);
         }
      }

      private IEnumerable<IKomorka> JeziornePrzylegleKomorki(
         IEnumerable<IKomorka> jeziorne, IKomorka komorka)
      {
         return komorka.PrzylegleKomorki.Where(s => jeziorne.Contains(s));
      }
   }
}