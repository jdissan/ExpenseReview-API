﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseReview.Data.Contracts;
using ReimbursementApp.EFRepository;

namespace ReimbursementApp.DatabaseHelpers
{
    public class RepositoryFactories
    {
        private IDictionary<Type, Func<Microsoft.EntityFrameworkCore.DbContext, object>> GetMovieReviewFactories()
        {
            return new Dictionary<Type, Func<Microsoft.EntityFrameworkCore.DbContext, object>>
            {
                {typeof(IRepository<>), dbContext => new RepositoryFactories()}

            };
        }

        public RepositoryFactories()
        {
            _repositoryFactories = GetMovieReviewFactories();
        }
        public RepositoryFactories(IDictionary<Type, Func<Microsoft.EntityFrameworkCore.DbContext, object>> factories)
        {
            _repositoryFactories = factories;
        }

        /// <summary>
        /// Get the repository factory function for the type.
        /// </summary>
        /// <typeparam name="T">Type serving as the repository factory lookup key.</typeparam>
        /// <returns>The repository function if found, else null.</returns>
        /// <remarks>
        /// The type parameter, T, is typically the repository type 
        /// but could be any type (e.g., an entity type)
        /// </remarks>
        public Func<Microsoft.EntityFrameworkCore.DbContext, object> GetRepositoryFactory<T>()
        {

            Func<Microsoft.EntityFrameworkCore.DbContext, object> factory;
            _repositoryFactories.TryGetValue(typeof(T), out factory);
            return factory;
        }

        /// <summary>
        /// Get the factory for <see cref="IRepository{T}"/> where T is an entity type.
        /// </summary>
        /// <typeparam name="T">The root type of the repository, typically an entity type.</typeparam>
        /// <returns>
        /// A factory that creates the <see cref="IRepository{T}"/>, given an EF <see cref="DbContext"/>.
        /// </returns>
        /// <remarks>
        /// Looks first for a custom factory in <see cref="_repositoryFactories"/>.
        /// If not, falls back to the <see cref="DefaultEntityRepositoryFactory{T}"/>.
        /// You can substitute an alternative factory for the default one by adding
        /// a repository factory for type "T" to <see cref="_repositoryFactories"/>.
        /// </remarks>
        public Func<Microsoft.EntityFrameworkCore.DbContext, object> GetRepositoryFactoryForEntityType<T>() where T : class
        {
            return GetRepositoryFactory<T>() ?? DefaultEntityRepositoryFactory<T>();
        }

        /// <summary>
        /// Default factory for a <see cref="IRepository{T}"/> where T is an entity.
        /// </summary>
        /// <typeparam name="T">Type of the repository's root entity</typeparam>
        protected virtual Func<Microsoft.EntityFrameworkCore.DbContext, object> DefaultEntityRepositoryFactory<T>() where T : class
        {
           return DbContext => new EFRepository<T>(DbContext);
        }

        /// <summary>
        /// Get the dictionary of repository factory functions.
        /// </summary>
        /// <remarks>
        /// A dictionary key is a System.Type, typically a repository type.
        /// A value is a repository factory function
        /// that takes a <see cref="DbContext"/> argument and returns
        /// a repository object. Caller must know how to cast it.
        /// </remarks>
        private readonly IDictionary<Type, Func<Microsoft.EntityFrameworkCore.DbContext, object>> _repositoryFactories;

    }
}