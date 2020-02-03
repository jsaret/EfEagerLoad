# EfEagerLoad

EfEagerLoad allows you to add eager loading to your Entities automatically without minimal setup.

It includes mechanims to selectively control how your entities are will be eager loaded both by built-in Attributes as well as programatically.

Internal caching of the resulting Include Functions to execute for the Entities being used is built-in but only rudimentary at the moment (limitations).

## Contents
- [License](#license)
- [About EfEagerLoad](#About-EfEagerLoad)
- [Getting Started](#Getting-Started)
  - [Setup](#Setup)
  - [Usage](#Usage)
    - [Controlling what gets Eager Loaded](#Controlling-what-gets-Eager-Loaded)
      - [Built-in Attribute](#Built-in-Attribute)
      - [Custom Attribute](#Custom-Attribute)
      - [Custom Filter](#Custom-Filter)
    - [Advanced EagerLoadAttribute](#Advanced-EagerLoadAttribute)
    - [Repository Integration](#Repository-Integration)
- [Contact](#Contact) 

## License
See license details [here](/LICENSE.md).

## About EfEagerLoad

More information about EfEagerLoad can be found at <a href="https://github.com/jsaret/EfEagerLoad/" target="_blank">EfEagerLoad</a>

## Getting Started
### Setup

First you'll need to install the library.

This can be done ether through the nu-get package manager or with the with the following commands:

    nu-get install.....


___
  
### Usage

Once set up, the eager loading functionality can be plugged into your system in various ways.

Worth noting is that to benefit from the Eager Loading one would need to use a query that utilizes the EfEagerLoad Extensions when loading an Entity by Id or write a custom GetById that does so under the cover.


___

#### Controlling what gets Eager Loaded:


##### Built-in Attribute:



    public class EntityFrameworkRepository : IRepository


##### Custom Attribute:

##### Custom Filter:
___

#### EagerLoadAttribute Usage:

The **EagerLoadAttribute** can be configured with various settings that can modify the way the EfEagerLoad library builds the Include functions at runtime.

Examples of these include:
* *OnlyIfOnRoot* (default = false): this stops EfEagerLoad from eager loading this Propery if the property's parent Entity type is not of the Root Entity type being queried
* *NotIfOnRoot* (default = false): this stops EfEagerLoad from eager loading this Propery if the property's parent Entity type is the Root Entity type being queried
___

### Repository Integration:

The simple way to include it into a Repository implementation:

    public class EntityFrameworkRepository : IRepository
    {
        private readonly DbContext _dbContext;

        public EntityFrameworkRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<TEntity>> GetAll<TEntity>(bool eagerLoad = true) where TEntity : class
        {
            return await _dbContext.Set<TEntity>().EagerLoad(_dbContext, eagerLoad).ToListAsync();
        }

        public async Task<IList<TEntity>> GetAllMatching<TEntity>(Expression<Func<TEntity, bool>> predicate, bool eagerLoad = true) where TEntity : class
        {
            return await _dbContext.Set<TEntity>().Where(predicate).EagerLoad(_dbContext, eagerLoad).ToListAsync();
        }

        public async Task<TEntity> GetFirstMatching<TEntity>(Expression<Func<TEntity, bool>> predicate, bool eagerLoad = true) where TEntity : class
        {
            return await _dbContext.Set<TEntity>().Where(predicate).EagerLoad(_dbContext, eagerLoad).FirstOrDefaultAsync();
        }

        public IQueryable<TEntity> Query<TEntity>(bool eagerLoad = false) where TEntity : class
        {
            return _dbContext.Set<TEntity>().EagerLoad(_dbContext, eagerLoad).AsQueryable();
        }
    }
