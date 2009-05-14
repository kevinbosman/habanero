//---------------------------------------------------------------------------------
// Copyright (C) 2008 Chillisoft Solutions
// 
// This file is part of the Habanero framework.
// 
//     Habanero is a free framework: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     The Habanero framework is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using System.Security.Cryptography;
using Habanero.Base;
using NUnit.Framework;

namespace Habanero.Test.Base
{
    [TestFixture]
    public class TestRSAPasswordCrypter
    {
        [Test]
        public void TestEncrypt()
        {
            RSA rsa = RSA.Create();
            ICrypter crypter = new RSAPasswordCrypter(rsa);
            string encrypted = crypter.EncryptString("testmessage");
            Assert.AreEqual(256, encrypted.Length);
        }

        [Test]
        public void TestDecrypt()
        {
            RSA rsa = RSA.Create();
            ICrypter crypter = new RSAPasswordCrypter(rsa);
            string encrypted = crypter.EncryptString("testmessage");
            string decrypted = crypter.DecryptString(encrypted);
            Assert.AreEqual("testmessage", decrypted);
        }
    }
}
