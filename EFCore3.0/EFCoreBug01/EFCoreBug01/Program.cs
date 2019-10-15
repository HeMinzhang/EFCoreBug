using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFCoreBug01
{
    class Program
    {
        static async Task InsertBug01(ApplicationDbContext dbContext)
        {
            //https://github.com/aspnet/EntityFrameworkCore/issues/18263#issuecomment-541369431
            var key = new CdKey
            {
                //Id = Guid.NewGuid().ToString(), //this bug fix solution
                Name = "asp.net",
                BuildType = BuildType.Test
            };
            await dbContext.AddAsync(key);
        }
        static async Task Change01(ApplicationDbContext dbContext)
        {
            //https://github.com/aspnet/EntityFrameworkCore/issues/18358
            if (await dbContext.CdKeys.CountAsync() == 0)
            {
                if (dbContext.CdKeys != null)
                {
                    Console.WriteLine("So, Cdkeys count is 0, but CdKeys not equal null in this query");
                }
            }
            else
            {
                Console.WriteLine("Must Cdkeys count == 0");
            }
        }
        static async Task Change02(ApplicationDbContext dbContext)
        {
            //https://github.com/aspnet/EntityFrameworkCore/issues/18358
            var firstkey = new CdKey
            {
                Id = Guid.NewGuid().ToString(), //this bug fix solution
                Name = "asp.net",
                BuildType = BuildType.Test
            };
            await dbContext.CdKeys.AddAsync(firstkey);
            await dbContext.SaveChangesAsync();
            var keys = dbContext.CdKeys?.Where(k => k.BuildType == BuildType.Test);
            foreach (var item in keys)
            {
                var values = dbContext.keyValues.Where(v => v.KeyId == item.Id).ToList();//InvalidOperationException: 
                //There is already an open DataReader associated with this Command which must be closed first.
            }
        }
        static async Task FixChange02(ApplicationDbContext dbContext)
        {
            /*
             I don't think this lazy loading is bad, instead I think it's very cool.
             I hope the official can have some documentation about this, or help other through my example.
             */
            var firstkey = new CdKey
            {
                Id = Guid.NewGuid().ToString(), //this bug fix solution
                Name = "asp.net",
                BuildType = BuildType.Test
            };
            await dbContext.CdKeys.AddAsync(firstkey);
            await dbContext.SaveChangesAsync();
            var keys = dbContext.CdKeys?.Where(k => k.BuildType == BuildType.Test)?.ToList();//Because the loading is done
            foreach (var item in keys)
            {
                var values = dbContext.keyValues.Where(v => v.KeyId == item.Id).ToList();//So,No error
            }
        }
        
        static IEnumerable<CdKey> GetTestKey(ApplicationDbContext dbContext)
        {
            return dbContext.CdKeys.Where(k => k.BuildType == BuildType.Test);
        }
        static void Change02_2(ApplicationDbContext dbContext)
        {
            var keys = GetTestKey(dbContext);
            foreach(var item in keys)
            {
                dbContext.keyValues.Where(v => v.Key == "c sharp...").ToList();//should same error...
            }
        }
        static IEnumerable<KeyValue> GetTestKeySecond(ApplicationDbContext dbContext)
        {
            var result = new List<KeyValue>();
            var keys = dbContext.CdKeys.Where(k => k.BuildType == BuildType.Test);
            foreach (var item in keys)
            {
                var values = dbContext.keyValues.Where(v => v.Key == "c sharp...");//should same error...
                result.AddRange(values);
            }
            return result;
        }
        static void Change02_3(ApplicationDbContext dbContext)
        {
            var values = GetTestKeySecond(dbContext);
        }
        static async Task Change03(ApplicationDbContext dbContext)
        {
            if(await dbContext.keyValues.CountAsync() == 0)
            {
                var values = dbContext.keyValues.ToList();
                if (values != null)
                    Console.WriteLine("So, to List value != null");
                if(values.Count() == 0)
                    Console.WriteLine("So, to List value == 0");

                values = dbContext.keyValues.Where(v => v.Key == "asp.net").ToList();
                if (values != null)
                    Console.WriteLine("So, where value != null");
                if (values.Count() == 0)
                    Console.WriteLine("So, where value == 0");

                foreach(var item in values)
                {
                    Console.WriteLine("error, call");
                }
                Console.WriteLine("So, foreach no exception");
            }
        }
        static void Main(string[] args)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Database.EnsureCreated();
                //InsertBug01(context).Wait();
                //Change01(context).Wait();
                //Change02(context).Wait();
                //FixChange02(context).Wait();
                //Change02_2(context);
                Change02_3(context);
                //Change03(context).Wait();
            }
        }
    }
}
