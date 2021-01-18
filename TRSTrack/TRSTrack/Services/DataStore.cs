using TRSTrack.Models;
using Realms;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TRSTrack.Services;
using Xamarin.Forms;
using System.Text;
using System.Globalization;
using TRSTrack.Controllers;

[assembly: Dependency(typeof(DataStore))]
namespace TRSTrack.Services
{
    /// <summary>
    /// Classe para transacionar data base local
    /// </summary>
    public class DataStore
    {
        private readonly Realm _realm;

        /// <summary>
        /// Default contructor
        /// </summary>
        public DataStore()
        {
            try
            {
                var config = new RealmConfiguration
                {
                    SchemaVersion = 1,
                    MigrationCallback = (migration, oldSchemaVersion) =>
                    {
                        var newOne = migration.NewRealm.All("LocalLogedUser");
                        var oldOne = migration.OldRealm.All("LocalLogedUser");
                        for (var i = 0; i < newOne.Count(); i++)
                        {
                            var oldObject = oldOne.ElementAt(i);
                            var newObject = newOne.ElementAt(i);
                            //Change neu property value if necessary
                            //newObject.Property = oldObject.Propety
                        }
                    }
                };
                _realm = Realm.GetInstance(config);
            }
            catch (Exception exception)
            {
                var t = exception.Message;
            }
        }

        public int CircuitosCount()
        {
            return _realm.All<Circuito>().Count();
        }

        public void SalvarWayPoint(WayPoint wayPoint)
        {
            var count = _realm.All<WayPoint>().Count();
            var newId = count == 0
                ? 1
                : count + 1;

            wayPoint.Id = newId;
            _realm.Write(() =>
            {
                _realm.Add(wayPoint);
            });
        }

        public void UpdateWayPoint(WayPoint wayPoint)
        {
            var obj = _realm.All<WayPoint>().FirstOrDefault(b => b.Id == wayPoint.Id && b.Circuito == wayPoint.Circuito);
            if (obj == null) return;
            using (var trans = _realm.BeginWrite())
            {
                obj.Nome = wayPoint.Nome;
                obj.Cor = wayPoint.Cor;
                trans.Commit();
            }
        }

        public void ExcluirWayPoint(WayPoint wayPoint)
        {
            var obj = _realm.All<WayPoint>().FirstOrDefault(b => b.Id == wayPoint.Id && b.Circuito == wayPoint.Circuito);
            using (var db = _realm.BeginWrite())
            {
                _realm.Remove(obj);
                db.Commit();
            }
        }

        public ObservableCollection<WayPoint> GetWayPoint(Circuito circuito = null, bool onlyWaypoints = false)
        {
            var objects = circuito == null
                ? _realm.All<WayPoint>()
                : _realm.All<WayPoint>().Where(b => b.Circuito == circuito.Id);
            var list = new ObservableCollection<WayPoint>();
            if (objects == null) return list;
            foreach (var obj in objects)
            {
                if (onlyWaypoints)
                {
                    if (obj.IsWayPont)
                    {
                        list.Add(obj);
                    }
                }
                else
                {
                    list.Add(obj);
                }
            }
            return list;
        }

        public ObservableCollection<Circuito> CircuitoGetList()
        {
            return new ObservableCollection<Circuito>(_realm.All<Circuito>().ToList());
        }

        public Circuito SalvarCircuito(Circuito circuito)
        {
            var count = CircuitosCount();
            var newId = count == 0
                ? 0
                : count + 1;

            circuito.Id = newId;
            _realm.Write(() => _realm.Add(circuito));
            return new Circuito 
            { 
                Id = newId,
                Nome = circuito.Nome,
                Cidade = circuito.Cidade,
                Data = circuito.Data,
                Distancia = circuito.Distancia
            };
        }

        public void UpdateCircuito(Circuito circuito)
        {
            var obj = _realm.All<Circuito>().FirstOrDefault(b => b.Id == circuito.Id);
            if (obj == null) return;
            using (var trans = _realm.BeginWrite())
            {
                obj.Nome = circuito.Nome;
                obj.Cidade = circuito.Cidade;
                trans.Commit();
            }
        }

