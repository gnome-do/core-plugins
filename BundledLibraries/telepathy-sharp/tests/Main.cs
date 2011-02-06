/*
 *   Copyright (C) 2009 Neil Loknath <neil.loknath@gmail.com>
 * 
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU Lesser General Public License as published 
 *   by the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU Lesser General Public License for more details.
 *
 *   You should have received a copy of the GNU Lesser General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU LesserGeneral Public License as published 
 *   by the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 */

using System;

namespace tests
{
    
    
    class MainStage
    {
        enum TestType
        {
            Misc, MissionControl, DTube, FileTransfer
        };
        
        public static void Main(string[] args)
        {
            
            TestType type = TestType.Misc;   
            
            if (args.Length > 0) {
                if (!args[0].StartsWith ("--")) {
                    DisplayUsage ();
                    return;
                }
                else {
                    if (args[0].Equals ("--misc")) type = TestType.Misc;
                    else if (args[0].Equals ("--missioncontrol")) type = TestType.MissionControl;
                    else if (args[0].Equals ("--dtube")) type = TestType.DTube;
                    else if (args[0].Equals ("--filetransfer")) type = TestType.FileTransfer;
                    else {
                        DisplayUsage ();
                        return;
                    }
                }
            }
            
            string account = "jabber3";
            string contact = "banshee_test1@jabber.org";
            
            switch (type) {
                case TestType.Misc:
                    MiscTest t = new MiscTest ();
                    t.Initialize ();
                    break;
                case TestType.MissionControl:
                    McTest mct = new McTest ();
                    mct.Initialize ();
                    break;
                case TestType.DTube:    
                    
                    if (args.Length > 2) {
                        account = args[1];
                        contact = args[2];
                    }
                    else if (args.Length == 2) {
                        account = args[1];
                        contact = null;
                    }
                                
                    DTubeTest tube = new DTubeTest (account, contact);
                    tube.Initialize ();
                    break;
                case TestType.FileTransfer:
                                        
                    if (args.Length > 2) {
                        account = args[1];
                        contact = args[2];
                    }
                    else if (args.Length == 2) {
                        account = args[1];
                        contact = null;
                    }
                
                    FileTransfer ft = new FileTransfer (account, contact);
                    ft.Initialize ();
                    break;
                          
            }
             
        }
    
        private static void DisplayUsage ()
        {
            string usage = "tests.exe [options]";
            string options = "Valid options:\n --misc\n --missioncontrol\n --dtube [account]\n --filetransfer [account]";
            
            Console.WriteLine (usage);
            Console.WriteLine (options);
        }
        
    }
}