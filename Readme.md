# bot-repo

A telegram bot to make sure that we don't mess up in our counting chat.

## Running a local copy
This will not talk or read from metacounting or counting.

1. Get added to the debug chat.
2. Acquire cntBotSettings.debug.json from TJ
4. Install docker
5. Run `docker build -t yourRepoName/count:tag.`
    * `yourRepoName` is usually your is supposed to be your username registered with docker. Locally it's fine to use whatever.
    * `count` is the name of the project.
    * `tag` is what the image will be called. You can leave it off (and the `:`) and docker will default to `latest`.
6. Run `docker run yourRepoName/count:tag`
7. Alternative to using docker, you can download and use `dotnet` cli to build and run by running `dotnet build TheCountBot.sln` and then `dotnet run TheCountBot.Application.csproj`
8. OR if you have vscode installed, you can just start debugging from within the IDE if you open the solution folder.

# Other topics
## Style
* Method names should be in PascalCase and not camelCase
  * `handleCoolThing` should be `HandleCoolThing`
* Spacing
  * `if (x%10!=firstDigit)` should be `if ( x % 10 != firstDigit )`
* Line endings
  * We haven't figured that one out yet.
* All `async` methods should end in `Async`
  * `public async Task GetResult()` should be `public async Task GetResultAsync()`

## `await` and `async`
[Docs](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/await)
[Tutorial](http://blog.stephencleary.com/2012/02/async-and-await.html)

```
// BAD (this probably won't compile)
public bool HandleMessage(string message)
{
  DoSomething();
}
private void DoSomething()
{
  // sends a message that awaits
  await SendMessageAsync("Hello, World!").ConfigureAwait( false );
}
```

```
// GOOD
public async Task<bool> HandleMessage(string message)
{
  await DoSomethingAsync().ConfigureAwait( false );
}
private async Task DoSomethingAsync()
{
  // sends a message that awaits
  await SendMessageAsync("Hello, World!").ConfigureAwait( false );
}
```
