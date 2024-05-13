using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Reflection;
using usercenter.Api.Common;
using usercenter.Api.Data;
using usercenter.Api.Models;
using usercenter.Api.Services.Users;
using usercenter.Contracts.user;

namespace usercenter.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;

        public UserController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }


        //User user = new User()
        //{
        //    username = request.username,
        //    userAccount = request.userAccount,
        //    avatarUrl = request.avatarUrl,
        //    gender = request.gender,
        //    userPassword = hashedPassword,
        //    phone = request.phone,
        //    email = request.email,
        //    userStatus = 1,
        //    createTime = DateTime.Now,
        //    updateTime = DateTime.Now,
        //    isDelete = false,
        //    userRole = request.userRole,
        //    planetCode = request.planetCode,
        //};

        [HttpPost]
        public async Task <IActionResult> CreateUser(CreateUserRequest request)
        {
            string hashedPassword = EncryptionService.HashPasswordWithKey(request.userPassword, "usercenter");
            User user = new User(
                0,
                request.username,
                request.userAccount,
                request.avatarUrl,
                request.gender,
                hashedPassword,
                request.phone,
                request.email,
                1,
                DateTime.Now,
                DateTime.Now,
                false,
                request.userRole,
                request.planetCode);

            //_context.Users.Add(user);
            //var result = _context.SaveChanges();
            await _userService.CreateUser(user);

            // Use Task.Run to call GetUser asynchronously and await the result
            var createdUser = await Task.Run(() => _userService.GetUser(user.Id));

            var response = new UserResponse(
                    createdUser.Id,
                    createdUser.userName,
                    createdUser.userAccount,
                    createdUser.avatarUrl,
                    createdUser.gender,
                    createdUser.phone,
                    createdUser.email,
                    createdUser.userStatus,
                    createdUser.createTime,
                    createdUser.updateTime,
                    createdUser.isDelete,
                    createdUser.userRole,
                    createdUser.planetCode
                );

            return CreatedAtAction(
                    actionName : nameof(GetUser),
                    routeValues: new {id = user.Id},
                    value      : response
                );
        }

        [HttpGet]
        public async Task <ActionResult<User>> GetUser(int id)
        {
            //var user = await _context.Users.FindAsync(id);
            //if(user is null) 
            //{
            //    return NotFound("user not found");
            //}
            //return Ok(user);

            //User user = _userService.GetUser(id);
            // Use Task.Run to call GetUser asynchronously and await the result
            var createdUser = await Task.Run(() => _userService.GetUser(id));

            var response = new UserResponse(
                createdUser.Id,
                createdUser.userName,
                createdUser.userAccount,
                createdUser.avatarUrl,
                createdUser.gender,
                createdUser.phone,
                createdUser.email,
                createdUser.userStatus,
                createdUser.createTime,
                createdUser.updateTime,
                createdUser.isDelete,
                createdUser.userRole,
                createdUser.planetCode
                );
            return Ok( response );
        }
        //private async Task<List<User>> getAllUsers()
        //{
        //    return await _context.Users.Where(u => !u.isDelete).ToListAsync();
        //}

        [HttpGet]
        public async Task <ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }


        [HttpPut]
        public async Task<ActionResult<List<User>>> UpsertUser(UpsertUserRequest request)
        {
            var dbUser = await _context.Users.FindAsync(request.id);
            if (dbUser is null)
                return NotFound("User not found");

            //dbUser.userName = request.username;
            //dbUser.userAccount = request.userAccount;

            //await _context.SaveChangesAsync();
            var result = await _userService.UpsertUser(dbUser, request);
            //if(result == 1)

            return Ok(await _userService.GetAllUsers());
        }
        [HttpDelete]
        public async Task<ActionResult<List<User>>> DeleteUser(int id)
        {

            //var createdUser = await Task.Run(() => _userService.GetUser(user.Id));
            //var dbUser = await _userService.GetUser(id);
            //if (dbUser is null)
            //    return NotFound("User not found");

            //dbUser.isDelete = true;
            //await _context.SaveChangesAsync();

            var result = await _userService.DeleteUser(id);


            return Ok(await _userService.GetAllUsers());
        }
    }
}
