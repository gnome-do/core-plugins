/*
 * TwitterizerException.cs
 *
 * Copyright © 2008 digitallyborn <digitallyborn@gmail.com>
 * Copyright © 2008 jmargolese  <jmargolese@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Twitterizer.Framework
{
    public class TwitterizerException : Exception
    {
        private TwitterRequestData requestData;
        public TwitterRequestData RequestData
        {
            get { return requestData; }
            set { requestData = value; }
        }

        public TwitterizerException(string Message, TwitterRequestData RequestData)
            : base(Message)
        {
            requestData = RequestData;
        }

        public TwitterizerException(string Message, TwitterRequestData RequestData, Exception InnerException)
            : base(Message, InnerException)
        {
            requestData = RequestData;
        }
    }
}
