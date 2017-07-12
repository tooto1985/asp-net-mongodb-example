using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{

    public class MongoSchema
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
    }

    public class HomeController : Controller
    {
        const string MongoDbUri = "mongodb://127.0.0.1:27017";
        const string DatabaseName = "mytest";
        const string CollectionName = "userlist";

        [HttpGet]
        public ActionResult Index()
        {
            return View("Insert");
        }

        [HttpGet]
        public ActionResult Insert()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insert(string name, int age, string gender)
        {
            var client = new MongoClient(MongoDbUri);
            var database = client.GetDatabase(DatabaseName);
            var collection = database.GetCollection<MongoSchema>(CollectionName);
            var data = new MongoSchema { Name = name, Age = age, Gender = gender };
            collection.InsertOneAsync(data).Wait();
            ViewBag.id = data._id;
            return View("ok");
        }

        [HttpGet]
        public ActionResult Find()
        {
            var client = new MongoClient(MongoDbUri);
            var database = client.GetDatabase(DatabaseName);
            var collection = database.GetCollection<MongoSchema>(CollectionName);
            var datas = collection.FindAsync(new BsonDocument()).Result;
            ViewBag.data = datas.ToList<MongoSchema>();
            return View();
        }

        [HttpGet]
        public ActionResult Update()
        {
            var client = new MongoClient(MongoDbUri);
            var database = client.GetDatabase(DatabaseName);
            var collection = database.GetCollection<MongoSchema>(CollectionName);
            var datas = collection.FindAsync(new BsonDocument()).Result;
            ViewBag.data = datas.ToList<MongoSchema>();
            return View();
        }

        [HttpGet]
        public ActionResult Edit(string uid)
        {
            if (uid == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var client = new MongoClient(MongoDbUri);
            var database = client.GetDatabase(DatabaseName);
            var collection = database.GetCollection<MongoSchema>(CollectionName);
            var filter = Builders<MongoSchema>.Filter.Eq("_id", new ObjectId(uid));
            var data = collection.FindAsync(filter).Result;
            ViewBag.data = data.ToList<MongoSchema>().Single();
            return View();
        }

        [HttpPost]
        public ActionResult Edit(string uid, string name, int age, string gender)
        {
            var client = new MongoClient(MongoDbUri);
            var database = client.GetDatabase(DatabaseName);
            var collection = database.GetCollection<MongoSchema>(CollectionName);
            var filter = Builders<MongoSchema>.Filter.Eq("_id", new ObjectId(uid));
            var update = Builders<MongoSchema>.Update.Set("Name", name).Set("Age", age).Set("Gender", gender);
            collection.UpdateOneAsync(filter, update).Wait();
            return RedirectToAction("Update");
        }
        [HttpGet]
        public ActionResult Remove(string uid)
        {
            var client = new MongoClient(MongoDbUri);
            var database = client.GetDatabase(DatabaseName);
            var collection = database.GetCollection<MongoSchema>(CollectionName);
            if (uid == null)
            {
                var datas = collection.FindAsync(new BsonDocument()).Result;
                ViewBag.data = datas.ToList<MongoSchema>();
                return View();
            }
            else
            {
                var delete = Builders<MongoSchema>.Filter.Eq("_id", new ObjectId(uid));
                collection.DeleteOneAsync(delete).Wait();
                return RedirectToAction("Remove");
            }
        }
        public ActionResult ok()
        {
            return View();
        }
    }
}