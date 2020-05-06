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

        private String CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Rights.ToString()),
                new Claim("Restaurant", user.RestaurantId.ToString())
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes("ABCDABCDEFGHEFGH"));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


        public UserService(IUsersRepository userRepo)//, IRestaurantGeneric<User> genericRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<string> AuthenticateUser(LoginModel model)
        {
            var user = await _userRepo.FindOne(user => user.Username == model.Username);
            if (user != null && PasswordHasher.ComparePassword(model.Password, user.Password)) //TODO Hash passwords
                return CreateToken(user);
            else
                return null;
        }

        public async Task<User> GetByUsername(string username)
        {
            var users = await _userRepo.GetAllFiltered(user => user.Username == username);
            if (users.Count != 1)
                return null;
            return users[0];
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

        public async Task<IEnumerable<User>> GetAllWaiters()
        {
            var waiters = await _userRepo.GetAllByRights(UserType.WAITER);
            var headWaiter = await _userRepo.GetAllByRights(UserType.HEAD_WAITER);
            waiters.AddRange(headWaiter);

            return waiters;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userRepo.GetAll();
        }

        public async Task<User> GetById(int id)
        {
            return await _userRepo.GetById(id);
        }

        public async Task<IEnumerable<User>> GetAllCooks()
        {
            return await _userRepo.GetAllByRights(UserType.COOK);
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
    }
}
