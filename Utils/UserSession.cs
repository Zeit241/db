﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCursovaya
{
    public class UserSession
    {
        private static UserSession _instance;
        private static readonly object _lock = new object();

        public int UserId { get; private set; }
        public string Username { get; private set; }

        public int RoleId { get; private set; }
        public string Role { get; private set; }
        public DateTime LoginTime { get; private set; }

        private UserSession() { }

        public static UserSession Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new UserSession();
                        }
                    }
                }
                return _instance;
            }
        }

        public void Initialize(int userId, string username, string role, int? patient_id, int? doctor_id)
        {
            UserId = userId;
            Username = username;
            Role = role;
            LoginTime = DateTime.Now;

            if(patient_id != null)
            {
                RoleId = (int)patient_id;
            }
            if(doctor_id != null)
            {
                RoleId = (int)doctor_id;
            }
        }

        public void Clear()
        {
            UserId = 0;
            Username = null;
            Role = null;
            LoginTime = DateTime.MinValue;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(Username);
    }
}
