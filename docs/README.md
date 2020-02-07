Master: ![Master](https://github.com/jsaret/EfEagerLoad/workflows/Build_Test_Publish/badge.svg?branch=master)

Development: ![Development](ttps://github.com/jsaret/EfEagerLoad/workflows/Build_Test_Publish/badge.svg?branch=development)

# EfEagerLoad

EfEagerLoad allows you to add eager loading to your Entities automatically with minimal setup.

It includes various extension points to selectively control how your entities are eager loaded such:
* Built-In configurable Attribute, 
* Custom user marker Attribute
* Automatically
* Custom user Predicates
* Custom built Strategies


While the built-in eager loading *Strategies* generally  Internal caching is built-in to speed up the  of the resulting Include Functions to execute for the Entities being used is built-in but only rudimentary at the moment (limitations).

___

## Contents
- [License](#license)
- [Getting Started](#Getting-Started)
  - [Setup](#Setup)
  - [Usage](#Usage)
    - [Integrating with Entity Framework](#Integrating-with-Entity-Framework)
    - [Repository Integration](#Repository-Integration)
    - [Controlling what gets Eager Loaded](#Controlling-what-gets-Eager-Loaded)
      - [Built-in Attribute](#Built-in-Attribute)
      - [Custom Attribute](#Custom-Attribute)
      - [Automatically](#Automatically)
      - [Functions](#Functions)
      - [Custom Strategy](#CustomStrategy)
      - [EagerLoadContext](#EagerLoadContext)
    - [Execution and Caching](#Execution-and-Caching)
- [Contact](#Contact) 


## License
See license details [here](/LICENSE.md).

___

## Getting Started
### Setup

First you'll need to install the library.

This can be done ether through the nu-get package manager or with the with the following commands:

    nu-get install.....


___
  
### Usage

Once set up, the eager loading functionality is easily plugged into your codebase.




### Repository Integration:


### Repository Integration:

The simple way to include it into a Repository implementation:

    public class SimplifiedEntityFrameworkRepository : IRepository
    {
        private readonly DbContext _dbContext;

        public EntityFrameworkRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TEntity> GetById<TEntity, TId>(long id) where TEntity : class, IEntity
        {
            return await _dbContext.Set<TEntity>().Where(entity.Id == id).EagerLoad(_dbContext).FirstOrDefaultAsync();
        }

        public async Task<IList<TEntity>> GetAll<TEntity>(bool eagerLoad = true, param string[] navPathsToIgnore) where TEntity : class
        {
            // If eager loading should always be run
            return await _dbContext.Set<TEntity>().EagerLoad(_dbContext).ToListAsync();
            
            // To Allow users of the Repository the ability to turn off the eager loading
            return await _dbContext.Set<TEntity>().EagerLoad(_dbContext, eagerLoad).ToListAsync();

            // To Allow users of the Repository to filter out Navigation paths explicitly
            // In this hypothetical example TEntity is a Book. 
            // We've been asked to display all Books (and their details) by the Author of a selected Book
            // The Navigation paths are as follows:
            // (Book): Author
            // (Book): Author - Books
            // (Book): Author - Books - Author (not really required as we already have the author loaded..)
            // (Book): Author - Books - Author - Books (- Author ... would repeat here till MaxDepth is reached)
            // (Book): Author - Books - Category
            // (Book): Author - Books - Publisher
            // (Book): Category
            // (Book): Publisher

            // We also haven't leaveraged the customizability of the EagerLoadAttribute and are using it with default settings


            return await _dbContext.Set<TEntity>().EagerLoad(_dbContext, navPathsToIgnore).ToListAsync();
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

Worth noting is that to benefit from the Eager Loading one would need to use a query that utilizes the EfEagerLoad Extensions when loading an Entity by Id or write a custom GetById that does so under the cover.


#### Controlling what gets Eager Loaded:


##### Built-in Attribute:



##### Custom Attribute:

##### Custom Filter:
___

#### EagerLoadAttribute Usage:

The **EagerLoadAttribute** can be configured with various settings that can modify the way the EfEagerLoad library builds the Include functions at runtime.

Examples of these include:
* *OnlyIfOnRoot* (default = false): this stops EfEagerLoad from eager loading this Propery if the property's parent Entity type is not of the Root Entity type being queried
* *NotIfOnRoot* (default = false): this stops EfEagerLoad from eager loading this Propery if the property's parent Entity type is the Root Entity type being queried
___

