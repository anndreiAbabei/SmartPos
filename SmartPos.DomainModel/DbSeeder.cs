﻿using System;
using System.Data;
using ServiceStack.OrmLite;
using SmartPos.DomainModel.Entities;
using SmartPos.DomainModel.Interfaces;
using SmartPos.GeneralLibrary;
using SmartPos.GeneralLibrary.Extensions;
using SmartPos.DomainModel.Security;

namespace SmartPos.DomainModel
{
    public class DbSeeder : ISeeder, IModelBuilder
    {
        #region Fields

        private readonly IPasswordHasher<User> _hasher;
        private readonly IDbConnection _connection;

        #endregion

        #region Constructors

        public DbSeeder(IDbContext context, IPasswordHasher<User> hasher)
        {
            _hasher = hasher;
            _connection = context.Connection;
        }

        #endregion

        #region Implementation of IModelBuilder

        public void Build()
        {
            _connection.CreateTableIfNotExists<User>();
            _connection.CreateTableIfNotExists<AccessRight>();
            _connection.CreateTableIfNotExists<UserAccessRight>();
        }

        #endregion

        #region Implementation of ISeeder

        public bool NeedSeed => _connection.Count<User>() <= 0;

        public void Seed()
        {
            var user = new User
                       {
                           FullName = "admin",
                           CreatedAt = DateTime.Now,
                           Email = "admin@smarpos.local",
                           Access = UserRank.Administrator
                       };

            user.Pin = _hasher.Hash("1234", user);
            _connection.Insert(user);
            
            foreach (var rightName in typeof(UserRights).GetPublicConstantValues<string>())
            {
                var right = new AccessRight
                            {
                                Name = rightName
                            };

                _connection.Insert(right);
            }
        }

        #endregion
    }
}