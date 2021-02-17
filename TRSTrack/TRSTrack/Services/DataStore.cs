using TRSTrack.Models;
using Realms;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TRSTrack.Services;
using Xamarin.Forms;
using System.Text;
using System.Globalization;
using TRSTrack.Helpers;

[assembly: Dependency(typeof(DataStore))]
namespace TRSTrack.Services
{
    /// <summary>
    /// Classe para transacionar data base local
    /// </summary>
    public class DataStore
    {
        private Realm _realm;

        /// <summary>
        /// Default contructor
        /// </summary>
        public DataStore()
        {
            var config = new RealmConfiguration
            {
                SchemaVersion = 2,
                MigrationCallback = (migration, oldSchemaVersion) =>
                {
                    var newOne = migration.NewRealm.All("Race");
                    var oldOne = migration.OldRealm.All("Race");
                    for (var i = 0; i < newOne.Count(); i++)
                    {
                        var oldObject = oldOne.ElementAt(i);
                        var newObject = newOne.ElementAt(i);
                    }
                }
            };
            _realm = Realm.GetInstance(config);
        }

        public int CircuitosCount()
        {
            return _realm.All<Circuito>().Count();
        }

        public bool CircuitoHasRaceStatistics(Circuito circuito)
        {
            var has = _realm.All<Race>().Where(p => p.Circuito == circuito.Id).FirstOrDefault();
            return has != null;
        }

        public int WayPointsCount()
        {
            return _realm.All<WayPoint>().Count();
        }

        public int ReceCount()
        {
            return _realm.All<Race>().Count();
        }

        public int LapsCount()
        {
            return _realm.All<RaceLap>().Count();
        }

        public int LapsPartialsCount()
        {
            return _realm.All<RaceLapPartial>().Count();
        }

        public int RaceLapTrackCount()
        {
            return _realm.All<RaceLapTrack>().Count();
        }

