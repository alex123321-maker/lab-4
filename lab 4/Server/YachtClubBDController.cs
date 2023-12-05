using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace Server
{
    internal class YachtClubBDController
    {

        public string RemoveRecord(int id)
        {
            using (var dbContext = new YachtClubContext())
            {
                var yachtClub = dbContext.YachtClubs.Find(id);
                if (yachtClub != null)
                {
                    dbContext.YachtClubs.Remove(yachtClub);
                    dbContext.SaveChanges();
                    return JsonConvert.SerializeObject(new { Message = "Запись успешко удалена" });
                }
                else
                {
                    return JsonConvert.SerializeObject(new { Message = "Запись не найдена" });
                }
            }
        }

        public string GetYachtClubs()
        {
            using (var dbContext = new YachtClubContext())
            {
                List<YachtClub> yachtClubs = dbContext.YachtClubs.ToList();
                return JsonConvert.SerializeObject(yachtClubs);
            }
        }

        public string GetYachtClub(int id)
        {
            using (var dbContext = new YachtClubContext())
            {
                YachtClub yachtClub = dbContext.YachtClubs.Find(id);

                if (yachtClub != null)
                {
                    return JsonConvert.SerializeObject(yachtClub);
                }
                else
                {
                    return JsonConvert.SerializeObject(new { Message = "Запись не найдена" });
                }
            }
        }

        public string AddRecord(YachtClub yachtClub)
        {
            using (var dbContext = new YachtClubContext())
            {
                dbContext.YachtClubs.Add(yachtClub);
                dbContext.SaveChanges();
            }
            return JsonConvert.SerializeObject(yachtClub);
        }
        public void UpdateRecord(int id, YachtClub updatedYachtClub)
        {
            using (var dbContext = new YachtClubContext())
            {

                var yachtClub = dbContext.YachtClubs.Find(id);
                if (yachtClub != null)
                {
                    yachtClub.Name = updatedYachtClub.Name;
                    yachtClub.Address = updatedYachtClub.Address;
                    yachtClub.NumberOfYachts = updatedYachtClub.NumberOfYachts;
                    yachtClub.NumberOfPlaces = updatedYachtClub.NumberOfPlaces;
                    yachtClub.HasPool = updatedYachtClub.HasPool;
                    dbContext.SaveChanges();
                }
                else
                {
                    throw new Exception("Record not found"); // Обработка случая, когда запись не найдена
                }
            }
        }
        public string DeleteAndLoadData(List<YachtClub> yachtClubs)
        {
            using (var context = new YachtClubContext())
            {
                var allEntities = context.YachtClubs.ToList();
                context.YachtClubs.RemoveRange(allEntities);
                context.YachtClubs.AddRange(yachtClubs);
                context.SaveChanges();
                return JsonConvert.SerializeObject(yachtClubs);


                
            }
        }
    }
}
