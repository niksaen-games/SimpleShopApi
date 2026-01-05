using SimpleShopApi.Data.Entities;
using SimpleShopApi.Data.Repositories;
using SimpleShopApi.Models.Dto;
using SimpleShopApi.Models.Requests;
using SimpleShopApi.Models.Responses;
using SimpleShopApi.Services;
using SimpleShopApi.Utils;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace SimpleShopApi.BLL
{
    public class UserService(UserRepository userRepository, JwtTokenGenerator tokenGenerator, MailService mailService, ConfirmCodeRepository codeRepository)
    {
        public async Task<ServiceResponse> CreateUserAsync(CreateUserRequest request)
        {
            var existUser = await userRepository.GetByEmailAsync(request.Email);
            if (existUser != null) return ServiceResponse.Failed("User already exist");
            var user = new User
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                Password = PasswordHasher.HashPassword(request.Password),
            };
            await userRepository.AddAsync(user);
            return new AuthResponse
            {
                Token = tokenGenerator.GenerateToken([new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())]),
                User = new UserDto
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    IsEmailConfirmed = user.IsEmailConfirmed
                }
            };
        }

        public async Task<ServiceResponse> LoginUserAsync(LoginRequest request)
        {
            var existUser = await userRepository.GetByEmailAsync(request.Email);
            if (existUser == null) return ServiceResponse.Failed("User not found");
            if (!PasswordHasher.VerifyPassword(request.Password, existUser.Password))
                return ServiceResponse.Failed("Incorrect credentials");
            return new AuthResponse
            {
                Token = tokenGenerator.GenerateToken([new Claim(ClaimTypes.NameIdentifier, existUser.Id.ToString())]),
                User = new UserDto
                {
                    Name = existUser.Name,
                    Surname = existUser.Surname,
                    Email = existUser.Email,
                    CreatedAt = existUser.CreatedAt,
                    IsEmailConfirmed = existUser.IsEmailConfirmed
                }
            };
        }

        public async Task<ServiceResponse> GetUserAsync(string token)
        {
            var uid = tokenGenerator.GetUserIdFromToken(token);
            if (uid == null) return ServiceResponse.Failed("User not found");
            var existUser = await userRepository.GetByIdAsync(uid.Value);
            if(existUser == null) return ServiceResponse.Failed("User not found");
            return ServiceResponse.Success(new UserDto
            {
                Name = existUser.Name,
                Surname = existUser.Surname,
                Email = existUser.Email,
                CreatedAt = existUser.CreatedAt,
                IsEmailConfirmed = existUser.IsEmailConfirmed
            });
        }

        public async Task<ServiceResponse> ChangePasswordAsync(string token, ChangePasswordRequest request)
        {
            var uid = tokenGenerator.GetUserIdFromToken(token);
            if (uid == null) return ServiceResponse.Failed("User not found");
            var user = await userRepository.GetByIdAsync(uid.Value);
            if (user == null) return ServiceResponse.Failed("User not found");

            if (!PasswordHasher.VerifyPassword(request.OldPassword, user.Password))
                return ServiceResponse.Failed("Incorrect credentials");

            user.Password = request.NewPassword;
            await userRepository.UpdateAsync(user);
            return ServiceResponse.Success("Password success updated");
        }

        public async Task<ServiceResponse> SendResetPasswordCodeAsync(string email)
        {
            var existUser = await userRepository.GetByEmailAsync(email);
            if (existUser == null) return ServiceResponse.Failed("User not found");
            var code = new Random(DateTime.UtcNow.Millisecond).Next(100_000,999_999);
            var confirmCode = new ConfirmCode
            {
                Email = email,
                Code = code.ToString(),
                ActionType = ConfirmCode.Action.ResetPassword
            };
            confirmCode = await codeRepository.AddAsync(confirmCode);
            await mailService.SendResetPasswordCodeAsync(email, existUser.Name, existUser.Surname, code.ToString());
            return ServiceResponse.Success("Confirm code send to you email");
        }
        public async Task<ServiceResponse> SendConfirmEmailCodeAsync(string token)
        {
            var uid = tokenGenerator.GetUserIdFromToken(token);
            if (uid == null) return ServiceResponse.Failed("User not found");
            var existUser = await userRepository.GetByIdAsync(uid.Value);
            if (existUser == null) return ServiceResponse.Failed("User not found");
            var code = new Random(DateTime.UtcNow.Millisecond).Next(100_000, 999_999);
            var confirmCode = new ConfirmCode
            {
                Email = existUser.Email,
                Code = code.ToString(),
                ActionType = ConfirmCode.Action.ConfirmEmail
            };
            confirmCode = await codeRepository.AddAsync(confirmCode);
            await mailService.SendConfirmEmailCodeAsync(existUser.Email, existUser.Name, existUser.Surname, code.ToString());
            return ServiceResponse.Success("Confirm code send to you email");
        }

        public async Task<ServiceResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var existUser = await userRepository.GetByEmailAsync(request.Email);
            if (existUser == null) return ServiceResponse.Failed("User not found");
            await codeRepository.CleanupCodesAsync();
            var existCode = await codeRepository.GetByEmailAsync(request.Email);
            if(existCode == null) return ServiceResponse.Failed("Confirm code not found");

            if (existCode.ActionType != ConfirmCode.Action.ResetPassword)
                return ServiceResponse.Failed("Incorrect confirm code");

            if(existCode.Code != request.Code)
                return ServiceResponse.Failed("Incorrect confirm code");

            existUser.Password = PasswordHasher.HashPassword(request.Password);
            await userRepository.UpdateAsync(existUser);
            await codeRepository.DeleteByIdAsync(existCode.Id);
            return ServiceResponse.Success("You password reset");
        }

        public async Task<ServiceResponse> ConfirmEmailAsync(string token, string confirmCode)
        {
            var uid = tokenGenerator.GetUserIdFromToken(token);
            if (uid == null) return ServiceResponse.Failed("User not found");
            var existUser = await userRepository.GetByIdAsync(uid.Value);
            if (existUser == null) return ServiceResponse.Failed("User not found");
            var existCode = await codeRepository.GetByEmailAsync(existUser.Email);
            if (existCode == null) return ServiceResponse.Failed("Confirm code not found");

            if (existCode.ActionType != ConfirmCode.Action.ConfirmEmail)
                return ServiceResponse.Failed("Incorrect confirm code");

            if (existCode.Code != confirmCode)
                return ServiceResponse.Failed("Incorrect confirm code");

            existUser.IsEmailConfirmed = true;
            await userRepository.UpdateAsync(existUser);
            await codeRepository.DeleteByIdAsync(existCode.Id);
            return ServiceResponse.Success("You email success confirmed");
        }

        public async Task<ServiceResponse> UpdateUserAsync(string token, UpdateUserRequest request)
        {
            var uid = tokenGenerator.GetUserIdFromToken(token);
            if (uid == null) return ServiceResponse.Failed("User not found");
            var existUser = await userRepository.GetByIdAsync(uid.Value);
            if (existUser == null) return ServiceResponse.Failed("User not found");

            if (string.IsNullOrWhiteSpace(request.Name) && string.IsNullOrWhiteSpace(request.Surname))
                return ServiceResponse.Success("No data to update");

            if (!string.IsNullOrWhiteSpace(request.Name))
                existUser.Name = request.Name;
            if (!string.IsNullOrWhiteSpace(request.Surname))
                existUser.Surname = request.Surname;

            existUser = await userRepository.UpdateAsync(existUser);
            return ServiceResponse.Success(new UserDto
            {
                Name = existUser.Name,
                Surname = existUser.Surname,
                Email = existUser.Email,
                CreatedAt = existUser.CreatedAt,
                IsEmailConfirmed = existUser.IsEmailConfirmed
            });
        }
    }
}
