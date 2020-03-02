![Master](https://github.com/jsaret/EfEagerLoad/workflows/Build%20for%20Publish/badge.svg)
![Development](https://github.com/jsaret/EfEagerLoad/workflows/Build/badge.svg?branch=development)

# EfEagerLoad

###### Not Production ready - Release targeted end of Feb 2020

EfEagerLoad allows you to add eager loading to your Entity querying when using Entity Framework Core with minimal setup.

It includes various extension points to selectively control how your entities are eager loaded such as:
* Built-In configurable Attribute, 
* Custom user marker Attribute
* Automatically
* Custom input Predicates
* Custom built Strategies
* Input Parameters

## Contents
- [License](#license)
- [Why use EfEagerLoader](#Why-use-EfEagerLoader)
- [How it Works](#How-it-Works)
- [Usage](#Usage)
  - [Setup](#Setup)
  - [Entity Framework Integration](#Entity-Framework-Integrating)
  - [Repository Pattern Integration](#Repository-Pattern-Integration)
  - [Controlling what gets Eager Loaded](#Controlling-what-gets-Eager-Loaded)
    - [EagerLoad Attribute](#EagerLoad-Attribute)
    - [Custom Attribute](#Custom-Attribute)
    - [Automatically](#Automatically)
    - [Predicates](#Predicates)
    - [Custom Strategy](#CustomStrategy)
    - [Input Parameters](#Input-Parameters)
      - [Execution Control Parameters](#Execution-Control-Parameters)
      - [Navigation Paths Parameters](#Navigation-Paths-Parameters)
  - [Advanced Usage](#Advanced-Usage)
    - [Caching and IncludeExecution](#Caching-and-IncludeExecution)
    - [Custom Strategy Implementation](#Development)
  - [Here be Dragons](#Here-be-Dragons) 
- [Development](#Development) 
- [Contact](#Contact) 


## License

This project is released under an MIT license. See further license details [here](/LICENSE.md).


## Why use EfEagerLoader

Maybe you shouldn't ... Include (ThenInclude) statements aren't really that hard to do ...

Entity Framework often doesn't lend itself well to being entirely hidden entirely from developers - such as behind a single implementation of a Repository.
One needs to often leak the concerns of the framework into the developers world such as dealing with *Detached* Entities, eager loading, streaming, early termination..

This often leads to alot of repeated code such as Include statements especially if eager loading all properties of an Entity's graph is the most common loading strategy. 

To solve this develpers often have derived Repositories / helper classes to centralize this querying logic. 
This often leads to many Entity querying methods there that the developer needs to be aware of and distinguish between as to which one is right (or just revert back to just doing the querying manually).

Forgetting to eager load when it should be done will also lead to bad performance characteristics in your code with issues such as N+1 queries.

EfEagerLoader can help eliminate or minimize many of these issues.

## How it Works

[Navigation and IncludePath destription...]

The eager load engine includes built-in controllable caching of the *IncludePaths* results to speed up the resulting Include Functions calls to execute for the Entities being loaded. 
The cached results can be further filtered on a per call basis.

## Usage


### Setup

First you'll need to install the library.

This can be done either through the nu-get package manager built into your IDE or with the following commands:

    nu-get install EfEagerLoad

Once set up, the eager loading functionality is easily plugged into your codebase.
___
  
### Entity Framework Integration:

The EfEagerLoad library plugs into Entity Framework Core through various extension methods off the IQueryable interface such as in this example:

    dbContext.Set<Book>().EagerLoad(_dbContext).ToList();

Which extension method to use will depend on which strategy one wishes to use when eager loading your entities.

The following extension methods are currently offered:

> **EagerLoad**
>
> Eager loading based on the configurable built-in EagerLoad Attribute.
> 
> **EagerLoadForAttribute**
>
> Eager loading based on a custom user defined marker Attribute.
>
> **EagerLoadAll**
> 
> Automatic eager loading based on your model.
> 
> **EagerLoadMatching**
>
> Eager load either using a custom user defined IncludeStrategy or using predicate functions
> 

These extension methods also offer various extra optional parameters one can use to control how the EfEagerLoad library will execute (such as caching and the ability to further filter which IncludePaths will be considered).

See the [Controlling what gets Eager Loaded](#Controlling-what-gets-Eager-Loaded) section for further details

### Repository Pattern Integration:

The following code is a simplified Repository showing how to integrate the EfEagerLoad functionality into a Repository implementation:

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

            // To Allow users of the Repository to filter out Navigation paths (and their children) explicitly
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

Worth noting is that to benefit from the Eager Loading when querying an Entity by Id one would need to use a query that utilizes the EfEagerLoad Extensions when loading the Entity (or write a custom GetById such as the one in the above Repository that does so under the covers).


#### Controlling what gets Eager Loaded:


##### Built-in Attribute:

This will use the built-in EagerLoad Attribute to configure and control which *Include Paths* are considered by the EfEagerLoad library.

Using this approach one would add metadata to your *Entities* through the EagerLoad Attribute that cont/.. 


##### Custom Attribute:

This will use a custom user defined Attribute to configure and control which *Include Paths* are considered by the EfEagerLoad library.

Developers can add their custom *Attribute* to *Properties* on their *Entities* and then use one of the **EagerLoadForAttribute** extension methods letting the EfEagerLoad library know which Attribute to utilize such as in this code example:

    dbContext.Set<Book>().EagerLoadForAttribute<MyMarkerAttribute>(_dbContext)...;


##### Custom Filter:
___

#### EagerLoadAttribute Usage:

The **EagerLoadAttribute** can be configured with various settings that modify the way the EfEagerLoad library builds the Include functions at runtime.

These include:

* **Always** (default: false)

  This Property will always be eager loaded

* **Never** (default: false)

  This Property will never be eager loaded. 

  While this can be achieved by just not putting the EagerLoad Attribute on the property it's purpose will be utilized in the future when hybrid Include Strategies are implemented. 

* **OnlyOnRoot** (default: false)

  This stops EfEagerLoad from eager loading this Propery if the property's parent Entity is **not** the Root Entity being queried

* **NotOnRoot** (default: false)

  This stops EfEagerLoad from eager loading this Propery if the property's parent Entity is the Root Entity being queried

* **NotIfTypeVisited** (default: false)

* **NotIfRootType** (default: false)

  This stops EfEagerLoad from eager loading this Propery if the property's Navigation type is the Root Entity being queried's type

* **MaxDepth** (default: 6)

The maximum Navigation Path of the current Navigation depth that will be considered for eager loading. 

This setting will be only applied by the root Navigation Property on the type being loaded.

* **MaxDepthPosition** (default: 3)

The maximum Navigation Path depth of the current Navigation to decide if the Property should be eager loaded.

* **MaxRootTypeCount** (default: 3)


* **MaxTypeCount** (default: false)
