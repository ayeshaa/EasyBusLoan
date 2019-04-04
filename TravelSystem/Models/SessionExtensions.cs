using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelSystem.Models
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, List<VehiclesCarts> value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static List<VehiclesCarts> GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            var list = new List<VehiclesCarts>();
            return value == null ? list : JsonConvert.DeserializeObject<List<VehiclesCarts>>(value);
        }
        public static List<OrderAddresses> GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            var list = new List<OrderAddresses>();
            return value == null ? list : JsonConvert.DeserializeObject<List<OrderAddresses>>(value);
        }
    }
}
