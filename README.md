#BackupSharp
[![Build Status](https://travis-ci.org/giacomelli/BackupSharp.png?branch=master)](https://travis-ci.org/giacomelli/BackupSharp)

A C# library and command-line to backup your items from any source to any destination.

##Sources & Destinations
Bellow are the current available sources and destinations:

* Sources
	* FTP
	* Local folder
	* MySQL
* Destinations
	* Dropbox 	
	* Local Folder
	* Zip
		
##Using the command-line	
###Command-line options
```bash
mono BackupSharp.CommandLine.exe --help
```

* -s, --sourceName
		  		
		The backup source class name

* -r, --sourceArgs         

		The backup source class constructor args

* -d, --destinationName    

		The backup destination class name

* -t, --destinationArgs

		The backup destination class constructor args

* -f, --file               

		Required. The backup configuration file

* -b, --backupName       

		Only backup with specified name will be run

* --sourceStartsWith       

		Only backup with source ID that starts with specified text

* --sourceEndsWith        
	
		Only backup with source ID that ends with specified text

* -v, --verbose            

		Should log everything

###Source arguments

####FTP
```
mono BackupSharp.CommandLine.exe 
--sourceName=FTP 
--sourceArgs=[server address],[username],[password] 
--destinationName=[destination] 
--destinationArgs=[destination args]
```
Where:
* [server address] = The address/IP of the source FTP server.
* [username] = The username used to connect on FTP server.
* [password] = The password used to connect on FTP server.

####Local folder
```
mono BackupSharp.CommandLine.exe 
--sourceName=LocalFolder 
--sourceArgs=[source id],[source folder] 
--destinationName=[destination] 
--destinationArgs=[destination args]
```
Where:
* [source id] = The ID used to identify source.
* [source folder] = The path to the source folder.

####MySQL
```
mono BackupSharp.CommandLine.exe 
--sourceName=MySql 
--sourceArgs=[connection string] 
--destinationName=[destination] 
--destinationArgs=[destination args]
```
Where:
* [connection string] = The connection string to MySQL database.

###Destination arguments

####Dropbox
```
mono BackupSharp.CommandLine.exe 
--sourceName=[source] 
--sourceArgs=[source args] 
--destinationName=Dropbox 
--destinationArgs=[API key],[API secret],[access token]
```
Where:
* [API key] = App key from Dropbox developer App Console.
* [API secret] = App secrect from Dropbox developer App Console.
* [access token] = Access token from Dropbox developer App Console.


> To use the Dropbox destination you will need to create an app on Dropbox developer App Console (https://www.dropbox.com/developers/apps), then access the app details where you can get the API key, API secret and the access token ('Generate acces token`).

####Local folder
```
mono BackupSharp.CommandLine.exe 
--sourceName=[source] 
--sourceArgs=[source args] 
--destinationName=LocalFolder 
--destinationArgs=[destination folder]
```
Where:
* [destination folder] = The destination folder path.

####Zip
```
mono BackupSharp.CommandLine.exe 
--sourceName=[source] 
--sourceArgs=[source args] 
--destinationName=Zip 
--destinationArgs=[destination folder]
```
Where:
* [destination folder] = The destination folder where the Zip file will be generated.

###Samples  

####Ftp2Dropbox
Backup items from a FTP server to a Dropbox account:
```
mono BackupSharp.CommandLine.exe 
--sourceName=FTP 
--sourceArgs=[server address],[username],[password] 
--destinationName=Dropbox 
--destinationArgs=[API key],[API secret],[access token]
```

####Ftp2LocalFolder
Backup items from a FTP server to a local folder:
```
mono BackupSharp.CommandLine.exe 
--sourceName=FTP 
--sourceArgs=[server address],[username],[password] 
--destinationName=LocalFolder 
--destinationArgs=[destination folder]
```

####Ftp2Zip
Backup items from a FTP server to a Zip file:
```
mono BackupSharp.CommandLine.exe 
--sourceName=FTP 
--sourceArgs=[server address],[username],[password] 
--destinationName=Zip 
--destinationArgs=[destination folder]
```

####LocalFolder2Zip
Backup items from a local folder to a Zip file:
```
mono BackupSharp.CommandLine.exe 
--sourceName=LocalFolder 
--sourceArgs=[source id],[source folder] 
--destinationName=Zip 
--destinationArgs=[destination folder]
```

###Using a .config file
```xml
<?xml version="1.0" encoding="UTF-8"?>
<backupSharp>
	<sources>
		<source type="LocalFolder" id="SourceFolder" args="temp/source" />		
        <source type="MySql" id="SourceMySql" args="server=my_server_address;user=my_user;pwd=my_password;database=my_database_name;allowzerodatetime=true;" />
	</sources>
	
	<destinations>
		<destination type="Zip" id="DestinationZip" args="temp" />
	</destinations>
	
	<backups>
		<backup source="SourceFolder" destination="DestinationZip" />
		<backup source="SourceMySql" destination="DestinationZip" />
	</backups>
</backupSharp>
```
```bash
mono BackupSharp.CommandLine.exe -f my_backups.config
```

##Using the library
##Running a backup from MySQL to Zip
```csharp
var source = new MySqlBackupSource(connectionString);
var destination = new ZipBackupDestination(destinationFolderPath);
var backup = new Backup(source, destination);
backup.Run();
```

###Creating a new backup source
To implement a new backup source you can implement the IBackupSource interface or inherit from BackupSourceBase:

```csharp
public class SampleBackupSource : BackupSourceBase
{
		/// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>
        /// The available items on source.
        /// </returns>
        public override IEnumerable<IBackupItem> GetItems()	
	    {
			// Get the available items on backup source.
			// Remember to use yield ;)
	    }

        /// <summary>
        /// Reads the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The item byte array.
        /// </returns>
        public override byte[] ReadItem(IBackupItem item)
		{
			// Reads the backup item in an array of bytes.
		}
}
```

###Creating a new backup destination
To implement a new backup destination you can implement the IBackupDestination interface or inherit from BackupDestinationBase:

```csharp
public class SampleBackupDestination : BackupDestinationBase
{
		/// <summary>
        /// Stores the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="data">The data.</param>
        public override void StoreItem(IBackupItem item, byte[] data)
		{
			// Writes the item using information on item.DestinationFullName and the bytes from data argument.
		}
}
```

##Running the functional tests
To run the tests from BackupSharp.FunctionalTests project you will need to set some environment variables:

- FTP
	- BackupSharpFtpServer
	- BackupSharpFtpUserName
	- BackupSharpFtpPassword
	- BackupSharpFtpFolder
- MySQL
	- BackupSharpMySqlServer
	- BackupSharpMySqlUserName
	- BackupSharpMySqlPassword
	- BackupSharpMySqlDatabase
- Dropbox
	- BackupSharpDropboxApiKey
	- BackupSharpDropboxApiSecret
	- BackupSharpDropboxAccessToken

```
We use environment variables to define those arguments to functional/integration tests because with environment variables we can encrypt the data to Travis-CI, like described in this tutorial [http://diegogiacomelli.com.br/2014/07/04/using-sensitive-data-on-your-travis-ci-build/](http://diegogiacomelli.com.br/2014/07/04/using-sensitive-data-on-your-travis-ci-build/ "Using sensitive data on your Travis-CI build")
```

##FAQ

####Having troubles? 
 - Ask on [Stack Overflow](http://stackoverflow.com/search?q=BackupSharp)

##Roadmap
* Package command-line to GitHub releases section.
* Publish NuGet package.
* New sources
	* MS SQL Server 
 
--------

##How to improve it?
- Create a fork of [BackupSharp](https://github.com/giacomelli/BackupSharp/fork). 
- Did you change it? [Submit a pull request](https://github.com/giacomelli/BackupSharp/pull/new/master).


##License

Licensed under the The MIT License (MIT).
In others words, you can use this library for developement any kind of software: open source, commercial, proprietary and alien.


##Change Log
 - 1.0.0 First version.