        public void ExcluirCircuito(Circuito circuito)
        {
            var obj = _realm.All<Circuito>().FirstOrDefault(b => b.Id == circuito.Id);

            //Exclui os waypoins vinculados ao circuito
            var wayPoints = GetWayPoint(circuito);
            foreach (var wayPoint in wayPoints)
            {
                using (var db = _realm.BeginWrite())
                {
                    _realm.Remove(wayPoint);
                    db.Commit();
                }
            }

            //Exclui o circuito
            using (var db = _realm.BeginWrite())
            {
                _realm.Remove(obj);
                db.Commit();
            }
        }

        public void ExcluirTodosCircuitos()
        {
            var circuitos = _realm.All<Circuito>();
            foreach (var item in circuitos)
            {
                ExcluirCircuito(item);
            };
        }

        public void SalvarRadialMenuPosition(RadialMenuPosition radialMenuPosition)
        {
            var obj = _realm.All<RadialMenuPosition>().FirstOrDefault();
            if (obj == null)
            {
                _realm.Write(() => _realm.Add(radialMenuPosition));
            }
            else
            {
                _realm.Write(() => _realm.Add(radialMenuPosition, true));
            }
        }

        public RadialMenuPosition GetRadialMenuPosition()
        {
            var obj = _realm.All<RadialMenuPosition>().FirstOrDefault();
            return obj ?? new RadialMenuPosition
            {
                Id = 1,
                X = 100,
                Y = 150
            };
        }

        public void SalvarCurrentMapZoom(MapZoom mapZoom)
        {
            var obj = _realm.All<MapZoom>().FirstOrDefault();
            if (obj == null)
            {
                _realm.Write(() => _realm.Add(mapZoom));
            }
            else
            {
                _realm.Write(() => _realm.Add(mapZoom, true));
            }
        }

        public MapZoom GetCurrentMapZoom()
        {
            var obj = _realm.All<MapZoom>().FirstOrDefault();
            return obj ?? new MapZoom
            {
                Id = 1,
                Level = 100
            };
        }

        public void SalvarScreenOptions(ScreenOptions screenOptions)
        {
            var obj = _realm.All<ScreenOptions>().FirstOrDefault();
            if (obj == null)
            {
                _realm.Write(() => _realm.Add(screenOptions));
            }
            else
            {
                _realm.Write(() => _realm.Add(screenOptions, true));
            }
        }

        public ScreenOptions GetScreenOptions()
        {
            var obj = _realm.All<ScreenOptions>().FirstOrDefault();
            return obj ?? new ScreenOptions
            {
                Id = 1,
                ShowMenuBotoes = false,
                ShowVelocimeter = true,
                ShowWayPointsList = false,
                MapType = 1,
                IsUnlocked = false
            };
        }

        public RecordAdjust GetRecordAdjust()
        {
            var obj = _realm.All<RecordAdjust>().FirstOrDefault();
            return obj ?? new RecordAdjust
            {
                Id = 1,
                MinimumMeters = 0,
                MinimumVelocity = 8
            };
        }

        public void SalvarRecordAdjust(RecordAdjust recordAdjust)
        {
            var obj = _realm.All<RecordAdjust>().FirstOrDefault();
            if (obj == null)
            {
                _realm.Write(() => _realm.Add(recordAdjust));
            }
            else
            {
                _realm.Write(() => _realm.Add(recordAdjust, true));
            }
        }

        public int CircuitosMesmoNome(string nome)
        {
            var repetidos = 0;
            var circuitos = CircuitoGetList();
            foreach (var circuito in circuitos)
            {
                if (RemoveDiacritics(circuito.Nome.ToLower()) == RemoveDiacritics(nome.ToLower()))
                {
                    repetidos++;
                }
            }
            return repetidos;
        }

        private static string RemoveDiacritics(string text)
        {
            return string.Concat(
                text.Normalize(NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
              ).Normalize(NormalizationForm.FormC);
        }
    }
}
