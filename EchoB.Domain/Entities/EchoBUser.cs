
using EchoB.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EchoB.Domain.Entities
{
    public class EchoBUser : IdentityUser<Guid>
    {
        [Required]
        [StringLength(20, ErrorMessage = "Full name can't be longer than 20 characters.")]
        public string FullName { get; set; }
        [StringLength(200)]
        public string? ProfilePictureUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// Used to invalidate JWT refresh tokens when user logs out from all devices or changes password.
        /// </summary>
        public int TokenVersion { get; set; } = 1;
        [StringLength(200)]
        public string? Bio { get; set; }
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        public bool IsOnline { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }
        public string? TwoStepVerificationContact { get; set; }
        public bool IsVerified {  get; set; }
        private readonly List<BlockedUser> _blockedUsers = new();
        public IReadOnlyCollection<BlockedUser> BlockedUsers => _blockedUsers.AsReadOnly();



        private bool IsPhone(string primaryContact)
        {
            string phonePattern = @"^\+?[0-9]{10,15}$";
            if (Regex.IsMatch(primaryContact, phonePattern))
                return true;
            return false;
        }
        public EchoBUser(string fullName,string userName)
        {
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            
            if(IsPhone(userName))
                PhoneNumber = userName;
            else
                Email = userName;
            
            TokenVersion = 1;
            LastSeen = DateTime.UtcNow;
            IsOnline = false;
            EmailConfirmed = false;
            PhoneNumberConfirmed = false;
            TwoFactorEnabled = false;
            LockoutEnabled = true;
            AccessFailedCount = 0;
            IsDeleted = false;
            IsLocked = false;
            IsVerified = false;
        }
        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateProfile(FullName? fullName = null, Bio? bio = null,
            ProfilePictureUrl? profilePictureUrl = null, DateTime? dateOfBirth = null)
        {
            if (fullName != null)
                FullName = fullName;

            if (bio != null)
                Bio = bio;

            if (profilePictureUrl != null)
                ProfilePictureUrl = profilePictureUrl;

            if (dateOfBirth.HasValue)
            {
                if (dateOfBirth.Value > DateTime.UtcNow.AddYears(-10))
                    throw new ArgumentException("User must be at least 10 years old.");

                DateOfBirth = dateOfBirth.Value;
            }

            UpdateTimestamp();
        }

      

    

        public void SetOnlineStatus(bool isOnline)
        {
            IsOnline = isOnline;
            LastSeen = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void EnableTwoFactor()
        {
            TwoFactorEnabled = true;
            UpdateTimestamp();
        }

        public void DisableTwoFactor()
        {
            TwoFactorEnabled = false;
            UpdateTimestamp();
        }

        public void LockAccountTemp(DateTime? lockoutEnd = null)
        {
            LockoutEnd = lockoutEnd ?? DateTime.UtcNow.AddMinutes(30);
            UpdateTimestamp();
        }
        public void LockAccount()
        {
            IsLocked = true;
            UpdateTimestamp();
        }
        public void UnlockAccount()
        {
            LockoutEnd = null;
            AccessFailedCount = 0;
            IsLocked = false;
            UpdateTimestamp();
        }

        public void IncrementAccessFailedCount()
        {
            AccessFailedCount++;
            UpdateTimestamp();
        }

        public void ResetAccessFailedCount()
        {
            AccessFailedCount = 0;
            UpdateTimestamp();
        }

        public void InvalidateTokens()
        {
            TokenVersion++;
            UpdateTimestamp();
        }

        public bool IsLockedOut => (LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow)|| IsLocked == true;

        public void BlockUser(Guid userIdToBlock)
        {
            if (userIdToBlock == Id)
                throw new InvalidOperationException("User cannot block themselves.");

            var existingBlock = _blockedUsers.Find(b => b.BlockedUserId == userIdToBlock);
            if (existingBlock != null)
                return; // Already blocked

            var blockedUser = new BlockedUser(Id, userIdToBlock);
            _blockedUsers.Add(blockedUser);
            UpdateTimestamp();
        }

        public void UnblockUser(Guid userIdToUnblock)
        {
            var blockedUser = _blockedUsers.Find(b => b.BlockedUserId == userIdToUnblock);
            if (blockedUser != null)
            {
                _blockedUsers.Remove(blockedUser);
                UpdateTimestamp();
            }
        }

        public bool HasBlocked(Guid userId)
        {
            return _blockedUsers.Exists(b => b.BlockedUserId == userId);
        }
    }
}
