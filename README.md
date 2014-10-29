#BackupSharp

A C# library and command-line to backup your files from any source to any destination.

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
###Command-line option
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
  
###Explicit source and destination

####Ftp2Dropbox
Backup items from a FTP server to a Dropbox account:
```bash
mono BackupSharp.CommandLine.exe --sourceName=FTP --sourceArgs=[server address],[username],[password] --destinationName=Dropbox --destinationArgs=[API key],[API secret], [access token]
```
Where:
* [server address] = The address/IP of the source FTP server.
* [username] = The username used to connect on FTP server.
* [password] = The password used to connect on FTP server.
* [API key] = ?
* [API secret] = ?
* [access token] = ?
  

####Ftp2LocalFolder
Backup items from a FTP server to a local folder:
```bash
mono BackupSharp.CommandLine.exe --sourceName=FTP --sourceArgs=[server address],[username],[password] --destinationName=LocalFolder --destinationArgs=[destination folder]
```

Where:
* [server address] = The address/IP of the source FTP server.
* [username] = The username used to connect on FTP server.
* [password] = The password used to connect on FTP server.
* [destination folder] = The destination folder path.

####Ftp2Zip
Backup items from a FTP server to a Zip file:
```bash
mono BackupSharp.CommandLine.exe --sourceName=FTP --sourceArgs=[server address],[username],[password] --destinationName=Zip --destinationArgs=[destination folder]
```

Where:
* [server address] = The address/IP of the source FTP server.
* [username] = The username used to connect on FTP server.
* [password] = The password used to connect on FTP server.
* [destination folder] = The destination folder where the Zip file will be generated.

####LocalFolder2Zip
Backup items from a local folder to a Zip file:
```bash
mono BackupSharp.CommandLine.exe --sourceName=LocalFolder --sourceArgs=[source id],[source folder] --destinationName=Zip --destinationArgs=[destination folder]
```
Where:
* [source id] = The ID used to identify source.
* [source folder] = The path to the source folder.
* [destination folder] = The destination folder where the Zip file will be generated.



###Using a .config file

##Using the library
##Backup from MySQL to Zip
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
			// Writes the item using informatou on item.DestinationFullName and the bytes from data argument.
		}
}
```

##Roadmap
* New sources
	* MS SQL Server 
