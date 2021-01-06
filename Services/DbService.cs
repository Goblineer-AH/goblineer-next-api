using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoblineerNextApi.Models;
using Npgsql;

namespace GoblineerNextApi.Services
{
    public class ItemNotFoundException : Exception {}

    public class DbService
    {
        private readonly string connString;
        public DbService(string connString) =>
            this.connString = connString;

        private async Task<NpgsqlConnection> OpenNewConnection()
        {
            var connection = new NpgsqlConnection(connString);
            await connection.OpenAsync();
            return connection;
        }

        private Item ReadItem(NpgsqlDataReader reader, int id)
        {
            var context = reader.GetInt32(0);
            var modifiers = reader.GetFieldValue<int[]>(1);
            var bonuses = reader.GetFieldValue<int[]>(2);
            var petBreedId = reader.GetInt32(3);
            var petLevel = reader.GetInt32(4);
            var petQualityId = reader.GetInt32(5);
            var petSpeciesId = reader.GetInt32(6);
            var internalId = reader.GetInt32(7);
            var itemId = reader.GetInt32(8);

            Func<int,int?> checkNullInt = (int n) => n == -1 ? (int?)null : n;
            Func<int[],int[]?> checkNullArray = (int[] arr) => arr.Length == 0 ? (int[]?)null : arr;

            return new Item
            {
                Id = itemId,
                InternalId = internalId,
                Context = checkNullInt(context),
                PetBreedId = checkNullInt(petBreedId),
                PetLevel = checkNullInt(petLevel),
                PetQualityId = checkNullInt(petQualityId),
                PetSpeciesId = checkNullInt(petSpeciesId),
                Modifiers = checkNullArray(modifiers),
                Bonuses = checkNullArray(bonuses),
            };
        }

        public async Task<List<Item>> GetItemsById(int id)
        {
            var query = @"
                SELECT context, modifiers, bonuses, petbreedid, petlevel, petqualityid, petspeciesid, id, originalitemid FROM items
                WHERE originalitemid = @id;
            ";

            await using var connection = await OpenNewConnection();
            await using var cmd = new NpgsqlCommand(query, connection);

            cmd.Parameters.AddWithValue("id", id);

            await using var reader = await cmd.ExecuteReaderAsync();

            var items = new List<Item>();
            while(await reader.ReadAsync())
            {
                var item = ReadItem(reader, id);
                items.Add(item);
            }

            return items;
        }

        public async Task<Item> GetItemByInternalId(int id)
        {
            var query = @"
                SELECT context, modifiers, bonuses, petbreedid, petlevel, petqualityid, petspeciesid, id, originalitemid FROM items
                WHERE id = @id;
            ";

            await using var connection = await OpenNewConnection();
            await using var cmd = new NpgsqlCommand(query, connection);

            cmd.Parameters.AddWithValue("id", id);

            await using var reader = await cmd.ExecuteReaderAsync();

            var items = new List<Item>();
            if(await reader.ReadAsync())
            {
                var item = ReadItem(reader, id);
                return item;
            }

            throw new ItemNotFoundException();
        }

        public async Task<(int, double)> GetMarketvalueByInternalItemId(int serverId, int itemId)
        {
            var query = @"
                SELECT quantity, marketvalue FROM marketvalues
                WHERE serverid = @serverId
                  AND itemid = @itemId;
            ";

            await using var connection = await OpenNewConnection();
            await using var cmd = new NpgsqlCommand(query, connection);

            cmd.Parameters.AddWithValue("serverId", serverId);
            cmd.Parameters.AddWithValue("itemId", itemId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if(await reader.ReadAsync())
            {
                var quantity = reader.GetInt32(0);
                var marketvalue = reader.GetDouble(1);

                return (quantity, marketvalue);
            }

            throw new ItemNotFoundException();
        }

        public async Task<(int, double)> GetMarketvalueByItemId(int serverId, int itemId)
        {
            var query = @"
                SELECT quantity, marketvalue FROM marketvalues
                WHERE serverid = @serverId
                  AND itemid in (
                      SELECT id FROM items
                      WHERE originalitemid = @itemId
                  )
                ORDER BY quantity DESC
                LIMIT 1;
            ";

            await using var connection = await OpenNewConnection();
            await using var cmd = new NpgsqlCommand(query, connection);

            cmd.Parameters.AddWithValue("serverId", serverId);
            cmd.Parameters.AddWithValue("itemId", itemId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if(await reader.ReadAsync())
            {
                var quantity = reader.GetInt32(0);
                var marketvalue = reader.GetDouble(1);

                return (quantity, marketvalue);
            }

            throw new ItemNotFoundException();
        }

        public async Task<List<Auction>> GetAuctionsByItemId(int serverId, int itemId)
        {
            var query = @"
                SELECT bid, price, quantity, timeleft FROM auctions
                WHERE serverid = @serverId
                  AND itemid = @itemId
                ORDER BY price ASC;
            ";

            await using var connection = await OpenNewConnection();
            await using var cmd = new NpgsqlCommand(query, connection);

            cmd.Parameters.AddWithValue("serverId", serverId);
            cmd.Parameters.AddWithValue("itemId", itemId);

            await using var reader = await cmd.ExecuteReaderAsync();

            var auctions = new List<Auction>();
            while(await reader.ReadAsync())
            {
                var bid = reader.GetInt32(0);
                var price = reader.GetInt64(1);
                var quantity = reader.GetInt32(2);
                var timeLeft = reader.GetString(3);

                var auction = new Auction
                {
                    Bid = bid,
                    Price = price,
                    Quantity = quantity,
                    TimeLeft = timeLeft,
                };

                auctions.Add(auction);
            }

            return auctions;
        }

        public async Task<List<Server>> GetServers()
        {
            var query = @"
                SELECT connectedrealmid, region, realmname FROM servers;
            ";

            await using var connection = await OpenNewConnection();
            await using var cmd = new NpgsqlCommand(query, connection);

            await using var reader = await cmd.ExecuteReaderAsync();

            var servers = new List<Server>();
            while(await reader.ReadAsync())
            {
                var server = new Server
                {
                    ConnectedRealmId = reader.GetInt32(0),
                    Region = reader.GetString(1),
                    RealmName = reader.GetString(2),
                };

                servers.Add(server);
            }

            return servers;
        }
    }
}