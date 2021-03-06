﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Data.Entities.Oauth
{
    public class OauthSystem : Entity<Guid>
    {
        public Guid MachineKey { get; set; }

        [MaxLength(32)]
        public string MotherboardKey { get; set; }

        public string PhysicalMac { get; set; }

        public bool IsAutogeneratedMachineKey { get; set; }

        [MaxLength(200)]
        public string PcName { get; set; }
    }
}