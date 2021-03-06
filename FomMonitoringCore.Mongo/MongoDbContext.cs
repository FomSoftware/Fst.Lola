﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Reflection;
using FomMonitoringCore.Mongo.Dto;
using MongoDB.Driver;

namespace FomMonitoringCore.Mongo
{
    public class MongoDbContext : IMongoDbContext
    {
        public IMongoDatabase Database;
        public MongoDbContext()
        {
            var mongoDatabaseName = ConfigurationManager.AppSettings["MongoDatabaseName"];
            var mongoUsername = ConfigurationManager.AppSettings["MongoUsername"]; 
            var mongoPassword = ConfigurationManager.AppSettings["MongoPassword"]; 
            var mongoPort = ConfigurationManager.AppSettings["MongoPort"];   
            var mongoHost = ConfigurationManager.AppSettings["MongoHost"];    
            // Creating credentials  
            var credential = MongoCredential.CreateCredential
            (mongoDatabaseName,
                mongoUsername,
                mongoPassword);

            // Creating MongoClientSettings  
            var settings = new MongoClientSettings
            {
                Credential = credential,
                Server = new MongoServerAddress(mongoHost, Convert.ToInt32((string) mongoPort))
            };
            var client = new MongoClient(settings);
            Database = client.GetDatabase(mongoDatabaseName);
        }

        public IMongoCollection<T> DbSet<T>() where T : BaseModel
        {
            var table = typeof(T).GetCustomAttribute<TableAttribute>(false).Name;
            return Database.GetCollection<T>(table);
        }

    }
}