        public void SalvarWayPoint(WayPoint wayPoint)
        {
            var lastId = WayPointsCount() == 0
                ? 0
                : _realm.All<WayPoint>().ToList().Max(x => x.Id);

            wayPoint.Id = lastId += 1;
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
                ? _realm.All<WayPoint>().OrderBy(b => b.Id)
                : _realm.All<WayPoint>().Where(b => b.Circuito == circuito.Id).OrderBy(b => b.Id);
            var list = new ObservableCollection<WayPoint>();
            if (objects == null) return list;
            foreach (var obj in objects)
            {
                if (onlyWaypoints)
                {
                    if (obj.IsWayPoint)
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

        public Circuito CircuitoGet(int id)
        {
            return _realm.All<Circuito>().Where(p => p.Id == id).FirstOrDefault();
        }

        public Circuito SalvarCircuito(Circuito circuito)
        {
            var lastId = CircuitosCount() == 0 
                ? 0 
                : _realm.All<Circuito>().ToList().Max(x => x.Id);
            var newId = lastId += 1;
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

            //Exclui estatisticas
            var races = GetReces(circuito);
            foreach (var race in races)
            {
                var laps = GetLaps(race);
                foreach (var lap in laps)
                {
                    var partials = GetLapPartials(lap);
                    foreach (var partial in partials)
                    {
                        using (var db = _realm.BeginWrite())
                        {
                            _realm.Remove(partial);
                            db.Commit();
                        }
                    }
                    var tracks = GetLapTrack(race, lap);
                    foreach (var track in tracks)
                    {
                        using (var db = _realm.BeginWrite())
                        {
                            _realm.Remove(track);
                            db.Commit();
                        }
                    }
                }
                using (var db = _realm.BeginWrite())
                {
                    _realm.Remove(race);
                    db.Commit();
                }
            }

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

        public Race SalvarCorrida(Circuito circuito, ObservableCollection<RaceLapTempItem> laps, string cpf)
        {
            var lastId = ReceCount() == 0
                ? 0
                : _realm.All<Race>().ToList().Max(x => x.Id);
            var raceId = lastId += 1;

            var race = new Race
            {
                Id = raceId,
                Cpf = cpf,
                Nome = $"Corrida {raceId}",
                Circuito = circuito.Id,
                DisplayName = $"Corrida {raceId}, {circuito.Nome} em {DateTime.Now:dd/MM/yyyy}"
            };

            //Salva Corrida
            _realm.Write(() => _realm.Add(race));

            foreach (var lap in laps)
            {
                lastId = LapsCount() == 0
                    ? 0
                    : _realm.All<RaceLap>().ToList().Max(x => x.Id);
                var lapId = lastId += 1;

                ///Media de velocidade
                var dadosVolta = laps.Where(m => m.LapNumber == lap.LapNumber).ToList();
                var velocidadeMedia = 0;
                var tempoTotal = 0;
                foreach (var dados in dadosVolta)
                {
                    velocidadeMedia += dados.Velocidade;
                    tempoTotal += Tools.StringTimeToMileSeconds(dados.TempoParcial);
                }
                velocidadeMedia /= dadosVolta.Count;
                TimeSpan ts = TimeSpan.FromMilliseconds(tempoTotal);
                var volta = new RaceLap
                {
                    Id = lapId,
                    Race = raceId,
                    LapNumber = lap.LapNumber,
                    VelocidadeMedia = velocidadeMedia,
                    TempoTotal = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}",
                };

                ///Salva a volta
                _realm.Write(() => _realm.Add(volta));

                ///Salva parciais da volta
                foreach (var dados in dadosVolta)
                {
                    lastId = LapsPartialsCount() == 0
                        ? 0
                        : _realm.All<RaceLapPartial>().ToList().Max(x => x.Id);
                    var partialId = lastId += 1;
                    var parcial = new RaceLapPartial
                    {

                        Id = partialId,
                        Corrida = lapId,
                        DescricaoPassagem = dados.CheckPoint,
                        NumeroPassagem = volta.LapNumber,
                        TempoPassagem = dados.TempoParcial,
                        VelocidadePassagem = dados.Velocidade
                    };

                    ///Salva parciais da volta
                    _realm.Write(() => _realm.Add(parcial));
                }

                ///Salvar step by step
                var idLapTrack = RaceLapTrackCount() == 0
                    ? 0
                    : _realm.All<RaceLapTrack>().ToList().Max(x => x.Id);
                var track = new RaceLapTrack
                {
                    Id = idLapTrack + 1,
                    Corrida = raceId,
                    LapNumber = lap.LapNumber,
                    Latitude = lap.Latitude,
                    Longitude = lap.Longitude
                };
                _realm.Write(() => _realm.Add(track));
            }

            return race;
        }

        public void ExcluirCorrida(Race race)
        {
            var laps = GetLaps(race);
            foreach (var lap in laps)
            {
                var partials = GetLapPartials(lap);
                foreach (var partial in partials)
                {
                    using (var db = _realm.BeginWrite())
                    {
                        _realm.Remove(partial);
                        db.Commit();
                    }
                }
                var tracks = GetLapTrack(race,lap);
                foreach (var track in tracks)
                {
                    using (var db = _realm.BeginWrite())
                    {
                        _realm.Remove(track);
                        db.Commit();
                    }
                }
                using (var db = _realm.BeginWrite())
                {
                    _realm.Remove(lap);
                    db.Commit();
                }
            }
            using (var db = _realm.BeginWrite())
            {
                _realm.Remove(race);
                db.Commit();
            }
        }

        public ObservableCollection<Race> GetReces(Circuito circuito)
        {
            var objects = _realm.All<Race>().Where(b => b.Circuito == circuito.Id).OrderBy(b => b.Id);
            var list = new ObservableCollection<Race>();
            if (objects == null) return list;
            foreach (var obj in objects)
            {
                list.Add(obj);
            }
            return list;
        }

        public ObservableCollection<Race> GetReces(Race race)
        {
            var objects = _realm.All<Race>().Where(b => b.Id == race.Id);
            var list = new ObservableCollection<Race>();
            if (objects == null) return list;
            foreach (var obj in objects)
            {
                list.Add(obj);
            }
            return list;
        }

        public ObservableCollection<Race> GetReces()
        {
            var objects = _realm.All<Race>().OrderBy(b => b.Id);
            var list = new ObservableCollection<Race>();
            if (objects == null) return list;
            foreach (var obj in objects)
            {
                list.Add(obj);
            }
            return list;
        }

        public ObservableCollection<RaceLap> GetLaps(Race race)
        {
            var objects = _realm.All<RaceLap>().Where(b => b.Race == race.Id).OrderBy(b => b.Id);
            var list = new ObservableCollection<RaceLap>();
            if (objects == null) return list;
            foreach (var obj in objects)
            {
                list.Add(obj);
            }
            return list;
        }

        public RaceLap GetLap(Race race, int lapNumber)
        {
            var objects = _realm.All<RaceLap>().Where(b => b.Race == race.Id);
            var list = new ObservableCollection<RaceLap>();
            if (objects == null) return null;
            foreach (var obj in objects)
            {
                if (obj.LapNumber == lapNumber)
                    list.Add(obj);
            }
            return list[0];
        }

        public ObservableCollection<RaceLapPartial> GetLapPartials(RaceLap raceLap)
        {
            var objects = _realm.All<RaceLapPartial>().Where(b => b.Corrida == raceLap.Id).OrderBy(b => b.Id);
            var list = new ObservableCollection<RaceLapPartial>();
            if (objects == null) return list;
            foreach (var obj in objects)
            {
                list.Add(obj);
            }
            return list;
        }

        public ObservableCollection<RaceLapTrack> GetLapTrack(Race race, RaceLap raceLap)
        {
            var objects = _realm.All<RaceLapTrack>().Where(b => b.Corrida == race.Id && b.LapNumber == raceLap.LapNumber).OrderBy(b => b.Id);
            var list = new ObservableCollection<RaceLapTrack>();
            if (objects == null) return list;
            foreach (var obj in objects)
            {
                list.Add(obj);
            }
            return list;
        }

        public RangeAdjust RangeAdjust()
        {
            var obj = _realm.All<RangeAdjust>().FirstOrDefault();
            return obj ?? new RangeAdjust
            {
                Id = 1,
                Range = 5
            };
        }

        public void SalvarRangeAdjust(RangeAdjust rangeAdjust)
        {
            var obj = _realm.All<RangeAdjust>().FirstOrDefault();
            if (obj == null)
            {
                _realm.Write(() => _realm.Add(rangeAdjust));
            }
            else
            {
                _realm.Write(() => _realm.Add(rangeAdjust, true));
            }
        }

        //public RaceLap SalvarReceLap(RaceShareLap raceLap, List<RaceShareLapPartial> parciais)
        //{
        //    var lastId = LapsCount() == 0
        //        ? 0
        //        : _realm.All<RaceLap>().ToList().Max(x => x.Id);
        //    var lapId = lastId += 1;

        //    ///Media de velocidade
        //    var dadosVolta = laps.Where(m => m.LapNumber == lap.LapNumber).ToList();
        //    var velocidadeMedia = 0;
        //    var tempoTotal = 0;
        //    foreach (var parcial in parciais)
        //    {
        //        velocidadeMedia += dados.Velocidade;
        //        tempoTotal += Tools.StringTimeToMileSeconds(dados.TempoParcial);
        //    }
        //    velocidadeMedia /= dadosVolta.Count;
        //    TimeSpan ts = TimeSpan.FromMilliseconds(tempoTotal);
        //    var volta = new RaceLap
        //    {
        //        Id = lapId,
        //        Race = raceId,
        //        LapNumber = lap.LapNumber,
        //        VelocidadeMedia = velocidadeMedia,
        //        TempoTotal = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}",
        //    };

        //    ///Salva a volta
        //    _realm.Write(() => _realm.Add(volta));
        //}
    }
}
