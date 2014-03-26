﻿/////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Philippe Leefsma 2014 - ADN/Developer Technical Services
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////////////////

namespace Autodesk.ADN.ReCapDemo
{
    // Place holder for credentials
    class UserSettings
    {
        public readonly static string OAUTH_URL = "https://accounts.autodesk.com/";

        public readonly static string CONSUMER_SECRET = "*** YOUR CONSUMER SECRET KEY HERE***";
        public readonly static string CONSUMER_KEY = "*** YOUR CONSUMER KEY HERE***";

        public readonly static string RECAP_CLIENTID = "*** YOUR RECAP CLIENTID HERE***";
        public readonly static string RECAP_URL = "http://rc-api-adn.autodesk.com/3.1/API/";
    }
}
