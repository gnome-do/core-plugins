// GmailContactItemSource.boo
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this
// source distribution.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Google

import System
import System.IO
import System.Collections.Generic

import Do
import Do.Universe

class GmailContactItemSource (IItemSource):

  static final BeginName  = "FN:"
  static final BeginEmail = "EMAIL;TYPE=INTERNET:"
  static final EndVCard   = "END:VCARD"
  
  items as List[of IItem]
  
  def constructor ():
    items = List[of IItem] ()
  
  Name:
    get: return "Gmail Contacts"

  Description:
    get: return "Indexes your Gmail contacts."

  Icon:
    get: return "stock_person"

  SupportedItemTypes:
    get: return (typeof (ContactItem),)
  
  Items:
    get: return items
  
  def ChildrenOfItem (parent as IItem):
    return null
  
  def UpdateItems ():
    items.Clear ()
    for contact in LoadContacts ():
      items.Add (contact)
  
  def LoadContacts ():
    seen = {}
    file = Paths.Combine (Paths.ApplicationData, "contacts.vcf")
    try:
      using reader = File.OpenText (file):
        while line = reader.ReadLine ():
          # Read email and name fields in current vcard:
          while line and not line.StartsWith (EndVCard):
            if line.StartsWith (BeginName):
              name = line [BeginName.Length:]
            elif line.StartsWith (BeginEmail):
              email = line [BeginEmail.Length:]
            line = reader.ReadLine ()
          # Skip contact if no email found
          if not email: continue
          # Create and save the contact.
          if name:
            contact = ContactItem.Create (name)
            key = name
          else:
            contact = ContactItem.CreateWithEmail (email)
            key = email
          seen [key] = contact
          contact ["email.gmail"] = email
    except e:
      Console.Error.WriteLine (
        "Could not read Gmail contacts file ${file}: $ {e.Message}")
    return seen.Values
