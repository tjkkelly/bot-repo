# bot-repo

A telegram bot to make sure that we don't mess up in our counting chat.

## Running a local copy
This will not talk or read from metacounting or counting.

1. Get added to the debug chat.
1. Update the `releaseMode` to `debug`' in `Program.cs`
1. Install docker
1. Run `docker build -t yourRepoName/count:tag.`
  * `yourRepoName` is usually your is supposed to be your username registered with docker. Locally it's fine to use whatever.
  * `count` is the name of the project.
  * `tag` is what the image will be called. You can leave it off (and the `:`) and docker will default to `latest`.
1. Run `docker run yourRepoName/count:tag`
