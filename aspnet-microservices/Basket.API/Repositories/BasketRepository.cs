using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using StackExchange.Redis;
using System.Linq;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {

        private readonly IDistributedCache _redisCache;

        private IConfiguration _configuration;
        public BasketRepository(IDistributedCache redisCache, IConfiguration configuration)
        {
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
            _configuration = configuration;
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basket = await _redisCache.GetStringAsync(userName);

            if (String.IsNullOrEmpty(basket))
                return null;

            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<List<ShoppingCart>> GetAllValuesInRedisCacheBasket()
        {

            var keys = GetAllKeysInRedisCache();

            var shopingCartList = new List<ShoppingCart>();

            foreach (var key in keys)
            {
                String basket = await _redisCache.GetStringAsync(key);

                var shopingCart = JsonConvert.DeserializeObject<ShoppingCart>(basket);

                shopingCartList.Add(shopingCart);
            }

            return shopingCartList;
        }
        public List<string> GetAllKeysInRedisCache()
        {
            var connectionString = _configuration.GetValue<string>("CacheSettings:ConnectionString");

            List<string> listKeys = new List<string>();
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString))
            {
                var keys = redis.GetServer(connectionString).Keys();
                var keyArr = keys.Select(key => (string)key).ToArray();
                foreach (var key in keyArr)
                {
                    if(key!="name" && key != "key")
                    {
                        listKeys.Add(key);
                    }     
                }
            }
            return listKeys;
        }
        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            return await GetBasket(basket.UserName);
        }

        public async Task DeleteBasket(string userName)
        {
            await _redisCache.RemoveAsync(userName);
        }
    }
}
