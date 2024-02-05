using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lesson7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //1
            using (var db = new ApplicationContext())
            {
                db.Database.ExecuteSqlRaw("INSERT INTO [Stations] ([Name], [Location]) VALUES ('Station D', 'City D')");
                db.Database.ExecuteSqlRaw("INSERT INTO [Trains] ([TrainNumber], [DepartureTime], [StationId]) VALUES ('Train 7', GETDATE(), 4)");
            }

            //2
            using (var db = new ApplicationContext())
            {
                var trains = db.Trains.FromSqlRaw("SELECT * FROM [Trains] WHERE DATEDIFF(HOUR, [DepartureTime], GETDATE()) > 5").ToList();
            }

            //3
            using (var db = new ApplicationContext())
            {
                var stations = db.Stations.FromSqlRaw("SELECT DISTINCT s.* FROM [Stations] s JOIN [Trains] t ON s.[StationId] = t.[StationId]").ToList();
            }

            //4
            using (var db = new ApplicationContext())
            {
                var stations = db.Stations
                    .FromSqlRaw("SELECT s.StationId, s.Name, s.Location FROM Stations s JOIN Trains t ON s.StationId = t.StationId GROUP BY s.StationId, s.Name, s.Location HAVING COUNT(t.TrainId) > 3")
                    .ToList();
            }

            //5
            using (var db = new ApplicationContext())
            {
                var stations = db.Stations.FromSqlRaw("SELECT * FROM [Stations] WHERE [Name] LIKE 'Sta%'").ToList();
            }

            //6
            using (var db = new ApplicationContext())
            {
                var trains = db.Trains
                    .FromSqlRaw("SELECT * FROM [Trains] WHERE DATEDIFF(YEAR, [DepartureTime], GETDATE()) > 15")
                    .ToList();
            }

            //7
            using (var db = new ApplicationContext())
            {
                var stations = db.Stations
                    .FromSqlRaw("SELECT * FROM [Stations] s WHERE EXISTS (SELECT 1 FROM [Trains] t WHERE s.[StationId] = t.[StationId] AND DATEDIFF(HOUR, t.[DepartureTime], GETDATE()) < 5)")
                    .ToList();
            }

            //8
            using (var db = new ApplicationContext())
            {
                var stations = db.Stations
                    .FromSqlRaw("SELECT * FROM [Stations] s WHERE NOT EXISTS (SELECT 1 FROM [Trains] t WHERE s.[StationId] = t.[StationId])")
                    .ToList();
            }



            //using (var db = new ApplicationContext())
            //{
            //    // Create stations
            //    var station1 = new Station { Name = "Station A", Location = "City A" };
            //    var station2 = new Station { Name = "Station B", Location = "City B" };
            //    var station3 = new Station { Name = "Station C", Location = "City C" };

            //    // Add stations to the context
            //    db.Stations.AddRange(station1, station2, station3);
            //    db.SaveChanges();

            //    // Create trains and associate them with stations
            //    var train1 = new Train { TrainNumber = "Train 1", DepartureTime = DateTime.Now, StationId = station1.StationId };
            //    var train2 = new Train { TrainNumber = "Train 2", DepartureTime = DateTime.Now, StationId = station1.StationId };
            //    var train3 = new Train { TrainNumber = "Train 3", DepartureTime = DateTime.Now, StationId = station2.StationId };
            //    var train4 = new Train { TrainNumber = "Train 4", DepartureTime = DateTime.Now, StationId = station2.StationId };
            //    var train5 = new Train { TrainNumber = "Train 5", DepartureTime = DateTime.Now, StationId = station3.StationId };
            //    var train6 = new Train { TrainNumber = "Train 6", DepartureTime = DateTime.Now, StationId = station3.StationId };

            //    // Add trains to the context
            //    db.Trains.AddRange(train1, train2, train3, train4, train5, train6);
            //    db.SaveChanges();
            //}
        }
    }
    public class Station
    {
        public int StationId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public ICollection<Train> Trains { get; set; }
    }
    public class Train
    {
        public int TrainId { get; set; }
        public string TrainNumber { get; set; }
        public DateTime DepartureTime { get; set; }
        public int StationId { get; set; }
        public Station Station { get; set; }
    }
    public class ApplicationContext : DbContext
    {
        public DbSet<Station> Stations { get; set; }
        public DbSet<Train> Trains { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Train>()
                .HasOne(t => t.Station)
                .WithMany(s => s.Trains)
                .HasForeignKey(t => t.StationId);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=localhost;Database=TrainStationDb;Trusted_Connection=True;TrustServerCertificate=True;")
                .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
        }

    }
}
