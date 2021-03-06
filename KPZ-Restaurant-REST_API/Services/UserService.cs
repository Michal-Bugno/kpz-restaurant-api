﻿using KPZ_Restaurant_REST_API.Models;
using KPZ_Restaurant_REST_API.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace KPZ_Restaurant_REST_API.Services
{
    public class UserService : IUserService
    {
        private IUsersRepository _userRepo;

        public UserService(IUsersRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User> GetByUsername(string username)
        {
            return await _userRepo.GetByUsername(username);
        }

        public async Task<User> AddNewWaiter(User newWaiter)
        {
            var waiterAlreadyRegistered = await _userRepo.CheckIfPresent(newWaiter);

            if (!waiterAlreadyRegistered && (newWaiter.Rights != UserType.HEAD_WAITER || newWaiter.Rights != UserType.WAITER))
            {
                await _userRepo.Add(newWaiter);
                await _userRepo.SaveAsync();
                return newWaiter;
            }
            else
                return null;
        }

        public async Task<User> AddNewManager(RegisterModel manager, int restaurantId)
        {
            User newManager = new User()
            {
                Username = manager.Username,
                Password = manager.Password,
                FirstName = manager.FirstName,
                LastName = manager.LastName,
                Rights = UserType.MANAGER,
                RestaurantId = restaurantId
            };

            var alreadyRegistered = await _userRepo.CheckIfPresent(newManager);
            if (alreadyRegistered == true || newManager.Rights != UserType.MANAGER)
                return null;
            await _userRepo.Add(newManager);
            await _userRepo.SaveAsync();
            return newManager;
        }

        public async Task<IEnumerable<User>> GetAllWaiters(int restaurantId)
        {
            var waiters = await _userRepo.GetAllByRights(UserType.WAITER, restaurantId);
            var headWaiter = await _userRepo.GetAllByRights(UserType.HEAD_WAITER, restaurantId);
            waiters.Concat(headWaiter);

            return waiters;
        }

        public async Task<IEnumerable<User>> GetAllUsers(int restaurantId)
        {
            return await _userRepo.GetAllUsers(restaurantId);
        }

        public async Task<User> GetById(int id, int restaurantId)
        {
            return await _userRepo.GetUserById(id, restaurantId);
        }

        public async Task<IEnumerable<User>> GetAllCooks(int restaurantId)
        {
            return await _userRepo.GetAllByRights(UserType.COOK, restaurantId);
        }

        public async Task<User> AddNewCook(User user)
        {
            var cookAlreadyRegistered = await _userRepo.CheckIfPresent(user);

            if (!cookAlreadyRegistered && user.Rights == UserType.COOK)
            {
                await _userRepo.Add(user);
                await _userRepo.SaveAsync();
                return user;
            }
            else
                return null;
        }

        public async Task<User> DeleteUserById(int id, int restaurantId)
        {
            var deletedUser = await _userRepo.DeleteUserById(id, restaurantId);
            if (deletedUser != null)
            {
                await _userRepo.SaveAsync();
                return deletedUser;
            }
            else
                return null;
        }

        public async Task<User> UpdateUserInfo(User user, int restaurantId)
        {
            var userToUpdate = await _userRepo.GetUserById(user.Id, restaurantId);

            if (userToUpdate != null)
            {
                userToUpdate.Username = user.Username;

                if (user.Password != userToUpdate.Password && !PasswordHasher.ComparePassword(user.Password, userToUpdate.Password))
                    userToUpdate.Password = PasswordHasher.HashPassword(user.Password);

                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.Rights = user.Rights;

                _userRepo.Update(userToUpdate);
                await _userRepo.SaveAsync();
                return userToUpdate;
            }
            else
                return null;
        }
    }
}
