# Reddilonia (reddit + avalonia)

A prototype of a reddit client UI built with [AvaloniaUI](https://avaloniaui.net/). Viable for "lurking around", but it can't be used to post anything yet. It's made to run on desktop and mobile (Android only so far). It's definitely not production-ready. Use it at your own risk.

## What you get
- use your `AppId` and `AppSecret`
- front page feeds (`/best`)
- your subscribed subreddits
- see posts and comments (with basic info like up/down votes, author, creation time)
- post's image preview when available

## What's missing
Well... a lot of things so far 😅, but regarding basic features we still miss:
- upvotes and downvotes
- commenting
- video previews
- posts and comments pagination
- HTML rendering for subreddit/post body
- API rate limit info

## Known bugs
- images cache resets when restarting
- low quality post's image preview
- `ListBox` acting weird when loading posts' images or on click
- you need to restart the app to refresh an expired auth token
- another billion of unknown bugs

## Build requirements
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- `android workload` (run the command `dotnet workload install android` to install it)
- Android SDK
