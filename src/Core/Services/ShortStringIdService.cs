﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public static class ShortStringIdService
    {
        private const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        public static string Generate(int length)
        {
            var newId = new char[length];
            var rand = new Random();

            for (int i = 0; i < length; i++)
            {
                newId[i] = chars[rand.Next(chars.Length)];
            }

            if (newId == null)
            {
                throw new Exception("generated short ID is null!-----");
            }

            var id = newId.ToString();

            if(id == null)
            {
                throw new Exception("generated short ID is null!-----");
            }

            return id;
        }
    }
}
