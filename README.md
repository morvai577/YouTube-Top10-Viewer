# Features
* Fetches videos from a YouTube channel using the channel ID.
* Displays the top 10 videos by view count for the current year.
* Caches playlist IDs to minimize API calls.
* Provides configuration via appsettings.json for flexible setup.

## Requirements
* .NET SDK 8.0 or higher
* A valid YouTube Data API v3 key
* Internet connection

## Installation
1. Clone the Repository

    `git clone https://github.com/your-username/YouTube-Top10-Viewer.git`
    
    `cd YouTube-Top10-Viewer`

2. Install Dependencies
   
   * Ensure you have the .NET SDK installed. 
   * Restore dependencies with: `dotnet restore`

3.	Configure API Key 

   * Open appsettings.json.
   * Replace "YOUR_API_KEY_HERE" with your YouTube Data API v3 key.

## Usage
1.	Build the Application: `dotnet build`
2.	Run the Application: `dotnet run`

You will be prompted to enter the YouTube channel ID. The app will then fetch and display the top 10 videos with the highest views for the current year.

## Sample Output

```
  Please enter the YouTube channel ID: UC_x5XG1OV2P6uZZ5FSM9Ttw
  1. Learn Python in 10 Minutes (500,123 views) - https://www.youtube.com/watch?v=abcdef12345
  2. C# for Beginners (350,987 views) - https://www.youtube.com/watch?v=ghijkl67890
  ...
  Press any key to exit...
```

## Caching

The app saves playlist IDs to a local cache.json file to minimize API calls. If the cache exists, it is loaded on subsequent runs to speed up operations.

## Customization
* Modify appsettings.json to set up custom logging levels or environment-specific configurations.
* Change the maximum number of videos or the filtering logic in Program.cs as needed.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests to improve this project.

## License

This project is licensed under the MIT License. See the LICENSE file for details.