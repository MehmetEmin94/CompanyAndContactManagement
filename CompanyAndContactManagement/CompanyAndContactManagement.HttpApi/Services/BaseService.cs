using System.Text.RegularExpressions;
using CompanyAndContactManagement.HttpApi.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CompanyAndContactManagement.HttpApi.Services;

public class BaseService<TEntity>
{
    private readonly IMongoCollection<TEntity> _mongoCollection;
    private readonly IMongoDatabase _mongo;
    private dynamic _value;

    public BaseService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        _mongo = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _mongoCollection = _mongo.GetCollection<TEntity>($"{typeof(TEntity).Name}");
    }

    public virtual async Task<IAggregateFluent<TEntity>> GetAggregateAsync( AggregateOptions aggregateOptions = null)
    {

        var dbContext = _mongo;
        var collection = _mongoCollection;

        var aggregate = collection.Aggregate(aggregateOptions);

        return aggregate;
    }
    
    
    public IAggregateFluent<TEntity> ApplyFilter(
        IAggregateFluent<TEntity> query, IFilters filters, string globalFilter)
    {
        var globalFilters=new List<FilterDefinition<TEntity>>();
        foreach (var (filterKey, filterValueList) in filters.ToKeyValuePairs())
        {
            if (globalFilter is not null)
            {
                globalFilters.Add(Builders<TEntity>.Filter.Regex(filterKey,
                    BsonRegularExpression.Create(
                        new Regex(".*" + Regex.Escape(globalFilter)
                            .Replace("i", "☺")
                            .Replace("ı", "☺")
                            .Replace("I", "☺")
                            .Replace("İ", "☺")
                            .Replace("☺", "[ıiIİ]") + ".*", RegexOptions.IgnoreCase))));
            }
            foreach (var filterItem in filterValueList)
            {
                var currentFilters = new List<FilterDefinition<TEntity>>();
                foreach (var value in filterItem.Value.Where(value => value != null))
                {
                    _value = value;
                    if (_value is FilterMagicValue filterMagicValue)
                    {
                        _value = filterMagicValue switch
                        {
                            FilterMagicValue.Null => BsonNull.Value,
                            _ => filterMagicValue
                        };
                    }
                    
                    
                    
                    switch (filterItem.MatchMode)
                    {
                        case FilterMatchMode.Equals:
                        case FilterMatchMode.Is:
                        case FilterMatchMode.DateIs:
                            
                                currentFilters.Add(Builders<TEntity>.Filter.Eq(filterKey, _value));
                            break; 
                        case FilterMatchMode.NotEquals:
                        case FilterMatchMode.IsNot:
                        case FilterMatchMode.DateIsNot:
                                currentFilters.Add(Builders<TEntity>.Filter.Ne(filterKey, _value));
                            break;
                        case FilterMatchMode.StartsWith:
                            currentFilters.Add(Builders<TEntity>.Filter.Regex(filterKey,
                                new BsonRegularExpression(new Regex("^" + Regex.Escape((string)_value ?? string.Empty) + ".*",
                                    RegexOptions.IgnoreCase))));
                            break;
                        case FilterMatchMode.EndsWith:
                            currentFilters.Add(Builders<TEntity>.Filter.Regex(filterKey,
                                new BsonRegularExpression(new Regex(".*" + Regex.Escape((string)_value ?? string.Empty) + "$",
                                    RegexOptions.IgnoreCase))));
                            break;
                        case FilterMatchMode.Contains:
                            currentFilters.Add(Builders<TEntity>.Filter.Regex(filterKey, new BsonRegularExpression(_value, "i")));
                            break;
                        case FilterMatchMode.NotContains:
                            currentFilters.Add(Builders<TEntity>.Filter.Not(Builders<TEntity>.Filter.Regex(filterKey, new BsonRegularExpression(_value, "i"))));
                            break;
                        case FilterMatchMode.LessThan:
                        case FilterMatchMode.Before:
                        case FilterMatchMode.DateBefore:
                                currentFilters.Add(Builders<TEntity>.Filter.Lt(filterKey, _value));
                                break;
                        case FilterMatchMode.LessThanOrEqualTo:
                            currentFilters.Add(Builders<TEntity>.Filter.Lte(filterKey, _value));
                            break;
                        case FilterMatchMode.GreaterThan:
                        case FilterMatchMode.After:
                        case FilterMatchMode.DateAfter:
                                currentFilters.Add(Builders<TEntity>.Filter.Gt(filterKey, _value));
                                break;
                        case FilterMatchMode.GreaterThanOrEqualTo:
                            currentFilters.Add(Builders<TEntity>.Filter.Gte(filterKey, _value));
                            break;
                    }
                }

                query = currentFilters.Count switch
                {
                    > 0 => filterItem.Operator switch
                    {
                        FilterOperator.Or => query.Match(Builders<TEntity>.Filter.Or(currentFilters)),
                        FilterOperator.And => query.Match(Builders<TEntity>.Filter.And(currentFilters)),
                        _ => query
                    },
                    _ => query
                };
            }
        }
        return globalFilters.IsNullOrEmpty()?query:query.Match(Builders<TEntity>.Filter.Or(globalFilters));
    }
}