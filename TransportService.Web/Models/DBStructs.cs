﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models
{
    
    public class DBStructs
    {
        public DBStructs()
        {
           
        }
    }
    public enum enumClientType
    {
        Transporter=1,
        Loader = 2,
        Broker=3

    }
    public enum enumIsCompany
    {
        Individual = 0,
        Company =1

    }
    public enum enumDBClientTypeID
    {
        Transporter=1,
        Corporater=2,
        Loader=3,
        Broker=4,
        IndividualTransporter=5,
        IndividualLoader=6,
        Both=7

    }

    public static class constLoadType
    {
        public const string FullTruckLoad = "Full Truck Load(Above 3000 Kg)";
        public const string PartTruckLoad = "Part Truck Load(500-3000 Kg)";
        public const string Parcel  = "Parcel (Less than 500 Kg)";

    }

}