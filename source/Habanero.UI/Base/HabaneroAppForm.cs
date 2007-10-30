//---------------------------------------------------------------------------------
// Copyright (C) 2007 Chillisoft Solutions
// 
// This file is part of Habanero Standard.
// 
//     Habanero Standard is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Habanero Standard is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with Habanero Standard.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using Habanero.DB;
using Habanero.Base;
//using Habanero.UI.Misc;
using Habanero.UI.Base;
using Habanero.UI.Forms;
using Habanero.Util;
using log4net;
using log4net.Config;

namespace Habanero.UI.Base
{
    /// <summary>
    /// Provides a template for a standard Habanero application, including
    /// standard fields and initialisations.  Specific details covered are:
    /// <ul>
    /// <li>The class definitions that define how the data is represented
    /// and limited</li>
    /// <li>The database configuration, connection and settings</li>
    /// <li>A logger to record debugging and error messages</li>
    /// <li>An exception notifier to communicate exceptions to the user</li>
    /// <li>Automatic version upgrades when an application is out-of-date</li>
    /// <li>A synchronisation controller</li>
    /// </ul>
    /// To set up and launch an application:
    /// <ol>
    /// <li>Instantiate the application with the constructor</li>
    /// <li>Specify any individual settings as required</li>
    /// <li>Call the Startup() method to launch the application</li>
    /// </ol>
    /// </summary>
    public class HabaneroAppForm : HabaneroApp
    {
        private IDefClassFactory _defClassFactory;
        private DatabaseConfig _databaseConfig;
        private string _privateKey;

        /// <summary>
        /// Constructor to initialise a new application with basic application
        /// information.  Use the Startup() method to launch the application.
        /// </summary>
        /// <param name="appName">The application name</param>
        /// <param name="appVersion">The application version</param>
        public HabaneroAppForm(string appName, string appVersion) : base(appName, appVersion) {}



        /// <summary>
        /// Sets the definition class factory.
        /// </summary>
        public IDefClassFactory DefClassFactory
        {
            set { _defClassFactory = value; }
        }

        /// <summary>
        /// Sets the database configuration object, which contains basic 
        /// connection information along with the database vendor name 
        /// (eg. MySql, Oracle).
        /// </summary>
        public DatabaseConfig DatabaseConfig {
            set { _databaseConfig = value; }
        }

        /// <summary>
        /// Sets the private key used to decrypt the database password. If your database password as supplied is
        /// in plaintext then this is not necessary. If you supply the DatabaseConfig object you can also set the
        /// private key on that instead.
        /// </summary>
        /// <param name="xmlPrivateKey">The private key (RSA) in xml format</param>
        public void SetPrivateKey(string xmlPrivateKey)
        {
            _privateKey = xmlPrivateKey;
        }

        /// <summary>
        /// Gets the loader for the xml class definitions
        /// </summary>
        /// <returns>Returns the loader</returns>
        private XmlClassDefsLoader GetXmlClassDefsLoader()
        {
            try
            {
                if (_defClassFactory != null)
                {
                    return new XmlClassDefsLoader(new StreamReader(ClassDefsFileName).ReadToEnd(), new DtdLoader(), _defClassFactory);
                } else {
                    return new XmlClassDefsLoader(new StreamReader(ClassDefsFileName).ReadToEnd(), new DtdLoader());
                }
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("Unable to find Class Definitions file. " +
                                                "This file contains all the class definitions that match " +
                                                "objects to database tables. Ensure that you have a classdefs.xml file " +
                                                "and that the file is being copied to your output directory (eg. bin/debug).", ex);
            }
        }

        protected override void SetupClassDefs()
        {
            if (LoadClassDefs) ClassDef.LoadClassDefs(GetXmlClassDefsLoader());
        }

        protected override void SetupSettings()
        {
            if (Settings == null) Settings = new DatabaseSettings();
            GlobalRegistry.Settings = Settings;
        }

         protected  override void  SetupDatabaseConnection()
                {
            if (_databaseConfig == null) _databaseConfig = DatabaseConfig.ReadFromConfigFile();
            if (_privateKey != null) _databaseConfig.SetPrivateKey(_privateKey);
            DatabaseConnection.CurrentConnection = _databaseConfig.GetDatabaseConnection();
        }

        protected  override void SetupExceptionNotifier() {
            if (ExceptionNotifier == null) ExceptionNotifier = new FormExceptionNotifier();
            GlobalRegistry.UIExceptionNotifier = ExceptionNotifier;
        }
    }
}