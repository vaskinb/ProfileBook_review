using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ProfileBook.Models;
using SQLite;

namespace ProfileBook.Services.Repository
{
    public class Repository : IRepository
    {
        private Lazy<SQLiteAsyncConnection> _database;

        public Repository()
        {
            _database = new Lazy<SQLiteAsyncConnection>(() =>
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constant.DB_Name);
                var database = new SQLiteAsyncConnection(path);

                database.CreateTableAsync<UserModel>();
                database.CreateTableAsync<ProfileModel>();

                return database;
            });
        }

        #region --- Implementation ---

        public Task<int> DeleteAsync<T>(T entity)
        where T : class, IEntityBase, new()
        {
            return _database.Value.DeleteAsync(entity);
        }

        public async Task DeleteWhereAsync<T>(Expression<Func<T, bool>> predicate)
        where T : class, IEntityBase, new()
        {
            var result = await _database.Value.Table<T>().Where(predicate).ToListAsync();

            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    await _database.Value.DeleteAsync(item);
                }
            }
        }

        public async Task DeleteAllAsync<T>()
        where T : class, IEntityBase, new()
        {
            await _database.Value.DropTableAsync<T>();
            await _database.Value.CreateTableAsync<T>();
        }

        public Task<List<T>> FindByAsync<T>(Expression<Func<T, bool>> predicate)
        where T : class, IEntityBase, new()
        {
            return _database.Value.Table<T>().Where(predicate).ToListAsync();
        }

        public Task<int> CountAsync<T>()
        where T : class, IEntityBase, new()
        {
            return _database.Value.Table<T>().CountAsync();
        }

        public Task<int> CountByAsync<T>(Expression<Func<T, bool>> predicate)
        where T : class, IEntityBase, new()
        {
            return _database.Value.Table<T>().Where(predicate).CountAsync();
        }

        public Task<List<T>> GetAllAsync<T>()
        where T : class, IEntityBase, new()
        {
            return _database.Value.Table<T>().ToListAsync();
        }

        public Task<T> GetSingleAsync<T>(Expression<Func<T, bool>> predicate)
        where T : class, IEntityBase, new()
        {
            return _database.Value.Table<T>().Where(predicate).FirstOrDefaultAsync();
        }

        public Task<T> GetSingleByIdAsync<T>(long id)
        where T : class, IEntityBase, new()
        {
            return _database.Value.Table<T>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveOrUpdateAsync<T>(T entity)
        where T : class, IEntityBase, new()
        {
            var row = 0;
            var exist = await GetSingleByIdAsync<T>(entity.Id);

            if (exist == null)
            {
                row = await _database.Value.InsertOrReplaceAsync(entity);
            }
            else
            {
                row = await _database.Value.UpdateAsync(entity);
            }

            return row;
        }

        public async Task SaveOrUpdateRangeAsync<T>(IEnumerable<T> entities)
        where T : class, IEntityBase, new()
        {
            if (entities.Any())
            {
                var updateInstances = new List<T>();
                var insertInstances = new List<T>();

                foreach (var entity in entities)
                {
                    var existingEntity = await GetSingleByIdAsync<T>(entity.Id);

                    if (existingEntity == null)
                    {
                        insertInstances.Add(entity);
                    }
                    else
                    {
                        updateInstances.Add(entity);
                    }
                }

                if (updateInstances.Any())
                {
                    var updateQuery = GetUpdateQueryString(updateInstances);

                    await ExecuteQuery(updateQuery);
                }

                if (insertInstances.Any())
                {
                    var insertQuery = GetInsertQueryString(insertInstances);

                    await ExecuteQuery(insertQuery);
                }
            }
        }

        public Task<int> SaveAsync<T>(T entity)
        where T : class, IEntityBase, new()
        {
            return _database.Value.InsertAsync(entity);
        }

        #endregion

        private async Task ExecuteQuery(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                await _database.Value.ExecuteAsync(query);
            }
        }

        private string GetUpdateQueryString<T>(IEnumerable<T> entities)
            where T : class, IEntityBase, new()
        {
            var result = string.Empty;

            if (entities.Any())
            {
                var updateQueryStringBuilder = new StringBuilder();

                var entityType = typeof(T);
                var entityProperties = entityType.GetProperties().Where(p => !p.GetCustomAttributes(typeof(IgnoreAttribute), false).Any());

                foreach (var entity in entities)
                {
                    updateQueryStringBuilder.Append($"UPDATE {entityType.Name} SET ");

                    foreach (var property in entityProperties)
                    {
                        var value = property.GetValue(entity);
                        var stringValue = PrepareValueToStore(value, property.PropertyType);

                        if (stringValue == null)
                        {
                            updateQueryStringBuilder.Append($"{property.Name}=null, ");
                        }
                        else
                        {
                            updateQueryStringBuilder.Append($"{property.Name}='{stringValue}', ");
                        }
                    }

                    updateQueryStringBuilder.Length -= 2;
                    updateQueryStringBuilder.Append($" WHERE {nameof(IEntityBase.Id)}='{entity.Id}';");
                }

                result = updateQueryStringBuilder.ToString();
            }

            return result;
        }

        private string GetInsertQueryString<T>(IEnumerable<T> entities)
            where T : class, IEntityBase, new()
        {
            var result = string.Empty;

            if (entities.Any())
            {
                var insertQueryStringBuilder = new StringBuilder();
                var entityType = typeof(T);
                var entityProperties = entityType.GetProperties().Where(p => !p.GetCustomAttributes(typeof(IgnoreAttribute), false).Any()).ToArray();

                insertQueryStringBuilder.Append($"INSERT INTO {entityType.Name} (");

                var isFirstElement = true;
                foreach (var property in entityProperties)
                {
                    if (!isFirstElement)
                    {
                        insertQueryStringBuilder.Append(", ");
                    }
                    else
                    {
                        isFirstElement = false;
                    }

                    insertQueryStringBuilder.Append(property.Name);
                }

                insertQueryStringBuilder.Append(") VALUES ");

                foreach (var entity in entities)
                {
                    insertQueryStringBuilder.Append("(");

                    foreach (var property in entityProperties)
                    {
                        var value = property.GetValue(entity);
                        var stringValue = PrepareValueToStore(value, property.PropertyType);

                        if (stringValue == null)
                        {
                            insertQueryStringBuilder.Append($"null, ");
                        }
                        else
                        {
                            insertQueryStringBuilder.Append($"'{stringValue}', ");
                        }
                    }

                    insertQueryStringBuilder.Length -= 2;
                    insertQueryStringBuilder.Append("),");
                }

                insertQueryStringBuilder.Length -= 1;
                insertQueryStringBuilder.Append(";");

                result = insertQueryStringBuilder.ToString();
            }

            return result;
        }

        private string PrepareValueToStore(object value, Type valueType)
        {
            var result = string.Empty;

            if (valueType.Equals(typeof(DateTime)))
            {
                result = ((DateTime)value).Ticks.ToString(CultureInfo.InvariantCulture);
            }
            else if (valueType.Equals(typeof(DateTime?)))
            {
                result = ((DateTime?)value)?.Ticks.ToString(CultureInfo.InvariantCulture);
            }
            else if (valueType.Equals(typeof(int?)))
            {
                result = ((int?)value)?.ToString();
            }
            else
            {
                var stringValue = Convert.ToString(value, CultureInfo.InvariantCulture);

                result = PrepareString(stringValue);
            }

            return result;
        }

        private string PrepareString(string @string)
        {
            if (@string != null)
            {
                if (@string.Contains("\'"))
                {
                    return @string.Replace("\'", "''");
                }
            }

            return @string;
        }
    }
}
