# TM Code Challenge Notes

[//]: # ( date: 06/03/22 )

## 1. Overview

- This document contains documentation and links for setting up, running, and maintaining:
  1. A C# .NET 6 service to access a Twitter stream API, compute various statistics for the random tweets that it provides, and provide access to the statistics via an HTTPS API call.
  2. A C# .NET 6 Razor Web application to display those results on demand by calling the aforementioned API.

### 1.1. Table of Contents

- [1. Overview](#1-overview)
  - [1.1. Table of Contents](#11-table-of-contents)
- [2. Specification](#2-specification)
- [3. Folders and Files](#3-folders-and-files)
- [4. Usage and Testing](#4-usage-and-testing)
  - [4.1. Setup](#41-setup)
  - [4.2. Manual Testing](#42-manual-testing)
  - [4.3. Automated Unit Tests](#43-automated-unit-tests)
- [5. Next Steps / To Do List](#5-next-steps--to-do-list)
  - [5.1. To Do before submission](#51-to-do-before-submission)
  - [5.2. Future features and steps for consideration](#52-future-features-and-steps-for-consideration)
- [6. Design diagrams](#6-design-diagrams)
  - [6.1. SampledStream Web API](#61-sampledstream-web-api)
  - [6.2. SampledStream Web Application](#62-sampledstream-web-application)
- [7. Twitter APIs](#7-twitter-apis)
  - [7.1. Using Postman to access Twitter APIs](#71-using-postman-to-access-twitter-apis)
  - [7.2. Run / Debug the solution projects](#72-run--debug-the-solution-projects)
- [8. References](#8-references)

[//]: # ( spell-checker: ignore blazor choco dockerignore plantuml sampledstream )

## 2. Specification

- Your app should consume this sample stream and keep track of the following:
  - Total number of tweets received
  - Top 10 Hashtags
- Your app should also provide some way to report these values to a user (periodically log to terminal, return from RESTful web service, etc).
- If there are other interesting statistics you’d like to collect, that would be great. There is no need to store this data in a database; keeping everything in-memory is fine. That said, you should think about how you would persist data if that was a requirement.
- It’s very important that when the application receives a tweet it does not block statistics reporting while performing tweet processing. Twitter regularly sees 5700 tweets/second, so your app may likely receive 57 tweets/second, with higher burst rates. The app should process tweets as concurrently as possible to take advantage of available computing resources.

## 3. Folders and Files

| **Folder or File**          | **Description**                                                                             |
| --------------------------- | ------------------------------------------------------------------------------------------- |
| SampledStreamApp            | Folder containing Web app to display Twitter data accumulated provided by our API           |
| SampledStreamAppTests       | Folder containing Web app unit test suite                                                   |
| SampledStreamCollector      | Folder containing Web service to collect and process tweets from the Twitter sampled stream |
| SampledStreamCollectorTests | Folder containing Web service unit test suite                                               |
| SampledStreamCommon         | Folder containing common classes to be shared between the Web app and the Web service       |
| `.dockerignore`             | List of files that Docker should ignore and not package                                     |
| `.gitattributes`            | List of files and Git attributes that Git should use when performing its actions            |
| `.gitignore`                | List of files that Git should ignore and not track                                          |
| `README.md`                 | This documentation file                                                                     |
| `TMCodeChallenge.sln`       | Main Code Challenge Visual Studio Solution file                                             |

## 4. Usage and Testing

### 4.1. Setup

- Create an environment variable `STREAM_BEARER_TOKEN` with your Twitter API bearer token
- Load the solution `TMCodeChallenge.sln` into Visual Studio

### 4.2. Manual Testing

- Select project `SampledStreamCollector` for the Web API service to collect tweet data and provide it in an API
  - Select `IIS Express` as Build Target on drop down (could also select `Docker` if you have Docker Desktop and WSL installed)
  - Select preferred Web Browser using Build Target drop down if required
  - Run (`Ctrl+F5`) or Debug (`F5`) the project, which should open a browser with the Swagger API
    - Refer to the Output window to see log messages such as for the Tweet processing
    - Use Swagger in the browser to execute the GET call to the API endpoint
      - Inspect the response body
      - Execute the API call repeatedly to see increasing numbers in stats
    - Test the API from the command line `curl -X 'GET' 'https://localhost:44355/api/GetSampledStream' -H 'accept: text/plain'`
- Select project `SampledStreamApp` for the Web App User Interface that calls the Web API (which must still be running)
  - Select `IIS Express` as Build Target (could also select `Docker` if you have Docker Desktop and WSL installed)
  - Select preferred Web Browser using Build Target drop down if required
  - Run (`Ctrl+F5`) or Debug (`F5`) the project, which should open a browser with the Swagger API
    - View the statistics
- Stop debugging (`Shift` `F5`)

### 4.3. Automated Unit Tests

- Select project SampledStreamApp
  - Note that all browser Unit Tests are currently hard coded to use Chrome (see To Do List)
  - Select `IIS Express` as Build Target on drop down (could also select `Docker` if you have Docker Desktop and WSL installed)
  - Start project without Debugging (`Ctrl+F5`) to show default Web page on localhost
    - If this is not run then the Selenium tests of the results web page will be skipped
      - Chrome will still load and exit a couple of times, but tests will still succeed
    - If this is run then the Selenium based tests of the results web page will be performed
  - Run all automated tests (`Ctrl+R, A`) or open the tests from Test Explorer (`Ctrl+E, T`) and run selected tests

## 5. Next Steps / To Do List

### 5.1. To Do before submission

- [x] Create SampleStreamedCollector project from ASP.NET Core Web API template in solution TMCodeChallenge
  - [x] Replace controller with dummy stream stats controller
  - [x] Add Test suite project and basic tests
  - [x] Add strategic exception handling
  - [x] Stats for API result to use shared class
    - [x] Return extra stats
  - [x] Create background task to attach to Twitter stream
    - [x] Add dummy tweets to concurrent queue
    - [x] Add incoming tweets to concurrent queue
  - [x] Create background service to pull incoming tweets from concurrent queue
    - [x] Loop through tweets extracting hashtags
    - [x] Add / update totals using a dictionary
    - [x] Add / update top 10 tag list
- [x] Create SampleStreamedApp project from ASP.NET Core Web App template in solution TMCodeChallenge
  - [x] Replace home page with dummy stream stats
  - [x] Add Test suite project and basic tests
  - [x] Add Index Page Object to simplify testing
  - [x] Add strategic exception handling and tests
  - [x] Stats for API result to use shared class
  - [x] Call SampleStreamCollector API to retrieve stats
- [x] Create SampleStreamShared project from shared code library template in solution TMCodeChallenge
  - [x] Shared stats class for API result
    - [x] Total tweets
    - [x] Tweets per hour and per day
    - [x] Tweet queue count
    - [x] Top 10 tags and counts
- [x] Documentation
  - [x] Usage and testing
  - [ ] Update diagrams with latest design
- [x] Submission email
  - [x] Set Twitter API bearer token in `STREAM_BEARER_TOKEN` environment variable
  - [x] GitHub URL and authentication (make repo public)

### 5.2. Future features and steps for consideration

- Use feature branches, merges, and pull requests for multi-user development, rather than just working on `main` branch!

| Priority | Category     | Effort | Description                                                                                                           |
| :------: | ------------ | :----: | --------------------------------------------------------------------------------------------------------------------- |
|    H     | Security     |   M    | Add OAUTH to secure the main API call                                                                                 |
|    H     | Tests        |   M    | Test and document running of tests using `dotnet` command for CI/CD automation                                        |
|    H     | Architecture |   M    | Improve exception handling for edge cases, network errors etc.                                                        |
|    H     | Architecture |   M    | Switch Web App to a full Razor MVC app for an improved UI                                                             |
|    M     | Security     |   M    | Add OAUTH to secure Swagger UI                                                                                        |
|    M     | Tooling      |   M    | Use gRPC for improving service performance and efficiency                                                             |
|    M     | Tooling      |   M    | Use GraphQL for more advanced APIs                                                                                    |
|    M     | Tooling      |   M    | Build a class library for the entity data model for persistence                                                       |
|    M     | Tooling      |   S    | Create deployment automation script / Docker compose file for Docker containers                                       |
|    M     | Architecture |   M    | Add ability to restart background services and tasks after a serious error                                            |
|    M     | Performance  |   M    | Create multiple TweetBlock processors that can each run concurrently                                                  |
|    M     | Performance  |   S    | Batch up (concatenate) incoming tweets before adding them as a block to the queue and then deserialize them as a list |
|    M     | Architecture |   M    | Move some of the hard coded values and URLs to configuration files                                                    |
|    M     | Tests        |   S    | Add testing of Blazor rendered pages using bUnit                                                                      |
|    M     | Tests        |   M    | Add more unit tests (always)                                                                                          |
|    M     | Tests        |   M    | Enhance Web UI tests to also run in FireFox and Edge                                                                  |
|    M     | Tests        |   M    | Finish setting up unit tests to run in parallel                                                                       |
|    M     | UI           |   S    | Add auto refresh of the Web app page and a combo box to control the frequency                                         |
|    L     | Architecture |   M    | Replace worker BackgroundService implementation with a .NET Queue Service                                             |
|    L     | UI           |   S    | Handle any multi-cultural and multi-lingual requirements such as date and number formatting                           |
|    L     | Tooling      |   S    | Minimize size of Docker containers by removing unneeded apps and tools                                                |
|    L     | Tests        |   S    | Handle any remaining edge cases of running the date and time tests at exactly midnight                                |

## 6. Design diagrams

### 6.1. SampledStream Web API

```mermaid
flowchart LR
  A("fa:fa-chrome External Application\ncalling API") <-->|Stats| PA
  B("fa:fa-twitter \nTwitter\nStream API") -->|Stream| CA
  subgraph SampledStreamCollector [.]
    subgraph Shared Data
      SA[Tweet queue]
      SB[Top 10 Hashtags]
      SC[Other Stats]
    end
    subgraph Collector
      CA(Tweet\nCollector) -->|Tweet data| SA
    end
    subgraph Updater
      SA -->|Tweet data| UA[[Parse out tweets]]
      UA --> UB[[Increment\nHashtag\nCounts]]
      UB <-->|AddOrUpdate| BA["All Hashtag Counts\n(ConcurrentDictionary)"]
      UB -->|Hashtag\n and Count| UC{Hashtag\nIn\nTop 10}
      UB --> SC
      UC -->|Yes| UD[[Replace\nin Top 10]]
      UD --> SB
      SB -.->|read| UC
    end
    subgraph Provider
      SB -.->|read| PA[GetStreamStats\nAPI call]
      SC -.->|read| PA
    end
  end
```

### 6.2. SampledStream Web Application

```mermaid
flowchart LR
  A("fa:fa-firefox Web Browser") <--> AA
  subgraph SampledStreamApp
    AA(Main)
  end
  AA[[Build Web Page]] <-->|Stats| PA
  subgraph SampledStreamCollector
    PA[GetStreamStats]
  end
```

## 7. Twitter APIs

- [Apply for API access](https://developer.twitter.com/en/apply-for-access)

  ```sh
  # Sample results
  API Key: QAktM6W6DF6F7XXXXXX
  API Key Secret: AJX560A2Omgwyjr6Mml2esedujnZLHXXXXXX
  Bearer Token: AAAAAAAAAAAAAAAAAAAAAL9v6AAAAAAA99t03huuqRYg0mpYAAFRbPR3XXXXXXX
  ```

- Test from command prompt

  ```powershell
  # Set bearer token for testing access
  $env:BEARER_TOKEN = 'AAAAAAAAAAAAAAAAAAAAAL9v6AAAAAAA99t03huuqRYg0mpYAAFRbPR3XXXXXXX'
  # Test API access
  curl -X GET "https://api.twitter.com/2/tweets/sample/stream" -H "Authorization: Bearer ${env:BEARER_TOKEN}"
  ```

### 7.1. Using Postman to access Twitter APIs

- Install Postman using Chocolatey
  - `choco install postman -y`
- Import collection from [Twitter API v2 collection](https://github.com/twitterdev/postman-twitter-api)
- Set up consumer keys and tokens in Environment

  ```sh
  # Example key values
  consumer_key: `QAktM6W6DF6F7XXXXXX`
  consumer_secret: `AJX560A2Omgwyjr6Mml2esedujnZLHXXXXXX`
  access_token: `1995XXXXX-0NGqVhk3s96IX6SgT3H2bbjOPjcyQXXXXXXX`
  token_secret: `rHVuh7dgDuJCOGeoe4tndtjKwWiDjBZHLaZXXXXXX`
  bearer_token: `AAAAAAAAAAAAAAAAAAAAAL9v6AAAAAAA99t03huuqRYg0mpYAAFRbPR3XXXXXXX`
  ```

- Click `Send` and check for 200 OK response and then `Cancel`
- Response body will be empty because Twitter does not fill streams  for Postman
- Use Code | cURL to view the command, then copy and run it

### 7.2. Run / Debug the solution projects

## 8. References

- [Additional Visual Studio Templates](https://dotnetnew.azurewebsites.net/)
