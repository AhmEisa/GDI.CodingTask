﻿#nullable disable

namespace TaxiBooking.Domain
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}